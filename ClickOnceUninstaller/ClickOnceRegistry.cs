using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CodeArtEng.ClickOnceUninstaller
{
    internal class ClickOnceRegistry
    {
        public const string ComponentsRegistryPath = @"Software\Classes\Software\Microsoft\Windows\CurrentVersion\Deployment\SideBySide\2.0\Components";
        public const string MarksRegistryPath = @"Software\Classes\Software\Microsoft\Windows\CurrentVersion\Deployment\SideBySide\2.0\Marks";
        
        public ClickOnceRegistry()
        {
            ReadComponents();
            ReadMarks();
        }

        private void ReadComponents()
        {
            Components = new List<Component>();

            Microsoft.Win32.RegistryKey components = Registry.CurrentUser.OpenSubKey(ComponentsRegistryPath);
            if (components == null) return;

            foreach (string keyName in components.GetSubKeyNames())
            {
                Microsoft.Win32.RegistryKey componentKey = components.OpenSubKey(keyName);
                if (componentKey == null) continue;

                Component component = new Component { Key = keyName };
                Components.Add(component);

                component.Dependencies = new List<string>();
                foreach (string dependencyName in componentKey.GetSubKeyNames().Where(n => n != "Files"))
                {
                    component.Dependencies.Add(dependencyName);
                }
            }
        }

        private void ReadMarks()
        {
            Marks = new List<Mark>();

            Microsoft.Win32.RegistryKey marks = Registry.CurrentUser.OpenSubKey(MarksRegistryPath);
            if (marks == null) return;

            foreach (string keyName in marks.GetSubKeyNames())
            {
                Microsoft.Win32.RegistryKey markKey = marks.OpenSubKey(keyName);
                if (markKey == null) continue;

                Mark mark = new Mark { Key = keyName };
                Marks.Add(mark);

                byte[] appid = markKey.GetValue("appid") as byte[];
                if (appid != null) mark.AppId = Encoding.ASCII.GetString(appid);

                byte[] identity = markKey.GetValue("identity") as byte[];
                if (identity != null) mark.Identity = Encoding.ASCII.GetString(identity);

                mark.Implications = new List<Implication>();
                IEnumerable<string> implications = markKey.GetValueNames().Where(n => n.StartsWith("implication"));
                foreach (string implicationName in implications)
                {
                    byte[] implication = markKey.GetValue(implicationName) as byte[];
                    if (implication != null)
                        mark.Implications.Add(new Implication
                                                  {
                                                      Key = implicationName,
                                                      Name = implicationName.Substring(12),
                                                      Value = Encoding.ASCII.GetString(implication)
                                                  });
                }
            }
        }

        public class RegistryKey
        {
            public string Key { get; set; }

            public override string ToString()
            {
                return Key ?? base.ToString();
            }
        }

        public class Component : RegistryKey
        {
            public List<string> Dependencies { get; set; }
        }

        public class Mark : RegistryKey
        {
            public string AppId { get; set; }

            public string Identity { get; set; }

            public List<Implication> Implications { get; set; }
        }

        public class Implication : RegistryKey
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        public List<Component> Components { get; set; }

        public List<Mark> Marks { get; set; }
    }
}
