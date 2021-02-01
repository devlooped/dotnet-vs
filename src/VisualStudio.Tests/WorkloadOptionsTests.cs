using Xunit;

namespace Devlooped.Tests
{
    public class WorkloadOptionsTests
    {
        [Theory]
        [InlineData("+", "--", "+core", "--requires Microsoft.VisualStudio.Workload.NetCoreTools", "")]
        [InlineData("+", "--", "+core +mobile", "--requires Microsoft.VisualStudio.Workload.NetCoreTools --requires Microsoft.VisualStudio.Workload.NetCrossPlat", "")]
        [InlineData("+", "--", "+core -version [\"16.8,)\"]", "--requires Microsoft.VisualStudio.Workload.NetCoreTools", "-version [\"16.8,)\"]")]
        [InlineData("+", "--", "+Microsoft.VisualStudio.SomeComponent", "--requires Microsoft.VisualStudio.SomeComponent", "")]
        [InlineData("+", "--", "-someswitch", "", "-someswitch")]
        public void when_parsing_requires_then_converts_alias_to_argument(string prefix, string argumentPrefix, string arguments, string parsed, string extra)
        {
            var options = new WorkloadOptions("requires", prefix, argumentPrefix);

            var extraArgs = options.Parse(arguments.Split(' '));

            Assert.Equal(extra, string.Join(' ', extraArgs));
            Assert.Equal(parsed, string.Join(' ', options.Value));
        }

        [Theory]
        [InlineData("-core", "--remove Microsoft.VisualStudio.Workload.NetCoreTools", "")]
        [InlineData("-core --config .vsconfig", "--remove Microsoft.VisualStudio.Workload.NetCoreTools", "--config .vsconfig")]
        public void when_parsing_removes_then_converts_alias_to_argument(string arguments, string parsed, string extra)
        {
            var options = new WorkloadOptions("remove", "-", "--");

            var extraArgs = options.Parse(arguments.Split(' '));

            Assert.Equal(parsed, string.Join(' ', options.Value));
            Assert.Equal(extra, string.Join(' ', extraArgs));
        }
    }
}
