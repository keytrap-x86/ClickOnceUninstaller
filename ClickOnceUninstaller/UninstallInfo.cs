using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace CodeArtEng.ClickOnceUninstaller
{
    internal class UninstallInfo
    {
        public const string UninstallRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

        private UninstallInfo()
        {
        }

        public static UninstallInfo Find(string softwareDisplayNameRegex)
        {

            UninstallInfo output = new UninstallInfo();

            const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";


            try
            {
                var baseKeyCU = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);

                SearchUninstallStrings(ref output, registryKey, baseKeyCU, softwareDisplayNameRegex);
                if (string.IsNullOrEmpty(output.UninstallString) == false)
                    return output;
            }
            catch { }


            try
            {
                //32 bits LocalMachine Registry
                var baseKey32Bits = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

                SearchUninstallStrings(ref output, registryKey, baseKey32Bits, softwareDisplayNameRegex);
                if (string.IsNullOrEmpty(output.UninstallString) == false)
                    return output;
            }
            catch { }


            try
            {
                //64 bits LocalMachine Registry
                var baseKey64Bits = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

                SearchUninstallStrings(ref output, registryKey, baseKey64Bits, softwareDisplayNameRegex);
                if (string.IsNullOrEmpty(output.UninstallString) == false)
                    return output;
            }
            catch { }

            // Return default
            return output;
        }

        public string Key { get; set; }

        public string UninstallString { get; private set; }

        public string ShortcutFolderName { get; set; }

        public string ShortcutSuiteName { get; set; }

        public string ShortcutFileName { get; set; }

        public string SupportShortcutFileName { get; set; }

        public string GetPublicKeyToken()
        {
            string token = UninstallString.Split(',').First(s => s.Trim().StartsWith("PublicKeyToken=")).Substring(16);
            if (token.Length != 16) throw new ArgumentException();
            return token;
        }

        private static void SearchUninstallStrings(ref UninstallInfo output, string registryKey, RegistryKey baseKey, string softwareDisplayNameRegex)
        {
            var subkeys = baseKey.OpenSubKey(registryKey);
            if (subkeys != null)
            {
                foreach (var subkey in subkeys.GetSubKeyNames().Select(subkeys.OpenSubKey))
                {
                    if (subkey?.GetValue("DisplayName") is string displayName && Regex.IsMatch(displayName, softwareDisplayNameRegex, RegexOptions.IgnoreCase))
                    {
                        // Prefer quiet uninstall string if available
                        string uninstallString = subkey?.GetValue("QuietUninstallString") as string ?? subkey?.GetValue("UninstallString") as string;
                        if (string.IsNullOrEmpty(uninstallString) == false)
                        {
                            output.UninstallString = uninstallString;
                            output.Key = subkey.Name?.Substring(subkey.Name.LastIndexOf('\\') + 1);
                            output.ShortcutFolderName = subkey.GetValue("ShortcutFolderName") as string;
                            output.ShortcutSuiteName = subkey.GetValue("ShortcutSuiteName") as string ?? "";
                            output.ShortcutFileName = subkey.GetValue("ShortcutFileName") as string;
                            output.SupportShortcutFileName = subkey.GetValue("SupportShortcutFileName") as string;
                        }
                        break;
                    }
                    subkey?.Close();
                }
                subkeys.Close();
            }
            baseKey.Close();
        }
    }
}
