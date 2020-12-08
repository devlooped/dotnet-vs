using System;

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
