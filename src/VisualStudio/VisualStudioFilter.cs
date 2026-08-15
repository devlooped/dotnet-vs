namespace Devlooped;

/// <summary>
/// Parsed Visual Studio selection criteria used by <see cref="WhereService"/> and predicates.
/// </summary>
record VisualStudioFilter(
    Channel? Channel = null,
    Sku? Sku = null,
    string Expression = null,
    bool First = false,
    bool All = false,
    string Version = null);
