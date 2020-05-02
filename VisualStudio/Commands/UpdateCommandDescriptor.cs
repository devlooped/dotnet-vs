using System;

namespace VisualStudio
{
    class UpdateCommandDescriptor : CommandDescriptor
    {
        public UpdateCommandDescriptor()
        {
            Description = "Updates an installtion of Visual Studio.";
            Options = VisualStudioOptions.Default("Update");
        }
    }
}
