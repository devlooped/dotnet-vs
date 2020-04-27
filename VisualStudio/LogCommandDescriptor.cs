using System;
using Mono.Options;

namespace VisualStudio
{
    class LogCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Open", showNickname: false, showExp: true);

        public LogCommandDescriptor() => OptionSet = new CompositeOptionSet(options);

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public bool IsExperimental => options.IsExperimental;
    }
}
