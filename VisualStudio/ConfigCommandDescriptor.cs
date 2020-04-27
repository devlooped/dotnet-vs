using System;
using Mono.Options;

namespace VisualStudio
{
    class ConfigCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Open", showNickname: false);
        bool exp;

        public ConfigCommandDescriptor() => OptionSet = new CompositeOptionSet(options, new OptionSet
            {
                { "exp", "Use experimental folder instead of regular.", e => exp = e != null },
            });

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public bool Experimental => exp;
    }
}
