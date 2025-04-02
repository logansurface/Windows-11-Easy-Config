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

            runScriptButton = new Button
            {
                Text = "Run Script",
                Dock = DockStyle.Top
            };
            runScriptButton.Click += RunScriptButton_Click;

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
                Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                UseShellExecute = false
            };
            Process.Start(psi);
        }
    }
}