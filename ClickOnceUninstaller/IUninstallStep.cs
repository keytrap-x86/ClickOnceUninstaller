using System;
using System.Collections.Generic;

namespace CodeArtEng.ClickOnceUninstaller
{
    internal interface IUninstallStep : IDisposable
    {
        void Prepare(List<string> componentsToRemove);

        void PrintDebugInformation();

        void Execute();
    }
}
