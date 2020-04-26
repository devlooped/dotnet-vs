using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using vswhere;

namespace VisualStudio
{
    class VisualStudioInstanceChooser
    {
        public VisualStudioInstance Choose(IEnumerable<VisualStudioInstance> instances, TextWriter output)
        {
            var instancesAsList = instances.ToList();

            if (instancesAsList.Count > 1)
            {
                output.WriteLine("Multiple instances found. Select the one to run:");
                for (int i = 0; i < instancesAsList.Count; i++)
                {
                    output.WriteLine($"{i + 1}: {instancesAsList[i].DisplayName} - Version {instancesAsList[i].Catalog.ProductDisplayVersion}");
                }

                if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= instancesAsList.Count)
                    return instancesAsList[index - 1];
            }

            return instances.FirstOrDefault();
        }
    }
}
