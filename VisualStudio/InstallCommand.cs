using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio
{
    class InstallCommand : Command
    {
        readonly InstallCommandDescriptor descriptor;

        public InstallCommand(InstallCommandDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var uri = new StringBuilder("https://aka.ms/vs/16/");
            if (descriptor.Preview)
                uri = uri.Append("pre/");
            else if (descriptor.Dogfood)
                uri = uri.Append("intpreview/");
            else
                uri = uri.Append("release/");

            uri = uri.Append("vs_");
            switch (descriptor.Sku)
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
            foreach (var arg in descriptor.WorkloadArgs)
            {
                psi.ArgumentList.Add(arg);
            }
            if (!string.IsNullOrEmpty(descriptor.Nickname))
            {
                psi.ArgumentList.Add("--nickname");
                psi.ArgumentList.Add(descriptor.Nickname);
            }
            foreach (var arg in descriptor.ExtraArguments)
            {
                psi.ArgumentList.Add(arg);
            }

            // TODO: for now, we assume we're always doing an install.
            var installBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "2019");

            // There is at least one install already, so use nicknames for the new one.
            if (Directory.Exists(installBase) && !psi.ArgumentList.Contains("--nickname"))
            {
                psi.ArgumentList.Add("--nickname");
                if (descriptor.Preview)
                    psi.ArgumentList.Add("Preview");
                else if (descriptor.Dogfood)
                    psi.ArgumentList.Add("IntPreview");
                else
                    psi.ArgumentList.Add(descriptor.Sku.ToString().Substring(0, 3));
            }

            var installPath = Path.Combine(installBase, descriptor.Sku.ToString());
            var customPath = Directory.Exists(installPath);
            if (customPath)
            {
                installPath = Path.Combine(installBase, descriptor.Preview ? "Preview" : descriptor.Dogfood ? "IntPreview" : descriptor.Sku.ToString());
                if (Directory.Exists(installPath))
                {
                    installPath = Path.Combine(installBase, descriptor.Preview ? "Pre" + descriptor.Sku.ToString() : descriptor.Dogfood ? "Int" + descriptor.Sku.ToString() : descriptor.Sku.ToString());
                }
            }

            if (customPath)
            {
                psi.ArgumentList.Add("--installPath");
                psi.ArgumentList.Add(installPath);
            }

            psi.Log(output);
            var process = Process.Start(psi);
            process.WaitForExit();
        }

        async Task<string> DownloadAsync(string bootstrapperUrl, TextWriter output)
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
