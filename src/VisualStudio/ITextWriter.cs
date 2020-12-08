using System.Collections.Immutable;
using Mono.Options;

namespace VisualStudio
{
    interface ITextWriter
    {
        void WriteLine();

        void WriteLine(string line);

        void WriteOptions(ImmutableArray<OptionSet> options);
    }
}
