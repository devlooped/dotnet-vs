using System;
using System.Linq;
using System.Collections.Immutable;
using Mono.Options;

namespace VisualStudio
{
    /// <summary>
    /// Provides visual studio selection options
    /// </summary>
    public class VisualStudioOptions : OptionSet
    {
        string[] channelShortcuts = new[] { "pre", "preview", "int", "internal", "master" };
        string[] skuShortcuts = new[] { "e", "ent", "enterprise", "p", "pro", "professional", "c", "com", "community" };

        public VisualStudioOptions(string channelVerb = "Install", bool showChannel = true, bool showSku = true, bool showNickname = true)
        {
            if (showChannel)
            {
                // Channel
                Add("pre|preview", channelVerb + " preview version", _ => Channel = VisualStudio.Channel.Preview);
                Add("int|internal", channelVerb + " internal (aka 'dogfood') version", _ => Channel = VisualStudio.Channel.IntPreview);
                Add("master", channelVerb + " master version", _ => Channel = VisualStudio.Channel.Master, hidden: true);
            }

            if (showSku)
            {
                // Sku
                Add("sku:", "Edition, one of [e|ent|enterprise], [p|pro|professional] or [c|com|community]", s => Sku = ParseSku(s));
            }

            if (showNickname)
            {
                // Nickname
                Add("nick|nickname:", "Optional nickname to use", n => Nickname = n);
            }
        }

        Sku ParseSku(string sku)
        {
            if (sku.StartsWith('e'))
                return VisualStudio.Sku.Enterprise;
            else if (sku.StartsWith("p"))
                return VisualStudio.Sku.Professional;
            else if (sku.StartsWith("c"))
                return VisualStudio.Sku.Community;
            else
                throw new OptionException($"Invalid SKU {sku}. Must be one of {string.Join(", ", Enum.GetNames(typeof(Sku)).Select(x => x.ToLowerInvariant()))}.", "sku");
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (channelShortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--" + argument;

            if (skuShortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--sku=" + argument;

            return base.Parse(argument, c);
        }

        public Channel? Channel { get; private set; }

        public Sku? Sku { get; private set; }

        public string Nickname { get; private set; }
    }
}
