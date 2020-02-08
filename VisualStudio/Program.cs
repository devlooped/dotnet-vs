using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualStudio
{
    class Program
    {
        static Dictionary<string, Command> commands = new Command[]
        {
            new InstallCommand(),
            new WhereCommand(),
        }.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        static async Task<int> Main(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            if (args.Length == 0 || !commands.ContainsKey(args[0]))
            {
                Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} [command] [options]");
                foreach (var item in commands)
                {
                    Console.WriteLine();
                    Console.WriteLine($"::{item.Key}::");
                    item.Value.ShowOptions(Console.Out);
                }
                return -1;
            }

            var command = commands[args[0]];
            return await command.ExecuteAsync(args.Skip(1), Console.Out);
        }
    }
}
