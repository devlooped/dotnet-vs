using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    public class ListCommand
    {
        readonly OptionSet options;
        ImmutableArray<string> parsed = ImmutableArray.Create<string>("-format", "json");
        bool help = false;
        
        public ListCommand()
        {
            options = new OptionSet
            {
                { "a|all", "finds all instances even if they are incomplete and may not launch", a => parsed = parsed.Add("-all") },
                { "pre", "also searches prereleases", a => parsed = parsed.Add("-prerelease") },
                { "p|property:", "name of a property to return, optionally containing '.' delimiter to separate object and property names", p => parsed = parsed.Add("-property").Add(p) },
                { "f|format:", "json, xml or text", f => parsed = parsed.Add("-format").Add(f) },
                { "?|h|help", "display this help", _ => help = true },
            };
        }

        public async Task<int> ExecuteAsync(IEnumerable<string> args, TextWriter output)
        {
            var vswhere = Path.Combine(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location), "vswhere.exe");
            var psi = new ProcessStartInfo(vswhere)
            {
                RedirectStandardOutput = true,
                ArgumentList = 
                {
                    "-nologo"
                }
            };

            try
            {
                var extraArgs = options.Parse(args);
                
                if (help)
                {
                    Console.WriteLine("Usage: visualstudio list [options]");
                    options.WriteOptionDescriptions(output);
                    output.WriteLine("vswhere.exe:");
                    psi.ArgumentList.Add("-?");
                    output.WriteLine(Process.Start(psi).StandardOutput.ReadToEnd());
                    return 0;
                }
                
                foreach (var arg in parsed)
                {
                    psi.ArgumentList.Add(arg);
                }
                foreach (var arg in extraArgs)
                {
                    psi.ArgumentList.Add(arg);
                }
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
                options.WriteOptionDescriptions(output);
                return -1;
            }

            var process = Process.Start(psi);
            string line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                output.WriteLine(line);
            }

            return process.ExitCode;
        }

        public void WriteHelp(TextWriter output) => options.WriteOptionDescriptions(output);
    }
}
