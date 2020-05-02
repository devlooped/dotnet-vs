using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    class Program
    {
        static readonly CommandFactory commandFactory = new CommandFactory();

        TextWriter output;
        bool execute;
        string[] args;

        static async Task<int> Main(string[] args)
        {
            if (args.Length == 0 || new[] { "?", "-?", "/?", "-h", "/h", "--help", "/help" }.Contains(args[0]))
                return ShowUsage();

            return await new Program(Console.Out, true, args).RunAsync();
        }

        public Program(TextWriter output, bool execute, params string[] args)
        {
            this.output = output;
            this.execute = execute;
            this.args = args;

            // Run is the default command if another one is not specified.
            if (!commandFactory.IsCommandRegistered(args[0]))
                this.args = args.Prepend("run").ToArray();
        }

        public Command Command { get; private set; }

        public async Task<int> RunAsync()
        {
            var commandName = args[0];
            try
            {
                Command = commandFactory.CreateCommand(commandName, args.Skip(1));

                if (execute)
                    await Command.ExecuteAsync(output);
            }
            catch (ShowUsageException ex)
            {
                var writer = new DefaultTextWriter(output);

                if (!string.IsNullOrEmpty(ex.CommandDescriptor.Description))
                {
                    writer.WriteLine(ex.CommandDescriptor.Description);
                    writer.WriteLine();
                }

                writer.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {commandName} [options]");

                ex.CommandDescriptor.ShowUsage(writer);

                // TODO try to extract the examples from the .md command documentation

                return ErrorCodes.ShowUsage;
            }
            catch (OptionException ex)
            {
                output.WriteLine(ex.Message);

                return ErrorCodes.OptionError;
            }

            return 0;
        }

        static int ShowUsage()
        {
            Console.WriteLine();
            Console.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} [command] [options|-?|-h|--help]");
            Console.WriteLine();
            Console.WriteLine("Supported commands:");

            var maxWidth = commandFactory.GetRegisteredCommands().Select(x => x.Key.Length).Max() + 2;
            foreach (var command in commandFactory.GetRegisteredCommands())
                Console.WriteLine($"\t- {command.Key.GetNormalizedString(maxWidth)}{command.Value.Description}");

            return 0;
        }
    }
}
