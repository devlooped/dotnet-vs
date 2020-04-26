using System;
using System.Collections.Generic;

namespace VisualStudio
{
    class ModifyCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showNickname: false);
        readonly WorkloadOptions addWorkloads = new WorkloadOptions("add", "+");
        readonly WorkloadOptions removeWorkloads = new WorkloadOptions("remove", "-");

        public ModifyCommandDescriptor() => optionSet = new CompositeOptionsSet(options, addWorkloads, removeWorkloads);

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public IEnumerable<string> WorkloadsAdded => addWorkloads.Arguments;

        public IEnumerable<string> WorkloadsRemoved => removeWorkloads.Arguments;
    }
}
