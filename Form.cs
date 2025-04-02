using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MyProject
{
    public class MainForm : Form
    {
        private Button runScriptButton;

        public MainForm()
        {
            runScriptButton = new Button
            {
                Text = "Run PowerShell Script",
                Dock = DockStyle.Top
            };
            runScriptButton.Click += RunScriptButton_Click;

            Controls.Add(runScriptButton);
            Text = "PowerShell Runner";
        }

        private void RunScriptButton_Click(object sender, EventArgs e)
        {
            // Adjust the script path as needed
            string scriptPath = @"C:\Scripts\MyScript.ps1";

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