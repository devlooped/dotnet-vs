using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Devlooped
{
    class InstallCommand : Command<InstallCommandDescriptor>
    {
        readonly InstallerService installerService;

        public InstallCommand(InstallCommandDescriptor descriptor, InstallerService installerService) : base(descriptor) =>
            this.installerService = installerService;

        public override async Task ExecuteAsync(TextWriter output)
        {
            var args = new List<string>();

            args.AddRange(Descriptor.WorkloadArgs);

            if (!string.IsNullOrEmpty(Descriptor.Nickname))
            {
                args.Add("--nickname");
                args.Add(Descriptor.Nickname);
            }

            if (!Descriptor.ExtraArguments.Any(x => x.TrimStart('-') == "config") && File.Exists(".vsconfig"))
                args.AddRange(new[] { "--config", ".vsconfig" });

            args.AddRange(Descriptor.ExtraArguments);

            var vs = await installerService.GetLatestMajorAsync();
            var installBase = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "Microsoft Visual Studio",
                vs);

            // There is at least one install already, so use nicknames for the new one.
            if (Directory.Exists(installBase) && !args.Contains("--nickname"))
            {
                args.Add("--nickname");
                args.Add(ChannelFolderName(Descriptor.Channel) ?? Descriptor.Sku.ToString().Substring(0, 3));
            }

            var installPath = Path.Combine(installBase, Descriptor.Sku.ToString());
            var customPath = Directory.Exists(installPath);
            if (customPath)
            {
                installPath = Path.Combine(installBase, ChannelFolderName(Descriptor.Channel) ?? Descriptor.Sku.ToString());
                if (Directory.Exists(installPath))
                {
                    var prefix = Descriptor.Channel == Channel.Insiders ? "Pre" :
                        Descriptor.Channel == Channel.IntPreview ? "Int" : string.Empty;
                    installPath = Path.Combine(installBase, prefix + Descriptor.Sku.ToString());
                }
            }

            if (customPath)
            {
                args.Add("--installPath");
                args.Add(installPath);
            }

            await installerService.InstallAsync(vs, Descriptor.Channel, Descriptor.Sku, args, output);
        }

        static string ChannelFolderName(Channel? channel)
            => channel switch
            {
                Channel.Insiders => "Insiders",
                Channel.IntPreview => "IntPreview",
                Channel.Main => "main",
                _ => null,
            };
    }
}
