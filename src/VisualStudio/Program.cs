using System;
using System.CommandLine;
using System.CommandLine.Help;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Devlooped;

partial class Program
{
    static readonly VersionChecker versionChecker = new();

    readonly TextWriter output;
    readonly string[] args;
    readonly VsRootCommand rootCommand;
    readonly ArgumentPreprocessor preprocessor;
    readonly CancellationTokenSource cts = new();

    static Task<int> Main(string[] args)
    {
        var program = new Program(Console.Out, args);

        Console.CancelKeyPress += async (sender, e) =>
        {
            e.Cancel = true;
            await program.CancelAsync();
        };

        return program.RunAsync();
    }

    public Program(TextWriter output, params string[] args)
        : this(output, new VsRootCommand(), new ArgumentPreprocessor(), args)
    {
    }

    public Program(TextWriter output, VsRootCommand rootCommand, ArgumentPreprocessor preprocessor, params string[] args)
    {
        this.output = output;
        this.args = args ?? Array.Empty<string>();
        this.rootCommand = rootCommand;
        this.preprocessor = preprocessor;
    }

    // Back-compat constructor used by older tests
    public Program(TextWriter output, ArgumentPreprocessor preprocessor, params string[] args)
        : this(output, new VsRootCommand(), preprocessor, args)
    {
    }

    public Task CancelAsync()
    {
        ClientCommand.ActiveSession?.Dispose();
        cts.Cancel();
        return Task.CompletedTask;
    }

    public async Task<int> RunAsync()
    {
        try
        {
            if (preprocessor.IsTopLevelHelp(args))
            {
                ShowUsage();
                return 0;
            }

            if (preprocessor.IsTopLevelVersion(args))
            {
                await ShowVersion();
                return 0;
            }

            try
            {
                var processed = preprocessor.Process(args);
                var parseResult = rootCommand.Parse(processed);

                var config = new InvocationConfiguration
                {
                    Output = output,
                    Error = output,
                    EnableDefaultExceptionHandler = false,
                };

                var exitCode = await parseResult.InvokeAsync(config, cts.Token);

                // Help / non-success: do not run version update check (matches prior behavior).
                if (exitCode != 0 || parseResult.Action is HelpAction or ExamplesHelpAction)
                    return exitCode;
            }
            catch (Exception ex) when (!SharedOptions.IsDebug(args))
            {
                output.WriteLine(ex.Message);
                return ErrorCodes.Error;
            }

            await versionChecker.ShowUpdateAsync(output);
            return 0;
        }
        finally
        {
            WriteLegacyMigrationNotice(output);
        }
    }

    protected bool NoVersionChecks
    {
        get => versionChecker.NoOp;
        set => versionChecker.NoOp = value;
    }

    protected virtual async Task ShowVersion() => await versionChecker.ShowVersionAsync(output);

    protected virtual void ShowUsage()
    {
        output.WriteLine();
        output.WriteLine($"Usage: dnx {ThisAssembly.Project.AssemblyName} -- [command] [options|-?|-h|--help] [--save=ALIAS[--global]]");
        output.WriteLine();
        output.WriteLine("Supported commands:");

        var commands = rootCommand.Subcommands.Where(c => !c.Hidden).OrderBy(c => c.Name).ToList();
        var maxWidth = commands.Select(x => x.Name.Length).DefaultIfEmpty(0).Max() + 5;
        foreach (var command in commands)
            output.WriteLine($"  {command.Name.GetNormalizedString(maxWidth)}{command.Description}");
    }

    /// <summary>
    /// Optional post-run hook implemented by the legacy <c>dotnet-vs</c> package only.
    /// </summary>
    partial void WriteLegacyMigrationNotice(TextWriter output);
}
