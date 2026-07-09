using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Devlooped;

class ConfigCommand : Command
{
    readonly WhereService whereService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> experimentalOption;
    readonly Option<bool> firstOption;

    public ConfigCommand(WhereService whereService)
        : base(Commands.Config, "Opens the config folder.")
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
        var experimental = parse.GetValue(experimentalOption);

        var instances = await whereService.GetAllInstancesAsync(filter);
        var instance = new Chooser("open").Choose(instances, output);

        if (instance != null)
        {
            var instanceDir = instance.InstallationVersion.Major + ".0_" + instance.InstanceId;
            if (experimental)
                instanceDir += "Exp";

            var path = Path.Combine(
                Environment.ExpandEnvironmentVariables("%LocalAppData%"),
                @"Microsoft\VisualStudio",
                instanceDir);

            if (Directory.Exists(path))
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
    }
}
