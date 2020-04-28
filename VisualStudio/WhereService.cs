using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using vswhere;

namespace VisualStudio
{
    class WhereService
    {
        readonly string vswherePath = Path.Combine(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location), "vswhere.exe");

        public Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(Sku? sku, Channel? channel) =>
            GetAllInstancesAsync(sku, channel, Enumerable.Empty<string>());

        public async Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(Sku? sku, Channel? channel, IEnumerable<string> extraArguments)
        {
            var psi = new ProcessStartInfo(vswherePath)
            {
                RedirectStandardOutput = true,
                ArgumentList =
                {
                    "-nologo",
                    "-prerelease",
                    "-format",
                    "json"
                }
            };

            foreach (var arg in extraArguments)
                psi.ArgumentList.Add(arg);

            if (sku != null)
            {
                psi.ArgumentList.Add("-products");
                psi.ArgumentList.Add("Microsoft.VisualStudio.Product." + sku);
            }

            var process = Process.Start(psi);

            var instances = JsonSerializer.Deserialize<VisualStudioInstance[]>(
                await process.StandardOutput.ReadToEndAsync(),
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            var channelId = GetChannelId(channel);
            return instances.Where(x => string.IsNullOrEmpty(channelId) || x.ChannelId == channelId);
        }

        public void ShowUsage(TextWriter output)
        {
            output.WriteLine();
            output.WriteLine("[vswhere.exe options]");

            var psi = new ProcessStartInfo(vswherePath)
            {
                RedirectStandardOutput = true,
                ArgumentList = { "-nologo", "-?" }
            };

            var process = Process.Start(psi);
            string line;
            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (line.StartsWith("Usage:") || line.StartsWith("Options:"))
                    continue;

                Console.WriteLine(line);
            }
        }

        string GetChannelId(Channel? channel)
        {
            switch (channel)
            {
                case Channel.Release:
                    return "VisualStudio.16.Release";
                case Channel.Preview:
                    return "VisualStudio.16.Preview";
                case Channel.IntPreview:
                    return "VisualStudio.16.IntPreview";
                case Channel.Master:
                    return "VisualStudio.16.int.master";
                default:
                    return default;
            }
        }
    }
}
