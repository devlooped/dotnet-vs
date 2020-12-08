namespace VisualStudio
{
    class ConfigCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("open")
            .WithFirst()
            .WithExperimental();

        public ConfigCommandDescriptor()
        {
            Description = "Opens the config folder.";
            Options = vsOptions;
        }

        public bool Experimental => vsOptions.IsExperimental;
    }
}
