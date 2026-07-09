using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;

namespace Devlooped;

/// <summary>
/// Wraps the built-in help action and appends Examples from embedded Docs/{command}.md.
/// </summary>
sealed class ExamplesHelpAction : SynchronousCommandLineAction
{
    readonly HelpAction inner = new();

    public override int Invoke(ParseResult parseResult)
    {
        var result = inner.Invoke(parseResult);
        WriteExamples(parseResult);
        return result;
    }

    static void WriteExamples(ParseResult parseResult)
    {
        var command = parseResult.CommandResult.Command;
        // Skip root-level help (no examples file for root)
        if (command is RootCommand)
            return;

        var commandName = command.Name;
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualStudio.Docs." + commandName + ".md");
        if (stream == null)
            return;

        var output = parseResult.InvocationConfiguration.Output;
        using var reader = new StreamReader(stream);
        var showLine = false;
        string line;
        while ((line = reader.ReadLine()) != null && !line.StartsWith("<!-- EXAMPLES_END"))
        {
            if (line.StartsWith("<!-- EXAMPLES_BEGIN"))
            {
                output.WriteLine();
                output.WriteLine("Examples:");
                output.WriteLine();
                showLine = true;
            }
            else if (showLine && !line.Trim().StartsWith("```"))
            {
                output.WriteLine(line);
            }
        }
    }
}
