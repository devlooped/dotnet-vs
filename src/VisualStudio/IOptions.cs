using System.Collections.Generic;
using Mono.Options;

namespace Devlooped
{
    interface IOptions
    {
        IOptions With(OptionSet optionSet);

        List<string> Parse(IEnumerable<string> arguments);

        void ShowUsage(ITextWriter writer);

        T GetValue<TOption, T>() where TOption : OptionSet<T>;
    }
}
