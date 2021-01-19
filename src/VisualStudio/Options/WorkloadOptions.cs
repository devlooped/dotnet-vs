using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Options;

namespace Devlooped
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
        readonly string[] prefixes;

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

        public WorkloadOptions(string argument, string aliasPrefix = "--", string argumentPrefix = "--") : base(ImmutableArray.Create<string>())
        {
            this.argument = argument;
            prefixes = aliasPrefix.Split('|', StringSplitOptions.RemoveEmptyEntries);

            Add("\n");
            Add($"{argument}:", "A workload ID", id => Value = Value.Add(argumentPrefix + argument).Add(id));

            Add("\n\tWorkload ID aliases:");
            foreach (var aliasPair in aliases)
                Add($"\t{(prefixes[0] + aliasPair.Key).GetNormalizedString()} {argumentPrefix}{argument} {aliasPair.Value}");
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            foreach (var prefix in prefixes)
            {
                if (aliases.Keys.Any(alias => argument.StartsWith(prefix + alias)))
                    return base.Parse("--" + this.argument + "=" + GetWorkloadId(argument.Substring(prefix.Length)), c);
                else if (argument.StartsWith(prefix))
                    return base.Parse("--" + this.argument + "=" + argument.Substring(prefix.Length), c);
            }

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
