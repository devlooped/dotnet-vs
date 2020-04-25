using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class ModifyCommand : Command
    {
        readonly ModifyCommandDescriptor descriptor;

        public ModifyCommand(ModifyCommandDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            foreach (var x in descriptor.WorkloadsAdded)
                output.WriteLine(x);

            foreach (var x in descriptor.WorkloadsRemoved)
                output.WriteLine(x);
        }
    }
}
