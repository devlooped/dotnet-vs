using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devlooped;

class GenerateReadmeCommand : Command
{
    const string ResourcePrefix = "VisualStudio.Docs.";
    const string ReadmeResource = ResourcePrefix + "readme.md";

    readonly Option<string> templateOption = new("--template")
    {
        Description = "The readme template file",
    };
    readonly Option<string> outputOption = new("--output", "-o")
    {
        Description = "The output file",
    };

    readonly IReadOnlyList<Command> commands;

    public GenerateReadmeCommand(IReadOnlyList<Command> commands)
        : base(Commands.System.GenerateReadme, "Generates the README from command metadata")
    {
        this.commands = commands;
        Hidden = true;

        Options.Add(templateOption);
        Options.Add(outputOption);

        // Default template path matches previous behavior
        templateOption.DefaultValueFactory = _ => @"Docs\readme.md";

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var templateFile = parse.GetValue(templateOption);
        var outputFile = parse.GetValue(outputOption);

        var commandsBuilder = new StringBuilder();
        foreach (var command in commands.OrderBy(x => x.Name))
        {
            try
            {
                var commandOptions = new StringBuilder();
                MarkdownOptionsTextWriter.WriteOptions(commandOptions, command);

                var content = (await ReadCommandTemplateContentAsync(command.Name))
                    .Replace("{CommandName}", command.Name)
                    .Replace("{Description}", command.Description)
                    .Replace("{Usage}", $"Usage: dnx {ThisAssembly.Project.AssemblyName} -- {command.Name} [options]")
                    .Replace("{Options}", commandOptions.ToString());

                commandsBuilder.AppendLine();
                commandsBuilder.Append(content);
            }
            catch (FileNotFoundException ex)
            {
                output.WriteLine(ex.Message);
            }
        }

        var readmeContent = (await ReadReadmeTemplateContentAsync(templateFile))
            .Replace("{Commands}", commandsBuilder.ToString());

        outputFile = outputFile?.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        if (!string.IsNullOrEmpty(outputFile))
        {
            var dir = Path.GetDirectoryName(outputFile);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await File.WriteAllTextAsync(outputFile, readmeContent);
        }
        else
        {
            output.WriteLine(readmeContent);
        }
    }

    protected virtual async Task<string> ReadReadmeTemplateContentAsync(string templateFile)
    {
        if (!string.IsNullOrEmpty(templateFile) && File.Exists(templateFile))
            return await File.ReadAllTextAsync(templateFile);

        using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ReadmeResource));
        return await reader.ReadToEndAsync();
    }

    protected virtual async Task<string> ReadCommandTemplateContentAsync(string commandName)
    {
        var probingResources = new[]
        {
            ResourcePrefix + commandName + ".md",
            ResourcePrefix + "default.md"
        };

        var commandTemplateResource = probingResources
            .Select(x => Assembly.GetExecutingAssembly().GetManifestResourceStream(x))
            .FirstOrDefault(x => x != null);

        if (commandTemplateResource == null)
        {
            throw new FileNotFoundException(
                $"Could not find a template resource file for command '{commandName}' in none of these locations:" +
                Environment.NewLine +
                string.Join(Environment.NewLine, probingResources.Select(probingPath => $"\t - {probingPath}")));
        }

        using var reader = new StreamReader(commandTemplateResource);
        return await reader.ReadToEndAsync();
    }
}
