using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VisualStudio
{
    class Program
    {
        CommandFactory commandFactory;
        TextWriter output;
        string[] args;

        static Task<int> Main(string[] args) => new Program(Console.Out, new CommandFactory(), args).RunAsync();

        public Program(TextWriter output, CommandFactory commandFactory, params string[] args)
        {
            this.output = output;
            this.args = args;
            this.commandFactory = commandFactory;
        }

        public async Task<int> RunAsync()
        {
            if (args.Length == 0 || new[] { "?", "-?", "/?", "-h", "/h", "--help", "/help" }.Contains(args[0]))
            {
                ShowUsage();
                return 0;
            }

            var commandName = args[0];
            var commandArgs = ImmutableArray.Create(args.Skip(1).ToArray());
            try
            {
                var command = await commandFactory.CreateCommandAsync(commandName, commandArgs);

                await command.ExecuteAsync(output);
            }
            catch (ShowUsageException ex)
            {
                var writer = new DefaultTextWriter(output);

                if (!string.IsNullOrEmpty(ex.CommandDescriptor.Description))
                {
                    writer.WriteLine(ex.CommandDescriptor.Description);
                    writer.WriteLine();
                }

                writer.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {commandName} [options] [--save=]");

                ex.CommandDescriptor.ShowUsage(writer);

                // TODO try to extract the examples from the .md command documentation

                return ErrorCodes.ShowUsage;
            }
            catch (Exception ex) when (!DebugOption.IsDefined(args))
            {
                output.WriteLine(ex.Message);

                return ErrorCodes.Error;
            }

            return 0;
        }

        protected virtual void ShowUsage()
        {
            Console.WriteLine();
            Console.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} [command] [options|-?|-h|--help] [--save=]");
            Console.WriteLine();
            Console.WriteLine("Supported commands:");

            var maxWidth = commandFactory.GetRegisteredCommands().Select(x => x.Key.Length).Max() + 5;
            foreach (var command in commandFactory.GetRegisteredCommands())
                Console.WriteLine($"  {command.Key.GetNormalizedString(maxWidth)}{command.Value.Description}");
        }
    }
}
