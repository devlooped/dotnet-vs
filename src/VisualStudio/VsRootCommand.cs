using System.CommandLine;
using System.CommandLine.Help;
using System.Linq;

namespace Devlooped;

/// <summary>
/// Root command for the vs CLI, registering all subcommands and shared help behavior.
/// </summary>
class VsRootCommand : RootCommand
{
    public WhereService WhereService { get; }
    public InstallerService InstallerService { get; }

    public VsRootCommand()
        : this(new WhereService(), new InstallerService())
    {
    }

    public VsRootCommand(WhereService whereService, InstallerService installerService)
        : base("A global tool for managing Visual Studio installations")
    {
        WhereService = whereService;
        InstallerService = installerService;

        // Public commands
        var run = new RunCommand(whereService);
        var where = new WhereCommand(whereService);
        var install = new InstallCommand(installerService);
        var update = new UpdateCommand(whereService, installerService);
        var modify = new ModifyCommand(whereService, installerService);
        var kill = new KillCommand(whereService);
        var config = new ConfigCommand(whereService);
        var log = new LogCommand(whereService);
        var alias = new AliasCommand();
        var client = new ClientCommand(whereService);

        Subcommands.Add(run);
        Subcommands.Add(where);
        Subcommands.Add(install);
        Subcommands.Add(update);
        Subcommands.Add(modify);
        Subcommands.Add(kill);
        Subcommands.Add(config);
        Subcommands.Add(log);
        Subcommands.Add(alias);
        Subcommands.Add(client);

        // System / hidden commands
        var publicCommands = Subcommands.Where(c => !c.Hidden).ToList();
        Subcommands.Add(new GenerateReadmeCommand(publicCommands));
        Subcommands.Add(new SaveCommand());
        Subcommands.Add(new UpdateSelfCommand());

        // Replace default help action to append Examples sections
        foreach (var help in Options.OfType<HelpOption>())
            help.Action = new ExamplesHelpAction();

        // Remove built-in --version; Program handles it only as the first argument.
        var versionOpt = Options.FirstOrDefault(o => o.Name == "--version");
        if (versionOpt != null)
            Options.Remove(versionOpt);

        // Recursive debug option for error handling detection (not required on every command)
        Options.Add(SharedOptions.DebugOption());
    }
}
