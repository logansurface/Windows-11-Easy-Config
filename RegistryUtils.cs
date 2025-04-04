using Microsoft.Win32;
using System;
using System.ComponentModel; // Required for Win32Exception
using System.Diagnostics; // For Debug.WriteLine
using System.Globalization; // For CultureInfo.InvariantCulture
using System.IO; // For IOException
using System.Linq; // For LINQ methods on arrays
using System.Security; // For SecurityException

public static class RegistryUtils
{
    /// <summary>
    /// Sets a value in the Windows Registry. Creates the key path if it doesn't exist.
    /// </summary>
    /// <param name="registryPath">The full path of the key (e.g., "HKEY_CURRENT_USER\Software\MyApp", "HKLM\System\CurrentControlSet").</param>
    /// <param name="valueName">The name of the value to set (use null or empty string for the '(Default)' value).</param>
    /// <param name="stringValue">The value to set, represented as a string. It will be converted based on keyType.</param>
    /// <param name="keyType">The type of registry value (e.g., "String", "DWord", "Binary", "MultiString"). Case-insensitive.</param>
    /// <returns>True if the value was set successfully, false otherwise.</returns>
    /// <remarks>
    /// Requires appropriate permissions to write to the specified registry hive and key.
    /// HKLM and other system hives often require Administrator privileges.
    /// Binary data format: Comma-separated decimal byte values (e.g., "10,255,0,128").
    /// MultiString data format: Pipe-separated strings (e.g., "Value1|Value2|Another Value").
    /// </remarks>
    public static bool SetRegistryKey(string registryPath, string valueName, string stringValue, string keyType)
    {
        try
        {
            // 1. Parse the registry path to get the base hive and subkey path
            (RegistryKey baseKey, string subKeyPath) = ParseRegistryPath(registryPath);
            if (baseKey == null)
            {
                // ParseRegistryPath already logged the error
                return false;
            }

            // Ensure valueName is not null (use empty string for default value)
            valueName ??= string.Empty;

            // 2. Convert the keyType string to RegistryValueKind enum
            RegistryValueKind valueKind = ConvertStringToValueKind(keyType);

            // 3. Convert the string value to the appropriate .NET object type
            object convertedValue = ConvertValueToObject(stringValue, valueKind);

            // 4. Create or open the key path for writing
            // Use CreateSubKey - it opens if exists, creates if not. Handles nested paths.
            // Need 'using' to ensure the key handle is released.
            using (RegistryKey key = baseKey.CreateSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                if (key == null)
                {
                    // This might happen if we have permission to read but not write/create
                    // or if the path is somehow invalid despite parsing.
                    Debug.WriteLine($"Error: Could not create or open registry key '{registryPath}' for writing.");
                    // Attempting to open for read might give a clue, but CreateSubKey should handle it.
                    // Let's try opening read-only to check existence vs permission.
                    using (RegistryKey readKey = baseKey.OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadSubTree)) {
                         if (readKey == null) {
                             Debug.WriteLine($"Reason: Key path likely does not exist and creation failed (perhaps intermediate key permissions?).");
                         } else {
                             Debug.WriteLine($"Reason: Key path exists but write/create permission was denied.");
                         }
                    }
                    // Fallback error message
                    throw new SecurityException($"Failed to create or open registry key '{registryPath}' with write access.");
                }

                // 5. Set the value
                key.SetValue(valueName, convertedValue, valueKind);
                Debug.WriteLine($"Successfully set registry value: Path='{registryPath}', Name='{valueName}', Type='{keyType}', Value='{stringValue}'");
            } // RegistryKey 'key' is disposed here

            // Dispose the base key handle obtained from ParseRegistryPath
            baseKey.Dispose();

            return true;
        }
        catch (SecurityException ex)
        {
            Debug.WriteLine($"Registry Security Error: {ex.Message}. Do you need to run as Administrator?");
            // Consider showing a user-friendly message about permissions
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.WriteLine($"Registry Access Denied Error: {ex.Message}. Do you need to run as Administrator?");
            // Consider showing a user-friendly message about permissions
            return false;
        }
        catch (IOException ex)
        {
            Debug.WriteLine($"Registry IO Error: {ex.Message}. Is the path valid or too long?");
            return false;
        }
        catch (ArgumentException ex) // Catches errors from parsing path, type, or value format
        {
            Debug.WriteLine($"Registry Argument Error: {ex.Message}");
            return false;
        }
        catch (FormatException ex) // Specifically for value conversion errors
        {
            Debug.WriteLine($"Registry Value Format Error: {ex.Message}. Value '{stringValue}' could not be converted to type '{keyType}'.");
            return false;
        }
        catch (Exception ex) // Catch-all for unexpected errors
        {
            Debug.WriteLine($"An unexpected error occurred: {ex.ToString()}");
            return false;
        }
    }

    /// <summary>
    /// Parses a full registry path string into a base RegistryKey object and the subkey path string.
    /// </summary>
    private static (RegistryKey BaseKey, string SubKeyPath) ParseRegistryPath(string registryPath)
    {
        if (string.IsNullOrWhiteSpace(registryPath))
        {
            throw new ArgumentException("Registry path cannot be empty.", nameof(registryPath));
        }

        registryPath = registryPath.Trim();
        string[] parts = registryPath.Split(new[] { '\\' }, 2); // Split into max 2 parts: Hive and the rest
        string hiveName = parts[0].ToUpperInvariant();
        string subKeyPath = parts.Length > 1 ? parts[1] : string.Empty;

        RegistryKey baseKey;

        switch (hiveName)
        {
            case "HKEY_CURRENT_USER":
            case "HKCU":
            case "HKCU:":
                baseKey = Registry.CurrentUser;
                break;
            case "HKEY_LOCAL_MACHINE":
            case "HKLM":
            case "HKLM:":
                baseKey = Registry.LocalMachine;
                break;
            case "HKEY_CLASSES_ROOT":
            case "HKCR":
            case "HKCR:":
                baseKey = Registry.ClassesRoot;
                break;
            case "HKEY_USERS":
            case "HKU":
            case "HKU:":
                baseKey = Registry.Users;
                break;
            case "HKEY_CURRENT_CONFIG":
            case "HKCC":
            case "HKCC:":
                baseKey = Registry.CurrentConfig;
                break;
            default:
                // Handle case where path might not start with a known hive (like the PS script defaulted to HKLM)
                // For C#, it's safer to require an explicit hive.
                 throw new ArgumentException($"Invalid registry hive name: '{parts[0]}'. Path must start with a valid hive (e.g., HKEY_LOCAL_MACHINE or HKLM).", nameof(registryPath));
                // OR, if you want to default like PowerShell (use with caution):
                // Debug.WriteLine($"Warning: Registry path '{registryPath}' did not start with a known hive. Assuming HKEY_LOCAL_MACHINE.");
                // baseKey = Registry.LocalMachine;
                // subKeyPath = registryPath; // Use the original path as the subkey path
                // break;
        }

        // Clean up subkey path (remove leading/trailing slashes if any)
        subKeyPath = subKeyPath.Trim('\\');

        return (baseKey, subKeyPath);
    }

    /// <summary>
    /// Converts a string representation of a registry type to the RegistryValueKind enum.
    /// </summary>
    private static RegistryValueKind ConvertStringToValueKind(string keyType)
    {
        if (string.IsNullOrWhiteSpace(keyType))
        {
            throw new ArgumentException("Key type cannot be empty.", nameof(keyType));
        }

        switch (keyType.Trim().ToUpperInvariant())
        {
            case "STRING":
                return RegistryValueKind.String;
            case "EXPANDSTRING":
                return RegistryValueKind.ExpandString;
            case "BINARY":
                return RegistryValueKind.Binary;
            case "DWORD":
                return RegistryValueKind.DWord;
            case "MULTISTRING":
                return RegistryValueKind.MultiString;
            case "QWORD":
                return RegistryValueKind.QWord;
            // RegistryValueKind.Unknown and None are less common for setting directly
            default:
                throw new ArgumentException($"Unsupported registry value type: '{keyType}'. Supported types are String, ExpandString, Binary, DWord, MultiString, QWord.", nameof(keyType));
        }
    }

    /// <summary>
    /// Converts a string value to the appropriate .NET object based on the target RegistryValueKind.
    /// </summary>
    private static object ConvertValueToObject(string stringValue, RegistryValueKind valueKind)
    {
        // Allow null input string, handle based on type if necessary
        stringValue ??= string.Empty;

        try
        {
            switch (valueKind)
            {
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    return stringValue; // No conversion needed

                case RegistryValueKind.DWord: // 32-bit integer
                    // Use int.Parse, allows various styles including hex if prefixed with 0x
                    return int.Parse(stringValue.Trim(), NumberStyles.Integer | NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

                case RegistryValueKind.QWord: // 64-bit integer
                     // Use long.Parse, allows various styles including hex if prefixed with 0x
                    return long.Parse(stringValue.Trim(), NumberStyles.Integer | NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

                case RegistryValueKind.MultiString: // String array
                    // Use pipe '|' as separator, consistent with PS script example
                    return stringValue.Split(new[] { '|' }, StringSplitOptions.None); // Keep empty entries if needed

                case RegistryValueKind.Binary: // Byte array
                    // Expect comma-separated decimal byte values (e.g., "10,255,0,128")
                    string[] byteStrings = stringValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    byte[] bytes = new byte[byteStrings.Length];
                    for (int i = 0; i < byteStrings.Length; i++)
                    {
                        // Use byte.Parse, allows various styles including hex if prefixed with 0x
                        bytes[i] = byte.Parse(byteStrings[i].Trim(), NumberStyles.Integer | NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                    }
                    return bytes;

                // case RegistryValueKind.Unknown:
                // case RegistryValueKind.None: // Typically not set directly
                default:
                    // Should have been caught by ConvertStringToValueKind, but defensive check
                    throw new ArgumentException($"Cannot convert value for unsupported registry type: {valueKind}");
            }
        }
        catch (FormatException ex)
        {
            // Re-throw with more context
            throw new FormatException($"Value '{stringValue}' is not in a correct format for the specified registry type '{valueKind}'.", ex);
        }
        catch (OverflowException ex)
        {
             // Re-throw with more context
            throw new OverflowException($"Value '{stringValue}' is too large or too small for the specified registry type '{valueKind}'.", ex);
        }
    }
}