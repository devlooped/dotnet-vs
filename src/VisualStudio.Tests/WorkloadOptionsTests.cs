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
            var tokens = TokenRewriter.RewriteWorkloadAliases(arguments.Split(' '), "requires", prefix, argumentPrefix);

            // Split into rewritten requires pairs vs leftover
            var requires = new System.Collections.Generic.List<string>();
            var leftover = new System.Collections.Generic.List<string>();
            for (var i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == argumentPrefix + "requires" && i + 1 < tokens.Length)
                {
                    requires.Add(tokens[i]);
                    requires.Add(tokens[i + 1]);
                    i++;
                }
                else
                    leftover.Add(tokens[i]);
            }

            Assert.Equal(extra, string.Join(' ', leftover));
            Assert.Equal(parsed, string.Join(' ', requires));
        }

        [Theory]
        [InlineData("-core", "--remove Microsoft.VisualStudio.Workload.NetCoreTools", "")]
        [InlineData("-core --config .vsconfig", "--remove Microsoft.VisualStudio.Workload.NetCoreTools", "--config .vsconfig")]
        public void when_parsing_removes_then_converts_alias_to_argument(string arguments, string parsed, string extra)
        {
            var tokens = TokenRewriter.RewriteWorkloadAliases(arguments.Split(' '), "remove", "-", "--");

            var removes = new System.Collections.Generic.List<string>();
            var leftover = new System.Collections.Generic.List<string>();
            for (var i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "--remove" && i + 1 < tokens.Length)
                {
                    removes.Add(tokens[i]);
                    removes.Add(tokens[i + 1]);
                    i++;
                }
                else
                    leftover.Add(tokens[i]);
            }

            Assert.Equal(parsed, string.Join(' ', removes));
            Assert.Equal(extra, string.Join(' ', leftover));
        }
    }
}
