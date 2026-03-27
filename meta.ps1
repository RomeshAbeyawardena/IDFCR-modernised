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
    [Profile] $SelectedProfile;
    [Meta] $Context;
[System.Collections.Generic.List[Tag]] $Tags;

    MetaProfile() {

    }

    MetaProfile([Meta] $meta, [string] $profileName) {
        $this.Context = $meta;
        $this.PackageDescription = $meta.PackageDescription;
        $this.PackageName = $meta.PackageName;
        $this.Tags = $meta.Tags;
        
        if (-not $meta.Profiles.ContainsKey($profileName)) {
            throw "Profile '$profileName' not found in meta.json"
        }

        $this.SelectedProfile = $meta.Profiles[$profileName];
    }

    static [MetaProfile] LoadMeta([string] $json) {
        $options = [System.Text.Json.JsonSerializerOptions]::new();
        $options.PropertyNameCaseInsensitive = $true;

        return [System.Text.Json.JsonSerializer]::Deserialize(
            $json, [MetaProfile], $options
        );
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
