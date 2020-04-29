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

        public async Task<Func<VisualStudioInstance, bool>> BuildPredicateAsync(VisualStudioOptions options)
        {
            Func<VisualStudioInstance, bool> skuPredicate = _ => true;
            if (options?.Sku != null)
                skuPredicate = x => x.GetSku() == options.Sku;

            Func<VisualStudioInstance, bool> channelPredicate = _ => true;
            if (options?.Channel != null)
                channelPredicate = x => x.GetChannel() == options.Channel;

            Func<VisualStudioInstance, bool> exprPredicate = _ => true;
            if (options?.Expression != null)
                exprPredicate = await CSharpScript.EvaluateAsync<Func<VisualStudioInstance, bool>>(options.Expression, scriptOptions);

            return x => skuPredicate(x) && channelPredicate(x) && exprPredicate(x);
        }
    }
}
