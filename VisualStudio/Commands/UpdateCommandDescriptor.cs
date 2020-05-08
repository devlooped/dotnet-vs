using System;

namespace VisualStudio
{
    class UpdateCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("Update").WithSelectAll();

        public UpdateCommandDescriptor()
        {
            Description = "Updates an installtion of Visual Studio.";
            Options = vsOptions;
        }

        public bool All => vsOptions.All;
    }
}
