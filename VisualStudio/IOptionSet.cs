using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Options;

namespace VisualStudio
{
    interface IOptionSet
    {
        public IOptionSet With(OptionSet optionSet);

        public OptionSet Add(string header);

        public OptionSet Add(string prototype, string description, Action<string> action);

        List<string> Parse(IEnumerable<string> arguments);

        void WriteOptionDescriptions(TextWriter writer);
    }
}
