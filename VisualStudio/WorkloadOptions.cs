using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
    public class WorkloadOptions : OptionSet
    {
        readonly string argument;
        private readonly string prefix;
        private readonly string outputArgumentPrefix;
        ImmutableArray<string> arguments = ImmutableArray.Create<string>();

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

        public WorkloadOptions(string argument, string aliasPrefix = "--", string outputArgumentPrefix = "--")
        {
            this.argument = argument;
            this.prefix = aliasPrefix;
            this.outputArgumentPrefix = outputArgumentPrefix;

            Add("\n");
            Add($"{argument}:", "A workload ID", id => arguments = arguments.Add(outputArgumentPrefix + argument).Add(id));

            Add("\n\tWorkload ID aliases:");
            foreach (var aliasPair in aliases)
                Add($"\t{GetNormalizedString(aliasPrefix + aliasPair.Key)}{outputArgumentPrefix}{argument} {aliasPair.Value}");
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (argument.StartsWith(prefix) && !argument.StartsWith(prefix + argument))
                argument = "--" + this.argument + "=" + GetWorkloadId(argument.Substring(prefix.Length));

            return base.Parse(argument, c);
        }

        string GetWorkloadId(string value)
        {
            if (aliases.TryGetValue(value, out var workloadId))
                return workloadId;

            return value;
        }

        string GetNormalizedString(string value, int length = 20)
        {
            if (length == 0)
                return string.Empty;
            else if (string.IsNullOrEmpty(value))
                return new string(' ', length);
            else if (value.Length < length)
                return string.Concat(value, new String(' ', length - value.Length));
            else if (value.Length >= length)
                return value.Substring(0, length - 4) + "... ";

            return value;
        }

        public IEnumerable<string> Arguments => arguments;

        public void ShowOptions(TextWriter output) => WriteOptionDescriptions(output);
    }
}
