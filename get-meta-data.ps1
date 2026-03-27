Add-Type -AssemblyName System.Text.Json

class Tag {
    [string] $Condition;
    [string] $DisplayName;
    [string] $Name;
}

class Profile {
    [string] $Name;
    [System.Collections.Generic.List[Tag]] $Tags;
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

$v = [Meta]::LoadMeta("./meta.json")

$v.Profiles;