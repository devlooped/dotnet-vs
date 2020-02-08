using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public abstract Task<int> ExecuteAsync(IEnumerable<string> args, TextWriter output);

        public abstract void ShowOptions(TextWriter output);

        /// <summary>
        /// Shows full usage information for this specific command, including the "Usage: [tool] [command] [options]" line.
        /// </summary>
        protected void ShowUsage(TextWriter output, OptionSet options)
        {
            output.WriteLine($"Usage: {ThisAssembly.Metadata.AssemblyName} {Name} [options]");
            options.WriteOptionDescriptions(output);
        }
    }
}
