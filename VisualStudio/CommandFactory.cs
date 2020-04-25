using System;
using System.Collections.Generic;

namespace VisualStudio
{
    sealed class CommandFactory
    {
        public Command CreateCommand(CommandDescriptor commandDescriptor, IEnumerable<string> args, bool addHelpOption = true)
        {
            bool help = false;

            if (addHelpOption)
            {
                commandDescriptor.Options.Add(Environment.NewLine);
                commandDescriptor.Options.Add("?|h|help", "Display this help", h => help = h != null);
            }

            commandDescriptor.Arguments = args;
            commandDescriptor.ExtraArguments = commandDescriptor.Options.Parse(args);
            if (help)
                throw new ShowUsageException();

            return (Command)Activator.CreateInstance(commandDescriptor.CommandType, commandDescriptor);
        }
    }
}
