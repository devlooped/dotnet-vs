using System;
using Xunit;

namespace VisualStudio.Tests
{
    public class VisualStudioOptionsTests
    {
        [Theory]
        [InlineData("", default)]
        [InlineData("rel", Channel.Release)]
        [InlineData("release", Channel.Release)]
        [InlineData("Release", Channel.Release)]
        [InlineData("--rel", Channel.Release)]
        [InlineData("--release", Channel.Release)]
        [InlineData("pre", Channel.Preview)]
        [InlineData("preview", Channel.Preview)]
        [InlineData("Preview", Channel.Preview)]
        [InlineData("--pre", Channel.Preview)]
        [InlineData("--preview", Channel.Preview)]
        [InlineData("int", Channel.IntPreview)]
        [InlineData("internal", Channel.IntPreview)]
        [InlineData("--int", Channel.IntPreview)]
        [InlineData("--internal", Channel.IntPreview)]
        [InlineData("master", Channel.Master)]
        [InlineData("--master", Channel.Master)]
        public void when_parsing_channel_argument_then_channel_is_set(string argument, Channel? expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithChannel();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Channel);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("e", Sku.Enterprise)]
        [InlineData("ent", Sku.Enterprise)]
        [InlineData("enterprise", Sku.Enterprise)]
        [InlineData("Ent", Sku.Enterprise)]
        [InlineData("Enterprise", Sku.Enterprise)]
        [InlineData("--sku=e", Sku.Enterprise)]
        [InlineData("--sku=ent", Sku.Enterprise)]
        [InlineData("--sku=enterprise", Sku.Enterprise)]
        [InlineData("--sku=Enterprise", Sku.Enterprise)]
        [InlineData("p", Sku.Professional)]
        [InlineData("pro", Sku.Professional)]
        [InlineData("professional", Sku.Professional)]
        [InlineData("Pro", Sku.Professional)]
        [InlineData("Professional", Sku.Professional)]
        [InlineData("--sku=p", Sku.Professional)]
        [InlineData("--sku=pro", Sku.Professional)]
        [InlineData("--sku=professional", Sku.Professional)]
        [InlineData("--sku=Professional", Sku.Professional)]
        [InlineData("c", Sku.Community)]
        [InlineData("com", Sku.Community)]
        [InlineData("community", Sku.Community)]
        [InlineData("Com", Sku.Community)]
        [InlineData("Community", Sku.Community)]
        [InlineData("--sku=c", Sku.Community)]
        [InlineData("--sku=com", Sku.Community)]
        [InlineData("--sku=community", Sku.Community)]
        [InlineData("--sku=Community", Sku.Community)]
        public void when_parsing_sku_argument_then_sku_is_set(string argument, Sku? expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithSku();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Sku);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("--nick=nick1", "nick1")]
        [InlineData("--nickname=nick2", "nick2")]
        public void when_parsing_nickname_argument_then_nickname_is_set(string argument, string expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithNickname();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Nickname);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("exp", true)]
        [InlineData("experimental", true)]
        [InlineData("--exp", true)]
        [InlineData("--experimental", true)]
        public void when_parsing_experimental_then_experimental_is_set(string argument, bool expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithExperimental();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.IsExperimental);
        }

        [Theory]
        [InlineData("", default)]
        [InlineData("x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("--expr= x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("--expression= x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("/expr: x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        [InlineData("/expression: x => x.Prop == 'value'", "x => x.Prop == \"value\"")]
        public void when_parsing_expression_then_exppression_is_set(string argument, string expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithExpression();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.Expression);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("all", true)]
        [InlineData("All", true)]
        [InlineData("--all", true)]
        public void when_parsing_all_argument_then_all_is_set(string argument, bool expectedValue)
        {
            var options = VisualStudioOptions.Empty().WithSelectAll();

            options.Parse(new[] { argument });

            Assert.Equal(expectedValue, options.All);
        }

        [Theory]
        [InlineData(Sku.Enterprise, Channel.Preview, "x => x.InstanceId == \"123\"", "Enterprise", "Preview", "x => x.InstanceId == '123'")]
        [InlineData(Sku.Enterprise, Channel.Preview, "x => x.InstanceId == \"123\"", "/sku:Enterprise", "--preview", "x => x.InstanceId == '123'")]
        public void when_parsing_with_default_options_then_sku_and_channel_and_expression_are_set(Sku expectedSku, Channel expectedChannel, string expectedExpression, params string[] arguments)
        {
            var options = VisualStudioOptions.Default();

            options.Parse(arguments);

            Assert.Equal(expectedSku, options.Sku);
            Assert.Equal(expectedChannel, options.Channel);
            Assert.Equal(expectedExpression, options.Expression);
        }
    }
}
