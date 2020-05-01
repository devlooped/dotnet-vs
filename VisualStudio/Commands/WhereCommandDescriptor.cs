using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mono.Options;
using vswhere;

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
            OptionSet = new CompositeOptionSet(
                options,
                new OptionSet
                {
                    { "prop|property:", "The name of a property to return", x => Property = x }
                },
                workloads,
                allOption);
            this.whereService = whereService;
        }

        protected override VisualStudioOptions VisualStudioOptions => options;

        public string Property { get; private set; }

        public bool ShowAll => allOption.All;

        public IEnumerable<string> WorkloadsArguments => workloads.Arguments;

        public override void ShowUsage(TextWriter output)
        {
            base.ShowUsage(output);

            whereService.ShowUsage(output);
        }
    }
}
