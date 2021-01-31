using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Devlooped
{
    class Program
    {
        readonly CommandFactory commandFactory;
        readonly TextWriter output;
        readonly string[] args;
        Command executingCommand;

        static Task<int> Main(string[] args)
        {
            var program = new Program(Console.Out, new CommandFactory(), args);

            Console.CancelKeyPress += async (sender, e) => await program.CancelAsync();

            return program.RunAsync();
        }

        public Program(TextWriter output, CommandFactory commandFactory, params string[] args)
        {
            this.output = output;
            this.args = args;
            this.commandFactory = commandFactory;
        }

        public async Task CancelAsync()
        {
            if (executingCommand is IAsyncDisposable disposableCommand)
                await disposableCommand.DisposeAsync();
            else
                (executingCommand as IDisposable)?.Dispose();
        }

        public async Task<int> RunAsync()
        {
            if (args.Length != 0 && new[] { "?", "-?", "/?", "-h", "/h", "--help", "/help" }.Contains(args[0]))
            {
                ShowUsage();
                return 0;
            }
            if (args.Length != 0 && VersionOption.IsDefined(new[] { args[0] }))
            {
                // Only consider --version if it's the *first* argument defined.
                ShowVersion();
                return 0;
            }

            var commandName = args.Length == 0 ? Commands.Run : args[0];
            var commandArgs = ImmutableArray.Create(args.Length == 0 ? args : args.Skip(1).ToArray());
            try
            {
                executingCommand = await commandFactory.CreateCommandAsync(commandName, commandArgs);

                await executingCommand.ExecuteAsync(output);
            }
            catch (ShowUsageException ex)
            {
                var writer = new DefaultTextWriter(output);

                if (!string.IsNullOrEmpty(ex.CommandDescriptor.Description))
                {
                    writer.WriteLine(ex.CommandDescriptor.Description);
                    writer.WriteLine();
                }

                writer.WriteLine($"Usage: {ThisAssembly.Project.AssemblyName} {commandName} [options] [--save=]");

                ex.CommandDescriptor.ShowUsage(writer);
                ShowExamples(commandName);

                return 0;
            }
            catch (Exception ex) when (!DebugOption.IsDefined(args))
            {
                output.WriteLine(ex.Message);

                return ErrorCodes.Error;
            }

            return 0;
        }

        protected virtual void ShowVersion()
        {
            output.WriteLine($"{ThisAssembly.Project.AssemblyName} {ThisAssembly.Info.InformationalVersion}");
            output.WriteLine();
        }

        protected virtual void ShowUsage()
        {
            output.WriteLine();
            output.WriteLine($"Usage: {ThisAssembly.Project.AssemblyName} [command] [options|-?|-h|--help] [--save=ALIAS[--global]]");
            output.WriteLine();
            output.WriteLine("Supported commands:");

            var maxWidth = commandFactory.GetRegisteredCommands().Select(x => x.Key.Length).Max() + 5;
            foreach (var command in commandFactory.GetRegisteredCommands())
                output.WriteLine($"  {command.Key.GetNormalizedString(maxWidth)}{command.Value.Description}");
        }

        protected virtual void ShowExamples(string commandName)
        {
            if (Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualStudio.Docs." + commandName + ".md") is Stream stream)
            {
                using var reader = new StreamReader(stream);
                var showLine = false;
                var line = default(string);
                while ((line = reader.ReadLine()) != null && !line.StartsWith("<!-- EXAMPLES_END"))
                {
                    if (line.StartsWith("<!-- EXAMPLES_BEGIN"))
                    {
                        // It means that we found the first comment and the file contains examples to be shown
                        output.WriteLine();
                        output.WriteLine("Examples:");
                        output.WriteLine();

                        showLine = true;
                    }
                    else if (showLine && !line.Trim().StartsWith("```"))
                    {
                        // Don't render the code formatting block
                        output.WriteLine(line);
                    }
                }
            }
        }
    }
}
