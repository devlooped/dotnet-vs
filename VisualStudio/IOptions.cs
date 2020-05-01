using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;

namespace VisualStudio
{
    interface IOptions
    {
        IOptions With(OptionSet optionSet);

        List<string> Parse(IEnumerable<string> arguments);

        void ShowUsage(TextWriter writer);

        T GetParsedValue<TOption, T>() where TOption : OptionSet<T>;
    }
}
