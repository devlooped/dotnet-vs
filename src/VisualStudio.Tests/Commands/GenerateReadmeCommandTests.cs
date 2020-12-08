using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using Xunit;
using Xunit.Abstractions;

namespace VisualStudio.Tests
{
    public class GenerateReadmeCommandTests
    {
        readonly TextWriter output;

        public GenerateReadmeCommandTests(ITestOutputHelper output) =>
            this.output = new OutputHelperTextWriter(output);

        [Fact]
        public async Task when_generating_readme_without_commands_then_readme_is_generated()
        {
            var descriptor = new GenerateReadmeCommandDescriptorTest();

            var command = new GenerateReadmeCommand(descriptor);

            await command.ExecuteAsync(output);

            Assert.Equal(ExpectedReadmeWithoutCommands, File.ReadAllText(descriptor.OutputFile));
        }

        [Fact]
        public async Task when_generating_readme_without_output_file_then_readme_is_generated()
        {
            var command = new GenerateReadmeCommand(new GenerateReadmeCommandDescriptorTest() { OutputFile = null });

            var sb = new StringBuilder();
            await command.ExecuteAsync(new RecordTextWriter(sb, output));

            Assert.Contains(ExpectedReadmeWithoutCommands, sb.ToString());
        }

        [Fact]
        public async Task when_generating_readme_with_commands_then_readme_is_generated()
        {
            var descriptor = new GenerateReadmeCommandDescriptorTest()
                    .WithCommand("test", new TestCommandDescriptor(), () => TestCommandTemplate);

            var command = new GenerateReadmeCommand(descriptor);

            await command.ExecuteAsync(output);

            Assert.Equal(ExpectedReadmeWithTestCommand, File.ReadAllText(descriptor.OutputFile));
        }

        [Fact]
        public async Task when_generating_readme_with_commands_and_read_command_template_fails_then_readme_is_generated_without_commands()
        {
            var descriptor = new GenerateReadmeCommandDescriptorTest()
                    .WithCommand("test", new TestCommandDescriptor(), () => throw new FileNotFoundException("template not found"));

            var command = new GenerateReadmeCommand(descriptor);

            await command.ExecuteAsync(output);

            Assert.Equal(ExpectedReadmeWithoutCommands, File.ReadAllText(descriptor.OutputFile));
        }

        class TestCommandDescriptor : CommandDescriptor
        {
            string arg;

            public TestCommandDescriptor()
            {
                Description = "test command description";

                Options = VisualStudio.Options.Empty.With(
                    new OptionSet
                    {
                            { "headers should not be included in the markdown" },
                            { "arg|argument" , "any of [x | y | z]", x => arg = x }
                    });
            }
        }

        class GenerateReadmeCommandDescriptorTest : GenerateReadmeCommandDescriptor
        {
            readonly Func<string> readmeContent;
            readonly Dictionary<string, Func<string>> commandTemplates = new Dictionary<string, Func<string>>();

            public GenerateReadmeCommandDescriptorTest()
                : this(() => ReadmeTemplate)
            { }

            public GenerateReadmeCommandDescriptorTest(Func<string> readmeContent)
                : base(new Dictionary<string, CommandDescriptor>())
            {
                this.readmeContent = readmeContent;

                OutputFile = Path.GetTempFileName();
            }

            public GenerateReadmeCommandDescriptorTest WithCommand(string commandName, CommandDescriptor descriptor, Func<string> templateProvider)
            {
                Commands[commandName] = descriptor;
                commandTemplates[commandName] = templateProvider;

                return this;
            }

            public override Task<string> ReadReadmeTemplateContentAsync() =>
                Task.FromResult(readmeContent());

            public override Task<string> ReadCommandTemplateContentAsync(string commandName) =>
                Task.FromResult(commandTemplates[commandName]());
        }

        class RecordTextWriter : TextWriter
        {
            readonly StringBuilder sb;
            TextWriter output;

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

        const string ExpectedReadmeWithTestCommand =
@"# Intro

## Supported Commands:


## test

test command description

```
Usage: vs test [options]
```

|Option|Description|
|-|-|
| `arg\|argument` | any of `x \| y \| z` |


Examples:

```
```


End";

    }
}
