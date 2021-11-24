using Mono.Options;

namespace Devlooped
{
    class FilterOption : OptionSet<string>
    {
        public FilterOption(string defaultValue = default) : base(defaultValue)
        {
            Add("filter:", "Expression to filter VS instances. E.g. `x => x.InstanceId = '123'`", e => Value = e?.Replace("'", "\"").Trim());
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            var result = base.Parse(argument, c);

            if (argument.Contains("=>") && string.IsNullOrEmpty(Value))
                result = base.Parse("--filter=" + argument, c);

            return result;
        }
    }
}
