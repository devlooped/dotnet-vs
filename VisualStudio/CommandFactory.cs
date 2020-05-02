using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace VisualStudio
{
    sealed class CommandFactory
    {
        Dictionary<string, (bool IsSystem, Func<CommandDescriptor> CreateDescriptor, Func<CommandDescriptor, Command> CreateCommand)> factories =
            new Dictionary<string, (bool IsSystem, Func<CommandDescriptor> CreateDescriptor, Func<CommandDescriptor, Command> CreateCommand)>();

        public CommandFactory()
        {
            var whereService = new WhereService();
            var installerService = new InstallerService();

            RegisterCommand<RunCommandDescriptor>(Commands.Run, x => new RunCommand(x, whereService));
            RegisterCommand(Commands.Where, () => new WhereCommandDescriptor(whereService), x => new WhereCommand(x, whereService));
            RegisterCommand<InstallCommandDescriptor>(Commands.Install, x => new InstallCommand(x, installerService));
            RegisterCommand<UpdateCommandDescriptor>(Commands.Update, x => new UpdateCommand(x, whereService, installerService));
            RegisterCommand<ModifyCommandDescriptor>(Commands.Modify, x => new ModifyCommand(x, whereService, installerService));
            RegisterCommand<KillCommandDescriptor>(Commands.Kill, x => new KillCommand(x, whereService));
            RegisterCommand<ConfigCommandDescriptor>(Commands.Config, x => new ConfigCommand(x, whereService));
            RegisterCommand<LogCommandDescriptor>(Commands.Log, x => new LogCommand(x, whereService));

            // System commands
            RegisterCommand(
                Commands.System.GenerateReadme,
                () => new GenerateReadmeCommandDescriptor(factories.Where(x => !x.Value.IsSystem).ToDictionary(x => x.Key, x => x.Value.CreateDescriptor())),
                x => new GenerateReadmeCommand(x),
                isSystem: true);
            RegisterCommand<SaveCommandDescriptor>(Commands.System.Save, x => new SaveCommand(x), isSystem: true);
            RegisterCommand<UpdateSelfCommandDescriptor>(Commands.System.UpdateSelf, x => new UpdateSelfCommand(x), isSystem: true);
        }

        public Dictionary<string, CommandDescriptor> GetRegisteredCommands(bool includeSystemCommands = false) =>
            factories.Where(x => includeSystemCommands || !x.Value.IsSystem).ToDictionary(x => x.Key, x => x.Value.CreateDescriptor());

        public bool IsCommandRegistered(string command) => factories.ContainsKey(command);

        public void RegisterCommand<TDescriptor>(string command, Func<TDescriptor, Command> commandFactory, bool isSystem = false) where TDescriptor : CommandDescriptor, new() =>
            RegisterCommand(command, () => new TDescriptor(), commandFactory, isSystem);

        public void RegisterCommand<TDescriptor>(string command, Func<TDescriptor> descriptorFactory, Func<TDescriptor, Command> commandFactory, bool isSystem = false) where TDescriptor : CommandDescriptor =>
            factories.Add(command, (isSystem, descriptorFactory, x => commandFactory((TDescriptor)x)));

        public Command CreateCommand(string command, ImmutableArray<string> args)
        {
            if (!factories.TryGetValue(command, out var factory))
                throw new InvalidOperationException($"The command '{command}' is not registered");

            var commandDescriptor = factory.CreateDescriptor();

            commandDescriptor.Parse(args);

            return factory.CreateCommand(commandDescriptor);
        }
    }
}
