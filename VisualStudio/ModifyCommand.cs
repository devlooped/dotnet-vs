using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class ModifyCommand : Command<ModifyCommandDescriptor>
    {
        public ModifyCommand(ModifyCommandDescriptor descriptor) : base(descriptor) { }

        public override async Task ExecuteAsync(TextWriter output)
        {
        }
    }
}
