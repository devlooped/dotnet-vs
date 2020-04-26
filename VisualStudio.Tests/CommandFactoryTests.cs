using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace VisualStudio.Tests
{
    public class CommandFactoryTests
    {
        [Fact]
        public void when_creating_command_with_empty_arguments_and_show_usage_is_enabled_then_show_usage()
        {
            var commandFactory = new CommandFactory();

            var commandDescriptor = new TestCommandDescriptor(showUsageWithEmtpyArguments: true);

            Assert.Throws<ShowUsageException>(() => commandFactory.CreateCommand(commandDescriptor, Enumerable.Empty<string>()));
        }

        [Fact]
        public void when_creating_command_with_empty_arguments_and_show_usage_is_not_enabled_then_command_is_created()
        {
            var commandFactory = new CommandFactory();

            var commandDescriptor = new TestCommandDescriptor(showUsageWithEmtpyArguments: false);
            var command = commandFactory.CreateCommand(commandDescriptor, Enumerable.Empty<string>());

            Assert.NotNull(command);
        }

        [Fact]
        public void when_creating_command_with_help_argument_then_throws_show_usage()
        {
            var commandFactory = new CommandFactory();

            var commandDescriptor = new TestCommandDescriptor();

            Assert.Throws<ShowUsageException>(() => commandFactory.CreateCommand(commandDescriptor, new[] { "/h" }));
        }

        class TestCommandDescriptor : CommandDescriptor<TestCommand>
        {
            public TestCommandDescriptor(bool showUsageWithEmtpyArguments = true)
            {
                ShowUsageWithEmptyArguments = showUsageWithEmtpyArguments;

                Options = new CompositeOptionsSet();
            }

            public override string Name => "test";
        }

        class TestCommand : Command<TestCommandDescriptor>
        {
            public TestCommand(TestCommandDescriptor descriptor) : base(descriptor) { }

            public override Task ExecuteAsync(TextWriter output) => Task.CompletedTask;
        }
    }
}
