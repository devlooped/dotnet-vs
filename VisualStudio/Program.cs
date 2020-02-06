using System;
using System.Linq;
using System.Threading.Tasks;

namespace VisualStudio
{
    class Program
    {
        static ListCommand list = new ListCommand();

        static async Task<int> Main(string[] args)
        {
            if (args.Length == 0 || !"list".Equals(args[0], StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Usage: visualstudio [command] [options]");
                Console.WriteLine("list [options]");
                list.WriteHelp(Console.Out);
                Console.ReadLine();
                return -1;
            }

            var exitCode = await list.ExecuteAsync(args.Skip(1), Console.Out);
            Console.ReadLine();
            return exitCode;
        }
    }
}
