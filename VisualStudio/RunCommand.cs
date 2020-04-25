using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using vswhere;

namespace VisualStudio
{
    class RunCommand : Command
    {
        static readonly ToolSettings settings = new ToolSettings(ThisAssembly.Metadata.AssemblyName);

        readonly RunCommandDescriptor descriptor;

        public RunCommand(RunCommandDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var devenv = settings.Get("devenv");
            if (!string.IsNullOrEmpty(devenv))
            {
                if (File.Exists(devenv) && !descriptor.Arguments.Any())
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
            if (descriptor.SetDefault == false)
                settings.Set<string>("devenv", null);

            var whereArgs = descriptor.WorkloadsArguments.ToList();
            if (descriptor.Sku != null)
            {
                whereArgs.Add("-products");
                whereArgs.Add("Microsoft.VisualStudio.Product." + descriptor.Sku);
            }

            // We must include prerelease also when a specific ID was provided.
            if (descriptor.Preview || descriptor.Dogfood || !string.IsNullOrEmpty(descriptor.Id))
                whereArgs.Add("-prerelease");

            whereArgs.Add("-format");
            whereArgs.Add("json");

            var where = new WhereCommand(new WhereCommandDescriptor() { Arguments = whereArgs, ExtraArguments = whereArgs }) { Quiet = true };
            await where.ExecuteAsync(output);

            IEnumerable<VisualStudioInstance> instances = where.Instances.OrderByDescending(i => i.Catalog.BuildVersion);

            if (!string.IsNullOrEmpty(descriptor.Id))
            {
                // Providing an ID overrides all other filters
                instances = instances.Where(i => i.InstanceId.Equals(descriptor.Id, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                if (descriptor.Preview && descriptor.Dogfood)
                    instances = instances.Where(i => i.ChannelId.EndsWith("Preview", StringComparison.OrdinalIgnoreCase));
                else if (descriptor.Preview)
                    instances = instances.Where(i => i.ChannelId.EndsWith(".Preview", StringComparison.OrdinalIgnoreCase));
                else if (descriptor.Dogfood)
                    instances = instances.Where(i => i.ChannelId.EndsWith(".IntPreview", StringComparison.OrdinalIgnoreCase));

                if (descriptor.Version != null)
                    instances = instances.Where(i => i.Catalog.ProductSemanticVersion.StartsWith(descriptor.Version));
            }

            var matches = instances.ToArray();
            if (matches.Length == 1 || (matches.Length > 0 && descriptor.First))
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
                output.WriteLine("Multiple instances found. Select the one to run:");
                for (var i = 0; i < matches.Length; i++)
                {
                    output.WriteLine($"{i}: {matches[i].DisplayName} - Version {matches[i].Catalog.ProductDisplayVersion}");
                }

                if (int.TryParse(Console.ReadLine(), out var index) &&
                    index < matches.Length)
                {
                    devenv = matches[index].ProductPath;
                }
            }

            var psi = new ProcessStartInfo(devenv);
            foreach (var arg in descriptor.ExtraArguments)
            {
                psi.ArgumentList.Add(arg);
            }

            if (descriptor.Exp)
            {
                psi.ArgumentList.Add("/rootSuffix");
                psi.ArgumentList.Add("Exp");
            }

            psi.Log(output);
            var process = Process.Start(psi);
            if (descriptor.Wait)
                process.WaitForExit();

            // Explicitly specified to set a new default.
            if (descriptor.SetDefault == true)
                settings.Set("devenv", devenv);
        }
    }
}
