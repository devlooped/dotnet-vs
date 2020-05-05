using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class AliasCommand : Command<AliasCommandDescriptor>
    {
        public AliasCommand(AliasCommandDescriptor descriptor) : base(descriptor)
        {
        }

        public override Task ExecuteAsync(TextWriter output)
        {
            output.WriteLine("Saved aliases:");

            var entries = Commands.DotNetConfig
                .GetConfig()
                .Where(x => x.Section == Commands.DotNetConfig.Section && x.Subsection == Commands.DotNetConfig.SubSection)
                .ToList();

            var maxWidth = entries.Select(x => x.Name.Length).Max() + 5;
            foreach (var entry in entries)
                output.WriteLine($"  {entry.Name.GetNormalizedString(maxWidth)}{entry.Value.Replace("|", " ")}");


            return Task.CompletedTask;
        }
    }
}
