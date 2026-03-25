param (
    [string] $propsFile,
    [string] $newVersion
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
Write-Verbose("`tAssembly version: $($propertyGroup.AssemblyVersion)")
Write-Verbose("`tInformational version: $($propertyGroup.InformationalVersion)")
Write-Verbose("`tFile version: $($propertyGroup.FileVersion)")
Write-Verbose("`tPackage version: $($propertyGroup.PackageVersion)")
Write-Verbose("`tVersion: $($propertyGroup.Version)")

$assemblyVersion = [Version]::Parse($propertyGroup.AssemblyVersion);

$newVer = [Version]::Parse($newVersion);

if ($newVer.Major -ge $assemblyVersion.Major) {
    $assemblyVersion = [System.Version]::new($newVer.Major, 0, 0, 0);
    $propertyGroup.AssemblyVersion = $assemblyVersion.ToString();
}

$propertyGroup.InformationalVersion = $newVer.ToString()
$propertyGroup.FileVersion = $newVer.ToString()
$propertyGroup.PackageVersion = $newVer.ToString()
$propertyGroup.Version = $newVer.ToString()

Write-Verbose("New version data.");
Write-Verbose("`tAssembly version: $($propertyGroup.AssemblyVersion)")
Write-Verbose("`tInformational version: $($propertyGroup.InformationalVersion)")
Write-Verbose("`tFile version: $($propertyGroup.FileVersion)")
Write-Verbose("`tPackage version: $($propertyGroup.PackageVersion)")
Write-Verbose("`tVersion: $($propertyGroup.Version)")

$xml.Save($propsFile);