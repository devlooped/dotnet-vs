using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using vswhere;

namespace VisualStudio
{
    class KillCommand : Command<KillCommandDescriptor>
    {
        readonly WhereService whereService;

        public KillCommand(KillCommandDescriptor descriptor, WhereService whereService) : base(descriptor) =>
            this.whereService = whereService;

        public override async Task ExecuteAsync(TextWriter output)
        {
            var devenvProcesses = Process.GetProcessesByName("devenv").ToList();
            var targetProcesses =
                (from instance in await whereService.GetAllInstancesAsync(Descriptor.Options)
                 from devenvProcess in devenvProcesses
                 where Match(devenvProcess, instance)
                 select devenvProcess).Distinct().ToList();

            if (!Descriptor.KillAll)
                targetProcesses = new Chooser("kill").ChooseMany(targetProcesses, output).ToList();

            foreach (var process in targetProcesses)
            {
                output.WriteLine($"Killing {process.MainWindowTitle} ({process.Id})...");
                process.Kill();
            }
        }

        bool Match(Process devenvProcess, VisualStudioInstance instance) =>
            devenvProcess.MainModule.FileName.StartsWith(instance.InstallationPath, StringComparison.OrdinalIgnoreCase) &&
            (!Descriptor.IsExperimental || devenvProcess.GetCommandLine().Contains("/rootSuffix Exp", StringComparison.OrdinalIgnoreCase));
    }
}
