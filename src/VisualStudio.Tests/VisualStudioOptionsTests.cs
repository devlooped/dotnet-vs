using System;
using System.CommandLine;
using System.Linq;
using Xunit;

namespace Devlooped.Tests
{
    public class VisualStudioOptionsTests
    {
        static ParseResult ParseSelection(params string[] args)
        {
            var rewritten = TokenRewriter.RewriteVisualStudioSelection(args.Where(a => !string.IsNullOrEmpty(a)));
            var cmd = new Command("test");
            var channel = SharedOptions.AddChannelOptions(cmd, "test");
            var sku = SharedOptions.SkuOption();
            var filter = SharedOptions.FilterOption();
            var first = SharedOptions.FirstOption("test");
            var all = SharedOptions.AllOption("test");
            var nick = SharedOptions.NicknameOption();
            var exp = SharedOptions.ExperimentalOption("test");
            var version = SharedOptions.VersionOption("test");
            cmd.Options.Add(sku);
            cmd.Options.Add(filter);
            cmd.Options.Add(first);
            cmd.Options.Add(all);
            cmd.Options.Add(nick);
            cmd.Options.Add(exp);
            cmd.Options.Add(version);
            cmd.TreatUnmatchedTokensAsErrors = false;

            var root = new RootCommand();
            root.Subcommands.Add(cmd);
            return root.Parse(new[] { "test" }.Concat(rewritten).ToArray());
        }

