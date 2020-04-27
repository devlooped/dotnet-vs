using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class LogCommand : Command<LogCommandDescriptor>
    {
        readonly WhereService whereService;

        public LogCommand(LogCommandDescriptor descriptor, WhereService whereService) : base(descriptor)
        {
            this.whereService = whereService;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var instances = await whereService.GetAllInstancesAsync(Descriptor.Sku, Descriptor.Channel);
            var instance = new VisualStudioInstanceChooser().Choose(instances, output);

            if (instance != null)
            {
                var instanceDir = instance.InstallationVersion.Major + ".0_" + instance.InstanceId;
                if (Descriptor.Experimental)
                    instanceDir += "Exp";

                var path = Path.Combine(
                    Environment.ExpandEnvironmentVariables("%AppData%"),
                    @"Microsoft\VisualStudio",
                    instanceDir,
                    "ActivityLog.xml");

                if (File.Exists(path))
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
        }
    }
}
