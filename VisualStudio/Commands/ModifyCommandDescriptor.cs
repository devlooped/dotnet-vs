using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace VisualStudio
{
    class ModifyCommandDescriptor : CommandDescriptor
    {
        readonly WorkloadOptions addWorkloads = new WorkloadOptions("add", "+");
        readonly WorkloadOptions removeWorkloads = new WorkloadOptions("remove", "-");

        public ModifyCommandDescriptor() =>
            Options = VisualStudioOptions.Default("modify").With(addWorkloads).With(removeWorkloads);

        public ImmutableArray<string> WorkloadsAdded => addWorkloads.Value;

        public ImmutableArray<string> WorkloadsRemoved => removeWorkloads.Value;
    }
}
