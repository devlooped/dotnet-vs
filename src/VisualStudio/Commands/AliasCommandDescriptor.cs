using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
