using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Mono.Options;

namespace VisualStudio
{
    class WhereCommandDescriptor : CommandDescriptor<WhereCommand>
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showChannel: false, showNickname: false);
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        public WhereCommandDescriptor() => Options = new CompositeOptionsSet(options, workloads);

        public override string Name => "where";

        public Sku? Sku => options.Sku;

        public IEnumerable<string> WorkloadsArguments => workloads.Arguments;
    }
}
