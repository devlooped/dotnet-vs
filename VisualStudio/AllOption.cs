using System;
using Mono.Options;

namespace VisualStudio
{
    public class AllOption : OptionSet
    {
        public AllOption(string verb)
        {
            Add("all", $"{verb} all instances.", e => All = e != null);
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if ("all".Equals(argument, StringComparison.OrdinalIgnoreCase))
                argument = "--all";

            return base.Parse(argument, c);
        }

        public bool All { get; private set; }
    }
}
