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
    [string] $PackageAlias;
    [Profile] $SelectedProfile;
    [Meta] $Context;
    [System.Collections.Generic.List[Tag]] $Tags;

    MetaProfile() {

    }

    MetaProfile([Meta] $meta, [string] $profileName) {
        $this.Context = $meta;
        $this.PackageDescription = $meta.PackageDescription;
        $this.PackageName = $meta.PackageName;
        $this.PackageAlias = $meta.PackageAlias;
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
    [string] $PackageAlias;
    [System.Collections.Generic.Dictionary[string, Profile]] $Profiles;
    [System.Collections.Generic.List[Tag]] $Tags;

    static [string] GenerateBlankJsonTemplate() {
        $options = [System.Text.Json.JsonSerializerOptions]::Web;

        $model = [Meta]::new();
        $model.PackageAlias = '';
        $model.PackageDescription = '';
        $model.PackageName = '';
        $model.Profiles = [System.Collections.Generic.Dictionary[string, Profile]]::new()
        $dummyProfile = [Profile]::new();
        $dummyProfile.Name = '';
        $dummyProfile.Tags = [System.Collections.Generic.List[Tag]]::new();
        $tag = [Tag]::new();
        $tag.Condition = '';
        $tag.DisplayName = '';
        $tag.Name = '';
        $dummyProfile.Tags.Add($tag);

        $model.Profiles.Add("", $dummyProfile)
        $model.Tags = [System.Collections.Generic.List[Tag]]::new();
        $model.Tags.Add($tag);

        return [System.Text.Json.JsonSerializer]::Serialize($model, [Meta], $options);
    }

    static [Meta] LoadMeta([string] $path) {
        
        $json = Get-Content $path -Raw;

        $options = [System.Text.Json.JsonSerializerOptions]::new();
        $options.PropertyNameCaseInsensitive = $true;

        return [System.Text.Json.JsonSerializer]::Deserialize(
            $json, [Meta], $options
        );
    }
}
