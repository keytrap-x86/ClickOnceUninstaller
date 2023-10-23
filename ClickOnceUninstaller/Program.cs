using System;

namespace CodeArtEng.ClickOnceUninstaller
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Usage:\nClickOnceUninstaller appName (can be regex)");
                return;
            }

            var appName = args[0];


            Console.WriteLine("Uninstalling application \"{0}\"", appName);
            var uninstaller = new Uninstaller();
            uninstaller.Uninstall(appName);

            Console.WriteLine("Uninstall complete");
        }
    }
}
