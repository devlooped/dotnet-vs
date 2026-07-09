using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Devlooped.Tests
{
    public class ProgramTests
    {
        readonly TextWriter output;

        public ProgramTests(ITestOutputHelper output) =>
            this.output = new OutputHelperTextWriter(output);

        [Theory]
        [InlineData("/help")]
        [InlineData("/?")]
        [InlineData("-?")]
        [InlineData("/h")]
        public async Task when_running_with_help_arg_then_usage_is_shown(params string[] args)
        {
            var program = new ProgramTest(output, args ?? Array.Empty<string>());

            var exitCode = await program.RunAsync();

            Assert.Equal(0, exitCode);
            Assert.True(program.UsageShown);
        }

        [Fact]
        public async Task when_running_with_version_arg_then_version_is_shown()
        {
            var program = new ProgramTest(output, "--version");

            var exitCode = await program.RunAsync();

            Assert.Equal(0, exitCode);
            Assert.True(program.VersionShown);
        }

        [Fact]
        public async Task when_running_command_with_version_arg_then_version_is_not_shown()
        {
            var root = new VsRootCommand();
            var executed = false;
            var test = new Command("test")
            {
                TreatUnmatchedTokensAsErrors = false,
            };
            // Allow --version as unmatched / no-op option so it doesn't fail parse
            test.Options.Add(new Option<bool>("--version") { Description = "ignored" });
            test.SetAction((ParseResult _) =>
            {
                executed = true;
                return 0;
            });
            root.Subcommands.Add(test);

            var program = new ProgramTest(output, root, "test", "--version");

            var exitCode = await program.RunAsync();

            Assert.Equal(0, exitCode);
            Assert.False(program.VersionShown);
            Assert.True(executed);
        }

        [Fact]
        public async Task when_running_command_then_command_is_executed()
        {
            var root = new VsRootCommand();
            var executed = false;
            var test = new Command("test");
            test.SetAction((ParseResult _) =>
            {
                executed = true;
                return 0;
            });
            root.Subcommands.Add(test);

            var program = new Program(output, root, new ArgumentPreprocessor(Commands.DotNetConfig.GetConfig(), new[] { "test" }), "test");

            var exitCode = await program.RunAsync();

            Assert.Equal(0, exitCode);
            Assert.True(executed);
        }

        [Fact]
        public async Task when_command_throws_then_error_code_is_returned()
        {
            var root = new VsRootCommand();
            var test = new Command("test");
            test.SetAction((ParseResult _) =>
            {
                throw new InvalidOperationException("boom");
#pragma warning disable CS0162
                return 0;
#pragma warning restore CS0162
            });
            root.Subcommands.Add(test);

            var program = new Program(output, root, new ArgumentPreprocessor(Commands.DotNetConfig.GetConfig(), new[] { "test" }), "test");

            var exitCode = await program.RunAsync();

            Assert.Equal(ErrorCodes.Error, exitCode);
        }

        [Fact]
        public async Task when_command_throws_and_debug_is_specified_then_throws()
        {
            var root = new VsRootCommand();
            var test = new Command("test");
            test.SetAction((ParseResult _) =>
            {
                throw new InvalidOperationException("boom");
#pragma warning disable CS0162
                return 0;
#pragma warning restore CS0162
            });
            root.Subcommands.Add(test);

            var program = new Program(output, root, new ArgumentPreprocessor(Commands.DotNetConfig.GetConfig(), new[] { "test" }), "test", "--debug");

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await program.RunAsync());
        }

        [Fact]
        public async Task when_program_is_cancelled_then_client_session_is_disposed()
        {
            var program = new ProgramTest(output, "alias");
            ClientCommand.ActiveSession = new ClientCommand.ClientSession();

            await program.CancelAsync();

            // Dispose is safe with null Server
            Assert.Null(ClientCommand.ActiveSession?.Server);
        }

        [Fact]
        public async Task when_command_help_requested_then_exits_zero()
        {
            var program = new ProgramTest(output, "where", "--help");

            var exitCode = await program.RunAsync();

            Assert.Equal(0, exitCode);
        }

        class ProgramTest : Program
        {
            public ProgramTest(TextWriter output, params string[] args)
                : base(output, args)
            {
                NoVersionChecks = true;
            }

            public ProgramTest(TextWriter output, VsRootCommand root, params string[] args)
                : base(output, root, new ArgumentPreprocessor(Commands.DotNetConfig.GetConfig(),
                    root.Subcommands.Select(c => c.Name)), args)
            {
                NoVersionChecks = true;
            }

            public bool UsageShown { get; private set; }

            public bool VersionShown { get; private set; }

            protected override void ShowUsage()
            {
                base.ShowUsage();
                UsageShown = true;
            }

            protected override async Task ShowVersion()
            {
                await base.ShowVersion();
                VersionShown = true;
            }
        }
    }
}
