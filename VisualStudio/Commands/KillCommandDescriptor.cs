using System;

namespace VisualStudio
{
    class KillCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("kill").WithExperimental().WithSelectAll();

        public KillCommandDescriptor()
        {
            Description = "Kills running devenv processes.";
            Options = vsOptions;
        }

        public bool IsExperimental => vsOptions.IsExperimental;

        public bool KillAll => vsOptions.All;
    }
}
