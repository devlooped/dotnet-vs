using System;
using System.Collections.Generic;
using System.Linq;
using DotNetConfig;

namespace Devlooped;

/// <summary>
/// Front-end routing that mirrors the old <c>CommandFactory.CreateCommandAsync</c> behavior:
/// saved-alias expansion, <c>--save</c> rerouting, <c>update --self</c>, and default <c>run</c> fallback.
/// Also normalizes legacy top-level help tokens.
/// </summary>
class ArgumentPreprocessor
{
    static readonly HashSet<string> TopLevelHelp = new(StringComparer.OrdinalIgnoreCase)
    {
        "?", "-?", "/?", "-h", "/h", "--help", "/help",
    };

    static readonly HashSet<string> KnownCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        Commands.Run,
        Commands.Where,
        Commands.Install,
        Commands.Update,
        Commands.Modify,
        Commands.Kill,
        Commands.Config,
        Commands.Log,
        Commands.Alias,
        Commands.Client,
        Commands.System.GenerateReadme,
        Commands.System.Save,
        Commands.System.UpdateSelf,
    };

    readonly Config config;
    readonly HashSet<string> commands;

    public ArgumentPreprocessor()
        : this(Commands.DotNetConfig.GetConfig())
    {
    }

    public ArgumentPreprocessor(Config config, IEnumerable<string> extraCommands = null)
    {
        this.config = config;
        commands = new HashSet<string>(KnownCommands, StringComparer.OrdinalIgnoreCase);
        if (extraCommands != null)
        {
            foreach (var c in extraCommands)
                commands.Add(c);
        }
    }

    public bool IsTopLevelHelp(string[] args) =>
        args.Length != 0 && TopLevelHelp.Contains(args[0]);

    public bool IsTopLevelVersion(string[] args) =>
        args.Length != 0 && IsVersionToken(args[0]);

    static bool IsVersionToken(string token) =>
        token.Equals("--version", StringComparison.OrdinalIgnoreCase) ||
        token.Equals("-version", StringComparison.OrdinalIgnoreCase) ||
        token.Equals("/version", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns tokens ready for <see cref="System.CommandLine.RootCommand.Parse(System.Collections.Generic.IReadOnlyList{string})"/>,
    /// including the command name as the first token.
    /// </summary>
    public string[] Process(string[] args)
    {
        if (args == null || args.Length == 0)
            return [Commands.Run];

        // Normalize legacy help as first token only when it's clearly top-level help
        if (TopLevelHelp.Contains(args[0]))
            return ["--help"];

        var command = args[0];
        var rest = args.Skip(1).ToArray();

        // Empty first-token edge case
        if (string.IsNullOrEmpty(command))
            return [Commands.Run];

        // Resolve command via known set or saved alias / run fallback (without consuming --save yet)
        if (!commands.Contains(command))
        {
            var saved = config.GetString(Commands.DotNetConfig.Section, Commands.DotNetConfig.SubSection, command);
            if (!string.IsNullOrEmpty(saved))
            {
                var savedArgs = saved.Split('|', StringSplitOptions.RemoveEmptyEntries);
                var savedCommandName = savedArgs.FirstOrDefault();
                if (!string.IsNullOrEmpty(savedCommandName) && commands.Contains(savedCommandName))
                {
                    command = savedCommandName;
                    rest = savedArgs.Skip(1).ToArray();
                }
                else
                {
                    // Saved payload is run-style args
                    command = Commands.Run;
                    rest = savedArgs;
                }
            }
            else
            {
                // Default command: prepend unknown token into run args
                rest = args;
                command = Commands.Run;
            }
        }

        // --save=ALIAS on any command → hidden save, original command prepended
        if (HasSaveOption(rest))
        {
            rest = new[] { command }.Concat(rest).ToArray();
            command = Commands.System.Save;
        }
        // update --self / update self → update-self
        else if (command.Equals(Commands.Update, StringComparison.OrdinalIgnoreCase) && HasSelfOption(rest))
        {
            command = Commands.System.UpdateSelf;
            rest = rest.Where(a => !IsSelfToken(a)).ToArray();
        }

        // Per-command token rewriting
        rest = RewriteForCommand(command, rest);

        return new[] { command }.Concat(rest).ToArray();
    }

    public static string[] RewriteForCommand(string command, string[] args) =>
        command.ToLowerInvariant() switch
        {
            Commands.Run or Commands.Where or Commands.Kill or Commands.Config or Commands.Log or Commands.Client
                => TokenRewriter.RewriteRequiresWorkloads(args),
            Commands.Install => TokenRewriter.RewriteInstallWorkloads(args),
            Commands.Modify => TokenRewriter.RewriteModifyWorkloads(args),
            Commands.Update => TokenRewriter.RewriteVisualStudioSelection(args),
            Commands.System.Save => TokenRewriter.RewriteLegacyOptionPrefixes(args),
            _ => TokenRewriter.RewriteLegacyOptionPrefixes(args),
        };

    static bool HasSaveOption(IEnumerable<string> args) =>
        args.Any(IsSaveToken);

    static bool IsSaveToken(string arg) =>
        arg.Equals("--save", StringComparison.OrdinalIgnoreCase) ||
        arg.Equals("-save", StringComparison.OrdinalIgnoreCase) ||
        arg.StartsWith("--save=", StringComparison.OrdinalIgnoreCase) ||
        arg.StartsWith("-save=", StringComparison.OrdinalIgnoreCase) ||
        arg.StartsWith("--save:", StringComparison.OrdinalIgnoreCase) ||
        arg.StartsWith("-save:", StringComparison.OrdinalIgnoreCase);

    static bool HasSelfOption(IEnumerable<string> args) =>
        args.Any(IsSelfToken);

    static bool IsSelfToken(string arg) =>
        arg.Equals("self", StringComparison.OrdinalIgnoreCase) ||
        arg.Equals("--self", StringComparison.OrdinalIgnoreCase) ||
        arg.Equals("-self", StringComparison.OrdinalIgnoreCase);
}
