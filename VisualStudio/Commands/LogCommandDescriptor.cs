using System;

namespace VisualStudio
{
    class LogCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("open").WithExperimental();

        public LogCommandDescriptor() => Options = vsOptions;

        public bool IsExperimental => vsOptions.IsExperimental;
    }
}
