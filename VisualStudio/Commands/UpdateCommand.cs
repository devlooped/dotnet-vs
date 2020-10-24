using System;
using System.Linq;
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

        public UpdateCommand(UpdateCommandDescriptor descriptor, WhereService whereService, InstallerService installerService)
            : base(descriptor)
        {
            this.whereService = whereService;
            this.installerService = installerService;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var instances = await whereService.GetAllInstancesAsync(Descriptor.Options);

            if (!Descriptor.All)
                instances = new Chooser().ChooseMany(instances, output);

            var extra =
                !Descriptor.ExtraArguments.Any(x => x.TrimStart('-') == "config") && File.Exists(".vsconfig") ?
                Descriptor.ExtraArguments.Add("--config").Add(".vsconfig") :
                Descriptor.ExtraArguments;

            foreach (var instance in instances)
            {
                var args = new List<string>(extra)
                {
                    "--passive",
                    "--installPath",
                    instance.InstallationPath
                };

                await installerService.UpdateAsync(instance.GetChannel(), instance.GetSku(), args, output);
            }
        }
    }
}
