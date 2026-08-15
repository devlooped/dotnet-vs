using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Devlooped;

class InstallCommand : Command
{
    readonly InstallerService installerService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<string> nicknameOption;
    readonly Option<string> versionOption;
    readonly Option<string[]> addOption = new("--add")
    {
        Description = "A workload ID",
    };

    public InstallCommand(InstallerService installerService)
        : base(Commands.Install, "Installs a specific edition of Visual Studio.")
    {
        this.installerService = installerService;

        channelOptions = SharedOptions.AddChannelOptions(this, "install");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        nicknameOption = SharedOptions.NicknameOption();
        versionOption = SharedOptions.VersionOption("Install");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(nicknameOption);
        Options.Add(versionOption);
        Options.Add(addOption);

        TreatUnmatchedTokensAsErrors = false;

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var channel = channelOptions.GetChannel(parse);
        var sku = SharedOptions.ParseSku(parse.GetValue(skuOption)) ?? Sku.Community;
        var nickname = parse.GetValue(nicknameOption);
        var version = parse.GetValue(versionOption);
        var workloads = CommandHelpers.GetWorkloadIds(parse, addOption);
        var extra = parse.UnmatchedTokens;

        var args = new List<string>();
        args.AddRange(CommandHelpers.ToWorkloadArgs("add", workloads));

        if (!string.IsNullOrEmpty(nickname))
        {
            args.Add("--nickname");
            args.Add(nickname);
        }

        if (!extra.Any(x => x.TrimStart('-') == "config") && File.Exists(".vsconfig"))
            args.AddRange(new[] { "--config", ".vsconfig" });

        args.AddRange(extra);

        var vs = VisualStudioVersion.GetMajor(version) ?? await installerService.GetLatestMajorAsync();
        var installBase = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Microsoft Visual Studio",
            vs);

        if (Directory.Exists(installBase) && !args.Contains("--nickname"))
        {
            args.Add("--nickname");
            args.Add(ChannelFolderName(channel) ?? sku.ToString().Substring(0, 3));
        }

        var installPath = Path.Combine(installBase, sku.ToString());
        var customPath = Directory.Exists(installPath);
        if (customPath)
        {
            installPath = Path.Combine(installBase, ChannelFolderName(channel) ?? sku.ToString());
            if (Directory.Exists(installPath))
            {
                var prefix = channel == Channel.Insiders ? "Pre" :
                    channel == Channel.IntPreview ? "Int" : string.Empty;
                installPath = Path.Combine(installBase, prefix + sku.ToString());
            }
        }

        if (customPath)
        {
            args.Add("--installPath");
            args.Add(installPath);
        }

        await installerService.InstallAsync(vs, channel, sku, args, output);
    }

    static string ChannelFolderName(Channel? channel) =>
        channel switch
        {
            Channel.Insiders => "Insiders",
            Channel.IntPreview => "IntPreview",
            Channel.Main => "main",
            _ => null,
        };
}
