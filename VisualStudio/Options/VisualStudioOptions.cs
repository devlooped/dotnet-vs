using System;
using Mono.Options;
using System.Collections.Generic;
using System.IO;

namespace VisualStudio
{
    /// <summary>
    /// Provides visual studio selection options
    /// </summary>
    class VisualStudioOptions : IOptions
    {
        public const string DefaultVerb = "run";

        readonly IOptions options;
        readonly string verb;

        private VisualStudioOptions(string verb)
            : this(verb, Options.Empty)
        { }

        private VisualStudioOptions(string verb, IOptions options)
        {
            this.verb = verb;
            this.options = options;
        }

        public static VisualStudioOptions Full(string verb = DefaultVerb) =>
            Default(verb).WithExperimental().WithNickname().WithSelectAll();

        public static VisualStudioOptions Default(string verb = DefaultVerb) =>
            Empty(verb).WithChannel().WithSku().WithFilter();

        public static VisualStudioOptions Empty(string verb = DefaultVerb) => new VisualStudioOptions(verb);

        public VisualStudioOptions WithChannel() => new VisualStudioOptions(verb, options.With(new ChannelOption(verb)));

        public VisualStudioOptions WithSku() => new VisualStudioOptions(verb, options.With(new SkuOption()));

        public VisualStudioOptions WithExperimental() => new VisualStudioOptions(verb, options.With(new ExperimentalOption(verb)));

        public VisualStudioOptions WithFilter() => new VisualStudioOptions(verb, options.With(new FilterOption()));

        public VisualStudioOptions WithSelectAll() => new VisualStudioOptions(verb, options.With(new SelectAllOption(verb)));

        public VisualStudioOptions WithNickname() => new VisualStudioOptions(verb, options.With(new NicknameOption()));

        public Channel? Channel => GetValue<ChannelOption, Channel?>();

        public Sku? Sku => GetValue<SkuOption, Sku?>();

        public bool IsExperimental => GetValue<ExperimentalOption, bool>();

        public string Expression => GetValue<FilterOption, string>();

        public bool All => GetValue<SelectAllOption, bool>();

        public string Nickname => GetValue<NicknameOption, string>();

        public IOptions With(OptionSet optionSet) => new VisualStudioOptions(verb, options.With(optionSet));

        public List<string> Parse(IEnumerable<string> arguments) => options.Parse(arguments);

        public void ShowUsage(ITextWriter writer) => options.ShowUsage(writer);

        public T GetValue<TOption, T>() where TOption : OptionSet<T> => options.GetValue<TOption, T>();
    }
}
