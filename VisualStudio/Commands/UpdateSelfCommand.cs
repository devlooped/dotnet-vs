using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class UpdateSelfCommand : Command<UpdateSelfCommandDescriptor>
    {
        public UpdateSelfCommand(UpdateSelfCommandDescriptor descriptor) : base(descriptor) { }

        public override async Task ExecuteAsync(TextWriter output)
        {
            var psi = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                ArgumentList = { "tool", "update", "-g", "dotnet-vs" }
            };

            output.WriteLine(await Process.Start(psi).StandardOutput.ReadToEndAsync());
        }
    }
}
