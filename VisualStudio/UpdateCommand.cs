using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class UpdateCommand : Command<UpdateCommandDescriptor>
    {
        public UpdateCommand(UpdateCommandDescriptor descriptor) : base(descriptor) { }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var instances = await WhereService.Instance.GetAllInstancesAsync(Descriptor.Sku, Descriptor.Channel);

            foreach (var instance in instances)
                output.WriteLine("Updating " + instance.InstallationPath + "...");
        }
    }
}
