﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.DotNet;

namespace VisualStudio
{
    class SaveCommand : Command<SaveCommandDescriptor>
    {
        public SaveCommand(SaveCommandDescriptor descriptor) : base(descriptor)
        {
        }

        public override Task ExecuteAsync(TextWriter output)
        {
            output.WriteLine($"Saving '{string.Join(" ", Descriptor.ExtraArguments)}' as '{Descriptor.Alias}'...");

            Commands.DotNetConfig
                .GetConfig(Descriptor.Global)
                .Set(Commands.DotNetConfig.Section, Commands.DotNetConfig.SubSection, Descriptor.Alias, string.Join('|', Descriptor.ExtraArguments));

            return Task.CompletedTask;
        }
    }
}
