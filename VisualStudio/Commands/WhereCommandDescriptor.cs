using System;
using System.Collections.Generic;
using System.IO;

namespace VisualStudio
{
    class WhereCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("show");
        readonly SelectPropertyOption propOption = new SelectPropertyOption();
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        readonly WhereService whereService;

        public WhereCommandDescriptor(WhereService whereService)
        {
            Description = "Locates the installed version(s) of Visual Studio that satisfy the requested requirements.";

            Options = vsOptions
                .With(propOption)
                .With(workloads);

            this.whereService = whereService;
        }

        public string Property => propOption.Value;

        public IEnumerable<string> WorkloadsArguments => workloads.Value;

        public override void ShowUsage(ITextWriter output)
        {
            base.ShowUsage(output);

            whereService.ShowUsage(output);
        }
    }
}
