using System.Threading.Tasks;
using vswhere;
using Xunit;

namespace Devlooped.Tests
{
    public class VisualStudioPredicateBuilderTests
    {
        [Fact]
        public async Task when_evaluating_sku_then_predicate_matches_configured_sku()
        {
            var builder = new VisualStudioPredicateBuilder();

            var predicate = await builder.BuildPredicateAsync(GetFilter(sku: Sku.Enterprise));

            Assert.True(predicate(new vswhere.VisualStudioInstance().WithSku(Sku.Enterprise)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithSku(Sku.Professional)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithSku(Sku.Community)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithSku(Sku.BuildTools)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithSku(Sku.TestAgent)));
        }

        [Fact]
        public async Task when_evaluating_channel_then_predicate_matches_configured_channel()
        {
            var builder = new VisualStudioPredicateBuilder();

            var predicate = await builder.BuildPredicateAsync(GetFilter(channel: Channel.Insiders));

            Assert.True(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.Insiders)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.Stable)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.Main)));
        }

        [Fact]
        public async Task when_evaluating_expression_then_predicate_matches_configured_expression()
        {
            var builder = new VisualStudioPredicateBuilder();

            var predicate = await builder.BuildPredicateAsync(GetFilter(expression: "x => x.InstanceId == \"123\""));

            Assert.True(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "456" }));
            Assert.False(predicate(new vswhere.VisualStudioInstance()));
        }

        [Fact]
        public async Task when_evaluating_combined_criterias_then_predicate_matches_configured_criterias()
        {
            var builder = new VisualStudioPredicateBuilder();

            var filter = GetFilter(
                sku: Sku.Professional,
                channel: Channel.IntPreview,
                expression: "x => x.InstanceId == \"123\"");

            var predicate = await builder.BuildPredicateAsync(filter);

            Assert.True(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }.WithSku(Sku.Professional).WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "456" }.WithSku(Sku.Professional).WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }.WithSku(Sku.Enterprise).WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }.WithSku(Sku.Professional).WithChannel(Channel.Stable)));
        }

        static VisualStudioFilter GetFilter(Sku? sku = null, Channel? channel = null, string expression = null) =>
            new VisualStudioFilter(Channel: channel, Sku: sku, Expression: expression);
    }
}
