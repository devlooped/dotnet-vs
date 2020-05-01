using System;
using System.Collections.Generic;
using System.IO;

namespace VisualStudio
{
    class WhereCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("show").WithSelectAll();
        readonly SelectPropertyOption selectProperty = new SelectPropertyOption();
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        readonly WhereService whereService;

        public WhereCommandDescriptor(WhereService whereService)
        {
            Options = vsOptions
                .With(selectProperty)
                .With(workloads);

            this.whereService = whereService;
        }

        public string Property { get; private set; }

        public bool ShowAll => vsOptions.All;

        public IEnumerable<string> WorkloadsArguments => workloads.Value;

        public override void ShowUsage(TextWriter output)
        {
            base.ShowUsage(output);

            whereService.ShowUsage(output);
        }
    }
}
