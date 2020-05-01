using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Options;

namespace VisualStudio
{
    interface IOptionSet
    {
        IOptionSet With(OptionSet optionSet);

        List<string> Parse(IEnumerable<string> arguments);

        void WriteOptionDescriptions(TextWriter writer);
    }
}
