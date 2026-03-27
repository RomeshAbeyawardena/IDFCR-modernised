param (
    [string]$secret,
    [string]$defaultProfile = "dev"
)

$env:DEV_CONN_STRING = "Server=localhost,5050;Initial Catalog=master;TrustServerCertificate=true;Connection Timeout=1000;User Id=sa;Password=$secret"
[System.Environment]::SetEnvironmentVariable("DevConnString", $env:DEV_CONN_STRING, "User")

$currentDirectory = Get-Location;
[System.IO.File]::WriteAllText([System.IO.Path]::Combine($currentDirectory, ".env"), "SA_PASSWORD=$secret
profile=$defaultProfile");

docker compose up -d