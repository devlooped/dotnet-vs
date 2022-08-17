using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using vswhere;

namespace Devlooped
{
    class Chooser
    {
        readonly string verb;
        readonly Func<string> inputProvider;

        public Chooser(string verb = "run") : this(verb, () => Console.ReadLine()) { }

        protected Chooser(string verb, Func<string> inputProvider)
        {
            this.verb = verb;
            this.inputProvider = inputProvider;
        }

        public T Choose<T>(IEnumerable<T> instances, TextWriter output) =>
            ChooseCore(instances, output, false).FirstOrDefault();

        public IEnumerable<T> ChooseMany<T>(IEnumerable<T> instances, TextWriter output) =>
            ChooseCore(instances, output);

        IEnumerable<T> ChooseCore<T>(IEnumerable<T> instances, TextWriter output, bool chooseMany = true)
        {
            var instancesAsList = instances.ToList();

            if (instancesAsList.Count > 1)
            {
                output.WriteLine($"Multiple instances found. Select the one to {verb}:");
                for (int i = 0; i < instancesAsList.Count; i++)
                    output.WriteLine($"{i + 1}: {GetItemDescription(instancesAsList[i])}");

                if (chooseMany)
                    output.WriteLine($"You can also {verb} [a|all] or skip one instance with !x");

                if (inputProvider() is string input)
                {
                    var inverseSelection = false;

                    if ("a".Equals(input, StringComparison.OrdinalIgnoreCase) || "all".Equals(input, StringComparison.OrdinalIgnoreCase))
                    {
                        return instancesAsList;
                    }
                    else if (input.StartsWith("!"))
                    {
                        input = input.Substring(1);
                        inverseSelection = true;
                    }

                    if (int.TryParse(input, out var index) && --index >= 0 && index < instancesAsList.Count)
                        return inverseSelection ? instancesAsList.Where((x, i) => i != index) : new[] { instancesAsList[index] };
                }

                instancesAsList.Clear();
            }

            return instancesAsList;
        }

        string GetItemDescription(object value)
        {
            if (value is VisualStudioInstance visualStudioInstance)
                return $"{visualStudioInstance.DisplayName} - Version {visualStudioInstance.Catalog.ProductDisplayVersion}";
            else if (value is Process process)
                return $"{process.MainWindowTitle} ({process.Id})";

            return value.ToString();
        }
    }
}
