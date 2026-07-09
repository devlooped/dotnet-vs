using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using DevEnv = vswhere.VisualStudioInstance;

namespace Devlooped.Tests
{
    public class ClientCommandTests
    {
        readonly TextWriter output;

        public ClientCommandTests(ITestOutputHelper output) =>
            this.output = new OutputHelperTextWriter(output);

        [Theory]
        [InlineData(true, "")]
        [InlineData(false, "")]
        [InlineData(true, @"c:\src\foo.sln")]
        [InlineData(false, @"c:\src\foo.sln")]
        public async Task when_workspace_id_is_not_specified_then_server_and_client_are_started(bool isExperimental, string solutionPath)
        {
            var command = new ClientCommandTest();
            var session = new ClientCommand.ClientSession();
            var extra = string.IsNullOrEmpty(solutionPath)
                ? new List<string>()
                : new List<string> { solutionPath };

            await command.RunServerAndClientAsync(session, new DevEnv(), extra, isExperimental, output);

            Assert.NotNull(session.Server);
            Assert.Contains("/server", session.Server.StartInfo.ArgumentList);

            if (isExperimental)
            {
                Assert.Contains("/rootSuffix", session.Server.StartInfo.ArgumentList);
                Assert.Contains("Exp", session.Server.StartInfo.ArgumentList);
            }

            if (!string.IsNullOrEmpty(solutionPath))
                Assert.Contains(solutionPath, session.Server.StartInfo.ArgumentList);

            Assert.NotNull(session.Client);
            Assert.Contains("/client", session.Client.StartInfo.ArgumentList);
            Assert.Contains("/joinworkspace", session.Client.StartInfo.ArgumentList);
            Assert.Contains($"vsls:?workspaceId={command.GeneratedServerWorkspaceId}&remoteJoin=true", session.Client.StartInfo.ArgumentList);

            if (isExperimental)
            {
                Assert.Contains("/rootSuffix", session.Client.StartInfo.ArgumentList);
                Assert.Contains("Exp", session.Client.StartInfo.ArgumentList);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task when_workspace_id_is_specified_then_client_is_started(bool isExperimental)
        {
            var command = new ClientCommandTest();
            var session = new ClientCommand.ClientSession();

            command.RunClient(session, new DevEnv(), "123", isExperimental, output);

            Assert.Null(session.Server);
            Assert.NotNull(session.Client);
            Assert.Contains("/client", session.Client.StartInfo.ArgumentList);
            Assert.Contains("/joinworkspace", session.Client.StartInfo.ArgumentList);
            Assert.Contains("vsls:?workspaceId=123&remoteJoin=true", session.Client.StartInfo.ArgumentList);

            if (isExperimental)
            {
                Assert.Contains("/rootSuffix", session.Client.StartInfo.ArgumentList);
                Assert.Contains("Exp", session.Client.StartInfo.ArgumentList);
            }
        }

        [Fact]
        public async Task when_starting_server_and_client_then_arguments_are_defined_in_correct_order()
        {
            var command = new ClientCommandTest();
            var session = new ClientCommand.ClientSession();

            await command.RunServerAndClientAsync(session, new DevEnv(), new List<string> { @"c:\src\foo.sln" }, true, output);

            Assert.NotNull(session.Server);
            Assert.Equal(
                @"c:\src\foo.sln /rootSuffix Exp /server",
                string.Join(" ", session.Server.StartInfo.ArgumentList));

            Assert.NotNull(session.Client);
            Assert.Equal(
                $"/rootSuffix Exp /client /joinworkspace vsls:?workspaceId={command.GeneratedServerWorkspaceId}&remoteJoin=true",
                string.Join(" ", session.Client.StartInfo.ArgumentList));
        }

        /// <summary>
        /// Testable surface over ClientCommand process creation / server output.
        /// </summary>
        class ClientCommandTest : ClientCommand
        {
            public ClientCommandTest() : base(null) { }

            public string GeneratedServerWorkspaceId { get; } = Guid.NewGuid().ToString();

            public Task RunServerAndClientAsync(ClientSession session, DevEnv devenv, List<string> extra, bool isExperimental, TextWriter output)
            {
                StartServerAndClientForTests(session, devenv, extra, isExperimental, output);
                return Task.CompletedTask;
            }

            public void RunClient(ClientSession session, DevEnv devenv, string workspaceId, bool isExperimental, TextWriter output) =>
                StartClientForTests(session, devenv, workspaceId, isExperimental, output);

            protected override Process CreateProcess(DevEnv devenv, IEnumerable<string> args, bool start = true) =>
                base.CreateProcess(devenv, args, false);

            protected override IEnumerable<string> ReadOutputLines(Process process)
            {
                yield return "Start Live Share Session command enabled: True";
                yield return "Start Live Share Session command succeeded: True";
                yield return $"Invitation link:https://prod.liveshare.vsengsaas.VisualStudio.com/join?{GeneratedServerWorkspaceId}";
            }
        }
    }
}
