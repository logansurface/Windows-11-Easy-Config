param(
    [Parameter(Mandatory=$true)]
    [string]$RegistryPath,
    
    [Parameter(Mandatory=$true)]
    [string]$KeyName,
    
    [Parameter(Mandatory=$true)]
    [string]$KeyValue,
    
    [Parameter(Mandatory=$true)]
    [ValidateSet('String', 'Binary', 'DWord', 'QWord', 'MultiString', 'ExpandString')]
    [string]$KeyType
)

# Function to convert value to appropriate type
function Convert-ValueToType {
    param(
        [string]$Value,
        [string]$Type
    )
    
    switch ($Type) {
        'String' { return $Value }
        'DWord' { return [int]$Value }
        'QWord' { return [long]$Value }
        'Binary' { return [byte[]]($Value -split ',') }
        'MultiString' { return $Value -split '|' }
        'ExpandString' { return $Value }
        default { throw "Unsupported registry value type: $Type" }
    }
}

# Function to convert registry path to PowerShell format
function Convert-RegistryPath {
    param([string]$Path)
    
    # Remove any leading/trailing whitespace
    $Path = $Path.Trim()
    
    # Convert common registry path formats
    $Path = $Path.Replace("HKEY_LOCAL_MACHINE", "HKLM:")
    $Path = $Path.Replace("HKEY_CURRENT_USER", "HKCU:")
    $Path = $Path.Replace("HKEY_USERS", "HKU:")
    $Path = $Path.Replace("HKEY_CLASSES_ROOT", "HKCR:")
    $Path = $Path.Replace("HKEY_CURRENT_CONFIG", "HKCC:")
    
    # If no prefix was found, assume HKLM
    if (-not ($Path.StartsWith("HKLM:") -or $Path.StartsWith("HKCU:") -or 
              $Path.StartsWith("HKU:") -or $Path.StartsWith("HKCR:") -or 
              $Path.StartsWith("HKCC:"))) {
        $Path = "HKLM:" + $Path
    }
    
    # Ensure proper path format
    $Path = $Path.Replace("\", "\")
    
    return $Path
}

try {
    # Convert registry path to PowerShell format
    $psRegistryPath = Convert-RegistryPath -Path $RegistryPath
    
    # Check if the registry path exists
    if (!(Test-Path $psRegistryPath)) {
        Write-Host "Creating registry path: $psRegistryPath"
        New-Item -Path $psRegistryPath -Force | Out-Null
    }
    else {
        Write-Host "Registry path already exists: $psRegistryPath"
    }

    # Convert the value to the appropriate type
    $convertedValue = Convert-ValueToType -Value $KeyValue -Type $KeyType

    # Set the registry value
    Write-Host "Setting registry value:"
    Write-Host "Path: $psRegistryPath"
    Write-Host "Name: $KeyName"
    Write-Host "Type: $KeyType"
    Write-Host "Value: $KeyValue"
    
    Set-ItemProperty -Path $psRegistryPath -Name $KeyName -Value $convertedValue -Type $KeyType

    Write-Host "`nRegistry modification completed successfully!"
} catch {
    Write-Host "Error: $_"
    exit 1
}

# Wait for user input before closing
Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")