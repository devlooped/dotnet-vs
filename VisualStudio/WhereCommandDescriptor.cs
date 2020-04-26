using System;
using System.Collections.Generic;
using System.IO;

namespace VisualStudio
{
    class WhereCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showChannel: false, showNickname: false);
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        readonly WhereService whereService;

        public WhereCommandDescriptor(WhereService whereService)
        {
            optionSet = new CompositeOptionsSet(options, workloads);
            this.whereService = whereService;
        }

        public Sku? Sku => options.Sku;

        public IEnumerable<string> WorkloadsArguments => workloads.Arguments;

        public override void ShowUsage(TextWriter output)
        {
            base.ShowUsage(output);

            whereService.ShowUsage(output);
        }
    }
}
