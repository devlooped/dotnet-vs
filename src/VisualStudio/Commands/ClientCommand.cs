using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevEnv = vswhere.VisualStudioInstance;

namespace Devlooped;

class ClientCommand : Command
{
    const string JoinToken = "/join?";

    readonly WhereService whereService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> experimentalOption;
    readonly Option<bool> firstOption;
    readonly Option<string> workspaceIdOption = new("--workspaceId", "-w")
    {
        Description = "The workspace ID to connect to",
    };

    // Tracked for Ctrl+C disposal when this command is the active handler
    internal static ClientSession ActiveSession { get; set; }

    public ClientCommand(WhereService whereService)
        : base(Commands.Client, "Launches Visual Studio in client mode")
    {
        this.whereService = whereService;

        channelOptions = SharedOptions.AddChannelOptions(this, "Run");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        experimentalOption = SharedOptions.ExperimentalOption("Run");
        firstOption = SharedOptions.FirstOption("Run");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(experimentalOption);
        Options.Add(firstOption);
        Options.Add(workspaceIdOption);

        TreatUnmatchedTokensAsErrors = false;

        SetAction(async (parseResult, cancellationToken) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output, cancellationToken);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output, CancellationToken cancellationToken)
    {
        var filter = CommandHelpers.GetFilter(parse, channelOptions, skuOption, filterOption, firstOption);
        var isExperimental = parse.GetValue(experimentalOption);
        var workspaceId = parse.GetValue(workspaceIdOption);
        var extra = parse.UnmatchedTokens.ToList();

        var devenv = new Chooser().Choose(await whereService.GetAllInstancesAsync(filter), output);
        if (devenv == null)
            return;

        using var session = new ClientSession();
        ActiveSession = session;
        try
        {
            using var reg = cancellationToken.Register(() => session.Dispose());

            if (string.IsNullOrEmpty(workspaceId))
                StartServerAndClient(session, devenv, extra, isExperimental, output);
            else
                StartClient(session, devenv, workspaceId, isExperimental, output);
        }
        finally
        {
            if (ActiveSession == session)
                ActiveSession = null;
        }
    }

    void StartClient(ClientSession session, DevEnv devenv, string workspaceId, bool isExperimental, TextWriter output) =>
        StartClientForTests(session, devenv, workspaceId, isExperimental, output);

    void StartServerAndClient(ClientSession session, DevEnv devenv, List<string> extra, bool isExperimental, TextWriter output) =>
        StartServerAndClientForTests(session, devenv, extra, isExperimental, output);

    // Internal entry points used by unit tests (and by private wrappers above).
    internal void StartClientForTests(ClientSession session, DevEnv devenv, string workspaceId, bool isExperimental, TextWriter output)
    {
        var args = new List<string>();

        if (isExperimental)
            args.AddRange(new[] { "/rootSuffix", "Exp" });

        args.Add("/client");
        args.AddRange(new[] { "/joinworkspace", $"vsls:?workspaceId={workspaceId}&remoteJoin=true" });

        output.WriteLine($"Starting client: {devenv.ProductPath} {string.Join(" ", args)}");
        session.Client = CreateProcess(devenv, args);
    }

    internal void StartServerAndClientForTests(ClientSession session, DevEnv devenv, List<string> extra, bool isExperimental, TextWriter output)
    {
        var args = new List<string>(extra);

        if (!args.Any())
            args.Add(Directory.GetCurrentDirectory());

        if (isExperimental)
            args.AddRange(new[] { "/rootSuffix", "Exp" });

        args.Add("/server");

        output.WriteLine($"Starting server: {devenv.ProductPath} {string.Join(" ", args)}");
        session.Server = CreateProcess(devenv, args);

        foreach (var line in ReadOutputLines(session.Server))
        {
            output.WriteLine("[devenv] " + line);

            if (line.LastIndexOf(JoinToken) is int joinIndexOf && joinIndexOf != -1)
                StartClientForTests(session, devenv, line.Substring(joinIndexOf + JoinToken.Length), isExperimental, output);
        }
    }

    protected virtual Process CreateProcess(DevEnv devenv, IEnumerable<string> args, bool start = true)
    {
        var psi = new ProcessStartInfo(devenv.ProductPath)
        {
            RedirectStandardOutput = true,
        };

        if (args != null)
        {
            foreach (var arg in args)
                psi.ArgumentList.Add(arg);
        }

        var process = new Process { StartInfo = psi };

        if (start)
            process.Start();

        return process;
    }

    protected virtual IEnumerable<string> ReadOutputLines(Process process)
    {
        string line;
        while ((line = process.StandardOutput.ReadLine()) != null)
            yield return line;
    }

    internal sealed class ClientSession : IDisposable
    {
        public Process Server { get; set; }
        public Process Client { get; set; }

        public void Dispose() => Server?.Kill();
    }
}
