using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace VisualStudio.Tests
{
    public class ProgramTests
    {
        readonly TextWriter output;

        public ProgramTests(ITestOutputHelper output) => 
            this.output = new OutputHelperTextWriter(output);

        [Fact]
        public async Task when_no_command_specified_run_is_default()
        {
            var program = new Program(output, false, "pre");

            var exit = await program.RunAsync();

            Assert.Equal(0, exit);

            Assert.IsType<RunCommand>(program.Command);
        }
    }
}
