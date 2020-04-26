using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class ModifyCommand : Command<ModifyCommandDescriptor>
    {
        readonly WhereService whereService;

        public ModifyCommand(ModifyCommandDescriptor descriptor, WhereService whereService) : base(descriptor)
        {
            this.whereService = whereService;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var instances = await whereService.GetAllInstancesAsync(Descriptor.Sku, Descriptor.Channel);

            foreach (var instance in instances)
                output.WriteLine(instance.InstallationPath);
        }
    }
}
