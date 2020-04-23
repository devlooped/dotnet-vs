using System.Diagnostics;
using System.IO;

namespace VisualStudio
{
    static class ProcessExtensions
    {
        public static void Log(this ProcessStartInfo info, TextWriter output)
        {
            output.Write(Quote(info.FileName));

            foreach (var arg in info.ArgumentList)
            {
                output.Write(' ');
                output.Write(Quote(arg));
            }

            output.WriteLine();
        }

        static string Quote(string value) => value.Contains(' ') ? "\"" + value + "\"" : value;
    }
}
