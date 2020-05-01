using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using System.Collections.Immutable;

namespace VisualStudio
{
    class Options : IOptions
    {
        public static readonly Options Empty = new Options();

        readonly ImmutableArray<OptionSet> optionSets;

        public Options(params OptionSet[] optionSets) =>
            this.optionSets = ImmutableArray.Create(optionSets);

        public IOptions With(OptionSet optionSet) =>
            new Options(optionSets.Add(optionSet).ToArray());

        public List<string> Parse(IEnumerable<string> arguments)
        {
            foreach (var optionSet in optionSets)
                arguments = optionSet.Parse(arguments);

            return arguments.ToList();
        }

        public void ShowUsage(ITextWriter writer) =>
            writer.WriteOptions(optionSets);

        public T GetValue<TOption, T>() where TOption : OptionSet<T> =>
            optionSets.OfType<TOption>().Select(x => x.Value).FirstOrDefault();
    }
}
