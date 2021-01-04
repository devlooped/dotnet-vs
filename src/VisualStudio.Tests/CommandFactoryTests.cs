using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace VisualStudio.Tests
{
    public class CommandFactoryTests
    {
        [Fact]
        public async Task when_creating_command_with_empty_arguments_then_command_is_created()
        {
            var commandFactory = new CommandFactory();
            commandFactory.RegisterCommand<TestCommandDescriptor>("test", x => new TestCommand(x));

            var command = await commandFactory.CreateCommandAsync("test", ImmutableArray.Create<string>());

            Assert.NotNull(command);
            Assert.True(command is TestCommand);
        }

        [Fact]
        public async Task when_creating_command_with_help_argument_then_throws_show_usage()
        {
            var commandFactory = new CommandFactory();
            commandFactory.RegisterCommand<TestCommandDescriptor>("test", x => new TestCommand(x));

            await Assert.ThrowsAsync<ShowUsageException>(async () => await commandFactory.CreateCommandAsync("test", ImmutableArray.Create("/h")));
        }

        [Fact]
        public async Task when_creating_builtin_command_with_help_argument_then_throws_show_usage()
        {
            var commandFactory = new CommandFactory();

            await Assert.ThrowsAsync<ShowUsageException>(async () => await commandFactory.CreateCommandAsync("modify", ImmutableArray.Create("-?")));
        }

        [Theory]
        [InlineData(Commands.Install, typeof(InstallCommand))]
        [InlineData(Commands.Run, typeof(RunCommand))]
        [InlineData(Commands.Where, typeof(WhereCommand))]
        [InlineData(Commands.Modify, typeof(ModifyCommand))]
        [InlineData(Commands.Update, typeof(UpdateCommand))]
        [InlineData(Commands.Config, typeof(ConfigCommand))]
        [InlineData(Commands.Log, typeof(LogCommand))]
        [InlineData(Commands.Kill, typeof(KillCommand))]
        [InlineData(Commands.Alias, typeof(AliasCommand))]
        [InlineData(Commands.Client, typeof(ClientCommand))]
        [InlineData(Commands.System.GenerateReadme, typeof(GenerateReadmeCommand))]
        [InlineData(Commands.System.Save, typeof(SaveCommand))]
        [InlineData(Commands.System.UpdateSelf, typeof(UpdateSelfCommand))]
        public async Task when_creating_builtin_command_then_then_command_is_created(string commandName, Type expectedCommandType)
        {
            var commandFactory = new CommandFactory();

            var command = await commandFactory.CreateCommandAsync(commandName, ImmutableArray.Create<string>());

            Assert.NotNull(command);
            Assert.Equal(expectedCommandType, command.GetType());
        }

        [Fact]
        public async Task when_no_command_specified_run_is_default()
        {
            var commandFactory = new CommandFactory();

            var command = await commandFactory.CreateCommandAsync("pre", ImmutableArray.Create<string>());

            Assert.IsType<RunCommand>(command);
        }

        [Fact]
        public async Task when_save_option_is_specified_then_save_command_is_created()
        {
            var commandFactory = new CommandFactory();

            var command = await commandFactory.CreateCommandAsync(
                "update",
                ImmutableArray.Create(new[] { "main", "--save=foo" }));

            Assert.IsType<SaveCommand>(command);
        }

        [Fact]
        public async Task when_update_command_and_self_option_is_specified_then_update_self_is_created()
        {
            var commandFactory = new CommandFactory();

            var command = await commandFactory.CreateCommandAsync(
                "update",
                ImmutableArray.Create(new[] { "self" }));

            Assert.IsType<UpdateSelfCommand>(command);
        }

        [Fact]
        public async Task when_saved_command_is_specified_then_saved_command_is_created()
        {
            var config = DotNetConfig.Config.Build(Path.GetTempFileName());
            config.SetString(Commands.DotNetConfig.Section, Commands.DotNetConfig.SubSection, "foo", "update|main", null);

            var commandFactory = new CommandFactory(config);

            var command = await commandFactory.CreateCommandAsync(
                "foo",
                ImmutableArray.Create<string>());

            Assert.IsType<UpdateCommand>(command);
        }


        class TestCommandDescriptor : CommandDescriptor
        {
        }

        class TestCommand : Command<TestCommandDescriptor>
        {
            public TestCommand(TestCommandDescriptor descriptor) : base(descriptor) { }

            public override Task ExecuteAsync(TextWriter output) => Task.CompletedTask;
        }
    }
}
