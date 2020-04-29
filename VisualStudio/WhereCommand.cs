using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using vswhere;
using System.Collections.Generic;

namespace VisualStudio
{
    class WhereCommand : Command<WhereCommandDescriptor>
    {
        readonly WhereService whereService;

        public WhereCommand(WhereCommandDescriptor descriptor, WhereService whereService) : base(descriptor)
        {
            this.whereService = whereService;
        }

        public IEnumerable<VisualStudioInstance> Instances { get; private set; } = Enumerable.Empty<VisualStudioInstance>();

        public override async Task ExecuteAsync(TextWriter output)
        {
            var instances = await whereService.GetAllInstancesAsync(
                await Descriptor.GetPredicateAsync(),
                Descriptor.WorkloadsArguments.Concat(Descriptor.ExtraArguments));

            if (!Descriptor.ShowAll)
                instances = new Chooser("show").ChooseMany(instances, output);

            foreach (var instance in instances)
                ShowProperties(instance, output);
        }

        void ShowProperties(VisualStudioInstance instance, TextWriter output)
        {
            output.WriteLine();
            output.WriteLine($"{ instance.DisplayName} - Version { instance.Catalog.ProductDisplayVersion}");

            var props = GetProperties(instance, "Catalog", "Properties").ToList();
            props.AddRange(GetProperties(instance.Catalog).Select(x => ($"Catalog.{x.PropertyName}", x.PropertyValue)));
            props.AddRange(GetProperties(instance.Properties).Select(x => ($"Properties.{x.PropertyName}", x.PropertyValue)));

            foreach (var prop in props)
                output.WriteLine($"{prop.PropertyName}: {prop.PropertyValue}");
        }

        IEnumerable<(string PropertyName, object PropertyValue)> GetProperties<T>(T instance, params string[] skipProps) =>
            instance.GetType().GetProperties()
                .Where(x => skipProps == null || !skipProps.Contains(x.Name))
                .Select(x => (x.Name, x.GetValue(instance)));
    }
}
