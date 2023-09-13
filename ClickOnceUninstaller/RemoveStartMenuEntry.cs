using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeArtEng.ClickOnceUninstaller
{
    internal class RemoveStartMenuEntry : IUninstallStep
    {
        private readonly UninstallInfo _uninstallInfo;
        private List<string> _foldersToRemove;
        private List<string> _filesToRemove;

        public RemoveStartMenuEntry(UninstallInfo uninstallInfo)
        {
            _uninstallInfo = uninstallInfo;
        }

        public void Prepare(List<string> componentsToRemove)
        {
            string programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string folder = Path.Combine(programsFolder, _uninstallInfo.ShortcutFolderName);
            string suiteFolder = Path.Combine(folder, _uninstallInfo.ShortcutSuiteName); //Optional
            string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string desktopShortcut = Path.Combine(desktopFolder, _uninstallInfo.ShortcutFileName + ".appref-ms");

            string startMenuFolder = string.IsNullOrEmpty(suiteFolder) ? folder : suiteFolder;

            string shortcut = Path.Combine(startMenuFolder, _uninstallInfo.ShortcutFileName + ".appref-ms");
            string supportShortcut = Path.Combine(startMenuFolder, _uninstallInfo.SupportShortcutFileName + ".url");

            _filesToRemove = new List<string>();
            if (File.Exists(desktopShortcut)) _filesToRemove.Add(desktopShortcut);
            if (File.Exists(shortcut)) _filesToRemove.Add(shortcut);
            if (File.Exists(supportShortcut)) _filesToRemove.Add(supportShortcut);

            _foldersToRemove = new List<string>();

            if (startMenuFolder == suiteFolder) _foldersToRemove.Add(suiteFolder);
            if (Directory.GetDirectories(folder).Count() == 1 && !Directory.GetFiles(folder).Any())
                _foldersToRemove.Add(folder);
        }

        public void PrintDebugInformation()
        {
            Trace.WriteLine("[RemoveStartMenuEntry]");
            if (_foldersToRemove == null)
                throw new InvalidOperationException("Call Prepare() first.");

            string programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            Trace.WriteLine("Remove start menu entries from " + programsFolder);

            if (_filesToRemove != null)
            {
                foreach (string file in _filesToRemove)
                {
                    Trace.WriteLine("Delete file " + file);
                }
            }

            if (_foldersToRemove != null)
            {
                foreach (string folder in _foldersToRemove)
                {
                    Trace.WriteLine("Delete folder " + folder);
                }
            }
        }

        public void Execute()
        {
            if (_foldersToRemove == null)
                throw new InvalidOperationException("Call Prepare() first.");

            try
            {
                if (_filesToRemove != null)
                {
                    foreach (string file in _filesToRemove)
                    {
                        File.Delete(file);
                    }
                }

                if (_foldersToRemove != null)
                {
                    foreach (string folder in _foldersToRemove)
                    {
                        Directory.Delete(folder, false);
                    }
                }
            }
            catch (IOException)
            {
            }
        }

        public void Dispose()
        {
        }
    }
}
