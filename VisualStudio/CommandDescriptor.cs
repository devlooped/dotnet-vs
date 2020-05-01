using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Mono.Options;

namespace VisualStudio
{
    abstract class CommandDescriptor
    {
        public IOptions Options { get; protected set; } = VisualStudio.Options.Empty;

        public ImmutableArray<string> ExtraArguments { get; private set; } = ImmutableArray.Create<string>();

        public virtual void ShowUsage(TextWriter output) =>
            Options?.ShowUsage(output);

        public virtual void Parse(IEnumerable<string> args)
        {
            var showHelp = false;
            var helpOption = new OptionSet();
            helpOption.Add(Environment.NewLine);
            helpOption.Add("?|h|help", "Display this help", h => showHelp = h != null);

            var extraArgs = Options.With(helpOption).Parse(args);

            if (showHelp)
                throw new ShowUsageException(this);

            ExtraArguments = ImmutableArray.Create(extraArgs.ToArray());
        }
    }
}