        static VisualStudioFilter GetFilter(ParseResult parse)
        {
            var cmd = parse.CommandResult.Command;
            // Rebuild option refs from command
            var channel = new SharedOptions.ChannelOptions(
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--stable"),
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--rel"),
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--insiders"),
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--pre"),
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--int"),
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--main"));

            return CommandHelpers.GetFilter(
                parse,
                channel,
                cmd.Options.OfType<Option<string>>().First(o => o.Name == "--sku"),
                cmd.Options.OfType<Option<string>>().First(o => o.Name == "--filter"),
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--first"),
                cmd.Options.OfType<Option<bool>>().First(o => o.Name == "--all"),
                cmd.Options.OfType<Option<string>>().First(o => o.Name == "--version"));
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("stable", Channel.Stable)]
        [InlineData("Stable", Channel.Stable)]
        [InlineData("--stable", Channel.Stable)]
        [InlineData("rel", Channel.Stable)]
        [InlineData("release", Channel.Stable)]
        [InlineData("Release", Channel.Stable)]
        [InlineData("--rel", Channel.Stable)]
        [InlineData("--release", Channel.Stable)]
        [InlineData("insiders", Channel.Insiders)]
        [InlineData("Insiders", Channel.Insiders)]
        [InlineData("--insiders", Channel.Insiders)]
        [InlineData("pre", Channel.Insiders)]
        [InlineData("preview", Channel.Insiders)]
        [InlineData("Preview", Channel.Insiders)]
        [InlineData("--pre", Channel.Insiders)]
        [InlineData("--preview", Channel.Insiders)]
        [InlineData("int", Channel.IntPreview)]
        [InlineData("internal", Channel.IntPreview)]
        [InlineData("--int", Channel.IntPreview)]
        [InlineData("--internal", Channel.IntPreview)]
        [InlineData("main", Channel.Main)]
        [InlineData("--main", Channel.Main)]
        public void when_parsing_channel_argument_then_channel_is_set(string argument, Channel? expectedValue)
        {
            var args = string.IsNullOrEmpty(argument) ? Array.Empty<string>() : new[] { argument };
            var filter = GetFilter(ParseSelection(args));
            Assert.Equal(expectedValue, filter.Channel);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("e", Sku.Enterprise)]
        [InlineData("ent", Sku.Enterprise)]
        [InlineData("enterprise", Sku.Enterprise)]
        [InlineData("Ent", Sku.Enterprise)]
        [InlineData("Enterprise", Sku.Enterprise)]
        [InlineData("--sku=e", Sku.Enterprise)]
        [InlineData("--sku=ent", Sku.Enterprise)]
        [InlineData("--sku=enterprise", Sku.Enterprise)]
        [InlineData("--sku=Enterprise", Sku.Enterprise)]
        [InlineData("p", Sku.Professional)]
        [InlineData("pro", Sku.Professional)]
        [InlineData("professional", Sku.Professional)]
        [InlineData("Pro", Sku.Professional)]
        [InlineData("Professional", Sku.Professional)]
        [InlineData("--sku=p", Sku.Professional)]
        [InlineData("--sku=pro", Sku.Professional)]
        [InlineData("--sku=professional", Sku.Professional)]
        [InlineData("--sku=Professional", Sku.Professional)]
        [InlineData("c", Sku.Community)]
        [InlineData("com", Sku.Community)]
        [InlineData("community", Sku.Community)]
        [InlineData("Com", Sku.Community)]
        [InlineData("Community", Sku.Community)]
        [InlineData("--sku=c", Sku.Community)]
        [InlineData("--sku=com", Sku.Community)]
        [InlineData("--sku=community", Sku.Community)]
        [InlineData("--sku=Community", Sku.Community)]
        [InlineData("b", Sku.BuildTools)]
        [InlineData("build", Sku.BuildTools)]
        [InlineData("buildtools", Sku.BuildTools)]
        [InlineData("Build", Sku.BuildTools)]
        [InlineData("BuildTools", Sku.BuildTools)]
        [InlineData("--sku=b", Sku.BuildTools)]
        [InlineData("--sku=build", Sku.BuildTools)]
        [InlineData("--sku=buildtools", Sku.BuildTools)]
        [InlineData("--sku=BuildTools", Sku.BuildTools)]
        [InlineData("t", Sku.TestAgent)]
        [InlineData("test", Sku.TestAgent)]
        [InlineData("testagent", Sku.TestAgent)]
        [InlineData("Test", Sku.TestAgent)]
        [InlineData("TestAgent", Sku.TestAgent)]
        [InlineData("--sku=t", Sku.TestAgent)]
        [InlineData("--sku=test", Sku.TestAgent)]
        [InlineData("--sku=testagent", Sku.TestAgent)]
        [InlineData("--sku=TestAgent", Sku.TestAgent)]
        public void when_parsing_sku_argument_then_sku_is_set(string argument, Sku? expectedValue)
        {
            var args = string.IsNullOrEmpty(argument) ? Array.Empty<string>() : new[] { argument };
            var filter = GetFilter(ParseSelection(args));
            Assert.Equal(expectedValue, filter.Sku);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("--nick=nick1", "nick1")]
        [InlineData("--nickname=nick2", "nick2")]
        public void when_parsing_nickname_argument_then_nickname_is_set(string argument, string expectedValue)
        {
            var args = string.IsNullOrEmpty(argument) ? Array.Empty<string>() : new[] { argument };
            var parse = ParseSelection(args);
            var nick = parse.CommandResult.Command.Options.OfType<Option<string>>().First(o => o.Name == "--nick");
            Assert.Equal(expectedValue, parse.GetValue(nick));
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("exp", true)]
        [InlineData("experimental", true)]
        [InlineData("--exp", true)]
        [InlineData("--experimental", true)]
        public void when_parsing_experimental_then_experimental_is_set(string argument, bool expectedValue)
        {
            var args = string.IsNullOrEmpty(argument) ? Array.Empty<string>() : new[] { argument };
            var parse = ParseSelection(args);
            var exp = parse.CommandResult.Command.Options.OfType<Option<bool>>().First(o => o.Name == "--exp");
            Assert.Equal(expectedValue, parse.GetValue(exp));
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("--filter= x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("/filter: x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        public void when_parsing_expression_then_exppression_is_set(string argument, string expectedValue)
        {
            var args = string.IsNullOrEmpty(argument) ? Array.Empty<string>() : new[] { argument };
            var filter = GetFilter(ParseSelection(args));
            Assert.Equal(expectedValue, filter.Expression);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("all", true)]
        [InlineData("All", true)]
        [InlineData("--all", true)]
        public void when_parsing_all_argument_then_all_is_set(string argument, bool expectedValue)
        {
            var args = string.IsNullOrEmpty(argument) ? Array.Empty<string>() : new[] { argument };
            var filter = GetFilter(ParseSelection(args));
            Assert.Equal(expectedValue, filter.All);
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--version=18.7", "18.7")]
        [InlineData("-v:18.7.3", "18.7.3")]
        [InlineData("--version", "18")]
        public void when_parsing_version_argument_then_version_is_set(string argument, string expectedValue)
        {
            var args = string.IsNullOrEmpty(argument)
                ? Array.Empty<string>()
                : argument == "--version"
                    ? new[] { "--version", "18" }
                    : new[] { argument };
            var filter = GetFilter(ParseSelection(args));
            Assert.Equal(expectedValue, filter.Version);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("first", true)]
        [InlineData("First", true)]
        [InlineData("--first", true)]
        public void when_parsing_first_argument_then_first_is_set(string argument, bool expectedValue)
        {
            var args = string.IsNullOrEmpty(argument) ? Array.Empty<string>() : new[] { argument };
            var filter = GetFilter(ParseSelection(args));
            Assert.Equal(expectedValue, filter.First);
        }

        [Theory]
        [InlineData(new[] { "enterprise", "insiders" }, Sku.Enterprise, Channel.Insiders, false, null, null)]
        [InlineData(new[] { "main", "exp" }, null, Channel.Main, true, null, null)]
        [InlineData(new[] { "all", "exp" }, null, null, true, true, null)]
        [InlineData(new[] { "ent", "main" }, Sku.Enterprise, Channel.Main, false, null, null)]
        [InlineData(new[] { "main", "x => x.InstanceId == '123'" }, null, Channel.Main, false, null, "x => x.InstanceId == \"123\"")]
        [InlineData(new[] { "pro", "stable", "--nick=foo" }, Sku.Professional, Channel.Stable, false, null, null)]
        [InlineData(new[] { "build", "release" }, Sku.BuildTools, Channel.Stable, false, null, null)]
        [InlineData(new[] { "test", "stable" }, Sku.TestAgent, Channel.Stable, false, null, null)]
        public void when_parsing_arguments_then_arguments_are_set(
            string[] args, Sku? sku, Channel? channel, bool experimental, bool? all, string expression)
        {
            var parse = ParseSelection(args);
            var filter = GetFilter(parse);
            var exp = parse.CommandResult.Command.Options.OfType<Option<bool>>().First(o => o.Name == "--exp");

            Assert.Equal(sku, filter.Sku);
            Assert.Equal(channel, filter.Channel);
            Assert.Equal(experimental, parse.GetValue(exp));
            if (all.HasValue)
                Assert.Equal(all.Value, filter.All);
            if (expression != null)
                Assert.Equal(expression, filter.Expression);

            if (args.Any(a => a.StartsWith("--nick=")))
            {
                var nick = parse.CommandResult.Command.Options.OfType<Option<string>>().First(o => o.Name == "--nick");
                Assert.Equal("foo", parse.GetValue(nick));
            }
        }
    }
}
