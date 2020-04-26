using System;
using Xunit;

namespace VisualStudio.Tests
{
    public class VisualStudioOptionsTests
    {
        [Theory]
        [InlineData("pre", Channel.Preview)]
        [InlineData("preview", Channel.Preview)]
        [InlineData("--pre", Channel.Preview)]
        [InlineData("--preview", Channel.Preview)]
        [InlineData("int", Channel.IntPreview)]
        [InlineData("internal", Channel.IntPreview)]
        [InlineData("--int", Channel.IntPreview)]
        [InlineData("--internal", Channel.IntPreview)]
        [InlineData("master", Channel.Master)]
        [InlineData("--master", Channel.Master)]
        public void when_parsing_channel_argument_then_channel_is_set(string channelArgument, Channel expectedChannel)
        {
            var options = new VisualStudioOptions();

            options.Parse(new[] { channelArgument });

            Assert.Equal(expectedChannel, options.Channel);
        }

        [Theory]
        [InlineData("e", Sku.Enterprise)]
        [InlineData("ent", Sku.Enterprise)]
        [InlineData("enterprise", Sku.Enterprise)]
        [InlineData("--sku=e", Sku.Enterprise)]
        [InlineData("--sku=ent", Sku.Enterprise)]
        [InlineData("--sku=enterprise", Sku.Enterprise)]
        [InlineData("p", Sku.Professional)]
        [InlineData("pro", Sku.Professional)]
        [InlineData("professional", Sku.Professional)]
        [InlineData("--sku=p", Sku.Professional)]
        [InlineData("--sku=pro", Sku.Professional)]
        [InlineData("--sku=professional", Sku.Professional)]
        [InlineData("c", Sku.Community)]
        [InlineData("com", Sku.Community)]
        [InlineData("community", Sku.Community)]
        [InlineData("--sku=c", Sku.Community)]
        [InlineData("--sku=com", Sku.Community)]
        [InlineData("--sku=community", Sku.Community)]
        public void when_parsing_sku_argument_then_sku_is_set(string skuArgument, Sku expectedSku)
        {
            var options = new VisualStudioOptions();

            options.Parse(new[] { skuArgument });

            Assert.Equal(expectedSku, options.Sku);
        }

        [Theory]
        [InlineData("--nick=nick1", "nick1")]
        [InlineData("--nickname=nick2", "nick2")]
        public void when_parsing_nickname_argument_then_nickname_is_set(string nicknameArgument, string expectedNickname)
        {
            var options = new VisualStudioOptions();

            options.Parse(new[] { nicknameArgument });

            Assert.Equal(expectedNickname, options.Nickname);
        }
    }
}
