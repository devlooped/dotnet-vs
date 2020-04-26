using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VisualStudio;

namespace vswhere
{
    static class VisualStudioInstanceExtensions
    {
        public static Sku GetSku(this VisualStudioInstance vsInstance)
        {
            switch (vsInstance.ProductId)
            {
                case "Microsoft.VisualStudio.Product.Enterprise":
                    return Sku.Enterprise;
                case "Microsoft.VisualStudio.Product.Professional":
                    return Sku.Professional;
                case "Microsoft.VisualStudio.Product.Community":
                    return Sku.Community;
            }

            throw new ArgumentException($"Invalid SKU {vsInstance.ProductId}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Sku)).Select(x => x.ToLowerInvariant()))}.", "sku");
        }

        public static Channel GetChannel(this VisualStudioInstance vsInstance)
        {
            switch (vsInstance.ChannelId)
            {
                case "VisualStudio.16.Release":
                    return Channel.Release;
                case "VisualStudio.16.Preview":
                    return Channel.Preview;
                case "VisualStudio.16.IntPreview":
                    return Channel.IntPreview;
                case "VisualStudio.16.int.master":
                    return Channel.Master;
            }

            throw new ArgumentException($"Invalid ChannelId {vsInstance.ChannelId}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Channel)).Select(x => x.ToLowerInvariant()))}.", "sku");
        }
    }
}
