using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace VisualStudio
{
    static class ProcessExtensions
    {
        public static void Log(this ProcessStartInfo info, TextWriter output)
        {
            output.Write(Quote(info.FileName));

            foreach (var arg in info.ArgumentList.Where(arg => arg != null))
            {
                output.Write(' ');
                output.Write(Quote(arg));
            }

            output.WriteLine();
        }

        static string Quote(string value) => value.Contains(' ') ? "\"" + value + "\"" : value;

        public static string GetCommandLine(this Process process)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                using (var objects = searcher.Get())
                    return objects.OfType<ManagementBaseObject>().FirstOrDefault()?["CommandLine"]?.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
