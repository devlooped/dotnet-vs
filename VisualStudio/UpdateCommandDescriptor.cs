using System;

namespace VisualStudio
{
    class UpdateCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showNickname: false);

        public UpdateCommandDescriptor() => optionSet = new CompositeOptionsSet(options);

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;
    }
}
