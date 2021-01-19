using System.Collections.Generic;

namespace Devlooped
{
    class SaveOption : OptionSet<string>
    {
        public SaveOption() => Add("save:", "Saves a command to be executed with a given alias", x => Value = x);

        public static bool IsDefined(IEnumerable<string> args)
        {
            var saveOption = new SaveOption();
            saveOption.Parse(args);

            return !string.IsNullOrEmpty(saveOption.Value);
        }
    }
}
