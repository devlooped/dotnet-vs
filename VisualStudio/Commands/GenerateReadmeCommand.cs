using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio
{
    class GenerateReadmeCommand : Command<GenerateReadmeCommandDescriptor>
    {
        public GenerateReadmeCommand(GenerateReadmeCommandDescriptor descriptor) : base(descriptor) { }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var commandsBuilder = new StringBuilder();
            foreach (var command in Descriptor.Commands)
            {
                try
                {
                    var commandOptions = new StringBuilder();
                    command.Value.ShowUsage(new MarkdownOptionsTextWriter(commandOptions));

                    var content = (await Descriptor.ReadCommandTemplateContentAsync(command.Key))
                        .Replace("{CommandName}", command.Key)
                        .Replace("{Description}", command.Value.Description)
                        .Replace("{Usage}", $"Usage: {ThisAssembly.Metadata.AssemblyName} {command.Key} [options]")
                        .Replace("{Options}", commandOptions.ToString());

                    commandsBuilder.AppendLine();
                    commandsBuilder.Append(content);
                }
                catch (FileNotFoundException ex)
                {
                    output.WriteLine(ex.Message);

                    continue;
                }
            }

            var readmeContent = (await Descriptor.ReadReadmeTemplateContentAsync())
                .Replace("{Commands}", commandsBuilder.ToString());

            if (!string.IsNullOrEmpty(Descriptor.OutputFile))
            {
                if (!Directory.Exists(Path.GetDirectoryName(Descriptor.OutputFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(Descriptor.OutputFile));

                await File.WriteAllTextAsync(Descriptor.OutputFile, readmeContent);
            }
            else
            {
                output.WriteLine(readmeContent);
            }
        }
    }
}
