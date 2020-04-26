using System;
using System.IO;
using System.Threading.Tasks;
using vswhere;
using System.Collections.Generic;
using System.Linq;

namespace VisualStudio
{
    class WhereCommand : Command<WhereCommandDescriptor>
    {
        readonly WhereService whereService;

        public WhereCommand(WhereCommandDescriptor descriptor, WhereService whereService) : base(descriptor)
        {
            this.whereService = whereService;
        }

        public IEnumerable<VisualStudioInstance> Instances { get; private set; } = Enumerable.Empty<VisualStudioInstance>();

        public override Task ExecuteAsync(TextWriter output) =>
            whereService.RunAsync(
                   Descriptor.Sku,
                   Descriptor.WorkloadsArguments.Concat(Descriptor.ExtraArguments),
                   output);
    }
}
