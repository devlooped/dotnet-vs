using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VisualStudio
{
    class ProcessRunner : IProcessRunner
    {
        public async Task<string> StartAsync(string fileName, params string[] args)
        {
            var psi = new ProcessStartInfo(fileName)
            {
                RedirectStandardOutput = true
            };

            if (args != null)
            {
                foreach (var arg in args)
                    psi.ArgumentList.Add(arg);
            }

            var process = Process.Start(psi);
            var output = await process.StandardOutput.ReadToEndAsync();

            if (process.ExitCode == 0)
                return output;

            throw new Exception(output);
        }
    }
}
