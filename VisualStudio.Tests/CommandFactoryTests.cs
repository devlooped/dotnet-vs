using System;
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

            var command = commandFactory.CreateCommand("test", Enumerable.Empty<string>());

            Assert.NotNull(command);
            Assert.True(command is TestCommand);
        }

        [Fact]
        public void when_creating_command_with_help_argument_then_throws_show_usage()
        {
            var commandFactory = new CommandFactory();
            commandFactory.RegisterCommand<TestCommandDescriptor>("test", x => new TestCommand(x));

            Assert.Throws<ShowUsageException>(() => commandFactory.CreateCommand("test", new[] { "/h" }));
        }

        [Theory]
        [InlineData("install", typeof(InstallCommand))]
        [InlineData("run", typeof(RunCommand))]
        [InlineData("where", typeof(WhereCommand))]
        [InlineData("modify", typeof(ModifyCommand))]
        [InlineData("update", typeof(UpdateCommand))]

        public void when_creating_builtin_command_then_then_command_is_created(string commandName, Type expectedCommandType)
        {
            var commandFactory = new CommandFactory();

            var command = commandFactory.CreateCommand(commandName, Enumerable.Empty<string>());

            Assert.NotNull(command);
            Assert.Equal(expectedCommandType, command.GetType());
        }

        class TestCommandDescriptor : CommandDescriptor
        {
            public TestCommandDescriptor()
            {
                optionSet = new CompositeOptionsSet();
            }
        }

        class TestCommand : Command<TestCommandDescriptor>
        {
            public TestCommand(TestCommandDescriptor descriptor) : base(descriptor) { }

            public override Task ExecuteAsync(TextWriter output) => Task.CompletedTask;
        }
    }
}
