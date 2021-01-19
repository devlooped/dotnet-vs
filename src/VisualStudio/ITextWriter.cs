using System.Collections.Immutable;
using Mono.Options;

namespace Devlooped
{
    interface ITextWriter
    {
        void WriteLine();

        void WriteLine(string line);

        void WriteOptions(ImmutableArray<OptionSet> options);
    }
}
