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
        ImmutableArray<string> arguments = ImmutableArray.Create<string>();

        static class Aliases
        {
            public const string mobile = "Microsoft.VisualStudio.Workload.NetCrossPlat";
            public const string xamarin = "Microsoft.VisualStudio.Workload.NetCrossPlat";
            public const string core = "Microsoft.VisualStudio.Workload.NetCoreTools";
            public const string azure = "Microsoft.VisualStudio.Workload.Azure";
            public const string data = "Microsoft.VisualStudio.Workload.Data";
            public const string desktop = "Microsoft.VisualStudio.Workload.ManagedDesktop";
            public const string unity = "Microsoft.VisualStudio.Workload.ManagedGame";
            public const string native = "Microsoft.VisualStudio.Workload.NativeDesktop";
            public const string web = "Microsoft.VisualStudio.Workload.NetWeb";
            public const string node = "Microsoft.VisualStudio.Workload.Node";
            public const string office = "Microsoft.VisualStudio.Workload.Office";
            public const string python = "Microsoft.VisualStudio.Workload.Python";
            public const string uwp = "Microsoft.VisualStudio.Workload.Universal";
            public const string vsx = "Microsoft.VisualStudio.Workload.VisualStudioExtension";
        }

        public WorkloadOptions(string argument)
        {
            this.argument = argument;

            foreach (var field in typeof(Aliases).GetFields())
            {
                var id = (string)field.GetValue(null);
                Add(field.Name, argument + " " + id, x =>
                {
                    if (x != null)
                        arguments = arguments.Add(argument).Add(id);
                });
            }
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (argument[0] == '+')
                argument = "--" + argument.Substring(1);

            return base.Parse(argument, c);
        }

        public IEnumerable<string> Arguments => arguments;

        public void ShowOptions(TextWriter output) => WriteOptionDescriptions(output);
    }
}
