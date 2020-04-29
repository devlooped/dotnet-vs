using System;

namespace VisualStudio
{
    class ConfigCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Open", showExp: true, showNickname: false);

        public ConfigCommandDescriptor() => OptionSet = new CompositeOptionSet(options);

        protected override VisualStudioOptions VisualStudioOptions => options;

        public bool Experimental => options.IsExperimental;
    }
}
