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

try {
    # Check if the registry path exists
    if (!(Test-Path $RegistryPath)) {
        Write-Host "Creating registry path: $RegistryPath"
        New-Item -Path $RegistryPath -Force | Out-Null
    }
    else {
        Write-Host "Registry path already exists: $RegistryPath"
    }

    # Convert the value to the appropriate type
    $convertedValue = Convert-ValueToType -Value $KeyValue -Type $KeyType

    # Set the registry value
    Write-Host "Setting registry value:"
    Write-Host "Path: $RegistryPath"
    Write-Host "Name: $KeyName"
    Write-Host "Type: $KeyType"
    Write-Host "Value: $KeyValue"
    
    Set-ItemProperty -Path $RegistryPath -Name $KeyName -Value $convertedValue -Type $KeyType

    Write-Host "`nRegistry modification completed successfully!"
} catch {
    Write-Host "Error: $_"
    exit 1
}

# Wait for user input before closing
Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")