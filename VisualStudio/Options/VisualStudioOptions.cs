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
        readonly IOptions options;
        readonly string verb;

        private VisualStudioOptions(string verb = "run")
            : this(verb, Options.Empty)
        { }

        private VisualStudioOptions(string verb, IOptions options)
        {
            this.verb = verb;
            this.options = options;
        }

        public static VisualStudioOptions Default(string verb = "run") =>
            new VisualStudioOptions(verb).WithChannel().WithSku().WithExpression();

        public static VisualStudioOptions Empty(string verb = "run") => new VisualStudioOptions(verb);

        public VisualStudioOptions WithChannel() => new VisualStudioOptions(verb, options.With(new ChannelOption(verb)));

        public VisualStudioOptions WithSku() => new VisualStudioOptions(verb, options.With(new SkuOption()));

        public VisualStudioOptions WithExperimental() => new VisualStudioOptions(verb, options.With(new ExperimentalOption(verb)));

        public VisualStudioOptions WithExpression() => new VisualStudioOptions(verb, options.With(new ExpressionOption()));

        public VisualStudioOptions WithSelectAll() => new VisualStudioOptions(verb, options.With(new SelectAllOption(verb)));

        public VisualStudioOptions WithNickname() => new VisualStudioOptions(verb, options.With(new NicknameOption()));

        public Channel? Channel => GetParsedValue<ChannelOption, Channel?>();

        public Sku? Sku => GetParsedValue<SkuOption, Sku?>();

        public bool IsExperimental => GetParsedValue<ExperimentalOption, bool>();

        public string Expression => GetParsedValue<ExpressionOption, string>();

        public bool All => GetParsedValue<SelectAllOption, bool>();

        public string Nickname => GetParsedValue<NicknameOption, string>();

        public IOptions With(OptionSet optionSet) => new VisualStudioOptions(verb, options.With(optionSet));

        public List<string> Parse(IEnumerable<string> arguments) => options.Parse(arguments);

        public void ShowUsage(TextWriter writer) => options.ShowUsage(writer);

        public T GetParsedValue<TOption, T>() where TOption : OptionSet<T> => options.GetParsedValue<TOption, T>();
    }
}
