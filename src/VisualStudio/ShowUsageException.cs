using System;
using System.Collections.Generic;
using System.Text;
using Mono.Options;

namespace VisualStudio
{
    class ShowUsageException : Exception
    {
        public ShowUsageException(CommandDescriptor commandDescriptor)
        {
            CommandDescriptor = commandDescriptor;
        }

        public CommandDescriptor CommandDescriptor { get; }
    }
}
