using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Devlooped
{
    class InstallerService
    {
        /// <summary>
        /// Resolves the latest Visual Studio major version by following the
        /// unversioned stable bootstrapper redirect (e.g. /vs/stable/ → /vs/18/stable/).
        /// </summary>
        public async Task<string> GetLatestMajorAsync()
        {
            try
            {
                using var handler = new HttpClientHandler { AllowAutoRedirect = false };
                using var client = new HttpClient(handler);
                using var request = new HttpRequestMessage(HttpMethod.Head, "https://aka.ms/vs/stable/vs_enterprise.exe");
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                var location = response.Headers.Location?.ToString();
                if (!string.IsNullOrEmpty(location))
                {
                    var match = Regex.Match(location, @"/vs/(\d+)/");
                    if (match.Success)
                        return match.Groups[1].Value;
                }
            }
            catch
            {
                // Fall back below.
            }

            return "18";
        }

        public async Task InstallAsync(Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
        {
            var vs = await GetLatestMajorAsync();
            await RunAsync(string.Empty, vs, channel, sku, args, output);
        }

        public Task InstallAsync(string vs, Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync(string.Empty, vs, channel, sku, args, output);

        public Task UpdateAsync(string vs, Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("update", vs, channel, sku, args, output);

        public Task ModifyAsync(string vs, Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("modify", vs, channel, sku, args, output);

        public Task UpdateAsync(string channelUri, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("update", channelUri, sku, args, output);

        public Task ModifyAsync(string channelUri, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("modify", channelUri, sku, args, output);

        Task RunAsync(string command, string vs, Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
        {
            // Microsoft.VisualStudio.Workload.NetCoreTools was replaced starting with VS 17.
            if (int.TryParse(vs, out var major) && major >= 17)
                args = args.Select(arg => arg == "Microsoft.VisualStudio.Workload.NetCoreTools" ? "Microsoft.NetCore.Component.DevelopmentTools" : arg);

            return RunAsync(command, $"https://aka.ms/vs/{vs}/{MapChannel(vs, channel)}", sku, args, output);
        }

        async Task RunAsync(string command, string channelUri, Sku? sku, IEnumerable<string> args, TextWriter output)
        {
            var bootstrapper = await DownloadAsync($"{channelUri}/vs_{MapSku(sku)}.exe", output);

            var psi = new ProcessStartInfo(bootstrapper)
            {
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            // install command should be empty
            if (!string.IsNullOrEmpty(command))
                psi.ArgumentList.Add(command);

            psi.ArgumentList.Add("--wait");

            if (args.Contains("--passive"))
                psi.ArgumentList.Add("--force");

            foreach (var arg in args)
                psi.ArgumentList.Add(arg);

            psi.Log(output);
            var process = Process.Start(psi);
            process.WaitForExit();
        }

        string MapChannel(string vs, Channel? channel)
            => (channel, vs) switch
            {
                (Channel.Insiders, "15" or "16" or "17") => "pre",
                (Channel.Insiders, _) => "insiders",
                (Channel.IntPreview, _) => "intpreview",
                (Channel.Main, _) => "int.main",
                // VS 2017-2022 used "release" for the current/stable channel.
                // VS 2026+ uses "stable".
                (_, "15" or "16" or "17") => "release",
                // Stable is the default; Channel.Stable and null both map here.
                // "release" is accepted on the CLI as a hidden alias for Stable.
                _ => "stable"
            };

        string MapSku(Sku? sku)
         => sku switch
         {
             Sku.Professional => "professional",
             Sku.Enterprise => "enterprise",
             Sku.BuildTools => "buildtools",
             Sku.TestAgent => "testagent",
             _ => "community"
         };

        async Task<string> DownloadAsync(string bootstrapperUrl, TextWriter output)
        {
            using var client = new HttpClient();
            output.WriteLine($"Downloading {bootstrapperUrl}...");
            using var request = new HttpRequestMessage(HttpMethod.Get, bootstrapperUrl);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), new Uri(bootstrapperUrl).Segments.Last());
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            using var httpStream = await response.Content.ReadAsStreamAsync();

            using var fileStream = File.Create(filePath);
            await httpStream.CopyToAsync(fileStream, 8 * 1024);

            return filePath;
        }
    }
}
