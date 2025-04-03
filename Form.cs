using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

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
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyScript.ps1");

                // Escape special characters in the parameters (e.g. " -> `")
                string escapedPath = registryPathBox.Text.Replace("\"", "`\"");
                string escapedName = keyNameBox.Text.Replace("\"", "`\"");
                string escapedValue = keyValueBox.Text.Replace("\"", "`\"");

                // Build the PowerShell arguments with parameters
                string arguments = $"-NoExit -ExecutionPolicy Bypass -File \"{scriptPath}\" " +
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
                    RedirectStandardError = false,
                    CreateNoWindow = false
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