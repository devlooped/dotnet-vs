namespace VisualStudio
{
    class UpdateCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("Update")
            .WithFirst()
            .WithSelectAll();

        public UpdateCommandDescriptor()
        {
            Description = "Updates an installation of Visual Studio.";
            Options = vsOptions;
        }

        public bool All => vsOptions.All;
    }
}
