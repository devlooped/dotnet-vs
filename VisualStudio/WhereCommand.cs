using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using vswhere;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace VisualStudio
{
    class WhereCommand : Command
    {
        readonly WhereCommandDescriptor descriptor;

        public WhereCommand(WhereCommandDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        public bool Quiet { get; set; }

        public IEnumerable<VisualStudioInstance> Instances { get; private set; } = Enumerable.Empty<VisualStudioInstance>();

        public override async Task ExecuteAsync(TextWriter output)
        {
            Sku? sku = null;

            var formatJson = string.Join('=', descriptor.Arguments).Contains("-format=json", StringComparison.OrdinalIgnoreCase);

            var psi = new ProcessStartInfo(descriptor.VsWherePath)
            {
                RedirectStandardOutput = true,
                ArgumentList = { "-nologo" }
            };

            foreach (var arg in descriptor.WorkloadsArguments)
            {
                psi.ArgumentList.Add(arg);
            }

            foreach (var arg in descriptor.ExtraArguments)
            {
                psi.ArgumentList.Add(arg);
            }

            if (sku != null)
            {
                psi.ArgumentList.Add("-products");
                psi.ArgumentList.Add("Microsoft.VisualStudio.Product." + sku);
            }

            if (!Quiet)
                psi.Log(output);

            await ProcessOutput(psi, line =>
            {
                if (!Quiet)
                    output.WriteLine(line);
            }, formatJson);
        }

        private async Task<int> ProcessOutput(ProcessStartInfo psi, Action<string> lineAction, bool readJson)
        {
            var process = Process.Start(psi);
            string line;

            if (readJson)
            {
                var builder = new StringBuilder();
                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                {
                    lineAction(line);
                    builder.Append(line);
                }

                try
                {
                    Instances = JsonSerializer.Deserialize<VisualStudioInstance[]>(builder.ToString(), new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    });
                }
                catch (JsonException e)
                {
                    if (!Quiet)
                        lineAction("Failed to parse JSON from vswhere output.");

                    Debug.WriteLine($"Failed to parse JSON from vswhere: {e.Message}");
                }
            }
            else
            {
                while ((line = await process.StandardOutput.ReadLineAsync()) != null)
                {
                    lineAction(line);
                }
            }

            return process.ExitCode;
        }
    }
}
