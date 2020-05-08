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

        public string Description { get; protected set; } = string.Empty;

        public ImmutableArray<string> ExtraArguments { get; private set; } = ImmutableArray.Create<string>();

        public virtual void ShowUsage(ITextWriter output) =>
            Options?.ShowUsage(output);

        public virtual void Parse(IEnumerable<string> args)
        {
            var helpOption = new HelpOption();
            var extraArgs = Options.With(helpOption).Parse(args);

            if (helpOption.Value)
                throw new ShowUsageException(this);

            ExtraArguments = ImmutableArray.Create(extraArgs.ToArray());
        }
    }
}
