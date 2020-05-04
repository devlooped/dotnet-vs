using System;
using System.Collections.Generic;

namespace VisualStudio
{
    class VersionOption : OptionSet<bool>
    {
        public VersionOption() => Add("version", "Tool version", x => Value = x != null);

        public static bool IsDefined(IEnumerable<string> args)
        {
            var option = new VersionOption();
            option.Parse(args);

            return option.Value;
        }
    }
}
