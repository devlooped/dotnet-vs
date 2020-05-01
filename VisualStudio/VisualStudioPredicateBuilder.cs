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
            if (options.GetParsedValue<SkuOption, Sku?>() is Sku sku)
                skuPredicate = x => x.GetSku() == sku;

            Func<VisualStudioInstance, bool> channelPredicate = _ => true;
            if (options.GetParsedValue<ChannelOption, Channel?>() is Channel channel)
                channelPredicate = x => x.GetChannel() == channel;

            Func<VisualStudioInstance, bool> exprPredicate = _ => true;
            if (options.GetParsedValue<ExpressionOption, string>() is string expression && !string.IsNullOrEmpty(expression))
                exprPredicate = await CSharpScript.EvaluateAsync<Func<VisualStudioInstance, bool>>(expression, scriptOptions);

            return x => skuPredicate(x) && channelPredicate(x) && exprPredicate(x);
        }
    }
}
