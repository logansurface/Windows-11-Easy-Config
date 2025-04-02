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
        private const int FORM_WIDTH = 800;
        private const int BUTTON_HEIGHT = 80;
        private const int LABEL_WIDTH = 100;
        private const int CONTROL_SPACING = 10;

        public MainForm()
        {
            // Calculate total height needed for all controls
            int totalHeight = VERTICAL_PADDING + // Top padding
                            (CONTROL_HEIGHT + VERTICAL_SPACING) * 4 + // 4 rows of controls with spacing
                            BUTTON_HEIGHT + // Run button height
                            VERTICAL_PADDING; // Bottom padding

            // Set fixed window size and disable resizing
            this.Size = new System.Drawing.Size(FORM_WIDTH, totalHeight);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create main panel with scrolling
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false, // Disable auto-scroll since we're controlling the size
                //Padding = new Padding(HORIZONTAL_PADDING, VERTICAL_PADDING, HORIZONTAL_PADDING, VERTICAL_PADDING)
            };

            // Calculate positions using constants
            int currentY = VERTICAL_PADDING;

            // Create labels and input fields
            pathLabel = new Label
            {
                Text = "Registry Path:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(LABEL_WIDTH, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            registryPathBox = new TextBox
            {
                Location = new System.Drawing.Point(LABEL_WIDTH + CONTROL_SPACING, currentY),
                Size = new System.Drawing.Size(FORM_WIDTH - (LABEL_WIDTH + CONTROL_SPACING + HORIZONTAL_PADDING * 2), CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Multiline = false
            };

            currentY += CONTROL_HEIGHT + VERTICAL_SPACING;

            nameLabel = new Label
            {
                Text = "Key Name:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(LABEL_WIDTH, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            keyNameBox = new TextBox
            {
                Location = new System.Drawing.Point(LABEL_WIDTH + CONTROL_SPACING, currentY),
                Size = new System.Drawing.Size(FORM_WIDTH - (LABEL_WIDTH + CONTROL_SPACING + HORIZONTAL_PADDING * 2), CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Multiline = false
            };

            currentY += CONTROL_HEIGHT + VERTICAL_SPACING;

            valueLabel = new Label
            {
                Text = "Key Value:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(LABEL_WIDTH, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            keyValueBox = new TextBox
            {
                Location = new System.Drawing.Point(LABEL_WIDTH + CONTROL_SPACING, currentY),
                Size = new System.Drawing.Size(FORM_WIDTH - (LABEL_WIDTH + CONTROL_SPACING + HORIZONTAL_PADDING * 2), CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Multiline = false
            };

            currentY += CONTROL_HEIGHT + VERTICAL_SPACING;

            typeLabel = new Label
            {
                Text = "Key Type:",
                Location = new System.Drawing.Point(0, currentY),
                Size = new System.Drawing.Size(LABEL_WIDTH, CONTROL_HEIGHT),
                Anchor = AnchorStyles.Left
            };

            keyTypeCombo = new ComboBox
            {
                Location = new System.Drawing.Point(LABEL_WIDTH + CONTROL_SPACING, currentY),
                Size = new System.Drawing.Size(FORM_WIDTH - (LABEL_WIDTH + CONTROL_SPACING + HORIZONTAL_PADDING * 2), CONTROL_HEIGHT),
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
                Size = new System.Drawing.Size(150, BUTTON_HEIGHT),
                Location = new System.Drawing.Point((FORM_WIDTH - 150) / 2, currentY),
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

            // Escape special characters in the parameters
            string escapedPath = registryPathBox.Text.Replace("\"", "`\"");
            string escapedName = keyNameBox.Text.Replace("\"", "`\"");
            string escapedValue = keyValueBox.Text.Replace("\"", "`\"");

            // Build the PowerShell arguments with parameters
            string arguments = $"-NoExit -ExecutionPolicy Bypass -File \"{scriptPath}\" " +
                             $"-RegistryPath \"{escapedPath}\" " +
                             $"-KeyName \"{escapedName}\" " +
                             $"-KeyValue \"{escapedValue}\" " +
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