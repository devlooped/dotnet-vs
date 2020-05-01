using System;

namespace VisualStudio
{
    class ConfigCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("open").WithExperimental();

        public ConfigCommandDescriptor() => Options = vsOptions;

        public bool Experimental => vsOptions.IsExperimental;
    }
}
