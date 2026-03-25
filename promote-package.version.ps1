param (
    [Parameter(Mandatory)]
    [string] $connectionString,
    [Parameter(Mandatory)]
    [string] $propsFile,
    [Parameter(Mandatory)]
    [string] $packageName,
    [Parameter(Mandatory)]
    [ValidateSet("Major", "Minor", "Patch", "Build")]
    [string] $targetComponent
)


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

$currentVersion = [version]::Parse($propertyGroup);
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
    "Patch" {
        $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor, $currentVersion.Patch + 1, 0);
        break;
    }
    "Build" {
        break;
    }
}
