using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Mono.Options;
using vswhere;

namespace VisualStudio
{
    public class WhereCommand : Command
    {
        static readonly string vswhere = Path.Combine(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location), "vswhere.exe");
        readonly ProcessStartInfo psi = new ProcessStartInfo(vswhere)
        {
            RedirectStandardOutput = true,
            ArgumentList = { "-nologo" }
        };

        public override string Name => "where";

        public bool Quiet { get; set; }

        public VisualStudioInstance[] Instances { get; private set; }

        public override async Task<int> ExecuteAsync(IEnumerable<string> args, TextWriter output)
        {
            var workloads = new WorkloadOptions("-requires");
            Sku? sku = null;
            var extra = workloads.Parse(new OptionSet
            {
                { "sku:", s => sku = SkuOption.Parse(s) }
            }.Parse(args));

            var formatJson = string.Join('=', args).Contains("-format=json", StringComparison.OrdinalIgnoreCase);

            foreach (var arg in workloads.Arguments)
            {
                psi.ArgumentList.Add(arg);
            }
            foreach (var arg in extra)
            {
                psi.ArgumentList.Add(arg);
            }

            if (sku != null)
            {
                psi.ArgumentList.Add("-products");
                psi.ArgumentList.Add("Microsoft.VisualStudio.Product." + sku);
            }

            if (args.Any(x => x == "-?" || x == "-h" || x == "-help"))
            {
                Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} {Name} ");
                ShowOptions(output);
                return 0;
            }

            return await ProcessOutput(psi, line =>
            {
                if (!Quiet)
                    output.WriteLine(line);
            }, formatJson);
        }

        public override void ShowOptions(TextWriter output)
        {
            Console.WriteLine("[vswhere.exe options]");
            psi.ArgumentList.Add("-?");
            var process = Process.Start(psi);
            string line;
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("Usage:") || line.StartsWith("Options:"))
                    continue;

                Console.WriteLine(line);
            }
        }

        private async Task<int> ProcessOutput(ProcessStartInfo psi, Action<string> lineAction, bool readJson)
        {
            var process = Process.Start(psi);
            string line;

            if (readJson)
            {
                var builder = new StringBuilder();
                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                {
                    lineAction(line);
                    builder.Append(line);
                }

                Instances = JsonSerializer.Deserialize<VisualStudioInstance[]>(builder.ToString(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
            }
            else
            {
                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                {
                    lineAction(line);
                }
            }

            return process.ExitCode;
        }
    }
}
