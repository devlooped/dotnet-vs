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

        public VisualStudioOptions()
        {
            // Channel
            Add("pre|preview", "Install preview version", _ => Channel = Channel.Preview);
            Add("int|internal", "Install internal (aka 'dogfood') version", _ => Channel = Channel.IntPreview);
            Add("master", "Install master version", _ => Channel = Channel.Master, hidden: true);

            // Sku
            Add("sku:", "Edition, one of [e|ent|enterprise], [p|pro|professional] or [c|com|community]. Defaults to 'community'.", s => Sku = SkuOption.Parse(s));

            // Nickname
            Add("nick|nickname:", "Optional nickname to assign to the installation", n => Nickname = n);
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (channelShortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--" + argument;

            if (skuShortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--sku=" + argument;

            return base.Parse(argument, c);
        }

        public Channel Channel { get; private set; } = Channel.Release;

        public Sku Sku { get; private set; } = Sku.Community;

        public string Nickname { get; private set; }
    }
}
