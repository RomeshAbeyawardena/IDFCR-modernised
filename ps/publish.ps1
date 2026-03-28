param (
    [string]$connectionString,
    [string]$solutionPath,
    [Parameter(Mandatory)]
    [ValidateSet("Major", "Minor", "Build", "Revision")]
    [string] $targetComponent,
    [string] $propsFile
)

$scriptRoot = $PSScriptRoot;
$metaDataScriptPath = [System.IO.Path]::Combine($scriptRoot, 'get-meta-data.ps1');

$json = . $metaDataScriptPath

$meta = [MetaProfile]::LoadMeta($json)

$selectedProfile = $meta.SelectedProfile

Write-Information("1/4 Checking if this commit ID has already been released...")
try {
    $commitID = git rev-parse HEAD
    $conn = [System.Data.SqlClient.SqlConnection]::new($connectionString)
    $conn.Open();
    $command = $conn.CreateCommand()
    $command.CommandText = 'USE [PackageManager];
   SELECT CASE 
    WHEN EXISTS (
        SELECT 1
        FROM dbo.PackageVersion pv
        INNER JOIN dbo.Package p ON p.PackageId = pv.PackageId
        WHERE p.Namespace = @Namespace
          AND pv.CommitId = @CommitId
    )
    THEN 1 ELSE 0 
END'
    
    $command.Parameters.AddWithValue("Namespace", $selectedProfile.Namespace);
    $command.Parameters.AddWithValue("CommitId", $commitID);

    $exists = $command.ExecuteScalar();
    $command.Dispose();
    if ($exists) {
        Write-Error("Commit ID: $commitId already has a release candidate.");
        exit 1;
    }
}
finally {
    $conn.Dispose();
}
Write-Information("Commit ID: [OK]");

Write-Information("2/4 Attempting a build, test and package on a first pass to ensure this this a worthwhile build")

dotnet build $solutionPath
if ($LASTEXITCODE -ne 0) {
    Write-Error("Unable to build package on first pass");
    exit $LASTEXITCODE 
}

dotnet test $solutionPath --no-build
if ($LASTEXITCODE -ne 0) { 
    Write-Error("Unable to test package on first pass");
    exit $LASTEXITCODE 
}

dotnet pack $solutionPath
if ($LASTEXITCODE -ne 0) { 
    Write-Error("Unable to pack package on first pass");
    exit $LASTEXITCODE 
}

$promotePackageVersion = [System.IO.Path]::Combine($scriptRoot, 'promote-package.version.ps1');

$params = @{
    connectionString = $connectionString
    targetComponent = $targetComponent
    propsFile = $propsFile
}

Write-Information("3/4 Publishing release data")

. $promotePackageVersion $params

Write-Information("4/4 Final release build and publishing package")