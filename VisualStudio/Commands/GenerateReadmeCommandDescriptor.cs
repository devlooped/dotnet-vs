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
        const string DefaultCommandTemplateFilename = "default.md";

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

        public virtual async Task<string> ReadReadmeTemplateContentAsync() =>
            await File.ReadAllTextAsync(TemplateFile);

        public virtual async Task<string> ReadCommandTemplateContentAsync(string commandName)
        {
            var commandTemplateFilename = commandName + ".md";
            var probingPaths = new string[]
                {
                    Path.Combine(Path.GetDirectoryName(TemplateFile), commandTemplateFilename),
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Docs", commandTemplateFilename),
                    Path.Combine(Path.GetDirectoryName(TemplateFile), DefaultCommandTemplateFilename),
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Docs", DefaultCommandTemplateFilename),
                };

            var commandTemplatePath = probingPaths.FirstOrDefault(x => File.Exists(x));

            if (string.IsNullOrEmpty(commandTemplatePath))
            {
                throw new FileNotFoundException(
                    $"Could not find a template file for command '{commandName}' in none of these locations:" +
                    Environment.NewLine +
                    string.Join(Environment.NewLine, probingPaths.Select(probingPath => $"\t - {probingPath}")));
            }

            return await File.ReadAllTextAsync(commandTemplatePath);
        }
    }
}
