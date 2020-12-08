using System;
using System.Collections.Generic;

namespace VisualStudio
{
    class InstallCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("install");
        readonly WorkloadOptions workloads = new WorkloadOptions("add", "+");

        public InstallCommandDescriptor()
        {
            Description = "Installs a specific edition of Visual Studio.";
            Options = vsOptions.With(workloads);
        }

        public Channel? Channel => vsOptions.Channel;

        public Sku? Sku => vsOptions.Sku ?? VisualStudio.Sku.Community;

        public string Nickname => vsOptions.Nickname;

        public IEnumerable<string> WorkloadArgs => workloads.Value;
    }
}
