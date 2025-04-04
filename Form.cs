using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

// TODO: Add button array with pre-defined registry paths and keys
// beneath the manual registry modification section.
// Surround manual registry modification section with a groupbox
// TODO: Add a "Restore Default" button
// TODO: Regex path validation
// TODO: Add a manifest file to the project to allow running the application as administrator

namespace EasyConfig
{
    public class MainForm : Form
    {
        private TableLayoutPanel tableLayoutPanel;
        private TextBox registryPathBox;
        private TextBox keyNameBox;
        private TextBox keyValueBox;
        private ComboBox keyTypeCombo;
        private Label pathLabel;
        private Label nameLabel;
        private Label valueLabel;
        private Label typeLabel;
        private Button submitChangeButton;
        private Button restoreDefaultButton;
        private Button taskbarLeftButton;
        private Button widgetToggleButton;
        private Button disableSearchButton;
        private const int CONTROL_ROWS = 6; // Number of input rows
        private const int CONTROL_HEIGHT = 250;
        private const int VERTICAL_SPACING = 0;
        private const int HORIZONTAL_PADDING = 10;
        private const int VERTICAL_PADDING = 0;
        private const int FORM_WIDTH = 1000;
        private const int FORM_HEIGHT = 400;
        private const int BUTTON_HEIGHT = 250;
        private const int LABEL_WIDTH = FORM_WIDTH / 3;
        private const int CONTROL_SPACING = 0;
        private const string[] predefinedPaths = {
            "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
            "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
            "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
            "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
        };

        public MainForm()
        {
            // Set fixed window size and disable resizing
            this.Size = new Size(FORM_WIDTH, FORM_HEIGHT);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Easy Config (Windows 11)";

            // Create TableLayoutPanel
            tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = CONTROL_ROWS + 1, // Add one row for the submit changes button
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
            submitChangeButton = new Button { Text = "Submit Change" , Anchor = AnchorStyles.Right, AutoSize = true};
            tableLayoutPanel.Controls.Add(submitChangeButton, 1, 4);
            submitChangeButton.Click += SubmitChangeButton_Click;

            // Predefined button array with registry paths and keys
            

            // Add the panel to the form
            this.Controls.Add(tableLayoutPanel);
        }

        // This function is called when the user clicks the "Submit Change" button
        private void SubmitChangeButton_Click(object sender, EventArgs e)
        {
            // Get values from UI controls
            string regPath = registryPathBox.Text;
            string valueName = keyNameBox.Text;
            string keyValue = keyValueBox.Text;
            string keyType = keyTypeCombo.SelectedItem?.ToString();

            // Basic validation (add more specific validation as needed)
            if (string.IsNullOrWhiteSpace(regPath) || string.IsNullOrWhiteSpace(keyType))
            {
                MessageBox.Show("Registry Path and Key Type cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Call the C# registry function
            bool success = RegistryUtils.SetRegistryKey(regPath, valueName, keyValue, keyType);

            // Provide feedback to the user
            if (success) {
                MessageBox.Show("Registry value set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                // The error details are printed to the Debug Output window (View -> Output)
                MessageBox.Show("Failed to set registry value.\nCheck application logs or Debug Output for details.\nEnsure you have the necessary permissions (run as Administrator if needed).",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}