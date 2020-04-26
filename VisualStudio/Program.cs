using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    class Program
    {
        static Dictionary<string, CommandDescriptor> descriptors = new CommandDescriptor[]
        {
            new InstallCommandDescriptor(),
            new RunCommandDescriptor(),
            new WhereCommandDescriptor(),
            new ModifyCommandDescriptor(),
            new UpdateCommandDescriptor()
        }.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        static async Task<int> Main(string[] args)
        {
#if DEBUG
            Console.WriteLine($"{ThisAssembly.Metadata.AssemblyName} {string.Join(" ", args)}");
            //System.Diagnostics.Debugger.Launch();
#endif

            if (args.Length == 0 || new[] { "?", "/?", "/h", "--help", "help" }.Contains(args[0]))
            {
                Console.WriteLine();
                Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} [{string.Join('|', descriptors.Keys)}] [options|-?|-h|--help]");
                Console.WriteLine();

                return 0;
            }

            if (args.Any() && descriptors.TryGetValue(args[0], out var commandDescriptor))
            {
                try
                {
                    var commandFactory = new CommandFactory();
                    var command = commandFactory.CreateCommand(commandDescriptor, args.Skip(1));

                    await command.ExecuteAsync(Console.Out);
                }
                catch (ShowUsageException)
                {
                    ShowUsage(commandDescriptor, commandDescriptor.Options, Console.Out);
                }
                catch (OptionException ex)
                {
                    Console.WriteLine(ex.Message);
                    ShowUsage(commandDescriptor, commandDescriptor.Options, Console.Out);
                }
            }

            return 0;
        }

        static void ShowUsage(CommandDescriptor commandDescriptor, IOptionSet options, TextWriter output)
        {
            output.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {commandDescriptor.Name} [options]");
            options.WriteOptionDescriptions(output);

            if (commandDescriptor is WhereCommandDescriptor)
                WhereService.Instance.ShowUsage(output);
        }
    }
}
