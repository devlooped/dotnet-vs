using System;

namespace VisualStudio
{
    class UpdateCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(channelVerb: "Update", showNickname: false);

        public UpdateCommandDescriptor() => OptionSet = new CompositeOptionSet(options);

        protected override VisualStudioOptions VisualStudioOptions => options;
    }
}
