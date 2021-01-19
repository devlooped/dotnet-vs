using Mono.Options;

namespace Devlooped
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
