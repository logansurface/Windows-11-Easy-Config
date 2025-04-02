# Get and display the current process list
Get-Process | Format-Table Id, ProcessName, CPU, WorkingSet -AutoSize

# Wait for user input before closing
Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")