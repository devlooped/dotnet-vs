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

    public static IEnumerable<string> ToWorkloadArgs(string argumentName, IEnumerable<string> ids) =>
        ids.SelectMany(id => new[] { "--" + argumentName, id });
}
