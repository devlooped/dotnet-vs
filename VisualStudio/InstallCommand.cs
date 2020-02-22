using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace VisualStudio
{
    public class InstallCommand : Command
    {
        readonly OptionSet options;
        readonly WorkloadOptions workloads;
        ImmutableArray<string> parsed = ImmutableArray.Create<string>();
        bool help = false;
        bool preview;
        bool dogfood;
        Sku sku = Sku.Community;

        public InstallCommand()
        {
            options = new OptionSet
            {
                { "pre|preview", "Install preview version", _ => preview = true },
                { "int|internal", "Install internal (aka 'dogfood') version", _ => dogfood = true },
                { "sku:", "Edition, one of [e|ent|enterprise], [p|pro|professional] or [c|com|community]. Defaults to 'community'.", s => sku = SkuOption.Parse(s) },
            };
            workloads = new WorkloadOptions("--add")
            {
                { "?|h|help", "Display this help", h => help = h != null },
            };
        }

        public override string Name => "install";

        public override async Task<int> ExecuteAsync(IEnumerable<string> args, TextWriter output)
        {
            try
            {
                var extra = workloads.Parse(options.Parse(args));
                if (help)
                {
                    ShowUsage(output);
                    return 0;
                }

                var uri = new StringBuilder("https://aka.ms/vs/16/");
                if (preview)
                    uri = uri.Append("pre/");
                else if (dogfood)
                    uri = uri.Append("intpreview/");
                else
                    uri = uri.Append("release/");

                uri = uri.Append("vs_");
                switch (sku)
                {
                    case Sku.Community:
                        uri = uri.Append("community");
                        break;
                    case Sku.Professional:
                        uri = uri.Append("professional");
                        break;
                    case Sku.Enterprise:
                        uri = uri.Append("enterprise");
                        break;
                    default:
                        break;
                }

                uri = uri.Append(".exe");
                var bootstrapper = await DownloadAsync(uri.ToString(), output);

                var psi = new ProcessStartInfo(bootstrapper);
                foreach (var arg in workloads.Arguments)
                {
                    psi.ArgumentList.Add(arg);
                }
                foreach (var arg in extra)
                {
                    psi.ArgumentList.Add(arg);
                }

                // TODO: for now, we assume we're always doing an install.
                var installBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "2019");

                // There is at least one install already, so use nicknames for the new one.
                if (Directory.Exists(installBase))
                {
                    psi.ArgumentList.Add("--nickname");
                    if (preview)
                        psi.ArgumentList.Add("Preview");
                    else if (dogfood)
                        psi.ArgumentList.Add("IntPreview");
                    else
                        psi.ArgumentList.Add(sku.ToString().Substring(0, 3));
                }

                var installPath = Path.Combine(installBase, sku.ToString());
                var customPath = Directory.Exists(installPath);
                if (customPath)
                {
                    installPath = Path.Combine(installBase, preview ? "Preview" : dogfood ? "IntPreview" : sku.ToString());
                    if (Directory.Exists(installPath))
                    {
                        installPath = Path.Combine(installBase, preview ? "Pre" + sku.ToString() : dogfood ? "Int" + sku.ToString() : sku.ToString());
                    }
                }

                if (customPath)
                {
                    psi.ArgumentList.Add("--installPath");
                    psi.ArgumentList.Add(installPath);
                }

                output.WriteLine($"Running {bootstrapper} {string.Join(' ', psi.ArgumentList)}");

                var process = Process.Start(psi);
                process.WaitForExit();

                return process.ExitCode;
            }
            catch (OptionException e)
            {
                output.WriteLine(e.Message);
                ShowUsage(output);
            }

            return 0;
        }

        public override void ShowOptions(TextWriter output)
        {
            options.WriteOptionDescriptions(output);
            output.WriteLine("      Workload ID aliases:");
            workloads.ShowOptions(output);
        }

        private void ShowUsage(TextWriter output)
        {
            ShowUsage(output, options);
            output.WriteLine("      Workload ID aliases:");
            workloads.ShowOptions(output);
        }

        private async Task<string> DownloadAsync(string bootstrapperUrl, TextWriter output)
        {
            using (var client = new HttpClient())
            {
                output.WriteLine($"Downloading {bootstrapperUrl}");
                var request = new HttpRequestMessage(HttpMethod.Get, bootstrapperUrl);
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var filePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(request.RequestUri.AbsolutePath));
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = File.Create(filePath))
                    {
                        await httpStream.CopyToAsync(fileStream, 8 * 1024);
                    }
                }

                return filePath;
            }
        }
    }
}
