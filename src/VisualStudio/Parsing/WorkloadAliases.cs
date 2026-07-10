using System.Collections.Generic;

namespace Devlooped;

static class WorkloadAliases
{
    public static readonly IReadOnlyDictionary<string, string> Map = new Dictionary<string, string>
    {
        { "mobile", "Microsoft.VisualStudio.Workload.NetCrossPlat" },
        { "xamarin", "Microsoft.VisualStudio.Workload.NetCrossPlat" },
        { "maui", "Microsoft.VisualStudio.Workload.NetCrossPlat" },
        { "core", "Microsoft.VisualStudio.Workload.NetCoreTools" },
        { "azure", "Microsoft.VisualStudio.Workload.Azure" },
        { "data", "Microsoft.VisualStudio.Workload.Data" },
        { "desktop", "Microsoft.VisualStudio.Workload.ManagedDesktop" },
        { "unity", "Microsoft.VisualStudio.Workload.ManagedGame" },
        { "native", "Microsoft.VisualStudio.Workload.NativeDesktop" },
        { "vc", "Microsoft.VisualStudio.Workload.VCTools" },
        { "web", "Microsoft.VisualStudio.Workload.NetWeb" },
        { "node", "Microsoft.VisualStudio.Workload.Node" },
        { "office", "Microsoft.VisualStudio.Workload.Office" },
        { "python", "Microsoft.VisualStudio.Workload.Python" },
        { "uwp", "Microsoft.VisualStudio.Workload.Universal" },
        { "vsx", "Microsoft.VisualStudio.Workload.VisualStudioExtension" },
    };

    public static string Resolve(string value) =>
        Map.TryGetValue(value, out var id) ? id : value;
}
