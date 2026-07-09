using System;
using System.CommandLine;
using System.Linq;

namespace Devlooped;

/// <summary>
/// Factory for shared System.CommandLine options used across commands.
/// </summary>
static class SharedOptions
{
    public static Option<bool> StableOption(string verb) =>
        new("--stable") { Description = verb + " stable version" };

    public static Option<bool> RelOption(string verb) =>
        new("--rel", "--release")
        {
            Description = verb + " stable version",
            Hidden = true,
        };

    public static Option<bool> InsidersOption(string verb) =>
        new("--insiders") { Description = verb + " insiders version" };

    public static Option<bool> PreviewOption(string verb) =>
        new("--pre", "--preview")
        {
            Description = verb + " insiders version",
            Hidden = true,
        };

    public static Option<bool> InternalOption(string verb) =>
        new("--int", "--internal") { Description = verb + " internal (aka 'dogfood') version" };

    public static Option<bool> MainOption(string verb) =>
        new("--main")
        {
            Description = verb + " main version",
            Hidden = true,
        };

    public static Option<string> SkuOption() =>
        new("--sku")
        {
            Description = "Edition, one of [e|ent|enterprise], [p|pro|professional], [c|com|community], [b|build|buildtools] or [t|test|testagent]",
        };

    public static Option<bool> ExperimentalOption(string verb) =>
        new("--exp", "--experimental")
        {
            Description = $"{verb} experimental instance instead of regular.",
        };

    public static Option<string> FilterOption() =>
        new("--filter")
        {
            Description = "Expression to filter VS instances. E.g. `x => x.InstanceId = '123'`",
        };

    public static Option<bool> FirstOption(string verb) =>
        new("--first")
        {
            Description = $"{verb} first matching instance.",
        };

    public static Option<bool> AllOption(string verb) =>
        new("--all")
        {
            Description = $"{verb} all instances.",
        };

    public static Option<string> NicknameOption() =>
        new("--nick", "--nickname")
        {
            Description = "Optional nickname to use",
        };

    public static Option<string> RequiresOption() =>
        new("--requires")
        {
            Description = "A workload ID",
            AllowMultipleArgumentsPerToken = false,
        };

    public static Option<string> AddWorkloadOption() =>
        new("--add")
        {
            Description = "A workload ID",
        };

    public static Option<string> RemoveWorkloadOption() =>
        new("--remove")
        {
            Description = "A workload ID",
        };

    public static Option<string> SaveOption() =>
        new("--save")
        {
            Description = "Saves a command to be executed with a given alias",
        };

    public static Option<bool> GlobalOption() =>
        new("--global")
        {
            Description = "Global option",
        };

    public static Option<bool> DebugOption() =>
        new("--debug")
        {
            Description = "Execute command in debug mode",
            Recursive = true,
            Hidden = true,
        };

    /// <summary>
    /// Adds the standard channel flags to a command and returns them for reading.
    /// </summary>
    public static ChannelOptions AddChannelOptions(Command command, string verb)
    {
        var opts = new ChannelOptions(
            StableOption(verb),
            RelOption(verb),
            InsidersOption(verb),
            PreviewOption(verb),
            InternalOption(verb),
            MainOption(verb));

        command.Options.Add(opts.Stable);
        command.Options.Add(opts.Rel);
        command.Options.Add(opts.Insiders);
        command.Options.Add(opts.Preview);
        command.Options.Add(opts.Internal);
        command.Options.Add(opts.Main);
        return opts;
    }

    public static VisualStudioFilter GetFilter(
        ParseResult parse,
        ChannelOptions channel,
        Option<string> sku,
        Option<string> filter = null,
        Option<bool> first = null,
        Option<bool> all = null)
    {
        return new VisualStudioFilter(
            Channel: channel.GetChannel(parse),
            Sku: ParseSku(parse.GetValue(sku)),
            Expression: filter != null ? parse.GetValue(filter) : null,
            First: first != null && parse.GetValue(first),
            All: all != null && parse.GetValue(all));
    }

    public static Sku? ParseSku(string sku)
    {
        if (string.IsNullOrEmpty(sku))
            return null;

        if (sku.StartsWith("e", StringComparison.OrdinalIgnoreCase))
            return Devlooped.Sku.Enterprise;
        if (sku.StartsWith("p", StringComparison.OrdinalIgnoreCase))
            return Devlooped.Sku.Professional;
        if (sku.StartsWith("c", StringComparison.OrdinalIgnoreCase))
            return Devlooped.Sku.Community;
        if (sku.StartsWith("b", StringComparison.OrdinalIgnoreCase))
            return Devlooped.Sku.BuildTools;
        if (sku.StartsWith("t", StringComparison.OrdinalIgnoreCase))
            return Devlooped.Sku.TestAgent;

        throw new ArgumentException(
            $"Invalid SKU {sku}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Sku)).Select(x => x.ToLowerInvariant()))}.");
    }

    public static bool IsDebug(string[] args) =>
        args != null && args.Any(a => a.Equals("--debug", StringComparison.OrdinalIgnoreCase));

    public record ChannelOptions(
        Option<bool> Stable,
        Option<bool> Rel,
        Option<bool> Insiders,
        Option<bool> Preview,
        Option<bool> Internal,
        Option<bool> Main)
    {
        public Channel? GetChannel(ParseResult parse)
        {
            if (parse.GetValue(Stable) || parse.GetValue(Rel))
                return Channel.Stable;
            if (parse.GetValue(Insiders) || parse.GetValue(Preview))
                return Channel.Insiders;
            if (parse.GetValue(Internal))
                return Channel.IntPreview;
            if (parse.GetValue(Main))
                return Channel.Main;
            return null;
        }
    }
}
