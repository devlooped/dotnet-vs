using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Options;
using System.Collections.Immutable;

namespace VisualStudio
{
    class CompositeOptionsSet : IOptionSet
    {
        readonly OptionSet defaultOptionSet;
        readonly ImmutableArray<OptionSet> optionSets;

        public CompositeOptionsSet(params OptionSet[] optionSets)
        {
            defaultOptionSet = new OptionSet();

            this.optionSets = ImmutableArray.Create<OptionSet>()
                .AddRange(optionSets)
                .Add(defaultOptionSet);
        }

        public OptionSet Add(string header) =>
            defaultOptionSet.Add(header);

        public OptionSet Add(string prototype, string description, Action<string> action) =>
            defaultOptionSet.Add(prototype, description, action);

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
