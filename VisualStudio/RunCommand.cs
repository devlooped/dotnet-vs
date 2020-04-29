using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using vswhere;

namespace VisualStudio
{
    class RunCommand : Command<RunCommandDescriptor>
    {
        static readonly ToolSettings settings = new ToolSettings(ThisAssembly.Metadata.AssemblyName);
        
        readonly WhereService whereService;

        public RunCommand(RunCommandDescriptor descriptor, WhereService whereService) : base(descriptor)
        {
            this.whereService = whereService;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var devenv = settings.Get("devenv");
            if (!string.IsNullOrEmpty(devenv))
            {

                if (File.Exists(devenv) && Descriptor.EmptyArguments)
                {
                    Process.Start(devenv);
                }
                else if (!File.Exists(devenv))
                {
                    // May have been uninstalled, remove the setting.
                    settings.Set<string>("devenv", null);
                }
            }

            // Explicitly specified to remove existing default.
            if (Descriptor.SetDefault == false)
                settings.Set<string>("devenv", null);

            var whereArgs = Descriptor.WorkloadsArguments.ToList();

            IEnumerable<VisualStudioInstance> instances = (await whereService
                .GetAllInstancesAsync(await Descriptor.GetPredicateAsync(), extraArguments: Descriptor.WorkloadsArguments))
                .OrderByDescending(i => i.Catalog.BuildVersion);

            if (!string.IsNullOrEmpty(Descriptor.Id))
            {
                // Providing an ID overrides all other filters
                instances = instances.Where(i => i.InstanceId.Equals(Descriptor.Id, StringComparison.OrdinalIgnoreCase));
            }
            else if (Descriptor.Version != null)
            {
                instances = instances.Where(i => i.Catalog.ProductSemanticVersion.StartsWith(Descriptor.Version));
            }

            var matches = instances.ToArray();
            if (matches.Length == 1 || (matches.Length > 0 && Descriptor.First))
            {
                devenv = matches[0].ProductPath;
            }
            else if (matches.Length == 0)
            {
                output.WriteLine("No installed Visual Studio found with the requested filters.");
                return;
            }
            else
            {
                // More than one match but no --first, we need to ask which one to run
                var instance = new Chooser().Choose(instances, output);
                if (instance == null)
                    return;

                devenv = instance.ProductPath;
            }

            var psi = new ProcessStartInfo(devenv);
            foreach (var arg in Descriptor.ExtraArguments)
            {
                psi.ArgumentList.Add(arg);
            }

            if (Descriptor.IsExperimental)
            {
                psi.ArgumentList.Add("/rootSuffix");
                psi.ArgumentList.Add("Exp");
            }

            psi.Log(output);
            var process = Process.Start(psi);

            // Explicitly specified to set a new default.
            if (Descriptor.SetDefault == true)
                settings.Set("devenv", devenv);

            if (Descriptor.Wait)
                process.WaitForExit();
        }
    }
}
