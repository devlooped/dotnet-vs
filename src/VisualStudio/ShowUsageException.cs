using System;

namespace Devlooped
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
