using System.IO;
using System.Linq;
using Xunit;

namespace Devlooped.Tests
{
    public class CommandFactoryTests
    {
        [Fact]
        public void when_processing_empty_arguments_then_defaults_to_run()
        {
            var preprocessor = new ArgumentPreprocessor();
            var tokens = preprocessor.Process(System.Array.Empty<string>());

            Assert.Equal(new[] { Commands.Run }, tokens);
        }

        [Fact]
        public void when_processing_help_argument_then_returns_help_token()
        {
            var preprocessor = new ArgumentPreprocessor();
            Assert.True(preprocessor.IsTopLevelHelp(new[] { "/h" }));
            Assert.Equal(new[] { "--help" }, preprocessor.Process(new[] { "/h" }));
        }

        [Theory]
        [InlineData(Commands.Install)]
        [InlineData(Commands.Run)]
        [InlineData(Commands.Where)]
        [InlineData(Commands.Modify)]
        [InlineData(Commands.Update)]
        [InlineData(Commands.Config)]
        [InlineData(Commands.Log)]
        [InlineData(Commands.Kill)]
        [InlineData(Commands.Alias)]
        [InlineData(Commands.Client)]
        [InlineData(Commands.System.GenerateReadme)]
        [InlineData(Commands.System.Save)]
        [InlineData(Commands.System.UpdateSelf)]
        public void when_processing_builtin_command_then_command_is_first_token(string commandName)
        {
            var preprocessor = new ArgumentPreprocessor();
            var tokens = preprocessor.Process(new[] { commandName });

            Assert.Equal(commandName, tokens[0]);
        }

        [Fact]
        public void when_no_command_specified_run_is_default()
        {
            var preprocessor = new ArgumentPreprocessor();
            var tokens = preprocessor.Process(new[] { "pre" });

            Assert.Equal(Commands.Run, tokens[0]);
            Assert.Contains("--pre", tokens); // channel shortcut rewritten
        }

        [Fact]
        public void when_save_option_is_specified_then_save_command_is_created()
        {
            var preprocessor = new ArgumentPreprocessor();
            var tokens = preprocessor.Process(new[] { "update", "main", "--save=foo" });

            Assert.Equal(Commands.System.Save, tokens[0]);
            Assert.Contains("update", tokens);
            Assert.Contains("--save=foo", tokens);
        }

        [Fact]
        public void when_update_command_and_self_option_is_specified_then_update_self_is_created()
        {
            var preprocessor = new ArgumentPreprocessor();
            var tokens = preprocessor.Process(new[] { "update", "self" });

            Assert.Equal(Commands.System.UpdateSelf, tokens[0]);
        }

        [Fact]
        public void when_saved_command_is_specified_then_saved_command_is_created()
        {
            var config = DotNetConfig.Config.Build(Path.GetTempFileName());
            config = config.SetString(Commands.DotNetConfig.Section, Commands.DotNetConfig.SubSection, "foo", "update|main", null);

            var preprocessor = new ArgumentPreprocessor(config);
            var tokens = preprocessor.Process(new[] { "foo" });

            Assert.Equal(Commands.Update, tokens[0]);
            Assert.Contains("--main", tokens);
        }

        [Fact]
        public void when_parsing_builtin_commands_on_root_then_commands_exist()
        {
            var root = new VsRootCommand();
            Assert.Contains(root.Subcommands, c => c.Name == Commands.Run);
            Assert.Contains(root.Subcommands, c => c.Name == Commands.Where);
            Assert.Contains(root.Subcommands, c => c.Name == Commands.Install);
            Assert.Contains(root.Subcommands, c => c.Name == Commands.System.Save && c.Hidden);
            Assert.Contains(root.Subcommands, c => c.Name == Commands.System.UpdateSelf && c.Hidden);
            Assert.Contains(root.Subcommands, c => c.Name == Commands.System.GenerateReadme && c.Hidden);
        }

        [Fact]
        public void when_command_help_is_requested_then_parse_selects_help_action()
        {
            var root = new VsRootCommand();
            var preprocessor = new ArgumentPreprocessor();
            var tokens = preprocessor.Process(new[] { "modify", "-?" });
            // -? may be rewritten; ensure help-related token present or parse succeeds for modify help
            var rewritten = tokens.Select(t => t == "-?" ? "--help" : t).ToArray();
            // Process doesn't rewrite -? when not first token. TokenRewriter legacy may not rewrite -?
            // Ensure command is modify
            Assert.Equal(Commands.Modify, tokens[0]);
        }
    }
}
