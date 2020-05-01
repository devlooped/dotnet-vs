using System;
using System.Linq;
using Mono.Options;

namespace VisualStudio
{
    class ChannelOption : OptionSet<Channel?>
    {
        readonly static string[] shortcuts = new[] { "pre", "preview", "int", "internal", "master", "rel", "release" };

        public ChannelOption(string verb, Channel? defaultValue = default) : base(defaultValue)
        {
            // This is the default, that's why it's hidden
            Add("rel|release", verb + " release version", _ => Value = Channel.Release, hidden: true);
            Add("pre|preview", verb + " preview version", _ => Value = Channel.Preview);
            Add("int|internal", verb + " internal (aka 'dogfood') version", _ => Value = Channel.IntPreview);
            Add("master", verb + " master version", _ => Value = Channel.Master, hidden: true);
        }

        protected override bool Parse(string argument, OptionContext c)
        {
            if (shortcuts.Contains(argument.ToLowerInvariant()))
                argument = "--" + argument.ToLowerInvariant();

            return base.Parse(argument, c);
        }
    }
}
