using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DotNetConfig;
using Xunit;
using Xunit.Abstractions;

namespace Devlooped.Tests
{
    public class VersionCheckerTests
    {
        readonly ITestOutputHelper output;

        public VersionCheckerTests(ITestOutputHelper output) => this.output = output;

        [Fact]
        public async Task when_showing_version_then_outputs_current_version()
        {
            var writer = new StringWriter();
            var config = Config.Build(Path.GetTempFileName());

            await new VersionChecker(new Version(42, 42, 42, 0), config).ShowVersionAsync(writer);
            await new VersionChecker(new Version(1, 0, 5), config).ShowVersionAsync(writer);

            var output = writer.ToString().Split(writer.NewLine);

            Assert.Contains(output, line => line.StartsWith("vs version 42.42.42"));
            Assert.Contains(output, line => line.StartsWith("vs version 1.0.5"));
        }

        [Fact]
        public async Task when_showing_version_for_development_version_then_renders_latest_tag()
        {
            var checker = new VersionChecker(new Version(42, 42, 42, 0), Config.Build(Path.GetTempFileName()));
            var writer = new StringWriter();

            await checker.ShowVersionAsync(writer);

            Assert.Contains(writer.ToString().Split(writer.NewLine), line => line.Contains($"{ThisAssembly.Project.RepositoryUrl}/releases/tag/v"));
        }

        [Fact]
        public async Task when_showing_version_for_old_version_then_renders_latest_tag()
        {
            var checker = new VersionChecker(new Version(0, 1, 1), Config.Build(Path.GetTempFileName()));
            var writer = new StringWriter();

            await checker.ShowVersionAsync(writer);

            var output = writer.ToString().Split(writer.NewLine);

            Assert.Contains(output, line => line.Contains("New version"));
            Assert.Contains(output, line => line.Contains("update-self"));
            Assert.Contains(output, line => line.Contains($"{ThisAssembly.Project.RepositoryUrl}/releases/tag/v"));
        }

        [Fact]
        public async Task when_showing_version_after_week_from_last_check_then_updates_to_latest()
        {
            var config = Config.Build(Path.GetTempFileName());
            config.SetString("vs", "latest", "0.1.5");
            config.SetDateTime("vs", "checked", DateTime.Now - TimeSpan.FromDays(10));

            var checker = new VersionChecker(new Version(0, 1, 1), config);

            var writer = new StringWriter();

            await checker.ShowVersionAsync(writer);

            var output = writer.ToString().Split(writer.NewLine);

            Assert.Contains(output, line => line.Contains("New version"));
            Assert.Contains(output, line => line.Contains("update-self"));
            Assert.Contains(output, line => line.Contains($"{ThisAssembly.Project.RepositoryUrl}/releases/tag/v"));
        }

        [Fact]
        public async Task when_showing_update_same_week_then_returns_last_checked()
        {
            var config = Config.Build(Path.GetTempFileName());
            config.SetString("vs", "latest", "99.99.99");
            config.SetDateTime("vs", "checked", (DateTime.Now - TimeSpan.FromDays(3)));

            var checker = new VersionChecker(new Version(0, 1, 1), config);

            var writer = new StringWriter();
            await checker.ShowUpdateAsync(writer);

            var output = writer.ToString().Split(writer.NewLine);

            Assert.Contains(output, line => line.Contains("New version"));
            Assert.Contains(output, line => line.Contains($"{ThisAssembly.Project.RepositoryUrl}/releases/tag/v99.99.99"));
        }
    }
}
