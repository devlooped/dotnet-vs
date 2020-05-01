using System;
using System.Linq;
using System.Text;
using System.Collections.Immutable;
using Mono.Options;

namespace VisualStudio
{
    class MarkdownOptionsTextWriter : ITextWriter
    {
        readonly StringBuilder builder;

        public MarkdownOptionsTextWriter(StringBuilder builder)
        {
            this.builder = builder;
        }

        public void WriteLine()
        { }

        public void WriteLine(string line)
        { }

        public void WriteOptions(ImmutableArray<OptionSet> options)
        {
            if (options.Any())
            {
                builder.AppendLine("|Option|Description|");
                builder.AppendLine("|-|-|");

                foreach (var optionSet in options)
                    foreach (var option in optionSet.Where(x => !x.Hidden && x.GetType().Name.EndsWith("ActionOption")))
                        builder.AppendLine($"| `{option.Prototype.Replace("|", "\\|")}` | {GetEscapedDescription(option.Description)} |");
            }
        }

        string GetEscapedDescription(string desciption) =>
            desciption.Replace('[', '`').Replace(']', '`').Replace("|", "\\|");
    }
}
