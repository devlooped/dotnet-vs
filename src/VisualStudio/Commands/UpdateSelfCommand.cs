using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    class UpdateSelfCommand : Command<UpdateSelfCommandDescriptor>
    {
        public UpdateSelfCommand(UpdateSelfCommandDescriptor descriptor) : base(descriptor) { }

        public override Task ExecuteAsync(TextWriter output)
        {
            // Fire and forget, otherwise we will get access denied
            Process.Start(
                new ProcessStartInfo("dotnet")
                {
                    ArgumentList = { "tool", "update", "-g", "dotnet-vs" }
                });

            output.WriteLine("Running \"dotnet tool update -g dotnet-vs\"...");
            output.WriteLine("dotnet will continue running in background");

            return Task.CompletedTask;
        }
    }
}
