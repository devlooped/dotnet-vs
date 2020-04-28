using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace VisualStudio.Tests
{
    internal class OutputHelperTextWriter : TextWriter
    {
        readonly ITestOutputHelper output;

        public OutputHelperTextWriter(ITestOutputHelper output) => this.output = output;

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(string value) => WriteLine(value);

        public override void WriteLine(string message) => output.WriteLine(message);

        public override void WriteLine(string format, params object[] args) => output.WriteLine(format, args);
    }
}
