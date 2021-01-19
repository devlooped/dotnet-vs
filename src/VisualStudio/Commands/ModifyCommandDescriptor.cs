using System.Collections.Immutable;

namespace Devlooped
{
    class ModifyCommandDescriptor : CommandDescriptor
    {
        readonly WorkloadOptions addWorkloads = new WorkloadOptions("add", "+");
        readonly WorkloadOptions removeWorkloads = new WorkloadOptions("remove", "-");

        public ModifyCommandDescriptor()
        {
            Description = "Modifies an installation of Visual Studio.";
            Options = VisualStudioOptions.Default("modify")
                .WithFirst()
                .With(addWorkloads)
                .With(removeWorkloads);
        }

        public ImmutableArray<string> WorkloadsAdded => addWorkloads.Value;

        public ImmutableArray<string> WorkloadsRemoved => removeWorkloads.Value;
    }
}
