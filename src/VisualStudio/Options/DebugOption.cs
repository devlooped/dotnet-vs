using System;
using System.Collections.Generic;

namespace VisualStudio
{
    class DebugOption : OptionSet<bool>
    {
        public DebugOption() => Add("debug", "Execute command in debug mode", x => Value = x != null);

        public static bool IsDefined(IEnumerable<string> args)
        {
            var option = new DebugOption();
            option.Parse(args);

            return option.Value;
        }
    }
}
