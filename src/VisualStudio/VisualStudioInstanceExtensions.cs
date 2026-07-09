using System;
using System.Linq;
using Devlooped;

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

        /// <summary>
        /// Maps installed channel IDs to CLI channels.
        /// Note: VS still uses VisualStudio.18.Release / .Preview channel IDs
        /// even though marketing/bootstrapper paths use Stable / Insiders.
        /// </summary>
        public static Channel? GetChannel(this VisualStudioInstance vsInstance)
            => vsInstance.ChannelId switch
            {
                "VisualStudio.18.Release" => Channel.Stable,
                "VisualStudio.18.Preview" => Channel.Insiders,
                "VisualStudio.18.IntPreview" => Channel.IntPreview,
                "VisualStudio.18.int.main" => Channel.Main,
                _ => null,
            };
    }
}
