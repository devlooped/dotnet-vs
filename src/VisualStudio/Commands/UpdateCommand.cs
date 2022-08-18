using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vswhere;

namespace Devlooped
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

                // If the channel is not a built-in one, use the existing Uri for updates.
                var channel = instance.GetChannel();
                if (channel != null)
                    await installerService.UpdateAsync(instance.InstallationVersion.Major.ToString(), channel, instance.GetSku(), args, output);
                else
                    await installerService.UpdateAsync(instance.ChannelUri.Replace("/channel", ""), instance.GetSku(), args, output);
            }
        }
    }
}
