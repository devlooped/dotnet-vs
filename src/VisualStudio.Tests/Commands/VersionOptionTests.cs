using System.CommandLine;
using System.Linq;
using Xunit;

namespace Devlooped.Tests
{
    public class VersionOptionTests
    {
        [Theory]
        [InlineData(Commands.Run)]
        [InlineData(Commands.Install)]
        [InlineData(Commands.Update)]
        public void when_command_is_defined_then_it_accepts_version_option(string commandName)
        {
            var root = new VsRootCommand();
            var command = root.Subcommands.Single(c => c.Name == commandName);

            Assert.Contains(command.Options, o => o.Name == "--version" && o.Aliases.Contains("-v"));
        }

        [Theory]
        [InlineData(Commands.Install, "--version", "18.7")]
        [InlineData(Commands.Install, "-v", "18.7.3")]
        [InlineData(Commands.Update, "--version", "18.7")]
        [InlineData(Commands.Update, "-v", "17.14")]
        [InlineData(Commands.Run, "--version", "18.7")]
        [InlineData(Commands.Run, "-v", "18.7.3")]
        public void when_parsing_version_then_option_is_bound_and_not_unmatched(
            string commandName, string option, string value)
        {
            var root = new VsRootCommand();
            var parse = root.Parse(new[] { commandName, option, value });

            Assert.Empty(parse.Errors);
            Assert.DoesNotContain(option, parse.UnmatchedTokens);
            Assert.DoesNotContain(value, parse.UnmatchedTokens);

            var version = parse.CommandResult.Command.Options.OfType<Option<string>>().Single(o => o.Name == "--version");
            Assert.Equal(value, parse.GetValue(version));
        }

        [Theory]
        [InlineData(Commands.Install, "-v:18.7", "18.7")]
        [InlineData(Commands.Update, "--version=18.7.3", "18.7.3")]
        [InlineData(Commands.Run, "-v:18", "18")]
        public void when_parsing_legacy_version_syntax_then_option_is_bound(
            string commandName, string token, string expected)
        {
            var rewritten = ArgumentPreprocessor.RewriteForCommand(commandName, new[] { token });
            var root = new VsRootCommand();
            var parse = root.Parse(new[] { commandName }.Concat(rewritten).ToArray());

            Assert.Empty(parse.Errors);
            var version = parse.CommandResult.Command.Options.OfType<Option<string>>().Single(o => o.Name == "--version");
            Assert.Equal(expected, parse.GetValue(version));
        }
    }
}
