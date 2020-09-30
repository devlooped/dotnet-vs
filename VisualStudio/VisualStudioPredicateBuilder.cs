using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using vswhere;

namespace VisualStudio
{
    class VisualStudioPredicateBuilder
    {
        static readonly ScriptOptions scriptOptions = ScriptOptions.Default.AddReferences(typeof(VisualStudioInstance).Assembly);

        public async Task<Func<VisualStudioInstance, bool>> BuildPredicateAsync(IOptions options)
        {
            Func<VisualStudioInstance, bool> skuPredicate = _ => true;
            if (options.GetValue<SkuOption, Sku?>() is Sku sku)
                skuPredicate = x => x.GetSku() == sku;

            Func<VisualStudioInstance, bool> channelPredicate = _ => true;
            if (options.GetValue<ChannelOption, Channel?>() is Channel channel)
                channelPredicate = x => x.GetChannel() == channel;

            Func<VisualStudioInstance, bool> filterPredicate = _ => true;
            if (options.GetValue<FilterOption, string>() is string filter && !string.IsNullOrEmpty(filter))
                filterPredicate = await CSharpScript.EvaluateAsync<Func<VisualStudioInstance, bool>>(filter, scriptOptions);

            return x => skuPredicate(x) && channelPredicate(x) && filterPredicate(x);
        }
    }
}
