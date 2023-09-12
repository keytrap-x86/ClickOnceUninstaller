using System;
using System.Collections.Generic;
using System.Linq;

namespace Wunder.ClickOnceUninstaller
{
    public class Uninstaller
    {
        private readonly ClickOnceRegistry _registry;

        public Uninstaller()
            : this(new ClickOnceRegistry())
        {
        }

        public Uninstaller(ClickOnceRegistry registry)
        {
            _registry = registry;
        }

        public void Uninstall(UninstallInfo uninstallInfo)
        {
            List<string> toRemove = FindComponentsToRemove(uninstallInfo.GetPublicKeyToken());

            Console.WriteLine("Components to remove:");
            toRemove.ForEach(Console.WriteLine);
            Console.WriteLine();

            List<IUninstallStep> steps = new List<IUninstallStep>
                            {
                                new RemoveFiles(),
                                new RemoveStartMenuEntry(uninstallInfo),
                                new RemoveRegistryKeys(_registry, uninstallInfo),
                                new RemoveUninstallEntry(uninstallInfo)
                            };

            steps.ForEach(s => s.Prepare(toRemove));
            steps.ForEach(s => s.PrintDebugInformation());
            steps.ForEach(s => s.Execute());

            steps.ForEach(s => s.Dispose());
        }

        private List<string> FindComponentsToRemove(string token)
        {
            List<ClickOnceRegistry.Component> components = _registry.Components.Where(c => c.Key.Contains(token)).ToList();

            List<string> toRemove = new List<string>();
            foreach (ClickOnceRegistry.Component component in components)
            {
                toRemove.Add(component.Key);

                foreach (string dependency in component.Dependencies)
                {
                    if (toRemove.Contains(dependency)) continue; // already in the list
                    if (_registry.Components.All(c => c.Key != dependency)) continue; // not a public component

                    ClickOnceRegistry.Mark mark = _registry.Marks.FirstOrDefault(m => m.Key == dependency);
                    if (mark != null && mark.Implications.Any(i => components.All(c => c.Key != i.Name)))
                    {
                        // don't remove because other apps depend on this
                        continue;
                    }

                    toRemove.Add(dependency);
                }
            }

            return toRemove;
        }
    }
}
