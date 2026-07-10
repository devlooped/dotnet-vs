using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vswhere;

namespace Devlooped;

class WhereCommand : Command
{
    readonly WhereService whereService;
    readonly SharedOptions.ChannelOptions channelOptions;
    readonly Option<string> skuOption;
    readonly Option<string> filterOption;
    readonly Option<bool> firstOption;
    readonly Option<string> propOption = new("--prop", "--property")
    {
        Description = "The name of a property to return",
    };
    readonly Option<bool> listOption = new("--list")
    {
        Description = "Shows result as a list",
    };
    readonly Option<string[]> requiresOption = new("--requires")
    {
        Description = "A workload ID",
    };

    public WhereCommand(WhereService whereService)
        : base(Commands.Where, "Locates the installed version(s) of Visual Studio that satisfy the requested requirements, optionally retrieving installation properties from it.")
    {
        this.whereService = whereService;

        channelOptions = SharedOptions.AddChannelOptions(this, "show");
        skuOption = SharedOptions.SkuOption();
        filterOption = SharedOptions.FilterOption();
        firstOption = SharedOptions.FirstOption("show");

        Options.Add(skuOption);
        Options.Add(filterOption);
        Options.Add(firstOption);
        Options.Add(propOption);
        Options.Add(listOption);
        Options.Add(requiresOption);

        TreatUnmatchedTokensAsErrors = false;

        SetAction(async (parseResult, _) =>
        {
            await ExecuteAsync(parseResult, parseResult.InvocationConfiguration.Output);
            return 0;
        });
    }

    async Task ExecuteAsync(ParseResult parse, TextWriter output)
    {
        var filter = CommandHelpers.GetFilter(parse, channelOptions, skuOption, filterOption, firstOption);
        var workloads = CommandHelpers.GetWorkloadIds(parse, requiresOption);
        // vswhere only accepts single-dash switches (e.g. -requires), not --requires.
        var extra = CommandHelpers.ToWorkloadArgs("requires", workloads, "-").Concat(parse.UnmatchedTokens);

        var property = parse.GetValue(propOption);
        var showList = parse.GetValue(listOption);

        var instances = (await whereService.GetAllInstancesAsync(filter, extra)).ToList();

        foreach (var instance in instances)
        {
            var properties = GetProperties(instance);

            if (string.IsNullOrEmpty(property))
            {
                output.WriteLine($"{instance.DisplayName} - Version {instance.Catalog.ProductDisplayVersion}");

                if (!showList)
                {
                    foreach (var prop in properties)
                        output.WriteLine($"{prop.PropertyName}: {prop.PropertyValue}");

                    output.WriteLine();
                }
            }
            else
            {
                Console.WriteLine(
                    properties
                        .Where(x => x.PropertyName == property)
                        .Select(x => x.PropertyValue)
                        .FirstOrDefault() ?? string.Empty);
            }
        }
    }

    static IEnumerable<(string PropertyName, object PropertyValue)> GetProperties(VisualStudioInstance instance)
    {
        var props = GetProperties(instance, "Catalog", "Properties").ToList();
        props.AddRange(GetProperties(instance.Catalog).Select(x => ($"Catalog.{x.PropertyName}", x.PropertyValue)));
        props.AddRange(GetProperties(instance.Properties).Select(x => ($"Properties.{x.PropertyName}", x.PropertyValue)));
        return props;
    }

    static IEnumerable<(string PropertyName, object PropertyValue)> GetProperties<T>(T instance, params string[] skipProps) =>
        instance.GetType().GetProperties()
            .Where(x => skipProps == null || !skipProps.Contains(x.Name))
            .Select(x => (x.Name, x.GetValue(instance)));
}
