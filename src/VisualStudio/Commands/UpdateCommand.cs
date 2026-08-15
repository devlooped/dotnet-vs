using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vswhere;

namespace Devlooped;

class UpdateCommand : Command
{
    readonly WhereService whereService;
    readonly InstallerService installerService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> firstOption;
    readonly Option<bool> allOption;
    readonly Option<string> versionOption;

    public UpdateCommand(WhereService whereService, InstallerService installerService)
        : base(Commands.Update, "Updates an installation of Visual Studio.")
    {
        this.whereService = whereService;
        this.installerService = installerService;

        channelOptions = SharedOptions.AddChannelOptions(this, "Update");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        firstOption = SharedOptions.FirstOption("Update");
        allOption = SharedOptions.AllOption("Update");
        versionOption = SharedOptions.VersionOption("Update");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(firstOption);
        Options.Add(allOption);
        Options.Add(versionOption);

        TreatUnmatchedTokensAsErrors = false;

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var filter = CommandHelpers.GetFilter(parse, channelOptions, skuOption, filterOption, firstOption, allOption, versionOption);
        var all = parse.GetValue(allOption);
        var extraArgs = parse.UnmatchedTokens.ToList();

        var instances = await whereService.GetAllInstancesAsync(filter);

        if (!all)
            instances = new Chooser().ChooseMany(instances, output);

        var extra =
            !extraArgs.Any(x => x.TrimStart('-') == "config") && File.Exists(".vsconfig") ?
            extraArgs.Concat(new[] { "--config", ".vsconfig" }).ToList() :
            extraArgs;

        foreach (var instance in instances)
        {
            var args = new List<string>(extra)
            {
                "--passive",
                "--installPath",
                instance.InstallationPath
            };

            var channel = instance.GetChannel();
            if (channel != null)
                await installerService.UpdateAsync(instance.InstallationVersion.Major.ToString(), channel, instance.GetSku(), args, output);
            else
                await installerService.UpdateAsync(instance.ChannelUri.Replace("/channel", ""), instance.GetSku(), args, output);
        }
    }
}
