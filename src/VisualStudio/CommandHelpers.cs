using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace Devlooped;

static class CommandHelpers
{
    public static VisualStudioFilter GetFilter(
        ParseResult parse,
        SharedOptions.ChannelOptions channelOptions,
        Option<string> skuOption,
        Option<string> filterOption = null,
        Option<bool> firstOption = null,
        Option<bool> allOption = null)
    {
        return new VisualStudioFilter(
            Channel: channelOptions.GetChannel(parse),
            Sku: SharedOptions.ParseSku(parse.GetValue(skuOption)),
            Expression: filterOption != null ? parse.GetValue(filterOption) : null,
            First: firstOption != null && parse.GetValue(firstOption),
            All: allOption != null && parse.GetValue(allOption));
    }

    public static string[] GetWorkloadIds(ParseResult parse, Option<string[]> option)
    {
        var values = parse.GetValue(option);
        return values ?? Array.Empty<string>();
    }

    /// <param name="argumentPrefix">
    /// Switch prefix for the underlying tool. Use <c>-</c> for vswhere (e.g. <c>-requires</c>)
    /// and <c>--</c> for the Visual Studio installer (e.g. <c>--add</c>).
    /// </param>
    public static IEnumerable<string> ToWorkloadArgs(string argumentName, IEnumerable<string> ids, string argumentPrefix = "--") =>
        ids.Where(id => !string.IsNullOrEmpty(id))
            .SelectMany(id => new[] { argumentPrefix + argumentName, id });
}
