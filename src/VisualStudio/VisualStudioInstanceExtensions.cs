using System;
using System.Linq;
using VisualStudio;

namespace vswhere
{
    static class VisualStudioInstanceExtensions
    {
        public static Sku GetSku(this VisualStudioInstance vsInstance)
            => vsInstance.ProductId switch
            {
                "Microsoft.VisualStudio.Product.Enterprise" => Sku.Enterprise,
                "Microsoft.VisualStudio.Product.Professional" => Sku.Professional,
                "Microsoft.VisualStudio.Product.Community" => Sku.Community,
                "Microsoft.VisualStudio.Product.BuildTools" => Sku.BuildTools,
                "Microsoft.VisualStudio.Product.TestAgent" => Sku.TestAgent,
                _ => throw new ArgumentException($"Invalid SKU {vsInstance.ProductId}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Sku)).Select(x => x.ToLowerInvariant()))}.", "sku"),
            };

        public static Channel? GetChannel(this VisualStudioInstance vsInstance)
            => vsInstance.ChannelId switch
            {
                "VisualStudio.16.Release" => Channel.Release,
                "VisualStudio.16.Preview" => Channel.Preview,
                "VisualStudio.16.IntPreview" => Channel.IntPreview,
                "VisualStudio.16.int.main" => Channel.Main,
                _ => null,
            };
    }
}
