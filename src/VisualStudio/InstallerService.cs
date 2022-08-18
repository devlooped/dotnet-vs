using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Devlooped.Web;

namespace Devlooped
{
    class InstallerService
    {
        public Task InstallAsync(Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
        {
            // determine what's the latest & greatest VS from the docs site
            // TODO: if the channel is preview, there' no easy way to get the major version from the web.
            var html = HtmlDocument.Load("https://docs.microsoft.com/en-us/visualstudio/install/use-command-line-parameters-to-install-visual-studio");
            var vs = html.CssSelectElements("a[href^=https://aka.ms/vs/][href$=/vs_enterprise.exe]")
                .Select(e => Regex.Match(e.Attribute("href").Value!, "/(\\d\\d)/").Groups[1].Value)
                .OrderByDescending(x => x)
                .FirstOrDefault() ?? "17";

            return RunAsync(string.Empty, vs, channel, sku, args, output);
        }

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
            // Microsoft.VisualStudio.Workload.NetCoreTools > Microsoft.NetCore.Component.DevelopmentTools
            if (int.TryParse(vs, out var major) && major >= 17)
                args = args.Select(arg => arg == "Microsoft.VisualStudio.Workload.NetCoreTools" ? "Microsoft.NetCore.Component.DevelopmentTools" : arg);

            return RunAsync(command, $"https://aka.ms/vs/{vs}/{MapChannel(channel)}", sku, args, output);
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

        string MapChannel(Channel? channel)
            => channel switch
            {
                Channel.Preview => "pre",
                Channel.IntPreview => "intpreview",
                Channel.Main => "int.main",
                _ => "release"
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
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using var httpStream = await response.Content.ReadAsStreamAsync();

            using var fileStream = File.Create(filePath);
            await httpStream.CopyToAsync(fileStream, 8 * 1024);

            return filePath;
        }
    }
}
