namespace Devlooped
{
    class SaveCommandDescriptor : CommandDescriptor
    {
        readonly SaveOption saveOption = new SaveOption();
        readonly GlobalOption globalOption = new GlobalOption();

        public SaveCommandDescriptor()
        {
            Options = new Options(saveOption).With(globalOption);
        }

        public string Alias => saveOption.Value;

        public bool Global => globalOption.Value;
    }
}
