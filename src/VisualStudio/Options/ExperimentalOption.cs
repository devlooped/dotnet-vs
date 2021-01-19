using System;
using System.Linq;
using Mono.Options;

namespace Devlooped
{
    class ExperimentalOption : OptionSet<bool>
    {
        readonly static string[] shortcuts = new[] { "exp", "experimental" };

        public ExperimentalOption(string verb)
        {
            Add("exp|experimental", $"{verb} experimental instance instead of regular.", e => Value = e != null);
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (shortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--" + argument;

            return base.Parse(argument, c);
        }
    }
}
