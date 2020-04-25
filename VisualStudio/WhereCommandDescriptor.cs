using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mono.Options;

namespace VisualStudio
{
    class WhereCommandDescriptor : CommandDescriptor<WhereCommand>
    {
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        public WhereCommandDescriptor()
        {
            Options = new CompositeOptionsSet(
                new OptionSet{
                    { "sku:", "Edition, one of [e|ent|enterprise], [p|pro|professional] or [c|com|community]. Defaults to 'community'.", s => Sku = SkuOption.Parse(s) },
                },
                workloads);
        }

        public override string Name => "where";

        public Sku? Sku { get; set; }

        public IEnumerable<string> WorkloadsArguments => workloads.Arguments;

        public string VsWherePath => Path.Combine(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location), "vswhere.exe");

        public override void ShowUsage(TextWriter output)
        {
            base.ShowUsage(output);

            output.WriteLine();
            output.WriteLine("[vswhere.exe options]");

            var psi = new ProcessStartInfo(VsWherePath)
            {
                RedirectStandardOutput = true,
                ArgumentList = { "-nologo", "-?" }
            };

            var process = Process.Start(psi);
            string line;
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("Usage:") || line.StartsWith("Options:"))
                    continue;

                Console.WriteLine(line);
            }
        }
    }
}
