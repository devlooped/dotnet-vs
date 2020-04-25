using System;
using System.IO;
using System.Threading.Tasks;

namespace VisualStudio
{
    abstract class Command
    {
        public abstract Task ExecuteAsync(TextWriter output);
    }
}
