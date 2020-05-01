using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudio
{
    class InstallCommand : Command<InstallCommandDescriptor>
    {
        readonly InstallerService installerService;

        public InstallCommand(InstallCommandDescriptor descriptor, InstallerService installerService) : base(descriptor)
        {
            this.installerService = installerService;
        }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var args = new List<string>();

            args.AddRange(Descriptor.WorkloadArgs);

            if (!string.IsNullOrEmpty(Descriptor.Nickname))
            {
                args.Add("--nickname");
                args.Add(Descriptor.Nickname);
            }

            args.AddRange(Descriptor.ExtraArguments);

            // TODO: for now, we assume we're always doing an install.
            var installBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "2019");

            // There is at least one install already, so use nicknames for the new one.
            if (Directory.Exists(installBase) && !args.Contains("--nickname"))
            {
                args.Add("--nickname");
                if (Descriptor.Channel == Channel.Preview)
                    args.Add("Preview");
                else if (Descriptor.Channel == Channel.IntPreview)
                    args.Add("IntPreview");
                else if (Descriptor.Channel == Channel.Master)
                    args.Add("master");
                else
                    args.Add(Descriptor.Sku.ToString().Substring(0, 3));
            }

            var installPath = Path.Combine(installBase, Descriptor.Sku.ToString());
            var customPath = Directory.Exists(installPath);
            if (customPath)
            {
                installPath = Path.Combine(installBase, Descriptor.Channel == Channel.Preview ? "Preview" : Descriptor.Channel == Channel.IntPreview ? "IntPreview" : Descriptor.Sku.ToString());
                if (Directory.Exists(installPath))
                {
                    installPath = Path.Combine(installBase, Descriptor.Channel == Channel.Preview ? "Pre" + Descriptor.Sku.ToString() : Descriptor.Channel == Channel.IntPreview ? "Int" + Descriptor.Sku.ToString() : Descriptor.Sku.ToString());
                }
            }

            if (customPath)
            {
                args.Add("--installPath");
                args.Add(installPath);
            }

            await installerService.InstallAsync(Descriptor.Channel, Descriptor.Sku, args, output);
        }
    }
}
