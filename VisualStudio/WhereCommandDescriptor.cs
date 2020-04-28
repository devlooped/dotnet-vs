using System;
using System.Collections.Generic;
using System.IO;

namespace VisualStudio
{
    class WhereCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showNickname: false);
        readonly AllOption allOption = new AllOption("show");
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        readonly WhereService whereService;

        public WhereCommandDescriptor(WhereService whereService)
        {
            OptionSet = new CompositeOptionSet(options, workloads, allOption);
            this.whereService = whereService;
        }

        public bool ShowAll => allOption.All;

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public IEnumerable<string> WorkloadsArguments => workloads.Arguments;

        public override void ShowUsage(TextWriter output)
        {
            base.ShowUsage(output);

            whereService.ShowUsage(output);
        }
    }
}
