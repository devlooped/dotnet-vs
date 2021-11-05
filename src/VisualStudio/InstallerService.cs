using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Devlooped
{
    class InstallerService
    {
        public Task InstallAsync(Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync(string.Empty, channel, sku, args, output);

        public Task UpdateAsync(Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("update", channel, sku, args, output);

        public Task ModifyAsync(Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("modify", channel, sku, args, output);

        public Task UpdateAsync(string channelUri, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("update", channelUri, sku, args, output);

        public Task ModifyAsync(string channelUri, Sku? sku, IEnumerable<string> args, TextWriter output)
            => RunAsync("modify", channelUri, sku, args, output);

        Task RunAsync(string command, Channel? channel, Sku? sku, IEnumerable<string> args, TextWriter output)
        {
            var vs = channel == null || channel == Channel.Release ? "16" : "17";

            // Microsoft.VisualStudio.Workload.NetCoreTools > Microsoft.NetCore.Component.DevelopmentTools
            if (vs == "17")
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
