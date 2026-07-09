using System.CommandLine;
using System.Linq;
using System.Text;

namespace Devlooped;

/// <summary>
/// Writes command options as a markdown table for readme generation.
/// </summary>
static class MarkdownOptionsTextWriter
{
    public static void WriteOptions(StringBuilder builder, Command command)
    {
        var options = command.Options
            .Where(o => !o.Hidden && o is not System.CommandLine.Help.HelpOption)
            .ToList();

        if (options.Count == 0)
            return;

        builder.AppendLine("|Option|Description|");
        builder.AppendLine("|-|-|");

        foreach (var option in options)
        {
            // Prefer shorter aliases first (e.g. w|workspaceId), matching historical Mono.Options tables.
            var names = new[] { option.Name }.Concat(option.Aliases)
                .Select(StripDash)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n.Length)
                .ThenBy(n => n)
                .ToList();

            var display = string.Join("|", names);
            builder.AppendLine($"| `{Escape(display)}` | {GetEscapedDescription(option.Description)} |");
        }
    }

    static string StripDash(string name)
    {
        if (name.StartsWith("--"))
            return name[2..];
        if (name.StartsWith('-'))
            return name[1..];
        return name;
    }

    static string Escape(string value) => value.Replace("|", "\\|");

    static string GetEscapedDescription(string description) =>
        (description ?? string.Empty).Replace('[', '`').Replace(']', '`').Replace("|", "\\|");
}
