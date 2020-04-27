using System;
using Mono.Options;

namespace VisualStudio
{
    class KillCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Kill", showNickname: false, showExp: true);
        readonly AllOption allOption = new AllOption("Kill");

        public KillCommandDescriptor() => OptionSet = new CompositeOptionSet(options, allOption);

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;

        public bool IsExperimental => options.IsExperimental;

        public bool KillAll => allOption.All;
    }
}
