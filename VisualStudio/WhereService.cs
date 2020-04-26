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

        static readonly object singletonLock = new object();
        static WhereService instance;

        public static WhereService Instance
        {
            get
            {
                lock (singletonLock)
                {
                    if (instance == null)
                        instance = new WhereService();
                }

                return instance;
            }
        }

        public Task RunAsync(Sku? sku, IEnumerable<string> extraArguments, TextWriter output) =>
            RunCore(sku, extraArguments, output: output);

        public Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(Sku? sku, Channel? channel) =>
            GetAllInstancesAsync(sku, channel, Enumerable.Empty<string>());

        public async Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(Sku? sku, Channel? channel, IEnumerable<string> extraArguments)
        {
            var channelId = GetChannelId(channel);

            var collectedInstances = new List<VisualStudioInstance>();
            await RunCore(sku, extraArguments.Concat(new[] { "-prerelease" }), collectedInstances);

            return collectedInstances.Where(x => string.IsNullOrEmpty(channelId) || x.ChannelId == channelId);
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

        async Task RunCore(
            Sku? sku,
            IEnumerable<string> extraArguments,
            List<VisualStudioInstance> instancesToBeCollected = null,
            TextWriter output = null)
        {
            var psi = new ProcessStartInfo(vswherePath)
            {
                RedirectStandardOutput = true,
                ArgumentList = { "-nologo" }
            };

            if (instancesToBeCollected != null)
            {
                psi.ArgumentList.Add("-format");
                psi.ArgumentList.Add("json");
            }

            foreach (var arg in extraArguments)
                psi.ArgumentList.Add(arg);

            if (sku != null)
            {
                psi.ArgumentList.Add("-products");
                psi.ArgumentList.Add("Microsoft.VisualStudio.Product." + sku);
            }

            if (output != null)
                psi.Log(output);

            await ProcessOutput(psi, output, instancesToBeCollected);
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

        async Task<int> ProcessOutput(ProcessStartInfo psi, TextWriter output, List<VisualStudioInstance> instancesToBeCollected)
        {
            var process = Process.Start(psi);
            string line;

            if (instancesToBeCollected != null)
            {
                var builder = new StringBuilder();
                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                {
                    if (output != null)
                        output.WriteLine(line);

                    builder.Append(line);
                }

                try
                {
                    instancesToBeCollected.AddRange(JsonSerializer.Deserialize<VisualStudioInstance[]>(builder.ToString(), new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    }));
                }
                catch (JsonException e)
                {
                    if (output != null)
                        output.WriteLine("Failed to parse JSON from vswhere output.");

                    Debug.WriteLine($"Failed to parse JSON from vswhere: {e.Message}");
                }
            }
            else if (output != null)
            {
                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                    output.WriteLine(line);
            }

            return process.ExitCode;
        }
    }
}
