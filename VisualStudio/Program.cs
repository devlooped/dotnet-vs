using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    class Program
    {
        static Dictionary<string, Command> commands = new Command[]
        {
            new InstallCommand(),
            new RunCommand(),
            new WhereCommand(),
        }.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        static async Task<int> Main(string[] args)
        {
#if DEBUG
            //System.Diagnostics.Debugger.Launch();
#endif

            var help = false;
            var command = "run";
            var options = new OptionSet
            {
                { "?|h|help", "Display this help", h => help = h != null },
            };

            options.Parse(args);

            if (args.Length == 1 && help)
            {
                Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} [command] [options]");
                foreach (var item in commands)
                {
                    Console.WriteLine();
                    Console.WriteLine($"::{item.Key}::");
                    item.Value.ShowOptions(Console.Out);
                }
                return 0;
            }

            var commandArgs = args.ToList();

            if (args.Length > 0 && commands.ContainsKey(args[0]))
            {
                command = args[0];
                commandArgs.RemoveAt(0);
            }

            return await commands[command].ExecuteAsync(commandArgs, Console.Out);
        }
    }
}
