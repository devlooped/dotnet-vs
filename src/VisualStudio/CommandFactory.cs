using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet;

namespace VisualStudio
{
    sealed class CommandFactory
    {
        readonly Config config;

        Dictionary<string, (bool IsSystem, Func<CommandDescriptor> CreateDescriptor, Func<CommandDescriptor, Command> CreateCommand)> factories =
            new Dictionary<string, (bool IsSystem, Func<CommandDescriptor> CreateDescriptor, Func<CommandDescriptor, Command> CreateCommand)>();

        public CommandFactory()
            : this(Commands.DotNetConfig.GetConfig())
        {
        }

        public CommandFactory(Config config)
        {
            this.config = config;

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
            RegisterCommand<AliasCommandDescriptor>(Commands.Alias, x => new AliasCommand(x));
            RegisterCommand<ClientCommandDescriptor>(Commands.Client, x => new ClientCommand(x, whereService));

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

        public void RegisterCommand<TDescriptor>(string command, Func<TDescriptor, Command> commandFactory, bool isSystem = false) where TDescriptor : CommandDescriptor, new() =>
            RegisterCommand(command, () => new TDescriptor(), commandFactory, isSystem);

        public void RegisterCommand<TDescriptor>(string command, Func<TDescriptor> descriptorFactory, Func<TDescriptor, Command> commandFactory, bool isSystem = false) where TDescriptor : CommandDescriptor =>
            factories.Add(command, (isSystem, descriptorFactory, x => commandFactory((TDescriptor)x)));

        public Task<Command> CreateCommandAsync(string command, ImmutableArray<string> args)
        {
            // Check if we should save the command
            if (SaveOption.IsDefined(args) && factories.ContainsKey(Commands.System.Save))
            {
                args = ImmutableArray.Create(args.Prepend(command).ToArray());
                command = Commands.System.Save;
            }
            // Or update self
            else if (command == Commands.Update && SelfOption.IsDefined(args) && factories.ContainsKey(Commands.System.UpdateSelf))
            {
                command = Commands.System.UpdateSelf;
            }

            // Try to get the command factory
            if (!factories.TryGetValue(command, out var factory))
            {
                // Or check if exists a saved command
                var savedCommand = config.GetString(Commands.DotNetConfig.Section, Commands.DotNetConfig.SubSection, command);
                if (!string.IsNullOrEmpty(savedCommand))
                {
                    // Initially the args contains the command name and the args
                    var savedCommandArgs = savedCommand.Split('|', StringSplitOptions.RemoveEmptyEntries);

                    // It's a saved command => the first argument should be the command name
                    var savedCommandName = savedCommandArgs.FirstOrDefault();

                    if (!string.IsNullOrEmpty(savedCommandName))
                    {
                        if (factories.TryGetValue(savedCommandName, out factory))
                        {
                            // Restore the saved args
                            args = ImmutableArray.Create(savedCommandArgs.Skip(1).ToArray());
                            command = savedCommandName;
                        }
                        // If the factory is still null, use the Run command as the default
                        else if (factories.TryGetValue(Commands.Run, out factory))
                        {
                            args = ImmutableArray.Create(savedCommandArgs.ToArray());
                            command = Commands.Run;
                        }
                    }
                }
            }

            // If the factory is still null, use the Run command as the default
            if (factory == default && factories.TryGetValue(Commands.Run, out factory))
            {
                // Run is the default command if another one is not specified.
                args = ImmutableArray.Create(args.Prepend(command).ToArray());
                command = Commands.Run;
            }

            if (factory == default)
                throw new InvalidOperationException($"The command '{command}' is not registered");

            // Create the descriptor
            var commandDescriptor = factory.CreateDescriptor();

            // Parse the arguments
            commandDescriptor.Parse(args);

            // And create the command
            return Task.FromResult(factory.CreateCommand(commandDescriptor));
        }
    }
}
