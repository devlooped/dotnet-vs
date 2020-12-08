using System;
using Mono.Options;

namespace VisualStudio
{
    class FirstOption : OptionSet<bool>
    {
        public FirstOption(string verb)
        {
            Add("first", $"{verb} first matching instance.", e => Value = e != null);
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if ("first".Equals(argument, StringComparison.OrdinalIgnoreCase))
                argument = "--first";

            return base.Parse(argument, c);
        }
    }
}
