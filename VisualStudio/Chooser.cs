using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using vswhere;
using System.Diagnostics;

namespace VisualStudio
{
    class Chooser
    {
        readonly string title;

        public Chooser(string verb = "run") => title = $"Multiple instances found. Select the one to {verb}:";

        public T Choose<T>(IEnumerable<T> instances, TextWriter output)
        {
            var instancesAsList = instances.ToList();

            if (instancesAsList.Count > 1)
            {
                output.WriteLine(title);
                for (int i = 0; i < instancesAsList.Count; i++)
                    output.WriteLine($"{i + 1}: {GetItemDescription(instancesAsList[i])}");

                if (int.TryParse(Console.ReadLine(), out var index) && index > 0 && index <= instancesAsList.Count)
                    return instancesAsList[index - 1];

                return default;
            }

            return instances.FirstOrDefault();
        }

        string GetItemDescription(object value)
        {
            if (value is VisualStudioInstance visualStudioInstance)
                return $"{ visualStudioInstance.DisplayName} - Version { visualStudioInstance.Catalog.ProductDisplayVersion}";
            else if (value is Process process)
                return $"{process.MainWindowTitle} ({process.Id})";

            return value.ToString();
        }
    }
}
