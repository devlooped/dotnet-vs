using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Mono.Options;
using vswhere;

namespace VisualStudio
{
    public class RunCommand : Command
    {
        static readonly ToolSettings settings = new ToolSettings(ThisAssembly.Metadata.AssemblyName);

        readonly OptionSet options;
        readonly WorkloadOptions workloads;
        bool help = false;
        bool preview;
        bool dogfood;
        Sku? sku;
        string version;
        bool first;
        bool wait;
        bool? setDefault;
        string id;
        bool exp;

        ImmutableArray<string> parsed = ImmutableArray.Create<string>();

        public RunCommand()
        {
            options = new OptionSet
            {
                { "pre|preview", "Run preview version", p => preview = p != null },
                { "int|internal", "Run internal (aka 'dogfood') version", d => dogfood = d != null },
                { "sku:", "Run specific edition. One of [e|ent|enterprise], [p|pro|professional] or [c|com|community].", s => sku = SkuOption.Parse(s) },
                { "id:", "Run a specific instance by its ID", i => id = i },
                { "exp", "Run with /rootSuffix Exp.", e => exp = e != null },
                { "f|first", "If more than one instance matches the criteria, run the first one sorted by descending build version.", f => first = f != null },
                { "v|version:", "Run specific (semantic) version, such as 16.4 or 16.5.3.", v => version = v },
                { "w|wait", "Wait for the started Visual Studio to exit.", w => wait = w != null },
                { "default", "Set as the default version to run when no arguments are provided, or remove the current default (with --default-).", d => setDefault = d != null },
            };
            workloads = new WorkloadOptions("-requires")
            {
                { "?|h|help", "Display this help", h => help = h != null },
            };
        }

        public override string Name => "run";

        public override async Task<int> ExecuteAsync(IEnumerable<string> args, TextWriter output)
        {
            try
            {
                var devenv = settings.Get("devenv");
                if (!string.IsNullOrEmpty(devenv))
                {
                    if (File.Exists(devenv) && !args.Any())
                    {
                        Process.Start(devenv);
                        return 0;
                    }
                    else if (!File.Exists(devenv))
                    {
                        // May have been uninstalled, remove the setting.
                        settings.Set<string>("devenv", null);
                    }
                }

                var extra = workloads.Parse(options.Parse(args));
                if (help)
                {
                    ShowUsage(output);
                    return 0;
                }

                // Explicitly specified to remove existing default.
                if (setDefault == false)
                    settings.Set<string>("devenv", null);

                var whereArgs = workloads.Arguments.ToList();
                if (sku != null)
                {
                    whereArgs.Add("-products");
                    whereArgs.Add("Microsoft.VisualStudio.Product." + sku);
                }

                // We must include prerelease also when a specific ID was provided.
                if (preview || dogfood || !string.IsNullOrEmpty(id))
                    whereArgs.Add("-prerelease");

                whereArgs.Add("-format");
                whereArgs.Add("json");

                var where = new WhereCommand { Quiet = true };
                var result = await where.ExecuteAsync(whereArgs, output);
                if (result != 0)
                    return result;

                IEnumerable<VisualStudioInstance> instances = where.Instances.OrderByDescending(i => i.Catalog.BuildVersion);

                if (!string.IsNullOrEmpty(id))
                {
                    // Providing an ID overrides all other filters
                    instances = instances.Where(i => i.InstanceId.Equals(id, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    if (preview && dogfood)
                        instances = instances.Where(i => i.ChannelId.EndsWith("Preview", StringComparison.OrdinalIgnoreCase));
                    else if (preview)
                        instances = instances.Where(i => i.ChannelId.EndsWith(".Preview", StringComparison.OrdinalIgnoreCase));
                    else if (dogfood)
                        instances = instances.Where(i => i.ChannelId.EndsWith(".IntPreview", StringComparison.OrdinalIgnoreCase));

                    if (version != null)
                        instances = instances.Where(i => i.Catalog.ProductSemanticVersion.StartsWith(version));
                }

                var matches = instances.ToArray();
                if (matches.Length == 1 || (matches.Length > 0 && first))
                {
                    devenv = matches[0].ProductPath;
                }
                else if (matches.Length == 0)
                {
                    output.WriteLine("No installed Visual Studio found with the requested filters.");
                    return -1;
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
                foreach (var arg in extra)
                {
                    psi.ArgumentList.Add(arg);
                }
                
                if (exp)
                {
                    psi.ArgumentList.Add("/rootSufix");
                    psi.ArgumentList.Add("Exp");
                }

                psi.Log(output);
                var process = Process.Start(psi);
                if (wait)
                    process.WaitForExit();

                // Explicitly specified to set a new default.
                if (setDefault == true)
                    settings.Set("devenv", devenv);

                return 0;
            }
            catch (OptionException e)
            {
                output.WriteLine(e.Message);
                ShowUsage(output);
            }

            return 0;
        }

        public override void ShowOptions(TextWriter output)
        {
            options.WriteOptionDescriptions(output);
            output.WriteLine("      Workload ID aliases:");
            workloads.ShowOptions(output);
        }

        private void ShowUsage(TextWriter output)
        {
            ShowUsage(output, options);
            output.WriteLine("      Workload ID aliases:");
            workloads.ShowOptions(output);
        }
    }
}
