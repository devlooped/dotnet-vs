using System;
using Mono.Options;

namespace VisualStudio
{
    class SelectAllOption : OptionSet<bool>
    {
        public SelectAllOption(string verb)
        {
            Add("all", $"{verb} all instances.", e => Value = e != null);
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if ("all".Equals(argument, StringComparison.OrdinalIgnoreCase))
                argument = "--all";

            return base.Parse(argument, c);
        }
    }
}
