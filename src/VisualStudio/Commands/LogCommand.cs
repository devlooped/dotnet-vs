using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Devlooped;

class LogCommand : Command
{
    readonly WhereService whereService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> experimentalOption;
    readonly Option<bool> firstOption;

    public LogCommand(WhereService whereService)
        : base(Commands.Log, "Opens the folder containing the Activity.log file.")
    {
        this.whereService = whereService;

        channelOptions = SharedOptions.AddChannelOptions(this, "open");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        experimentalOption = SharedOptions.ExperimentalOption("open");
        firstOption = SharedOptions.FirstOption("open");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(experimentalOption);
        Options.Add(firstOption);

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var filter = CommandHelpers.GetFilter(parse, channelOptions, skuOption, filterOption, firstOption);
        var isExperimental = parse.GetValue(experimentalOption);

        var instances = await whereService.GetAllInstancesAsync(filter);
        var instance = new Chooser().Choose(instances, output);

        if (instance != null)
        {
            var instanceDir = instance.InstallationVersion.Major + ".0_" + instance.InstanceId;
            if (isExperimental)
                instanceDir += "Exp";

            var path = Path.Combine(
                Environment.ExpandEnvironmentVariables("%AppData%"),
                @"Microsoft\VisualStudio",
                instanceDir,
                "ActivityLog.xml");

            if (File.Exists(path))
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select, \"{path}\"") { UseShellExecute = true });
        }
    }
}
