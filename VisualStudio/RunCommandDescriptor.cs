﻿using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Options;

namespace VisualStudio
{
    class RunCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showChannel: true, showExp: true, showNickname: false);
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        public RunCommandDescriptor()
        {
            OptionSet = new CompositeOptionSet(
                options,
                new OptionSet
                {
                    { "id:", "Run a specific instance by its ID", i => Id = i },
                    { "f|first", "If more than one instance matches the criteria, run the first one sorted by descending build version.", f => First = f != null },
                    { "v|version:", "Run specific (semantic) version, such as 16.4 or 16.5.3.", v => Version = v },
                    { "w|wait", "Wait for the started Visual Studio to exit.", w => Wait = w != null },
                    { "default", "Set as the default version to run when no arguments are provided, or remove the current default (with --default-).", d => SetDefault = d != null },
                },
                workloads);
        }

        protected override VisualStudioOptions VisualStudioOptions => options;

        public string Version { get; private set; }

        public bool First { get; private set; }

        public bool Wait { get; private set; }

        public bool? SetDefault { get; private set; }

        public string Id { get; private set; }

        public bool IsExperimental => options.IsExperimental;

        public IEnumerable<string> WorkloadsArguments => workloads.Arguments;

        public bool EmptyArguments { get; set; }

        public override void Parse(IEnumerable<string> args)
        {
            base.Parse(args);

            EmptyArguments = !args.Any();
        }
    }
}
