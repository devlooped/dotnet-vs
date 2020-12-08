using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Mono.Options;

namespace VisualStudio
{
    class DefaultTextWriter : ITextWriter
    {
        readonly TextWriter output;

        public DefaultTextWriter(TextWriter output)
        {
            this.output = output;
        }

        public void WriteLine() => output.WriteLine();

        public void WriteLine(string value) => output.WriteLine(value);

        public void WriteOptions(ImmutableArray<OptionSet> options)
        {
            foreach (var optionSet in options)
                optionSet.WriteOptionDescriptions(output);
        }
    }
}
