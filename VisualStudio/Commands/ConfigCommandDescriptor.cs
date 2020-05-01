using System;

namespace VisualStudio
{
    class ConfigCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("open").WithExperimental();

        public ConfigCommandDescriptor()
        {
            Description = "Opens the config folder";
            Options = vsOptions;
        }

        public bool Experimental => vsOptions.IsExperimental;
    }
}
