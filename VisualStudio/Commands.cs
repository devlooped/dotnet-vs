using System;
using Microsoft.DotNet;

namespace VisualStudio
{
    class Commands
    {
        public const string Run = "run";
        public const string Where = "where";
        public const string Install = "install";
        public const string Modify = "modify";
        public const string Update = "update";
        public const string Kill = "kill";
        public const string Config = "config";
        public const string Log = "log";

        public class System
        {
            public const string GenerateReadme = "generate-readme";
            public const string Save = "save";
            public const string UpdateSelf = "update-self";
        }

        public class DotNetConfig
        {
            public const string Section = ThisAssembly.Metadata.AssemblyName;
            public const string SubSection = "alias";

            public static Config GetConfig(bool global = false) =>
                global ? Microsoft.DotNet.Config.Read(ConfigLevel.Global) : Microsoft.DotNet.Config.Build();
        }
    }
}
