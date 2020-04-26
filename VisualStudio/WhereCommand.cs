using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using vswhere;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace VisualStudio
{
    class WhereCommand : Command<WhereCommandDescriptor>
    {
        public WhereCommand(WhereCommandDescriptor descriptor) : base(descriptor) { }

        public bool Quiet { get; set; }

        public IEnumerable<VisualStudioInstance> Instances { get; private set; } = Enumerable.Empty<VisualStudioInstance>();

        public override Task ExecuteAsync(TextWriter output) =>
            WhereService.Instance.RunAsync(
                   Descriptor.Sku,
                   Descriptor.WorkloadsArguments.Concat(Descriptor.ExtraArguments),
                   output);
    }
}
