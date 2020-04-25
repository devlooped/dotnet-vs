using System;
using System.Collections.Generic;
using Mono.Options;

namespace VisualStudio
{
    class InstallCommandDescriptor : CommandDescriptor<InstallCommand>
    {
        readonly WorkloadOptions workloads = new WorkloadOptions("add", "+");

        public InstallCommandDescriptor()
        {
            Options = new CompositeOptionsSet(
                new OptionSet
                {
                    { "pre|preview", "Install preview version", _ => Preview = true },
                    { "int|internal", "Install internal (aka 'dogfood') version", _ => Dogfood = true },
                    { "sku:", "Edition, one of [e|ent|enterprise], [p|pro|professional] or [c|com|community]. Defaults to 'community'.", s => Sku = SkuOption.Parse(s) },
                    { "nick|nickname:", "Optional nickname to assign to the installation", n => Nickname = n },
                },
                workloads);
        }

        public override string Name => "install";

        public bool Preview { get; private set; }

        public bool Dogfood { get; private set; }

        public Sku Sku { get; private set; } = Sku.Community;

        public string Nickname { get; private set; }

        public IEnumerable<string> WorkloadArgs => workloads.Arguments;
    }
}
