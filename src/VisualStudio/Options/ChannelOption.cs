using System;
using System.Linq;
using Mono.Options;

namespace Devlooped
{
    class ChannelOption : OptionSet<Channel?>
    {
        readonly static string[] shortcuts = new[]
        {
            "stable",
            "rel", "release",
            "insiders",
            "pre", "preview",
            "int", "internal",
            "main",
        };

        public ChannelOption(string verb, Channel? defaultValue = default) : base(defaultValue)
        {
            Add("stable", verb + " stable version", _ => Value = Channel.Stable);
            // Hidden alias kept for minimal backwards-compat with older CLI usage.
            Add("rel|release", verb + " stable version", _ => Value = Channel.Stable, hidden: true);
            Add("insiders", verb + " insiders version", _ => Value = Channel.Insiders);
            // Hidden aliases for the old preview channel name.
            Add("pre|preview", verb + " insiders version", _ => Value = Channel.Insiders, hidden: true);
            Add("int|internal", verb + " internal (aka 'dogfood') version", _ => Value = Channel.IntPreview);
            Add("main", verb + " main version", _ => Value = Channel.Main, hidden: true);
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (shortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--" + argument.ToLowerInvariant();

            return base.Parse(argument, c);
        }
    }
}
