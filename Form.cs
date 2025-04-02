using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace EasyConfig
{
    public class MainForm : Form
    {
        private Button runScriptButton;
        private TextBox registryPathBox;
        private TextBox keyNameBox;
        private TextBox keyValueBox;
        private ComboBox keyTypeCombo;
        private Label pathLabel;
        private Label nameLabel;
        private Label valueLabel;
        private Label typeLabel;

        public MainForm()
        {
            // Set fixed window size and disable resizing
            this.Size = new System.Drawing.Size(800, 450);  // Increased form height
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create labels
            pathLabel = new Label
            {
                Text = "Registry Path:",
                Location = new System.Drawing.Point(50, 30),
                Size = new System.Drawing.Size(100, 80),
                Anchor = AnchorStyles.None
            };

            nameLabel = new Label
            {
                Text = "Key Name:",
                Location = new System.Drawing.Point(50, 130),
                Size = new System.Drawing.Size(100, 80),
                Anchor = AnchorStyles.None
            };

            valueLabel = new Label
            {
                Text = "Key Value:",
                Location = new System.Drawing.Point(50, 230),
                Size = new System.Drawing.Size(100, 80),
                Anchor = AnchorStyles.None
            };

            typeLabel = new Label
            {
                Text = "Key Type:",
                Location = new System.Drawing.Point(50, 330),
                Size = new System.Drawing.Size(100, 80),
                Anchor = AnchorStyles.None
            };

            // Create input fields
            registryPathBox = new TextBox
            {
                Location = new System.Drawing.Point(160, 30),
                Size = new System.Drawing.Size(580, 80),
                Anchor = AnchorStyles.None,
                Multiline = true
            };

            keyNameBox = new TextBox
            {
                Location = new System.Drawing.Point(160, 130),
                Size = new System.Drawing.Size(580, 80),
                Anchor = AnchorStyles.None,
                Multiline = true
            };

            keyValueBox = new TextBox
            {
                Location = new System.Drawing.Point(160, 230),
                Size = new System.Drawing.Size(580, 80),
                Anchor = AnchorStyles.None,
                Multiline = true
            };

            keyTypeCombo = new ComboBox
            {
                Location = new System.Drawing.Point(160, 330),
                Size = new System.Drawing.Size(580, 80),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.None
            };
            keyTypeCombo.Items.AddRange(new string[] { "String", "DWord", "QWord", "Binary", "MultiString", "ExpandString" });
            keyTypeCombo.SelectedIndex = 0;

            // Create and configure the run script button
            runScriptButton = new Button
            {
                Text = "Run Script",
                Size = new System.Drawing.Size(150, 40),
                Location = new System.Drawing.Point(325, 380),
                Anchor = AnchorStyles.None
            };
            runScriptButton.Click += RunScriptButton_Click;

            // Add all controls to the form
            Controls.AddRange(new Control[] {
                pathLabel, nameLabel, valueLabel, typeLabel,
                registryPathBox, keyNameBox, keyValueBox, keyTypeCombo,
                runScriptButton
            });

            Text = "Windows 11 Easy Config";
        }

        private void RunScriptButton_Click(object sender, EventArgs e)
        {
            // Get the script path relative to the application directory
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyScript.ps1");

            // Build the PowerShell arguments with parameters
            string arguments = $"-NoExit -ExecutionPolicy Bypass -File \"{scriptPath}\" " +
                             $"-RegistryPath \"{registryPathBox.Text}\" " +
                             $"-KeyName \"{keyNameBox.Text}\" " +
                             $"-KeyValue \"{keyValueBox.Text}\" " +
                             $"-KeyType \"{keyTypeCombo.SelectedItem}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = arguments,
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = false
            };

            Process.Start(psi);
        }
    }
}