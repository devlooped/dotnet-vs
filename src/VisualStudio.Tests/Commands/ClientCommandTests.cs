using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using vswhere;
using Xunit;
using Xunit.Abstractions;
using DevEnv = vswhere.VisualStudioInstance;

namespace VisualStudio.Tests
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
            var command = new ClientCommandTest(default, isExperimental, solutionPath);

            await command.ExecuteAsync(new DevEnv(), output);

            Assert.NotNull(command.Server);
            Assert.Contains("/server", command.Server.StartInfo.ArgumentList);

            if (isExperimental)
            {
                Assert.Contains("/rootSuffix", command.Server.StartInfo.ArgumentList);
                Assert.Contains("Exp", command.Server.StartInfo.ArgumentList);
            }

            if (!string.IsNullOrEmpty(solutionPath))
            {
                Assert.Contains(solutionPath, command.Server.StartInfo.ArgumentList);
            }

            Assert.NotNull(command.Client);
            Assert.Contains("/client", command.Client.StartInfo.ArgumentList);
            Assert.Contains("/joinworkspace", command.Client.StartInfo.ArgumentList);
            Assert.Contains($"vsls:?workspaceId={command.GeneratedServerWorkspaceId}&remoteJoin=true", command.Client.StartInfo.ArgumentList);

            if (isExperimental)
            {
                Assert.Contains("/rootSuffix", command.Client.StartInfo.ArgumentList);
                Assert.Contains("Exp", command.Client.StartInfo.ArgumentList);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task when_workspace_id_is_specified_then_client_is_started(bool isExperimental)
        {
            var command = new ClientCommandTest("123", isExperimental);

            await command.ExecuteAsync(new DevEnv(), output);

            Assert.Null(command.Server);

            Assert.NotNull(command.Client);
            Assert.Contains("/client", command.Client.StartInfo.ArgumentList);
            Assert.Contains("/joinworkspace", command.Client.StartInfo.ArgumentList);
            Assert.Contains($"vsls:?workspaceId=123&remoteJoin=true", command.Client.StartInfo.ArgumentList);

            if (isExperimental)
            {
                Assert.Contains("/rootSuffix", command.Client.StartInfo.ArgumentList);
                Assert.Contains("Exp", command.Client.StartInfo.ArgumentList);
            }
        }

        [Fact]
        public async Task when_starting_server_and_client_then_arguments_are_defined_in_correct_order()
        {
            var command = new ClientCommandTest(default, true, @"c:\src\foo.sln");

            await command.ExecuteAsync(new DevEnv(), output);

            Assert.NotNull(command.Server);
            Assert.Equal(
                @"c:\src\foo.sln /rootSuffix Exp /server",
                string.Join(" ", command.Server.StartInfo.ArgumentList));

            Assert.NotNull(command.Client);
            Assert.Equal(
                $"/rootSuffix Exp /client /joinworkspace vsls:?workspaceId={command.GeneratedServerWorkspaceId}&remoteJoin=true",
                string.Join(" ", command.Client.StartInfo.ArgumentList));
        }

        class ClientCommandTest : ClientCommand
        {
            public ClientCommandTest(string workspaceId, bool isExperimental = false, string solutionPath = null)
                : base(new ClientCommandDescriptorTest(workspaceId, isExperimental, solutionPath), default)
            { }

            public string GeneratedServerWorkspaceId { get; } = Guid.NewGuid().ToString();

            protected override Process CreateProcess(DevEnv devenv, IEnumerable<string> args, bool start = true) =>
                base.CreateProcess(devenv, args, false);

            protected override IEnumerable<string> ReadOutputLines(Process process)
            {
                if (process == Server)
                {
                    yield return "Start Live Share Session command enabled: True";
                    yield return "Start Live Share Session command succeeded: True";
                    yield return $"Invitation link:https://prod.liveshare.vsengsaas.visualstudio.com/join?{GeneratedServerWorkspaceId}";
                }
            }

            class ClientCommandDescriptorTest : ClientCommandDescriptor
            {
                readonly string workspaceId;
                readonly bool isExperimental;

                public ClientCommandDescriptorTest(string workspaceId, bool isExperimental, string solutionPath)
                {
                    this.workspaceId = workspaceId;
                    this.isExperimental = isExperimental;

                    if (!string.IsNullOrEmpty(solutionPath))
                        ExtraArguments = ImmutableArray.Create(solutionPath);
                }

                public override bool IsExperimental => isExperimental;

                public override string WorkspaceId => workspaceId;
            }
        }
    }
}
