using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio
{
    class SaveCommand : Command<SaveCommandDescriptor>
    {
        public SaveCommand(SaveCommandDescriptor descriptor) : base(descriptor) { }

        public override async Task ExecuteAsync(TextWriter output)
        {
            output.WriteLine($"Saving {string.Join(" ", Descriptor.ExtraArguments)} as '{Descriptor.Alias}'...");
        }
    }
}
