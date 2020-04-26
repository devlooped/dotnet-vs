using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using vswhere;

namespace VisualStudio
{
    class UpdateCommand : Command<UpdateCommandDescriptor>
    {
        readonly WhereService whereService;
        readonly InstallerService installerService;

        public UpdateCommand(UpdateCommandDescriptor descriptor, WhereService whereService, InstallerService installerService) : base(descriptor)
        {
            this.whereService = whereService;
            this.installerService = installerService;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var instances = await whereService.GetAllInstancesAsync(Descriptor.Sku, Descriptor.Channel);

            var instance = new VisualStudioInstanceChooser().Choose(instances, output);

            if (instance != null)
            {
                var args = new List<string>();

                args.Add("--passive");

                args.Add("--installPath");
                args.Add(instance.InstallationPath);

                args.AddRange(Descriptor.ExtraArguments);

                await installerService.RunAsync("update", instance.GetChannel(), instance.GetSku(), args, output);
            }
        }
    }
}
