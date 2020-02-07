using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            foreach (var arg in args)
            {
                psi.ArgumentList.Add(arg);
            }

            // If we only have the default `-nologo` we add
            if (psi.ArgumentList.Count == 1)
                return await ShowOptions(output);

            return await ProcessOutput(psi, line => output.WriteLine(line));
        }

        public override async Task<int> ShowOptions(TextWriter output)
        {
            psi.ArgumentList.Add("-?");
            return await ProcessOutput(psi, line =>
            {
                if (line.StartsWith("Usage:"))
                    Console.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {Name} [vswhere.exe options]");
                else
                    Console.WriteLine(line);
            });
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
