param (
    [Parameter(Mandatory)]
    [string] $connectionString,
    [Parameter(Mandatory)]
    [string] $packageName,
    [Parameter(Mandatory)]
    [ValidateSet("Major", "Minor", "Build", "Revision")]
    [string] $targetComponent,
    [string] $propsFile
)

$currentDirectory = Get-Location;

if ([string]::IsNullOrWhiteSpace($propsFile)) {
    $propsFile = "$currentDirectory\Directory.Build.props";
}


if ([System.IO.File]::Exists($propsFile) -eq $false) {
    $propsFile = [System.IO.File]::Combine($currentDirectory, $propsFile);

    if ([System.IO.File]::Exists($propsFile) -eq $false) {
        Write-Error("Unable to find path '$propsFile'");
        exit 1;
    }
}

$xml = New-Object -TypeName "System.Xml.XmlDocument"

$xml.Load($propsFile);

$propertyGroups = $xml.SelectNodes("//Project/PropertyGroup")

if ($propertyGroups.Count -ne 1) {
    Write-Error "Expected exactly one PropertyGroup, found $($propertyGroups.Count). Unable to continue this is against msdn guidelines and will impact solution build performance. Microsoft recommends one PropertyGroup in this file, unless you know what you're doing!"
    exit 1
}

$propertyGroup = $propertyGroups[0]

Write-Verbose("Currently recorded version data.");
Write-Verbose("`tAssembly version: $($propertyGroup.AssemblyVersion)");
Write-Verbose("`tInformational version: $($propertyGroup.InformationalVersion)");
Write-Verbose("`tFile version: $($propertyGroup.FileVersion)");
Write-Verbose("`tPackage version: $($propertyGroup.PackageVersion)`t");
Write-Output("Current Version: $($propertyGroup.Version)");

$currentVersion = [version]::Parse($propertyGroup.Version);

$newVersion = [version]::new();

switch ($targetComponent) {
    "Major" {  
        $newVersion = [version]::new($currentVersion.Major + 1, 0, 0, 0);
        break;
    }
    "Minor" {
        $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor + 1, 0, 0);
        break;
    }
    "Build" {
        $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor, $currentVersion.Build + 1, 0);
        break;
    }
    "Revision" {
        $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor, $currentVersion.Build, 0);
        break;
    }
    default {
        $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor, $currentVersion.Build, 0);
        break;
    }
}

$versionPrefix = "$($newVersion.Major).$($newVersion.Minor).$($newVersion.Build)"

Write-Output "New version suffix: $versionPrefix"

$newVersion = . ./get-next-package-version.ps1 -connectionString $connectionString -packageName $packageName -versionPrefix $versionPrefix

$newVersion
