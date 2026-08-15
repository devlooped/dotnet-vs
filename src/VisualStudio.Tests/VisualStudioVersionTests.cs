using Xunit;

namespace Devlooped.Tests
{
    public class VisualStudioVersionTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("18", "18")]
        [InlineData("18.7", "18")]
        [InlineData("18.7.3", "18")]
        [InlineData("17.14.16", "17")]
        public void when_getting_major_then_returns_major_component(string version, string expected) =>
            Assert.Equal(expected, VisualStudioVersion.GetMajor(version));

        [Theory]
        [InlineData("18.7.3", null, true)]
        [InlineData("18.7.3", "", true)]
        [InlineData("18.7.3", "18", true)]
        [InlineData("18.7.3", "18.7", true)]
        [InlineData("18.7.3", "18.7.3", true)]
        [InlineData("18.8.0", "18.7", false)]
        [InlineData("17.14.16", "18", false)]
        [InlineData(null, "18.7", false)]
        public void when_matching_then_uses_semantic_prefix(string product, string requested, bool expected) =>
            Assert.Equal(expected, VisualStudioVersion.Matches(product, requested));
    }
}
