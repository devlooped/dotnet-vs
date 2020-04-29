using System;
using System.Collections.Generic;

namespace VisualStudio
{
    class ModifyCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Modify", showNickname: false);
        readonly WorkloadOptions addWorkloads = new WorkloadOptions("add", "+");
        readonly WorkloadOptions removeWorkloads = new WorkloadOptions("remove", "-");

        public ModifyCommandDescriptor() => OptionSet = new CompositeOptionSet(options, addWorkloads, removeWorkloads);

        protected override VisualStudioOptions VisualStudioOptions => options;

        public IEnumerable<string> WorkloadsAdded => addWorkloads.Arguments;

        public IEnumerable<string> WorkloadsRemoved => removeWorkloads.Arguments;
    }
}
