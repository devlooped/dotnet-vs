using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class UpdateCommand : Command<UpdateCommandDescriptor>
    {
        readonly WhereService whereService;

        public UpdateCommand(UpdateCommandDescriptor descriptor, WhereService whereService) : base(descriptor)
        {
            this.whereService = whereService;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var instances = await whereService.GetAllInstancesAsync(Descriptor.Sku, Descriptor.Channel);

            foreach (var instance in instances)
                output.WriteLine("Updating " + instance.InstallationPath + "...");
        }
    }
}
