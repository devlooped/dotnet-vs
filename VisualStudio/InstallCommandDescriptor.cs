using System;
using System.Collections.Generic;

namespace VisualStudio
{
    class InstallCommandDescriptor : CommandDescriptor<InstallCommand>
    {
        readonly VisualStudioOptions options = new VisualStudioOptions();
        readonly WorkloadOptions workloads = new WorkloadOptions("add", "+");

        public InstallCommandDescriptor() =>
            Options = new CompositeOptionsSet(options, workloads);

        public override string Name => "install";

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public string Nickname => options.Nickname;

        public IEnumerable<string> WorkloadArgs => workloads.Arguments;
    }
}
