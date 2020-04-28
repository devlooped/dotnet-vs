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
                return ShowUsage(commandFactory);

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
                output.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {commandName} [options]");
                ex.CommandDescriptor.ShowUsage(output);
                return -1;
            }
            catch (OptionException ex)
            {
                output.WriteLine(ex.Message);
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
