using System;
using System.IO;
using System.Threading.Tasks;
using DotNetConfig;
using NuGet.Versioning;
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

            await new VersionChecker(new NuGetVersion(42, 42, 42), config).ShowVersionAsync(writer);
            await new VersionChecker(new NuGetVersion(1, 0, 5), config).ShowVersionAsync(writer);

            var output = writer.ToString().Split(writer.NewLine);

            Assert.Contains(output, line => line.StartsWith("vs version 42.42.42"));
            Assert.Contains(output, line => line.StartsWith("vs version 1.0.5"));
        }

        [Fact]
        public async Task when_showing_version_for_development_version_then_renders_latest_tag()
        {
            var checker = new VersionChecker(new NuGetVersion(42, 42, 42), Config.Build(Path.GetTempFileName()));
            var writer = new StringWriter();

            await checker.ShowVersionAsync(writer);

            Assert.Contains(writer.ToString().Split(writer.NewLine), line => line.Contains($"{ThisAssembly.Project.RepositoryUrl}/releases/tag/v"));
        }

        [Fact]
        public async Task when_showing_version_for_old_version_then_renders_latest_tag()
        {
            var checker = new VersionChecker(new NuGetVersion(0, 1, 1), Config.Build(Path.GetTempFileName()));
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
            config = config
                .SetString("vs", "latest", "0.1.5", ConfigLevel.Local)
                .SetDateTime("vs", "checked", DateTime.Now - TimeSpan.FromDays(10), ConfigLevel.Local);

            var checker = new VersionChecker(new NuGetVersion(0, 1, 1), config);

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
            config = config
                .SetString("vs", "latest", "99.99.99", ConfigLevel.Local)
                .SetDateTime("vs", "checked", (DateTime.Now - TimeSpan.FromDays(3)), ConfigLevel.Local);

            var checker = new VersionChecker(new NuGetVersion(0, 1, 1), config);

            var writer = new StringWriter();
            await checker.ShowUpdateAsync(writer);

            var output = writer.ToString().Split(writer.NewLine);

            Assert.Contains(output, line => line.Contains("New version"));
            Assert.Contains(output, line => line.Contains($"{ThisAssembly.Project.RepositoryUrl}/releases/tag/v99.99.99"));
        }

        [Fact]
        public async Task when_latest_is_prerelease_then_compares_with_semver()
        {
            var config = Config.Build(Path.GetTempFileName());
            config = config
                .SetString("vs", "latest", "2.0.0-beta", ConfigLevel.Local)
                .SetDateTime("vs", "checked", DateTime.Now - TimeSpan.FromDays(1), ConfigLevel.Local);

            // Pre-release latest is newer than an older stable
            var writer = new StringWriter();
            await new VersionChecker(NuGetVersion.Parse("1.0.0"), config).ShowUpdateAsync(writer);
            Assert.Contains(writer.ToString().Split(writer.NewLine), line => line.Contains("New version 2.0.0-beta"));

            // Same pre-release is not newer
            writer = new StringWriter();
            await new VersionChecker(NuGetVersion.Parse("2.0.0-beta"), config).ShowUpdateAsync(writer);
            Assert.DoesNotContain(writer.ToString().Split(writer.NewLine), line => line.Contains("New version"));

            // Stable release is newer than pre-release with same major.minor.patch
            config = config.SetString("vs", "latest", "2.0.0", ConfigLevel.Local);
            writer = new StringWriter();
            await new VersionChecker(NuGetVersion.Parse("2.0.0-beta"), config).ShowUpdateAsync(writer);
            Assert.Contains(writer.ToString().Split(writer.NewLine), line => line.Contains("New version 2.0.0"));
            Assert.Contains(writer.ToString().Split(writer.NewLine), line => line.Contains("/releases/tag/v2.0.0"));
        }
    }
}
