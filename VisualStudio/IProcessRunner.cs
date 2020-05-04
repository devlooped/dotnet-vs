using System;
using System.Threading.Tasks;

namespace VisualStudio
{
    interface IProcessRunner
    {
        Task<string> StartAsync(string fileName, params string[] args);
    }
}
