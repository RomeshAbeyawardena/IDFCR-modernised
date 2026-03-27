
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



$env = Get-Content ".\.env" -Raw;

$envDict = [System.Collections.Generic.Dictionary[string, string]]::new();

foreach ($line in $env.Split([System.Environment]::NewLine)) {
    
    if ([string]::IsNullOrWhiteSpace($line)) {
        continue;
    }

    $lineItems = $line.Split("=");
    
    if ($lineItems.Length -eq 2) {
        $envDict.Add($lineItems[0], $lineItems[1]);
    }
}

$currentProfile = $envDict["profile"];
$v = [Meta]::LoadMeta("./meta.json")

$v.Profiles;