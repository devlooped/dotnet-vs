using System;
using Mono.Options;

namespace VisualStudio
{
    class ConfigCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Open", showExp: true, showNickname: false);

        public ConfigCommandDescriptor() => OptionSet = new CompositeOptionSet(options);

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public bool Experimental => options.IsExperimental;
    }
}
