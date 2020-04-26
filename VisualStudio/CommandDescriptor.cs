using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Mono.Options;

namespace VisualStudio
{
    abstract class CommandDescriptor
    {
        protected IOptionSet optionSet;

        protected CommandDescriptor() { }

        protected CommandDescriptor(IOptionSet optionSet) => this.optionSet = optionSet;

        public ImmutableArray<string> Arguments { get; set; } = ImmutableArray.Create<string>();

        public ImmutableArray<string> ExtraArguments { get; set; } = ImmutableArray.Create<string>();

        public virtual void ShowUsage(TextWriter output) =>
            optionSet.WriteOptionDescriptions(output);

        public void Parse(IEnumerable<string> args)
        {
            var showHelp = false;
            var helpOption = new OptionSet();
            helpOption.Add(Environment.NewLine);
            helpOption.Add("?|h|help", "Display this help", h => showHelp = h != null);

            var extraArgs = optionSet.With(helpOption).Parse(args);

            if (showHelp)
                throw new ShowUsageException(this);

            Arguments = ImmutableArray.Create(args.ToArray());
            ExtraArguments = ImmutableArray.Create(extraArgs.ToArray());
        }
    }
}
