using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Devlooped;

class UpdateSelfCommand : Command
{
    public UpdateSelfCommand()
        : base(Commands.System.UpdateSelf, "Updates the dotnet-vs tool itself")
    {
        Hidden = true;

        SetAction((parseResult, _) =>
        {
            Execute(parseResult.InvocationConfiguration.Output);
            return Task.FromResult(0);
        });
    }

    static void Execute(TextWriter output)
    {
        Process.Start(
            new ProcessStartInfo("dotnet")
            {
                ArgumentList = { "tool", "update", "-g", "dotnet-vs" }
            });

        output.WriteLine("Running \"dotnet tool update -g dotnet-vs\"...");
        output.WriteLine("dotnet will continue running in background");
    }
}
