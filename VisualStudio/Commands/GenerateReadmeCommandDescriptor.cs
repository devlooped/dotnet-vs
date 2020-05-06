using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    class GenerateReadmeCommandDescriptor : CommandDescriptor
    {
        const string ResourcePrefix = "VisualStudio.Docs.";
        const string ReadmeResource = ResourcePrefix + "README.md";

        public GenerateReadmeCommandDescriptor(Dictionary<string, CommandDescriptor> commands)
        {
            Commands = commands;

            Options = new Options(
                new OptionSet
                {
                    { "template:", "The readme template file", x => TemplateFile = x },
                    { "output:", "The output file", x => OutputFile = x },
                });
        }

        public string TemplateFile { get; set; } = @"Docs\README.md";

        public string OutputFile { get; set; }

        public Dictionary<string, CommandDescriptor> Commands { get; }

        public virtual async Task<string> ReadReadmeTemplateContentAsync()
        {
            if (File.Exists(TemplateFile))
                return await File.ReadAllTextAsync(TemplateFile);

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ReadmeResource)))
                return await reader.ReadToEndAsync();
        }

        public virtual async Task<string> ReadCommandTemplateContentAsync(string commandName)
        {
            var probingResources = new string[]
                {
                    ResourcePrefix + commandName + ".md",
                    ResourcePrefix + "default.md"
                };

            var commandTemplateResource = probingResources.Select(x => Assembly.GetExecutingAssembly().GetManifestResourceStream(x)).FirstOrDefault();

            if (commandTemplateResource == null)
            {
                throw new FileNotFoundException(
                    $"Could not find a template resource file for command '{commandName}' in none of these locations:" +
                    Environment.NewLine +
                    string.Join(Environment.NewLine, probingResources.Select(probingPath => $"\t - {probingPath}")));
            }

            using (var reader = new StreamReader(commandTemplateResource))
                return await reader.ReadToEndAsync();
        }
    }
}
