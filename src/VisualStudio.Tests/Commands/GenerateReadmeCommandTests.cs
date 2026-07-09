using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Devlooped.Tests
{
    public class GenerateReadmeCommandTests
    {
        readonly TextWriter output;

        public GenerateReadmeCommandTests(ITestOutputHelper output) =>
            this.output = new OutputHelperTextWriter(output);

        [Fact]
        public async Task when_generating_readme_without_commands_then_readme_is_generated()
        {
            var outputFile = Path.GetTempFileName();
            var command = new TestGenerateReadmeCommand(Array.Empty<Command>(), () => ReadmeTemplate, _ => throw new FileNotFoundException());
            var parse = command.Parse(["--output", outputFile]);

            await parse.InvokeAsync(new InvocationConfiguration { Output = output });

            Assert.Equal(ExpectedReadmeWithoutCommands, File.ReadAllText(outputFile));
        }

        [Fact]
        public async Task when_generating_readme_without_output_file_then_readme_is_generated()
        {
            var command = new TestGenerateReadmeCommand(Array.Empty<Command>(), () => ReadmeTemplate, _ => throw new FileNotFoundException());
            var sb = new StringBuilder();
            var parse = command.Parse(Array.Empty<string>());

            await parse.InvokeAsync(new InvocationConfiguration { Output = new RecordTextWriter(sb, output) });

            Assert.Contains("# Intro", sb.ToString());
            Assert.Contains("Supported Commands:", sb.ToString());
        }

        [Fact]
        public async Task when_generating_readme_with_commands_then_readme_is_generated()
        {
            var testCmd = new Command("test", "test command description");
            testCmd.Options.Add(new Option<bool>("--arg", "--argument")
            {
                Description = "any of [x | y | z]",
            });

            var outputFile = Path.GetTempFileName();
            var command = new TestGenerateReadmeCommand(
                new[] { testCmd },
                () => ReadmeTemplate,
                name => name == "test" ? TestCommandTemplate : throw new FileNotFoundException());

            var parse = command.Parse(["--output", outputFile]);
            await parse.InvokeAsync(new InvocationConfiguration { Output = output });

            var actual = File.ReadAllText(outputFile);
            Assert.Contains("## test", actual);
            Assert.Contains("test command description", actual);
            Assert.Contains("Usage: dnx vs -- test [options]", actual);
            Assert.Contains("|Option|Description|", actual);
            Assert.Contains("arg", actual);
            Assert.Contains("any of `x \\| y \\| z`", actual);
        }

        [Fact]
        public async Task when_generating_readme_with_commands_and_read_command_template_fails_then_readme_is_generated_without_commands()
        {
            var testCmd = new Command("test", "test command description");
            var outputFile = Path.GetTempFileName();
            var command = new TestGenerateReadmeCommand(
                new[] { testCmd },
                () => ReadmeTemplate,
                _ => throw new FileNotFoundException("template not found"));

            var parse = command.Parse(["--output", outputFile]);
            await parse.InvokeAsync(new InvocationConfiguration { Output = output });

            Assert.Equal(ExpectedReadmeWithoutCommands, File.ReadAllText(outputFile));
        }

        class TestGenerateReadmeCommand : GenerateReadmeCommand
        {
            readonly Func<string> readmeContent;
            readonly Func<string, string> commandTemplate;

            public TestGenerateReadmeCommand(
                IReadOnlyList<Command> commands,
                Func<string> readmeContent,
                Func<string, string> commandTemplate)
                : base(commands)
            {
                this.readmeContent = readmeContent;
                this.commandTemplate = commandTemplate;
            }

            protected override Task<string> ReadReadmeTemplateContentAsync(string templateFile) =>
                Task.FromResult(readmeContent());

            protected override Task<string> ReadCommandTemplateContentAsync(string commandName) =>
                Task.FromResult(commandTemplate(commandName));
        }

        class RecordTextWriter : TextWriter
        {
            readonly StringBuilder sb;
            readonly TextWriter output;

            public RecordTextWriter(StringBuilder sb, TextWriter output)
            {
                this.sb = sb;
                this.output = output;
            }

            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(string value) => WriteLine(value);

            public override void WriteLine(string message)
            {
                sb.AppendLine(message);
                output.WriteLine(message);
            }

            public override void WriteLine(string format, params object[] args)
            {
                sb.AppendFormat(format, args);
                sb.AppendLine();
                output.WriteLine(format, args);
            }
        }

        const string ReadmeTemplate =
@"# Intro

## Supported Commands:

{Commands}

End";

        const string TestCommandTemplate =
@"## {CommandName}

{Description}

```
{Usage}
```

{Options}

Examples:

```
```
";

        const string ExpectedReadmeWithoutCommands =
@"# Intro

## Supported Commands:



End";
    }
}
