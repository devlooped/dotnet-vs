using System;
using System.Linq;
using Mono.Options;

namespace VisualStudio
{
    public class SkuOption
    {
        public static Sku Parse(string sku)
        {
            if (sku.StartsWith('e'))
                return Sku.Enterprise;
            else if (sku.StartsWith("p"))
                return Sku.Professional;
            else if (sku.StartsWith("c"))
                return Sku.Community;
            else
                throw new OptionException($"Invalid SKU {sku}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Sku)).Select(x => x.ToLowerInvariant()))}.", "sku");
        }
    }
}
