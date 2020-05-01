using System;
using System.Linq;
using Mono.Options;

namespace VisualStudio
{
    class ExpressionOption : OptionSet<string>
    {
        public ExpressionOption(string defaultValue = default) : base(defaultValue)
        {
            Add("expr|expression:", "Expression to filter VS instances. E.g. \"x => x.InstanceId = '123'\"", e => Value = e.Replace("'", "\"").Trim());
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            var result = base.Parse(argument, c);

            if (argument.Contains("=>") && string.IsNullOrEmpty(Value))
                result = base.Parse("--expr=" + argument, c);

            return result;
        }
    }
}
