﻿using System;
using System.Collections.Generic;
using System.IO;

namespace VisualStudio
{
    class WhereCommandDescriptor : CommandDescriptor
    {
        readonly VisualStudioOptions vsOptions = VisualStudioOptions.Default("show").WithFirst();
        readonly SelectPropertyOption propOption = new SelectPropertyOption();
        readonly ListOption listOption = new ListOption();
        readonly WorkloadOptions workloads = new WorkloadOptions("requires", "--", "-");

        readonly WhereService whereService;

        public WhereCommandDescriptor(WhereService whereService)
        {
            Description = "Locates the installed version(s) of Visual Studio that satisfy the requested requirements.";

            Options = vsOptions
                .With(propOption)
                .With(listOption)
                .With(workloads);

            this.whereService = whereService;
        }

        public string Property => propOption.Value;

        public bool ShowList => listOption.Value;

        public IEnumerable<string> WorkloadsArguments => workloads.Value;
    }
}
