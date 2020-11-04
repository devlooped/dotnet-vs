using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using DevEnv = vswhere.VisualStudioInstance;

namespace VisualStudio
{
    class ClientCommand : Command<ClientCommandDescriptor>, IDisposable
    {
        const string JoinToken = "/join?";

        readonly WhereService whereService;

        public ClientCommand(ClientCommandDescriptor descriptor, WhereService whereService)
            : base(descriptor)
        {
            this.whereService = whereService;
        }

        public Process Server { get; private set; }

        public Process Client { get; private set; }

        public override async Task ExecuteAsync(TextWriter output) =>
            await ExecuteAsync(new Chooser().Choose(await whereService.GetAllInstancesAsync(Descriptor.Options), output), output);

        public Task ExecuteAsync(DevEnv devenv, TextWriter output)
        {
            if (string.IsNullOrEmpty(Descriptor.WorkspaceId))
                StartServerAndClient(devenv, output);
            else
                StartClient(devenv, Descriptor.WorkspaceId, output);

            return Task.CompletedTask;
        }

        void StartClient(DevEnv devenv, string workspaceId, TextWriter output)
        {
            var args = new List<string>();

            if (Descriptor.IsExperimental)
                args.AddRange(new[] { "/rootSuffix", "Exp" });

            args.Add("/client");
            args.AddRange(new[] { "/joinworkspace", $"vsls:?workspaceId={workspaceId}&remoteJoin=true" });

            output.WriteLine($"Starting client: {devenv.ProductPath} {string.Join(" ", args)}");

            Client = CreateProcess(devenv, args);

        }

        void StartServerAndClient(DevEnv devenv, TextWriter output)
        {
            var args = new List<string>(Descriptor.ExtraArguments);

            // If no sln or folder path was provided, use the current directory
            if (!args.Any())
                args.Add(Directory.GetCurrentDirectory());

            if (Descriptor.IsExperimental)
                args.AddRange(new[] { "/rootSuffix", "Exp" });

            args.Add("/server");

            output.WriteLine($"Starting server: {devenv.ProductPath} {string.Join(" ", args)}");

            Server = CreateProcess(devenv, args);

            foreach (var line in ReadOutputLines(Server))
            {
                output.WriteLine("[devenv] " + line);

                if (line.LastIndexOf(JoinToken) is int joinIndexOf && joinIndexOf != -1)
                    StartClient(devenv, line.Substring(joinIndexOf + JoinToken.Length), output);
            }
        }

        protected virtual Process CreateProcess(DevEnv devenv, IEnumerable<string> args, bool start = true)
        {
            var psi = new ProcessStartInfo(devenv.ProductPath)
            {
                RedirectStandardOutput = true,
            };

            if (args != null)
            {
                foreach (var arg in args)
                    psi.ArgumentList.Add(arg);
            }

            var process = new Process() { StartInfo = psi };

            if (start)
                process.Start();

            return process;
        }

        protected virtual IEnumerable<string> ReadOutputLines(Process process)
        {
            string line;
            while ((line = process.StandardOutput.ReadLine()) != null)
                yield return line;
        }

        public void Dispose() => Server?.Kill();
    }
}
