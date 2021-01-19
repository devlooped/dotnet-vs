using System.Collections.Generic;
using Mono.Options;

namespace Devlooped
{
    class SelfOption : OptionSet<bool>
    {
        public SelfOption() => Add("self", "Executes commands against the tool itself", x => Value = x != null);

        public static bool IsDefined(IEnumerable<string> args)
        {
            var option = new SelfOption();
            option.Parse(args);

            return option.Value;
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (argument == "self")
                argument = "--self";

            return base.Parse(argument, c);
        }
    }
}
