using System;

namespace Wunder.ClickOnceUninstaller
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Usage:\nClickOnceUninstaller appName");
                return;
            }

            string appName = args[0];

            UninstallInfo uninstallInfo = UninstallInfo.Find(appName);
            if (uninstallInfo == null)
            {
                Console.WriteLine("Could not find application \"{0}\"", appName);
                return;
            }

            Console.WriteLine("Uninstalling application \"{0}\"", appName);
            Uninstaller uninstaller = new Uninstaller();
            uninstaller.Uninstall(uninstallInfo);

            Console.WriteLine("Uninstall complete");
        }
    }
}
