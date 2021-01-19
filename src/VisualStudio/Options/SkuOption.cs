using System;
using System.Linq;
using Mono.Options;

namespace Devlooped
{
    class SkuOption : OptionSet<Sku?>
    {
        readonly static string[] shortcuts = new[]
        {
            "e", "ent", "enterprise",
            "p", "pro", "professional",
            "c", "com", "community",
            "b", "build", "buildtools",
            "t", "test", "testagent" };

        public SkuOption(Sku? defaultValue = default) : base(defaultValue)
        {
            Value = defaultValue;

            Add("sku:", "Edition, one of [e|ent|enterprise], [p|pro|professional], [c|com|community], [b|bld|buildtools] or [t|tsa|testagent]", s => Value = ParseSku(s));
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
                return Devlooped.Sku.Enterprise;
            else if (sku.StartsWith("p", StringComparison.OrdinalIgnoreCase))
                return Devlooped.Sku.Professional;
            else if (sku.StartsWith("c", StringComparison.OrdinalIgnoreCase))
                return Devlooped.Sku.Community;
            else if (sku.StartsWith("b", StringComparison.OrdinalIgnoreCase))
                return Devlooped.Sku.BuildTools;
            else if (sku.StartsWith("t", StringComparison.OrdinalIgnoreCase))
                return Devlooped.Sku.TestAgent;
            else
                throw new OptionException($"Invalid SKU {sku}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Sku)).Select(x => x.ToLowerInvariant()))}.", "sku");
        }
    }
}
