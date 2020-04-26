using System;

namespace VisualStudio
{
    class UpdateCommandDescriptor : CommandDescriptor<UpdateCommand>
    {
        readonly VisualStudioOptions options = new VisualStudioOptions(showNickname: false);

        public UpdateCommandDescriptor() =>
            Options = new CompositeOptionsSet(options);

        public override string Name => "update";

        public Channel? Channel => options.Channel;

        public Sku? Sku => options.Sku;
    }
}
