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
        static async Task Main(string[] args)
        {
            var commandFactory = new CommandFactory();

            if (args.Any() && commandFactory.IsCommandRegistered(args[0]))
            {
                var commandName = args[0];

                try
                {
                    var command = commandFactory.CreateCommand(commandName, args.Skip(1));

                    await command.ExecuteAsync(Console.Out);
                }
                catch (ShowUsageException ex)
                {
                    Console.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {commandName} [options]");
                    ex.CommandDescriptor.ShowUsage(Console.Out);
                }
                catch (OptionException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine();
                Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} [{string.Join('|', commandFactory.RegisteredCommands)}] [options|-?|-h|--help]");
                Console.WriteLine();
            }
        }
    }
}
