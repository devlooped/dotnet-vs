using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualStudio
{
    abstract class CommandDescriptor
    {
        public abstract string Name { get; }

        public IOptionSet Options { get; protected set; }

        public IEnumerable<string> Arguments { get; set; } = Enumerable.Empty<string>();

        public IEnumerable<string> ExtraArguments { get; set; } = Enumerable.Empty<string>();

        public abstract Type CommandType { get; }

        public bool ShowUsageWithEmptyArguments { get; protected set; } = true;
    }

    abstract class CommandDescriptor<T> : CommandDescriptor where T : Command
    {
        public override Type CommandType => typeof(T);
    }
}
