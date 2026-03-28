param (
    [string]$secret,
    [string]$defaultProfile = "dev"
)

$currentDirectory = Get-Location;
$scriptRoot = $PSScriptRoot;
$metaPath = [System.IO.Path]::Combine($currentDirectory, "meta.json");
if ([System.IO.File]::Exists($metaPath) -eq $false)
{
    $metaFunctions = [System.IO.Path]::Combine($scriptRoot, "meta.ps1");
    . $metaFunctions
    $json = [Meta]::GenerateBlankJsonTemplate();
    [System.IO.File]::WriteAllText($metaPath, $json);
}

$env:DEV_CONN_STRING = "Server=localhost,5050;Initial Catalog=master;TrustServerCertificate=true;Connection Timeout=1000;User Id=sa;Password=$secret"
[System.Environment]::SetEnvironmentVariable("DevConnString", $env:DEV_CONN_STRING, "User")


[System.IO.File]::WriteAllText([System.IO.Path]::Combine($currentDirectory, ".env"), "SA_PASSWORD=$secret
profile=$defaultProfile");

docker compose up -d