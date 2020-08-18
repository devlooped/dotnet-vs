using System;
using System.Collections.Generic;
using VisualStudio;

namespace vswhere
{
    static class VisualStudioInstanceExtensions
    {
        static readonly Dictionary<Sku, string> productIdBySku = new Dictionary<Sku, string>
        {
            { Sku.Enterprise, "Microsoft.VisualStudio.Product.Enterprise" },
            { Sku.Professional, "Microsoft.VisualStudio.Product.Professional" },
            { Sku.Community, "Microsoft.VisualStudio.Product.Community" }
        };

        static readonly Dictionary<Channel, string> productIdByChannel = new Dictionary<Channel, string>
        {
            { Channel.Release, "VisualStudio.16.Release" },
            { Channel.Preview, "VisualStudio.16.Preview" },
            { Channel.IntPreview, "VisualStudio.16.IntPreview" },
            { Channel.Main, "VisualStudio.16.int.main" },
        };

        public static VisualStudioInstance WithSku(this VisualStudioInstance vsInstance, Sku sku)
        {
            vsInstance.ProductId = productIdBySku[sku];
            return vsInstance;
        }

        public static VisualStudioInstance WithChannel(this VisualStudioInstance vsInstance, Channel channel)
        {
            vsInstance.ChannelId = productIdByChannel[channel];
            return vsInstance;
        }
    }
}
