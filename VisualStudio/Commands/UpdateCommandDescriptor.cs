using System;

namespace VisualStudio
{
    class UpdateCommandDescriptor : CommandDescriptor
    {
        public UpdateCommandDescriptor() => Options = VisualStudioOptions.Default("Update");
    }
}
