using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using System.Collections.Immutable;

namespace VisualStudio
{
    class CompositeOptionSet : IOptionSet
    {
        readonly OptionSet defaultOptionSet;
        readonly ImmutableArray<OptionSet> optionSets;

        public CompositeOptionSet(params OptionSet[] optionSets)
        {
            defaultOptionSet = new OptionSet();

            this.optionSets = ImmutableArray.Create<OptionSet>()
                .AddRange(optionSets)
                .Add(defaultOptionSet);
        }

        public IOptionSet With(OptionSet optionSet) =>
            new CompositeOptionSet(optionSets.Add(optionSet).ToArray());

        public List<string> Parse(IEnumerable<string> arguments)
        {
            foreach (var optionSet in optionSets)
                arguments = optionSet.Parse(arguments);

            return arguments.ToList();
        }

        public void WriteOptionDescriptions(TextWriter writer)
        {
            foreach (var optionSet in optionSets)
                optionSet.WriteOptionDescriptions(writer);
        }
    }
}
