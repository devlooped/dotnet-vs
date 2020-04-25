﻿using System;
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
            new ModifyCommandDescriptor()
        }.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        static async Task<int> Main(string[] args)
        {
#if DEBUG
            Console.WriteLine($"{ThisAssembly.Metadata.AssemblyName} {string.Join(" ", args)}");
            //System.Diagnostics.Debugger.Launch();
#endif

            var help = false;
            var options = new OptionSet
            {
                { "?|h|help", "Display this help", h => help = h != null },
            };

            options.Parse(args);

            if (args.Length == 1 && help)
            {
                Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} [{string.Join('|', descriptors.Keys)}] [options|-?|-h|--help]");
                //foreach (var item in commands)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine($"::{item.Key}::");
                //    item.Value.ShowOptions(Console.Out);
                //}
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

            commandDescriptor.ShowUsage(output);
        }
    }
}
