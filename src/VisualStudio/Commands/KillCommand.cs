using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vswhere;

namespace Devlooped;

class KillCommand : Command
{
    readonly WhereService whereService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> experimentalOption;
    readonly Option<bool> firstOption;
    readonly Option<bool> allOption;

    public KillCommand(WhereService whereService)
        : base(Commands.Kill, "Kills running devenv processes.")
    {
        this.whereService = whereService;

        channelOptions = SharedOptions.AddChannelOptions(this, "kill");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        experimentalOption = SharedOptions.ExperimentalOption("kill");
        firstOption = SharedOptions.FirstOption("kill");
        allOption = SharedOptions.AllOption("kill");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(experimentalOption);
        Options.Add(firstOption);
        Options.Add(allOption);

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var filter = CommandHelpers.GetFilter(parse, channelOptions, skuOption, filterOption, firstOption, allOption);
        var isExperimental = parse.GetValue(experimentalOption);
        var killAll = parse.GetValue(allOption);

        var devenvProcesses = Process.GetProcessesByName("devenv").ToList();
        var targetProcesses =
            (from instance in await whereService.GetAllInstancesAsync(filter)
             from devenvProcess in devenvProcesses
             where Match(devenvProcess, instance, isExperimental)
             select devenvProcess).Distinct().ToList();

        if (!killAll)
            targetProcesses = new Chooser("kill").ChooseMany(targetProcesses, output).ToList();

        foreach (var process in targetProcesses)
        {
            output.WriteLine($"Killing {process.MainWindowTitle} ({process.Id})...");
            process.Kill();
        }
    }

    static bool Match(Process devenvProcess, VisualStudioInstance instance, bool isExperimental) =>
        devenvProcess.MainModule.FileName.StartsWith(instance.InstallationPath, StringComparison.OrdinalIgnoreCase) &&
        (!isExperimental || devenvProcess.GetCommandLine().Contains("/rootSuffix Exp", StringComparison.OrdinalIgnoreCase));
}
