using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class ConfigCommand : Command<ConfigCommandDescriptor>
    {
        readonly WhereService whereService;

        public ConfigCommand(ConfigCommandDescriptor descriptor, WhereService whereService) : base(descriptor)
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
                    Environment.ExpandEnvironmentVariables("%LocalAppData%"),
                    @"Microsoft\VisualStudio",
                    instanceDir);

                if (Directory.Exists(path))
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
        }
    }
}
