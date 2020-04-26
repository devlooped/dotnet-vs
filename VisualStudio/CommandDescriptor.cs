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
        protected IOptionSet OptionSet { get; set; }

        public ImmutableArray<string> Arguments { get; private set; } = ImmutableArray.Create<string>();

        public ImmutableArray<string> ExtraArguments { get; private set; } = ImmutableArray.Create<string>();

        public virtual void ShowUsage(TextWriter output) =>
            OptionSet?.WriteOptionDescriptions(output);

        public void Parse(IEnumerable<string> args)
        {
            var showHelp = false;
            var helpOption = new OptionSet();
            helpOption.Add(Environment.NewLine);
            helpOption.Add("?|h|help", "Display this help", h => showHelp = h != null);

            var extraArgs = OptionSet.With(helpOption).Parse(args);

            if (showHelp)
                throw new ShowUsageException(this);

            Arguments = ImmutableArray.Create(args.ToArray());
            ExtraArguments = ImmutableArray.Create(extraArgs.ToArray());
        }
    }
}
