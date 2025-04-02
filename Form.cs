using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace EasyConfig
{
    public class MainForm : Form
    {
        private Button runScriptButton;

        public MainForm()
        {
            // Set fixed window size and disable resizing
            this.Size = new System.Drawing.Size(800, 350);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create and configure the run script button
            runScriptButton = new Button
            {
                Text = "Run Script",
                Size = new System.Drawing.Size(150, 80),
                Location = new System.Drawing.Point(300, 100),
                Anchor = AnchorStyles.None
            };
            // Add click event handler to the run script button
            runScriptButton.Click += RunScriptButton_Click;

            // Add the button and text to the form
            Controls.Add(runScriptButton);
            Text = "Windows 11 Easy Config";
        }

        private void RunScriptButton_Click(object sender, EventArgs e)
        {
            // Get the script path relative to the application directory
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyScript.ps1");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoExit -ExecutionPolicy Bypass \"{scriptPath}\"",
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = false
            };

            Process.Start(psi);
        }
    }
}