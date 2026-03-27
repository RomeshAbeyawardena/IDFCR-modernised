
class Tag {
    [string] $Condition;
    [string] $DisplayName;
    [string] $Name;
}

class Profile {
    [string] $Name;
    [System.Collections.Generic.List[Tag]] $Tags;
}

class MetaProfile {
    [string] $PackageDescription;
    [string] $PackageName;
    [Profile] $MetaProfile;
    [Meta] $Context;

    MetaProfile([Meta] $meta, [string] $profileName) {
        $this.Context = $meta;
        $this.PackageDescription = $meta.PackageDescription;
        $this.PackageName = $meta.PackageName;

        if (-not $meta.Profiles.ContainsKey($profileName)) {
            throw "Profile '$profileName' not found in meta.json"
        }

        $this.MetaProfile = $meta.Profiles[$profileName];
    }

    [string] ToJson() {
        $options = [System.Text.Json.JsonSerializerOptions]::Web;
        return [System.Text.Json.JsonSerializer]::Serialize($this, [MetaProfile], $options);
    }
}

class Meta {
    [string] $PackageDescription;
    [string] $PackageName;
    [System.Collections.Generic.Dictionary[string, Profile]] $Profiles;
    [System.Collections.Generic.List[Tag]] $Tags;

    static [Meta] LoadMeta([string] $path) {
        
        $json = Get-Content $path -Raw;

        $options = [System.Text.Json.JsonSerializerOptions]::new();
        $options.PropertyNameCaseInsensitive = $true;

        return [System.Text.Json.JsonSerializer]::Deserialize(
            $json, [Meta], $options
        );
    }
}

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

$v = [Meta]::LoadMeta("./meta.json")

Write-Output([MetaProfile]::new($v, $currentProfile).ToJson());