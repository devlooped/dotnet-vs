using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Mono.Options;
using vswhere;

namespace VisualStudio
{
    abstract class CommandDescriptor
    {
        protected IOptionSet OptionSet { get; set; }

        public ImmutableArray<string> ExtraArguments { get; private set; } = ImmutableArray.Create<string>();

        public virtual void ShowUsage(TextWriter output) =>
            OptionSet?.WriteOptionDescriptions(output);

        public virtual void Parse(IEnumerable<string> args)
        {
            var showHelp = false;
            var helpOption = new OptionSet();
            helpOption.Add(Environment.NewLine);
            helpOption.Add("?|h|help", "Display this help", h => showHelp = h != null);

            var extraArgs = OptionSet.With(helpOption).Parse(args);

            if (showHelp)
                throw new ShowUsageException(this);

            ExtraArguments = ImmutableArray.Create(extraArgs.ToArray());
        }

        protected virtual VisualStudioOptions VisualStudioOptions { get; }

        public virtual Task<Func<VisualStudioInstance, bool>> GetPredicateAsync() =>
            new VisualStudioPredicateBuilder().BuildPredicateAsync(VisualStudioOptions);
    }
}
