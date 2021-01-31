using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Devlooped
{
    class GenerateReadmeCommand : Command<GenerateReadmeCommandDescriptor>
    {
        public GenerateReadmeCommand(GenerateReadmeCommandDescriptor descriptor) : base(descriptor) { }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var commandsBuilder = new StringBuilder();
            foreach (var command in Descriptor.Commands.OrderBy(x => x.Key))
            {
                try
                {
                    var commandOptions = new StringBuilder();
                    command.Value.ShowUsage(new MarkdownOptionsTextWriter(commandOptions));

                    var content = (await Descriptor.ReadCommandTemplateContentAsync(command.Key))
                        .Replace("{CommandName}", command.Key)
                        .Replace("{Description}", command.Value.Description)
                        .Replace("{Usage}", $"Usage: {ThisAssembly.Project.AssemblyName} {command.Key} [options]")
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

            var outputFile = Descriptor.OutputFile?.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (!string.IsNullOrEmpty(outputFile))
            {
                if (!Directory.Exists(Path.GetDirectoryName(outputFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                await File.WriteAllTextAsync(outputFile, readmeContent);
            }
            else
            {
                output.WriteLine(readmeContent);
            }
        }
    }
}
