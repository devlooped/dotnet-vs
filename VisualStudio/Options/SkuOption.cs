using System;
using System.Linq;
using Mono.Options;

namespace VisualStudio
{
    class SkuOption : OptionSet<Sku?>
    {
        readonly static string[] shortcuts = new[] { "e", "ent", "enterprise", "p", "pro", "professional", "c", "com", "community" };

        public SkuOption(Sku? defaultValue = default) : base(defaultValue)
        {
            Value = defaultValue;

            Add("sku:", "Edition, one of [e|ent|enterprise], [p|pro|professional] or [c|com|community]", s => Value = ParseSku(s));
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (shortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--sku=" + argument;

            return base.Parse(argument, c);
        }

        Sku ParseSku(string sku)
        {
            if (sku.StartsWith("e", StringComparison.OrdinalIgnoreCase))
                return VisualStudio.Sku.Enterprise;
            else if (sku.StartsWith("p", StringComparison.OrdinalIgnoreCase))
                return VisualStudio.Sku.Professional;
            else if (sku.StartsWith("c", StringComparison.OrdinalIgnoreCase))
                return VisualStudio.Sku.Community;
            else
                throw new OptionException($"Invalid SKU {sku}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Sku)).Select(x => x.ToLowerInvariant()))}.", "sku");
        }
    }
}
