using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace VisualStudio
{
    class AliasCommandDescriptor : CommandDescriptor
    {
        public AliasCommandDescriptor()
        {
            Description = "Shows the list of saved aliases";
        }
    }
}
