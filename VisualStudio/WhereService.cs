using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using vswhere;

namespace VisualStudio
{
    class WhereService
    {
        readonly string vswherePath = Path.Combine(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location), "vswhere.exe");

        public Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync() =>
            GetAllInstancesAsync(Options.Empty);

        public Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(IOptions options) =>
            GetAllInstancesAsync(options, Enumerable.Empty<string>());

        public async Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(IOptions options, IEnumerable<string> extraArguments)
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

            var process = Process.Start(psi);

            var instances = JsonSerializer.Deserialize<VisualStudioInstance[]>(
                await process.StandardOutput.ReadToEndAsync(),
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            return instances.Where(await new VisualStudioPredicateBuilder().BuildPredicateAsync(options));
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
    }
}
