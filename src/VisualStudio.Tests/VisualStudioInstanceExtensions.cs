using System;
using System.Collections.Generic;
using Devlooped;

namespace vswhere
{
    static class VisualStudioInstanceExtensions
    {
        static readonly Dictionary<Sku, string> productIdBySku = new Dictionary<Sku, string>
        {
            { Sku.Enterprise, "Microsoft.VisualStudio.Product.Enterprise" },
            { Sku.Professional, "Microsoft.VisualStudio.Product.Professional" },
            { Sku.Community, "Microsoft.VisualStudio.Product.Community" },
            { Sku.BuildTools, "Microsoft.VisualStudio.Product.BuildTools" },
            { Sku.TestAgent, "Microsoft.VisualStudio.Product.TestAgent" }
        };

        // Channel IDs still use Release/Preview names even though CLI uses Stable/Insiders.
        static readonly Dictionary<Channel, string> productIdByChannel = new Dictionary<Channel, string>
        {
            { Channel.Stable, "VisualStudio.18.Release" },
            { Channel.Insiders, "VisualStudio.18.Preview" },
            { Channel.IntPreview, "VisualStudio.18.IntPreview" },
            { Channel.Main, "VisualStudio.18.int.main" },
        };

        public static VisualStudioInstance WithSku(this VisualStudioInstance vsInstance, Sku sku)
        {
            vsInstance.ProductId = productIdBySku[sku];
            return vsInstance;
        }

        public static VisualStudioInstance WithChannel(this VisualStudioInstance vsInstance, Channel channel)
        {
            vsInstance.ChannelId = productIdByChannel.TryGetValue(channel, out var channelId) ?
                vsInstance.ChannelId = channelId :
                throw new NotSupportedException("Cannot filter instances by the given channel.");

            return vsInstance;
        }
    }
}
