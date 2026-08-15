using System;

namespace Devlooped;

/// <summary>
/// Helpers for the shared <c>--version</c>/<c>-v</c> Visual Studio version filter.
/// </summary>
static class VisualStudioVersion
{
    public static string GetMajor(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return null;

        var dot = version.IndexOf('.');
        return dot < 0 ? version : version[..dot];
    }

    public static bool Matches(string productSemanticVersion, string requested)
    {
        if (string.IsNullOrEmpty(requested))
            return true;

        return !string.IsNullOrEmpty(productSemanticVersion) &&
            productSemanticVersion.StartsWith(requested, StringComparison.OrdinalIgnoreCase);
    }
}
