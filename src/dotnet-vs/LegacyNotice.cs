using System;
using System.IO;

namespace Devlooped;

partial class Program
{
    partial void WriteLegacyMigrationNotice(TextWriter output)
    {
        var useColor = !Console.IsOutputRedirected && ReferenceEquals(output, Console.Out);

        output.WriteLine();

        if (useColor)
            Console.ForegroundColor = ConsoleColor.Yellow;

        output.WriteLine("⚠  Package 'dotnet-vs' is obsolete.");

        if (useColor)
            Console.ForegroundColor = ConsoleColor.Cyan;

        output.WriteLine("   Use:  dnx vs -- [command] [options]");

        if (useColor)
            Console.ResetColor();

        output.WriteLine();
    }
}
