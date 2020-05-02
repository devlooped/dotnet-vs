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

        [Fact]
        public async Task when_save_option_is_specified_then_save_command_is_executed()
        {
            var program = new Program(output, false, "run", "Enteprise", "--save=foo");

            var exit = await program.RunAsync();

            Assert.Equal(0, exit);

            Assert.IsType<SaveCommand>(program.Command);
        }
    }
}
