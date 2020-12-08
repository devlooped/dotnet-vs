namespace VisualStudio
{
    class LogCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("open")
            .WithFirst()
            .WithExperimental();

        public LogCommandDescriptor()
        {
            Description = "Opens the folder containing the Activity.log file.";
            Options = vsOptions;
        }

        public bool IsExperimental => vsOptions.IsExperimental;
    }
}
