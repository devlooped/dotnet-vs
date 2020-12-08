using System;
using Mono.Options;

namespace VisualStudio
{
    class ClientOptions : OptionSet
    {
        public ClientOptions()
        {
            Add("workspaceId:", "The workspace ID to connect to", x => WorkspaceId = x);
        }

        public string WorkspaceId { get; set; }
    }
}
