using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using vswhere;

namespace Devlooped
{
    class WhereService
    {
        readonly string vswherePath = Path.Combine(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location), "vswhere.exe");

        public Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync() =>
            GetAllInstancesAsync(new VisualStudioFilter());

        public Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(VisualStudioFilter filter) =>
            GetAllInstancesAsync(filter, Enumerable.Empty<string>());

        public async Task<IEnumerable<VisualStudioInstance>> GetAllInstancesAsync(VisualStudioFilter filter, IEnumerable<string> extraArguments)
        {
            filter ??= new VisualStudioFilter();

            var psi = new ProcessStartInfo(vswherePath)
            {
                RedirectStandardOutput = true,
                ArgumentList =
                {
                    "-products",
                    "*",
                    "-nologo",
                    "-prerelease",
                    "-format",
                    "json"
                }
            };

            foreach (var arg in extraArguments)
            {
                if (arg != null)
                    psi.ArgumentList.Add(arg);
            }

            var process = Process.Start(psi);
            var output = await process.StandardOutput.ReadToEndAsync();

            if (process.ExitCode != 0)
                throw new WhereException(output);

            var instances = JsonSerializer.Deserialize<VisualStudioInstance[]>(
                output,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            var result = instances.Where(await new VisualStudioPredicateBuilder().BuildPredicateAsync(filter));

            if (filter.First)
                return result.Take(1);

            return result;
        }
    }
}
