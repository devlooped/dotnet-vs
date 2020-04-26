using System;
using System.Linq;
using System.Collections.Generic;

namespace VisualStudio
{
    sealed class CommandFactory
    {
        Dictionary<string, (Func<CommandDescriptor> CreateDescriptor, Func<CommandDescriptor, Command> CreateCommand)> factories =
            new Dictionary<string, (Func<CommandDescriptor> CreateDescriptor, Func<CommandDescriptor, Command> CreateCommand)>();

        public CommandFactory()
        {
            var whereService = new WhereService();
            var installerService = new InstallerService();

            RegisterCommand<InstallCommandDescriptor>("install", x => new InstallCommand(x, installerService));
            RegisterCommand<RunCommandDescriptor>("run", x => new RunCommand(x, whereService));
            RegisterCommand<WhereCommandDescriptor>("where", () => new WhereCommandDescriptor(whereService), x => new WhereCommand(x, whereService));
            RegisterCommand<UpdateCommandDescriptor>("update", x => new UpdateCommand(x, whereService));
            RegisterCommand<ModifyCommandDescriptor>("modify", x => new ModifyCommand(x, whereService, installerService));
        }

        public IEnumerable<string> RegisteredCommands => factories.Keys;

        public bool IsCommandRegistered(string command) => factories.ContainsKey(command);

        public void RegisterCommand<TDescriptor>(string command, Func<TDescriptor, Command> commandFactory) where TDescriptor : CommandDescriptor, new() =>
            RegisterCommand(command, () => new TDescriptor(), commandFactory);

        public void RegisterCommand<TDescriptor>(string command, Func<TDescriptor> descriptorFactory, Func<TDescriptor, Command> commandFactory) where TDescriptor : CommandDescriptor =>
            factories.Add(command, (descriptorFactory, x => commandFactory((TDescriptor)x)));

        public Command CreateCommand(string command, IEnumerable<string> args)
        {
            if (!factories.TryGetValue(command, out var factory))
                throw new InvalidOperationException($"The command '{command}' is not registered");

            var commandDescriptor = factory.CreateDescriptor();

            commandDescriptor.Parse(args);

            return factory.CreateCommand(commandDescriptor);
        }
    }
}
