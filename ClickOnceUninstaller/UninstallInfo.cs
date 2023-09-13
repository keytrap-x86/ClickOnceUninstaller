﻿using System;
using System.Linq;
using Microsoft.Win32;

namespace CodeArtEng.ClickOnceUninstaller
{
    internal class UninstallInfo
    {
        public const string UninstallRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

        private UninstallInfo()
        {
        }

        public static UninstallInfo Find(string appName)
        {
            RegistryKey uninstall = Registry.CurrentUser.OpenSubKey(UninstallRegistryPath);
            if (uninstall != null)
            {
                foreach (string app in uninstall.GetSubKeyNames())
                {
                    RegistryKey sub = uninstall.OpenSubKey(app);
                    if (sub != null && sub.GetValue("DisplayName") as string == appName)
                    {
                        return new UninstallInfo
                        {
                            Key = app,
                            UninstallString = sub.GetValue("UninstallString") as string,
                            ShortcutFolderName = sub.GetValue("ShortcutFolderName") as string,
                            ShortcutSuiteName = sub.GetValue("ShortcutSuiteName") as string,
                            ShortcutFileName = sub.GetValue("ShortcutFileName") as string,
                            SupportShortcutFileName = sub.GetValue("SupportShortcutFileName") as string
                        };
                    }
                }
            }

            return null;
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
    }
}
