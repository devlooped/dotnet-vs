using System;
using System.Linq;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var commandFactory = new CommandFactory();

            if (args.Length == 0 || new[] { "?", "-?", "/?", "-h", "/h", "--help", "/help" }.Contains(args[0]))
                return ShowUsage(commandFactory);

            // Run is the default command if another one is not specified.
            if (!commandFactory.IsCommandRegistered(args[0]))
                args = args.Prepend("run").ToArray();

            var commandName = args[0];
            try
            {
                var command = commandFactory.CreateCommand(commandName, args.Skip(1));

                await command.ExecuteAsync(Console.Out);
                // TODO: other exceptions might provide an exit code?
            }
            catch (ShowUsageException ex)
            {
                Console.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {commandName} [options]");
                ex.CommandDescriptor.ShowUsage(Console.Out);
                return -1;
            }
            catch (OptionException ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            return 0;
        }

        static int ShowUsage(CommandFactory commandFactory)
        {
            Console.WriteLine();
            Console.Write($"Usage: {ThisAssembly.Metadata.AssemblyName} [{string.Join('|', commandFactory.RegisteredCommands)}] [options|-?|-h|--help]");
            Console.WriteLine();

            return 0;
        }
    }
}
