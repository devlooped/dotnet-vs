using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vswhere;

namespace Devlooped;

class ModifyCommand : Command
{
    readonly WhereService whereService;
    readonly InstallerService installerService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> firstOption;
    readonly Option<string[]> addOption = new("--add")
    {
        Description = "A workload ID",
    };
    readonly Option<string[]> removeOption = new("--remove")
    {
        Description = "A workload ID",
    };

    public ModifyCommand(WhereService whereService, InstallerService installerService)
        : base(Commands.Modify, "Modifies an installation of Visual Studio.")
    {
        this.whereService = whereService;
        this.installerService = installerService;

        channelOptions = SharedOptions.AddChannelOptions(this, "modify");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        firstOption = SharedOptions.FirstOption("modify");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(firstOption);
        Options.Add(addOption);
        Options.Add(removeOption);

        TreatUnmatchedTokensAsErrors = false;

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var filter = CommandHelpers.GetFilter(parse, channelOptions, skuOption, filterOption, firstOption);
        var added = CommandHelpers.GetWorkloadIds(parse, addOption);
        var removed = CommandHelpers.GetWorkloadIds(parse, removeOption);
        var extra = parse.UnmatchedTokens;

        var instances = await whereService.GetAllInstancesAsync(filter);
        var instance = new Chooser().Choose(instances, output);

        if (instance != null)
        {
            var args = new List<string>();

            if (added.Length > 0 || removed.Length > 0 || extra.Contains("--config"))
                args.Add("--passive");

            args.AddRange(CommandHelpers.ToWorkloadArgs("add", added));
            args.AddRange(CommandHelpers.ToWorkloadArgs("remove", removed));

            args.Add("--installPath");
            args.Add(instance.InstallationPath);

            args.AddRange(extra);

            var channel = instance.GetChannel();
            if (channel != null)
                await installerService.ModifyAsync(instance.InstallationVersion.Major.ToString(), channel, instance.GetSku(), args, output);
            else
                await installerService.ModifyAsync(instance.ChannelUri.Replace("/channel", ""), instance.GetSku(), args, output);
        }
    }
}
