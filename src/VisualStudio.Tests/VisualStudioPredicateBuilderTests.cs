using System.Threading.Tasks;
using vswhere;
using Xunit;

namespace VisualStudio.Tests
{
    public class VisualStudioPredicateBuilderTests
    {
        [Fact]
        public async Task when_evaluating_sku_then_predicate_matches_configured_sku()
        {
            var builder = new VisualStudioPredicateBuilder();

            var predicate = await builder.BuildPredicateAsync(GetOptions(sku: Sku.Enterprise));

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

            var predicate = await builder.BuildPredicateAsync(GetOptions(channel: Channel.Preview));

            Assert.True(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.Preview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.Release)));
            Assert.False(predicate(new vswhere.VisualStudioInstance().WithChannel(Channel.Main)));
        }

        [Fact]
        public async Task when_evaluating_expression_then_predicate_matches_configured_expression()
        {
            var builder = new VisualStudioPredicateBuilder();

            var predicate = await builder.BuildPredicateAsync(GetOptions(expression: "x => x.InstanceId == \"123\""));

            Assert.True(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "456" }));
            Assert.False(predicate(new vswhere.VisualStudioInstance()));
        }

        [Fact]
        public async Task when_evaluating_combined_criterias_then_predicate_matches_configured_criterias()
        {
            var builder = new VisualStudioPredicateBuilder();

            var options = GetOptions(
                sku: Sku.Professional,
                channel: Channel.IntPreview,
                expression: "x => x.InstanceId == \"123\"");

            var predicate = await builder.BuildPredicateAsync(options);

            Assert.True(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }.WithSku(Sku.Professional).WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "456" }.WithSku(Sku.Professional).WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }.WithSku(Sku.Enterprise).WithChannel(Channel.IntPreview)));
            Assert.False(predicate(new vswhere.VisualStudioInstance() { InstanceId = "123" }.WithSku(Sku.Professional).WithChannel(Channel.Release)));
        }

        IOptions GetOptions(Sku? sku = null, Channel? channel = null, string expression = null) =>
            new Options(
                new SkuOption(sku),
                new ChannelOption("test", channel),
                new FilterOption(expression));
    }
}
