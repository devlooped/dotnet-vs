using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using vswhere;

namespace Devlooped
{
    class VisualStudioPredicateBuilder
    {
        static readonly ScriptOptions scriptOptions = ScriptOptions.Default.AddReferences(typeof(VisualStudioInstance).Assembly);

        public async Task<Func<VisualStudioInstance, bool>> BuildPredicateAsync(VisualStudioFilter filter)
        {
            filter ??= new VisualStudioFilter();

            Func<VisualStudioInstance, bool> skuPredicate = _ => true;
            if (filter.Sku is Sku sku)
                skuPredicate = x => x.GetSku() == sku;

            Func<VisualStudioInstance, bool> channelPredicate = _ => true;
            if (filter.Channel is Channel channel)
                channelPredicate = x => x.GetChannel() == channel;

            Func<VisualStudioInstance, bool> filterPredicate = _ => true;
            if (!string.IsNullOrEmpty(filter.Expression))
                filterPredicate = await CSharpScript.EvaluateAsync<Func<VisualStudioInstance, bool>>(filter.Expression, scriptOptions);

            return x => skuPredicate(x) && channelPredicate(x) && filterPredicate(x);
        }
    }
}
