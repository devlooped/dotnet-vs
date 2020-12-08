namespace VisualStudio
{
    class HelpOption : OptionSet<bool>
    {
        public HelpOption() => Add("?|h|help", "Display this help", h => Value = h != null);
    }
}
