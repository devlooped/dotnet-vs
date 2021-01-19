using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Management;

namespace Devlooped
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

        [SuppressMessage("Design", "CA1416", Justification = "Checked at run-time.")]
        public static string GetCommandLine(this Process process)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new PlatformNotSupportedException("Can only run on Windows.");

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
