using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

        public override async Task<int> ExecuteAsync(IEnumerable<string> args, TextWriter output)
        {
            var workloads = new WorkloadOptions("-requires");
            var extra = workloads.Parse(args);

            foreach (var arg in workloads.Arguments)
            {
                psi.ArgumentList.Add(arg);
            }
            foreach (var arg in extra)
            {
                psi.ArgumentList.Add(arg);
            }

            if (args.Any(x => x == "-?" || x == "-h" || x == "-help"))
            {
                Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} {Name} ");
                ShowOptions(output);
                return 0;
            }                

            return await ProcessOutput(psi, line => output.WriteLine(line));
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

        private async Task<int> ProcessOutput(ProcessStartInfo psi, Action<string> lineAction)
        {
            var process = Process.Start(psi);
            string line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                lineAction(line);
            }

            return process.ExitCode;
        }
    }
}
