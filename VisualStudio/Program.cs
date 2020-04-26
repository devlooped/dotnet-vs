using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Mono.Options;
using System.Collections.Generic;

namespace VisualStudio
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var commandFactory = new CommandFactory();

#if DEBUG
            Console.WriteLine($"{ThisAssembly.Metadata.AssemblyName} {string.Join(" ", args)}");
            //System.Diagnostics.Debugger.Launch();
#endif

            if (args.Length == 0 || new[] { "?", "/?", "/h", "--help", "help" }.Contains(args[0]))
            {
                ShowUsage(commandFactory.RegisteredCommands);

                return 0;
            }

            var commandName = args[0];

            if (args.Any() && commandFactory.IsCommandRegistered(commandName))
            {
                try
                {
                    var command = commandFactory.CreateCommand(commandName, args.Skip(1));

                    await command.ExecuteAsync(Console.Out);
                }
                catch (ShowUsageException ex)
                {
                    ShowUsage(commandName, ex.CommandDescriptor, Console.Out);
                }
                catch (OptionException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                ShowUsage(commandFactory.RegisteredCommands);
            }

            return 0;
        }

        static void ShowUsage(IEnumerable<string> registeredCommands)
        {
            Console.WriteLine();
            Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} [{string.Join('|', registeredCommands)}] [options|-?|-h|--help]");
            Console.WriteLine();
        }

        static void ShowUsage(string commandName, CommandDescriptor commandDescriptor, TextWriter output)
        {
            output.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {commandName} [options]");
            commandDescriptor.ShowUsage(output);
        }
    }
}
