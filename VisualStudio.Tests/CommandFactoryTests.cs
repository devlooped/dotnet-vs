using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace VisualStudio.Tests
{
    public class CommandFactoryTests
    {
        [Fact]
        public void when_creating_command_with_empty_arguments_then_command_is_created()
        {
            var commandFactory = new CommandFactory();
            commandFactory.RegisterCommand<TestCommandDescriptor>("test", x => new TestCommand(x));

            var command = commandFactory.CreateCommand("test", ImmutableArray.Create<string>());

            Assert.NotNull(command);
            Assert.True(command is TestCommand);
        }

        [Fact]
        public void when_creating_command_with_help_argument_then_throws_show_usage()
        {
            var commandFactory = new CommandFactory();
            commandFactory.RegisterCommand<TestCommandDescriptor>("test", x => new TestCommand(x));

            Assert.Throws<ShowUsageException>(() => commandFactory.CreateCommand("test", ImmutableArray.Create("/h")));
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
        [InlineData(Commands.System.GenerateReadme, typeof(GenerateReadmeCommand))]
        [InlineData(Commands.System.Save, typeof(SaveCommand))]
        public void when_creating_builtin_command_then_then_command_is_created(string commandName, Type expectedCommandType)
        {
            var commandFactory = new CommandFactory();

            Assert.True(commandFactory.IsCommandRegistered(commandName));

            var command = commandFactory.CreateCommand(commandName, ImmutableArray.Create<string>());

            Assert.NotNull(command);
            Assert.Equal(expectedCommandType, command.GetType());
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
