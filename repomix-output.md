This file is a merged representation of the entire codebase, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of the entire repository's contents.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

## Additional Info

# Directory Structure
```
.gitignore
EasyConfig.cs
EasyConfig.csproj
Form.cs
README.md
Set-RegistryKey.ps1
```

# Files

## File: EasyConfig.csproj
```
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  
  <ItemGroup>
    <None Update="Set-RegistryKey.ps1">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
</Project>
```

## File: .gitignore
```
# .NET Core build folders
bin/
obj/

# Visual Studio files
.vs/
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/
[Oo]ut/
msbuild.log
msbuild.err
msbuild.wrn

# Visual Studio cache/options directory
.vs/
```

## File: EasyConfig.cs
```csharp
using System;
using System.Windows.Forms;

namespace EasyConfig
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
```

## File: README.md
```markdown
# Easy Config (Windows 11)
### Registry Paths (Planned Addition and Tested Working)
path: HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarAl \
value: 0 \
action: Align the taskbar left \\

path: HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDa \
value: 0 \
action: Disable widgets on the taskbar \
```

## File: Set-RegistryKey.ps1
```powershell
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
    exit 0
}
catch {
    Write-Host "Error: $_"
    exit 1
}
```

## File: Form.cs
```csharp
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

// TODO: Add button array with pre-defined registry paths and keys
// beneath the manual registry modification section.
// TODO: Add a "Restore Default" button
// TODO: Regex path validation

namespace EasyConfig
{
    public class MainForm : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private Button runScriptButton;
        private TextBox registryPathBox;
        private TextBox keyNameBox;
        private TextBox keyValueBox;
        private ComboBox keyTypeCombo;
        private Label pathLabel;
        private Label nameLabel;
        private Label valueLabel;
        private Label typeLabel;
        private const int CONTROL_ROWS = 4; // Number of input rows
        private const int CONTROL_HEIGHT = 250;
        private const int VERTICAL_SPACING = 0;
        private const int HORIZONTAL_PADDING = 10;
        private const int VERTICAL_PADDING = 0;
        private const int FORM_WIDTH = 1000;
        private const int FORM_HEIGHT = 400;
        private const int BUTTON_HEIGHT = 250;
        private const int LABEL_WIDTH = FORM_WIDTH / 3;
        private const int CONTROL_SPACING = 0;

        public MainForm()
        {
            // Set fixed window size and disable resizing
            this.Size = new System.Drawing.Size(FORM_WIDTH, FORM_HEIGHT);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Windows 11 Easy Config";

            // Create TableLayoutPanel
            tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = CONTROL_ROWS,
                Padding = new Padding(HORIZONTAL_PADDING, VERTICAL_PADDING, HORIZONTAL_PADDING, VERTICAL_PADDING)
            };

            // Configure TableLayoutPanel rows and columns
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, LABEL_WIDTH));
            // Add another column style for the second column
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Create labels and input fields
            pathLabel = new Label { Text = "Registry Path:" , Anchor = AnchorStyles.Left , AutoSize = true};
            registryPathBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            nameLabel = new Label { Text = "Key Name:" , Anchor = AnchorStyles.Left , AutoSize = true};
            keyNameBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            valueLabel = new Label { Text = "Key Value:" , Anchor = AnchorStyles.Left , AutoSize = true};
            keyValueBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            typeLabel = new Label { Text = "Key Type:" , Anchor = AnchorStyles.Left , AutoSize = true};
            keyTypeCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList , Anchor = AnchorStyles.Left | AnchorStyles.Right };
            keyTypeCombo.Items.AddRange(new string[] { "String", "DWord", "QWord", "Binary", "MultiString", "ExpandString" });
            keyTypeCombo.SelectedIndex = 0;

            // Add controls to the TableLayoutPanel
            tableLayoutPanel.Controls.Add(pathLabel, 0, 0);
            tableLayoutPanel.Controls.Add(registryPathBox, 1, 0);

            tableLayoutPanel.Controls.Add(nameLabel, 0, 1);
            tableLayoutPanel.Controls.Add(keyNameBox, 1, 1);

            tableLayoutPanel.Controls.Add(valueLabel, 0, 2);
            tableLayoutPanel.Controls.Add(keyValueBox, 1, 2);

            tableLayoutPanel.Controls.Add(typeLabel, 0, 3);
            tableLayoutPanel.Controls.Add(keyTypeCombo, 1, 3);

            // Create and configure the run script button
            runScriptButton = new Button { Text = "Run Script" , Anchor = AnchorStyles.Right, AutoSize = true};
            tableLayoutPanel.Controls.Add(runScriptButton, 1, 4);
            runScriptButton.Click += RunScriptButton_Click;

            // Add the panel to the form
            this.Controls.Add(tableLayoutPanel);
        }

        // This function is called when the user clicks the "Run Script" button
        private void RunScriptButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the script path relative to the application directory
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Set-RegistryKey.ps1");

                // Escape special characters in the parameters (e.g. " -> `")
                string escapedPath = registryPathBox.Text.Replace("\"", "`\"");
                string escapedName = keyNameBox.Text.Replace("\"", "`\"");
                string escapedValue = keyValueBox.Text.Replace("\"", "`\"");

                // Build the PowerShell arguments with parameters
                string arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\" " +
                                 $"-RegistryPath \"{escapedPath}\" " +
                                 $"-KeyName \"{escapedName}\" " +
                                 $"-KeyValue \"{escapedValue}\" " +
                                 $"-KeyType \"{keyTypeCombo.SelectedItem}\"";

                // Create a new ProcessStartInfo object (ProcessStartInfo is a class that contains information about a process)
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = arguments,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };
                // Run the powershell registry modification script
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
```
