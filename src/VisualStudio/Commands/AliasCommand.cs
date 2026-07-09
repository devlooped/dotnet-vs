using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Devlooped;

class AliasCommand : Command
{
    public AliasCommand()
        : base(Commands.Alias, "Shows the list of saved aliases")
    {
        SetAction((parseResult, _) =>
        {
            Execute(parseResult.InvocationConfiguration.Output);
            return Task.FromResult(0);
        });
    }

    static void Execute(TextWriter output)
    {
        output.WriteLine("Saved aliases:");

        var entries = Commands.DotNetConfig
            .GetConfig()
            .Where(x => x.Section == Commands.DotNetConfig.Section && x.Subsection == Commands.DotNetConfig.SubSection)
            .ToList();

        if (entries.Count == 0)
            return;

        var maxWidth = entries.Select(x => x.Variable.Length).Max() + 5;
        foreach (var entry in entries)
            output.WriteLine($"  {entry.Variable.GetNormalizedString(maxWidth)}{entry.RawValue.Replace("|", " ")}");
    }
}
