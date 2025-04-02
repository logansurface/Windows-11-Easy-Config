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
        private Panel mainPanel;
        private const int CONTROL_HEIGHT = 80;
        private const int VERTICAL_SPACING = 20;
        private const int HORIZONTAL_PADDING = 20;
        private const int VERTICAL_PADDING = 20;

        public MainForm()
        {
            // Set fixed window size and disable resizing
            this.Size = new System.Drawing.Size(800, 450);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create main panel with scrolling
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(HORIZONTAL_PADDING, VERTICAL_PADDING, HORIZONTAL_PADDING, VERTICAL_PADDING)
            };

            // Calculate positions using constants
            int currentY = VERTICAL_PADDING;

            // Create labels and input fields
            pathLabel = new Label
            {
                Text = "Registry Path:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(100, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            registryPathBox = new TextBox
            {
                Location = new System.Drawing.Point(110, currentY),
                Size = new System.Drawing.Size(mainPanel.ClientSize.Width - 130, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Multiline = true
            };

            currentY += CONTROL_HEIGHT + VERTICAL_SPACING;

            nameLabel = new Label
            {
                Text = "Key Name:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(100, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            keyNameBox = new TextBox
            {
                Location = new System.Drawing.Point(110, currentY),
                Size = new System.Drawing.Size(mainPanel.ClientSize.Width - 130, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Multiline = true
            };

            currentY += CONTROL_HEIGHT + VERTICAL_SPACING;

            valueLabel = new Label
            {
                Text = "Key Value:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(100, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            keyValueBox = new TextBox
            {
                Location = new System.Drawing.Point(110, currentY),
                Size = new System.Drawing.Size(mainPanel.ClientSize.Width - 130, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Multiline = true
            };

            currentY += CONTROL_HEIGHT + VERTICAL_SPACING;

            typeLabel = new Label
            {
                Text = "Key Type:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(100, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            keyTypeCombo = new ComboBox
            {
                Location = new System.Drawing.Point(110, currentY),
                Size = new System.Drawing.Size(mainPanel.ClientSize.Width - 130, CONTROL_HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            keyTypeCombo.Items.AddRange(new string[] { "String", "DWord", "QWord", "Binary", "MultiString", "ExpandString" });
            keyTypeCombo.SelectedIndex = 0;

            currentY += CONTROL_HEIGHT + VERTICAL_SPACING;

            // Create and configure the run script button
            runScriptButton = new Button
            {
                Text = "Run Script",
                Size = new System.Drawing.Size(150, 40),
                Location = new System.Drawing.Point((mainPanel.ClientSize.Width - 150) / 2, currentY),
                Anchor = AnchorStyles.None
            };
            runScriptButton.Click += RunScriptButton_Click;

            // Add all controls to the panel
            mainPanel.Controls.AddRange(new Control[] {
                pathLabel, nameLabel, valueLabel, typeLabel,
                registryPathBox, keyNameBox, keyValueBox, keyTypeCombo,
                runScriptButton
            });

            // Add the panel to the form
            Controls.Add(mainPanel);

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