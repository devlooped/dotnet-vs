using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace VisualStudio
{
    class SaveCommandDescriptor : CommandDescriptor
    {
        readonly SaveOption saveOption = new SaveOption();

        public SaveCommandDescriptor()
        {
            Options = new Options(saveOption);
        }

        public string Alias => saveOption.Value;
    }
}
