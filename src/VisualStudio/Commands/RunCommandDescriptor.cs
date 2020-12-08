using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;

namespace VisualStudio
{
    class RunCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default().WithExperimental();
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        public RunCommandDescriptor()
        {
            Description = "This is default command, so typically it does not need to be provided as an argument.";
            Options = vsOptions
                .With(
                    new OptionSet
                    {
                        { "id:", "Run a specific instance by its ID", i => Id = i },
                        { "f|first", "If more than one instance matches the criteria, run the first one sorted by descending build version.", f => First = f != null },
                        { "v|version:", "Run specific (semantic) version, such as 16.4 or 16.5.3.", v => Version = v },
                        { "w|wait", "Wait for the started Visual Studio to exit.", w => Wait = w != null },
                        { "nr|nodereuse", "Disable MSBuild node reuse. Useful when testing analyzers, tasks and targets. Defaults to true when running experimental instance.", nr => DisableNodeReuse = nr != null },
                        { "default", "Set as the default version to run when no arguments are provided, or remove the current default (with --default-).", d => SetDefault = d != null },
                    })
                .With(workloads);
        }

        public string Version { get; private set; }

        public bool First { get; private set; }

        public bool Wait { get; private set; }

        public bool? SetDefault { get; private set; }

        public string Id { get; private set; }

        public bool IsExperimental => vsOptions.IsExperimental;

        public bool DisableNodeReuse { get; private set; }

        public IEnumerable<string> WorkloadsArguments => workloads.Value;

        public bool EmptyArguments { get; set; }

        public override void Parse(IEnumerable<string> args)
        {
            base.Parse(args);

            EmptyArguments = !args.Any();
        }
    }
}
