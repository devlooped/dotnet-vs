using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vswhere;

namespace Devlooped;

class RunCommand : Command
{
    static readonly ToolSettings settings = new(ThisAssembly.Project.AssemblyName);

    readonly WhereService whereService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> experimentalOption;
    readonly Option<string> idOption = new("--id") { Description = "Run a specific instance by its ID" };
    readonly Option<bool> firstOption = new("--first", "-f")
    {
        Description = "If more than one instance matches the criteria, run the first one sorted by descending build version.",
    };
    readonly Option<string> versionOption = new("--version", "-v")
    {
        Description = "Run specific (semantic) version, such as 18.7 or 18.7.3",
    };
    readonly Option<bool> waitOption = new("--wait", "-w")
    {
        Description = "Wait for the started Visual Studio to exit.",
    };
    readonly Option<bool> nodeReuseOption = new("--nodereuse", "-nr")
    {
        Description = "Disable MSBuild node reuse. Useful when testing analyzers, tasks and targets. Defaults to true when running experimental instance.",
    };
    readonly Option<bool> defaultOption = new("--default")
    {
        Description = "Set as the default version to run when no arguments are provided, or remove the current default (with --default-).",
    };
    readonly Option<bool> clearDefaultOption = new("--clear-default")
    {
        Description = "Remove the current default instance.",
        Hidden = true,
    };
    readonly Option<string[]> requiresOption = new("--requires")
    {
        Description = "A workload ID",
    };

    public RunCommand(WhereService whereService)
        : base(Commands.Run, "This is default command, so typically it does not need to be provided as an argument.")
    {
        this.whereService = whereService;

        channelOptions = SharedOptions.AddChannelOptions(this, "run");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        experimentalOption = SharedOptions.ExperimentalOption("run");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(experimentalOption);
        Options.Add(idOption);
        Options.Add(firstOption);
        Options.Add(versionOption);
        Options.Add(waitOption);
        Options.Add(nodeReuseOption);
        Options.Add(defaultOption);
        Options.Add(clearDefaultOption);
        Options.Add(requiresOption);

        TreatUnmatchedTokensAsErrors = false;

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var emptyArguments = parse.Tokens.All(t => t.Type == System.CommandLine.Parsing.TokenType.Command);
        var unmatched = parse.UnmatchedTokens;
        var workloads = CommandHelpers.GetWorkloadIds(parse, requiresOption);
        var setDefault = parse.GetValue(clearDefaultOption) ? false
            : parse.GetValue(defaultOption) ? true
            : (bool?)null;

        var id = parse.GetValue(idOption);
        var version = parse.GetValue(versionOption);
        var first = parse.GetValue(firstOption);
        var wait = parse.GetValue(waitOption);
        var disableNodeReuse = parse.GetValue(nodeReuseOption);
        var isExperimental = parse.GetValue(experimentalOption);
        var filter = CommandHelpers.GetFilter(parse, channelOptions, skuOption, filterOption, firstOption);

        var devenv = settings.Get("devenv");
        if (!string.IsNullOrEmpty(devenv))
        {
            if (File.Exists(devenv) && emptyArguments)
            {
                Process.Start(devenv);
            }
            else if (!File.Exists(devenv))
            {
                settings.Set<string>("devenv", null);
            }
        }

        if (setDefault == false)
            settings.Set<string>("devenv", null);

        IEnumerable<VisualStudioInstance> instances = (await whereService
            .GetAllInstancesAsync(filter, extraArguments: CommandHelpers.ToWorkloadArgs("requires", workloads, "-")))
            .OrderByDescending(i => i.Catalog.BuildVersion);

        if (!string.IsNullOrEmpty(id))
        {
            instances = instances.Where(i => i.InstanceId.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
        else if (version != null)
        {
            instances = instances.Where(i => i.Catalog.ProductSemanticVersion.StartsWith(version));
        }

        var matches = instances.ToArray();
        if (matches.Length == 1 || (matches.Length > 0 && first))
        {
            devenv = matches[0].ProductPath;
        }
        else if (matches.Length == 0)
        {
            output.WriteLine("No installed Visual Studio found with the requested filters.");
            return;
        }
        else
        {
            var instance = new Chooser().Choose(instances, output);
            if (instance == null)
                return;

            devenv = instance.ProductPath;
        }

        var psi = new ProcessStartInfo(devenv);
        foreach (var arg in unmatched)
            psi.ArgumentList.Add(arg);

        if (isExperimental)
        {
            psi.ArgumentList.Add("/rootSuffix");
            psi.ArgumentList.Add("Exp");
        }

        if (disableNodeReuse || isExperimental)
            psi.EnvironmentVariables["MSBUILDDISABLENODEREUSE"] = "1";

        var log = psi.ArgumentList.FirstOrDefault(arg => "/log".Equals(arg, StringComparison.OrdinalIgnoreCase));
        if (log != null)
        {
            psi.ArgumentList.Remove(log);
            psi.ArgumentList.Add(log);
        }

        psi.Log(output);
        var process = Process.Start(psi);

        if (setDefault == true)
            settings.Set("devenv", devenv);

        if (wait)
            process.WaitForExit();
    }
}
