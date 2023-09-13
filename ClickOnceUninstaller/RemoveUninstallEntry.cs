using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace CodeArtEng.ClickOnceUninstaller
{
    internal class RemoveUninstallEntry : IUninstallStep
    {
        private readonly UninstallInfo _uninstallInfo;
        private RegistryKey _uninstall;

        public RemoveUninstallEntry(UninstallInfo uninstallInfo)
        {
            _uninstallInfo = uninstallInfo;
        }

        public void Prepare(List<string> componentsToRemove)
        {
            _uninstall = Registry.CurrentUser.OpenSubKey(UninstallInfo.UninstallRegistryPath, true);
        }

        public void PrintDebugInformation()
        {
            Trace.WriteLine("[RemoveUninstallEntry]");
            if (_uninstall == null)
                throw new InvalidOperationException("Call Prepare() first.");

            Trace.WriteLine("Remove uninstall info from " + _uninstall.OpenSubKey(_uninstallInfo.Key).Name);
        }

        public void Execute()
        {
            if (_uninstall == null)
                throw new InvalidOperationException("Call Prepare() first.");

            _uninstall.DeleteSubKey(_uninstallInfo.Key);
        }

        public void Dispose()
        {
            if (_uninstall != null)
            {
                _uninstall.Close();
                _uninstall = null;
            }
        }
    }
}
