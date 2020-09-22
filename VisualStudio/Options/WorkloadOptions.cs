using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Mono.Options;

namespace VisualStudio
{
    /// <summary>
    /// Provides shorthand notation for workload identifiers.
    /// </summary>
    /// <remarks>
    /// We extend the options syntax so that a plus can also be used to specify 
    /// the switch, so that `+mobile` is equivalent to `--mobile` and `-mobile`.
    /// </remarks>
    class WorkloadOptions : OptionSet<ImmutableArray<string>>
    {
        readonly string argument;
        private readonly string prefix;
        private readonly string outputArgumentPrefix;

        public WorkloadOptions() : base(ImmutableArray.Create<string>())
        { }

        Dictionary<string, string> aliases = new Dictionary<string, string>
        {
            { "mobile", "Microsoft.VisualStudio.Workload.NetCrossPlat" },
            { "xamarin", "Microsoft.VisualStudio.Workload.NetCrossPlat" },
            { "core", "Microsoft.VisualStudio.Workload.NetCoreTools" },
            { "azure", "Microsoft.VisualStudio.Workload.Azure" },
            { "data", "Microsoft.VisualStudio.Workload.Data" },
            { "desktop", "Microsoft.VisualStudio.Workload.ManagedDesktop" },
            { "unity", "Microsoft.VisualStudio.Workload.ManagedGame" },
            { "native", "Microsoft.VisualStudio.Workload.NativeDesktop" },
            { "web", "Microsoft.VisualStudio.Workload.NetWeb" },
            { "node", "Microsoft.VisualStudio.Workload.Node" },
            { "office", "Microsoft.VisualStudio.Workload.Office" },
            { "python", "Microsoft.VisualStudio.Workload.Python" },
            { "uwp", "Microsoft.VisualStudio.Workload.Universal" },
            { "vsx", "Microsoft.VisualStudio.Workload.VisualStudioExtension" },
        };

        public WorkloadOptions(string argument, string aliasPrefix = "--", string outputArgumentPrefix = "--") : base(ImmutableArray.Create<string>())
        {
            this.argument = argument;
            this.prefix = aliasPrefix;
            this.outputArgumentPrefix = outputArgumentPrefix;

            Add("\n");
            Add($"{argument}:", "A workload ID", id => Value = Value.Add(outputArgumentPrefix + argument).Add(id));

            Add("\n\tWorkload ID aliases:");
            foreach (var aliasPair in aliases)
                Add($"\t{(aliasPrefix + aliasPair.Key).GetNormalizedString()}{outputArgumentPrefix}{argument} {aliasPair.Value}");
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (aliases.Keys.Any(alias => argument.StartsWith(prefix + alias)))
                argument = "--" + this.argument + "=" + GetWorkloadId(argument.Substring(prefix.Length));
            else if (argument.StartsWith(prefix))
                argument = "--" + this.argument + "=" + argument.Substring(1);

            return base.Parse(argument, c);
        }

        string GetWorkloadId(string value)
        {
            if (aliases.TryGetValue(value, out var workloadId))
                return workloadId;

            return value;
        }
    }
}
