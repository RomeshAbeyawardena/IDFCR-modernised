$scriptRoot = $PSScriptRoot;

. $scriptRoot/meta.ps1

$currentDirectory = Get-Location;
$env = Get-Content ".\.env" -Raw;

$envDict = [System.Collections.Generic.Dictionary[string, string]]::new();

foreach ($line in $env.Split([System.Environment]::NewLine)) {
    
    if ([string]::IsNullOrWhiteSpace($line)) {
        continue;
    }

    $lineItems = $line.Split("=", 2);
    
    if ($lineItems.Length -eq 2) {
        $envDict.Add($lineItems[0], $lineItems[1]);
    }
}

$currentProfile = $envDict["profile"];

if ([string]::IsNullOrWhiteSpace($currentProfile) -eq $true) {
    Write-Error("Must provide a profile");
    exit 1;
}

$v = [Meta]::LoadMeta([System.IO.Path]::Combine($currentDirectory, './meta.json'))

Write-Output([MetaProfile]::new($v, $currentProfile).ToJson());