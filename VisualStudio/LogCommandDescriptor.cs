using System;
using Mono.Options;

namespace VisualStudio
{
    class LogCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Open", showNickname: false);
        bool exp;

        public LogCommandDescriptor() => OptionSet = new CompositeOptionSet(options, new OptionSet
            {
                { "exp", "Use experimental instance instead of regular.", e => exp = e != null },
            });

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public bool Experimental => exp;
    }
}
