using System;
using System.Collections.Generic;
using Mono.Options;

namespace VisualStudio
{
    class ModifyCommandDescriptor : CommandDescriptor<ModifyCommand>
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showNickname: false);
        readonly WorkloadOptions addWorkloads = new WorkloadOptions("add", "+");
        readonly WorkloadOptions removeWorkloads = new WorkloadOptions("remove", "-");

        public ModifyCommandDescriptor() =>
            Options = new CompositeOptionsSet(options, addWorkloads, removeWorkloads);

        public override string Name => "modify";

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public IEnumerable<string> WorkloadsAdded => addWorkloads.Arguments;

        public IEnumerable<string> WorkloadsRemoved => removeWorkloads.Arguments;
    }
}
