using System;
using System.Collections.Generic;
using Mono.Options;

namespace VisualStudio
{
    class ModifyCommandDescriptor : CommandDescriptor<ModifyCommand>
    {
        readonly WorkloadOptions addWorkloads = new WorkloadOptions("add", "+");
        readonly WorkloadOptions removeWorkloads = new WorkloadOptions("remove", "-");

        public ModifyCommandDescriptor()
        {
            Options = new CompositeOptionsSet(
                new OptionSet
                {
                    { "nick|nickname:", "The nickname of the VS instance to modify", n => Nickname = n },
                },
                addWorkloads,
                removeWorkloads);
        }

        public override string Name => "modify";

        public string Nickname { get; private set; }

        public IEnumerable<string> WorkloadsAdded => addWorkloads.Arguments;

        public IEnumerable<string> WorkloadsRemoved => removeWorkloads.Arguments;
    }
}
