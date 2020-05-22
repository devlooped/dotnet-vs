using System;

namespace VisualStudio
{
    class ClientCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("Run").WithExperimental();
        readonly ClientOptions clientOptions = new ClientOptions();

        public ClientCommandDescriptor()
        {
            Description = "Launches Visual Studio in client mode";
            Options = vsOptions.With(clientOptions);
        }

        public virtual string WorkspaceId => clientOptions.WorkspaceId;

        public virtual bool IsExperimental => vsOptions.IsExperimental;
    }
}
