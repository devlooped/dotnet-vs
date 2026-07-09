using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetConfig;
using NuGet.Versioning;

namespace Devlooped
{
    /// <summary>
    /// Checks the current version of the assembly/tool against the GitHub releases.
    /// </summary>
    class VersionChecker
    {
        static readonly NuGetVersion developmentVersion = new(42, 42, 42);

        readonly NuGetVersion currentVersion;
        readonly ConfigLevel saveLevel;
        readonly string repositoryUrl;
        readonly Task<NuGetVersion> getLatest;
        ConfigSection configuration;

        public VersionChecker()
            : this(ParseVersion(ThisAssembly.Project.Version) ?? developmentVersion,
                  Config.Build(ConfigLevel.Global),
                  saveLevel: ConfigLevel.Global)
        {
        }

        // For testing
        internal VersionChecker(NuGetVersion currentVersion, Config configuration,
            string section = ThisAssembly.Project.AssemblyName,
            string repositoryUrl = ThisAssembly.Project.RepositoryUrl,
            ConfigLevel saveLevel = ConfigLevel.Local)
        {
            this.currentVersion = currentVersion;
            this.configuration = configuration.GetSection(section);
            this.repositoryUrl = repositoryUrl;
            this.saveLevel = saveLevel;

            // Send version check async up-front, to be awaited only when some action 
            // is requested.
            getLatest = Task.Run(() => GetLatestAsync());
        }

        public bool NoOp { get; set; }

        public async Task ShowVersionAsync(TextWriter output)
        {
            output.WriteLine($"{ThisAssembly.Project.AssemblyName} version {currentVersion.ToNormalizedString()} ({ThisAssembly.Project.DateTime})");

            if (NoOp)
                return;

            // Showing version explicitly checks upstream, so we'll clear the last checked date.
            configuration.Unset("checked");

            var latestVersion = await getLatest;

            if (latestVersion == developmentVersion)
                // Couldn't check latest version for some reason
                output.WriteLine($"Latest version at {repositoryUrl}/releases/latest");
            else if (latestVersion > currentVersion)
                output.WriteLine($"New version {latestVersion.ToNormalizedString()} is available. Run 'dnx {ThisAssembly.Project.AssemblyName} -- update-self' to update. See {repositoryUrl}/releases/tag/v{latestVersion.ToNormalizedString()}");
            else if (currentVersion == developmentVersion)
                output.WriteLine($"Latest version {latestVersion.ToNormalizedString()} is available at {repositoryUrl}/releases/tag/v{latestVersion.ToNormalizedString()}");

            output.WriteLine();
        }

        public async Task ShowUpdateAsync(TextWriter output)
        {
            if (NoOp)
                return;

            var latestVersion = await getLatest;

            // We expect a redirect from /latest to the actual latest tag URL
            if (currentVersion != developmentVersion &&
                latestVersion != developmentVersion &&
                latestVersion > currentVersion)
            {
                output.WriteLine($"New version {latestVersion.ToNormalizedString()} is available. Run 'dnx {ThisAssembly.Project.AssemblyName} -- update-self' to update. See {repositoryUrl}/releases/tag/v{latestVersion.ToNormalizedString()}");
            }
        }

        async Task<NuGetVersion> GetLatestAsync()
        {
            var lastChecked = configuration.GetDateTime("checked");
            var latestSaved = configuration.GetString("latest");
            var latestVersion = ParseVersion(latestSaved) ?? developmentVersion;

            // We check once a week at most.
            if (lastChecked == null ||
                (DateTime.Now - lastChecked) > TimeSpan.FromDays(7))
            {
                var response = await new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
                    .SendAsync(new HttpRequestMessage(HttpMethod.Head, $"{ThisAssembly.Project.RepositoryUrl}/releases/latest"));

                if (response.StatusCode == HttpStatusCode.Found)
                {
                    var latestTagUrl = response.Headers.Location.ToString();
                    var tag = latestTagUrl.Split('/').Last().TrimStart('v');
                    if (ParseVersion(tag) is NuGetVersion parsedLatest)
                    {
                        latestVersion = parsedLatest;
                        configuration = configuration
                            .SetString("latest", latestVersion.ToNormalizedString(), saveLevel)
                            // NOTE: we only save checked date if we succeeded at checking latest.
                            .SetDateTime("checked", DateTime.Now, saveLevel);
                    }
                }
            }

            return latestVersion;
        }

        static NuGetVersion ParseVersion(string value)
            => !string.IsNullOrEmpty(value) && NuGetVersion.TryParse(value, out var version) ? version : null;
    }
}
