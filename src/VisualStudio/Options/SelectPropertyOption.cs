namespace Devlooped
{
    class SelectPropertyOption : OptionSet<string>
    {
        public SelectPropertyOption()
        {
            Add("prop|property:", "The name of a property to return", x => Value = x);
        }
    }
}
