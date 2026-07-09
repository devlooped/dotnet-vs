using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Devlooped;

class SaveCommand : Command
{
    readonly Option<string> saveOption = SharedOptions.SaveOption();
    readonly Option<bool> globalOption = SharedOptions.GlobalOption();

    public SaveCommand()
        : base(Commands.System.Save, "Saves a command to be executed with a given alias")
    {
        Hidden = true;
        Options.Add(saveOption);
        Options.Add(globalOption);
        TreatUnmatchedTokensAsErrors = false;

        SetAction((parseResult, _) =>
        {
            Execute(parseResult, parseResult.InvocationConfiguration.Output);
            return Task.FromResult(0);
        });
    }

    void Execute(ParseResult parse, TextWriter output)
    {
        var alias = parse.GetValue(saveOption);
        var global = parse.GetValue(globalOption);
        // Extra arguments are the original command + its args (save option already consumed)
        var extra = parse.UnmatchedTokens
            .Where(t => !t.StartsWith("--save") && !t.Equals("--global", System.StringComparison.OrdinalIgnoreCase))
            .ToArray();

        // Also filter tokens that were parsed as options from the joined list — UnmatchedTokens is correct.
        output.WriteLine($"Saving '{string.Join(" ", extra)}' as '{alias}'...");

        Commands.DotNetConfig
            .GetConfig(global)
            .SetString(Commands.DotNetConfig.Section, Commands.DotNetConfig.SubSection, alias, string.Join('|', extra), null);
    }
}
