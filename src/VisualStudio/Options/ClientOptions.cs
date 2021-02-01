using Mono.Options;

namespace Devlooped
{
    class ClientOptions : OptionSet
    {
        public ClientOptions()
        {
            Add("w|workspaceId:", "The workspace ID to connect to", x => WorkspaceId = x);
        }

        public string WorkspaceId { get; set; }
    }
}
