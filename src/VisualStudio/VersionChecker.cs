using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetConfig;

namespace Devlooped
{
    /// <summary>
    /// Checks the current version of the assembly/tool against the GitHub releases.
    /// </summary>
    class VersionChecker
    {
        static readonly Version developmentVersion = new Version(42, 42, 42, 0);

        readonly Version currentVersion;
        readonly ConfigLevel saveLevel;
        readonly string repositoryUrl;
        readonly Task<Version> getLatest;
        ConfigSection configuration;

        public VersionChecker()
            : this(new Version(ThisAssembly.Info.Version), Config.Build(ConfigLevel.Global), saveLevel: ConfigLevel.Global)
        {
        }

        // For testing
        internal VersionChecker(Version currentVersion, Config configuration,
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
            output.WriteLine($"{ThisAssembly.Project.AssemblyName} version {currentVersion.ToString(3)} ({ThisAssembly.Project.DateTime})");

            if (NoOp)
                return;

            // Showing version explicitly checks upstream, so we'll clear the last checked date.
            configuration.Unset("checked");

            var latestVersion = await getLatest;

            if (latestVersion == developmentVersion)
                // Couldn't check latest version for some reason
                output.WriteLine($"Latest version at {repositoryUrl}/releases/latest");
            else if (latestVersion > currentVersion)
                output.WriteLine($"New version {latestVersion} is available. Run '{ThisAssembly.Project.AssemblyName} update-self' to update. See {repositoryUrl}/releases/tag/v{latestVersion}");
            else if (currentVersion == developmentVersion)
                output.WriteLine($"Latest version {latestVersion} is available at {repositoryUrl}/releases/tag/v{latestVersion}");

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
                output.WriteLine($"New version {latestVersion} is available. Run '{ThisAssembly.Project.AssemblyName} update-self' to update. See {repositoryUrl}/releases/tag/v{latestVersion}");
            }
        }

        async Task<Version> GetLatestAsync()
        {
            var lastChecked = configuration.GetDateTime("checked");
            var latestSaved = configuration.GetString("latest");
            var latestVersion = Version.TryParse(latestSaved, out var parsed) ? parsed : developmentVersion;

            // We check once a week at most.
            if (lastChecked == null ||
                (DateTime.Now - lastChecked) > TimeSpan.FromDays(7))
            {
                var response = await new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
                    .SendAsync(new HttpRequestMessage(HttpMethod.Head, $"{ThisAssembly.Project.RepositoryUrl}/releases/latest"));

                if (response.StatusCode == HttpStatusCode.Found)
                {
                    var latestTagUrl = response.Headers.Location.ToString();
                    latestVersion = new Version(latestTagUrl.Split('/').Last().Trim('v'));
                    configuration = configuration
                        .SetString("latest", latestVersion.ToString(3), saveLevel)
                        // NOTE: we only save checked date if we succeeded at checking latest.
                        .SetDateTime("checked", DateTime.Now, saveLevel);
                }
            }

            return latestVersion;
        }
    }
}
