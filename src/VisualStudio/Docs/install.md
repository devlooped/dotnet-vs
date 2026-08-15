## install

{Description}

```
{Usage}
```

{Options}

You can add specific workload IDs by using the supported [workload switches](#workload-id-switches) 
using the `+` prefix.

See the [documentation for the Visual Studio installer command line options](https://learn.microsoft.com/en-us/visualstudio/install/use-command-line-parameters-to-install-visual-studio) 
for the full list of arguments that can be provided.

Common options are `--passive`, `quiet` and `--wait`, for example.

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Installs VS enterprise with the Maui/Mobile workload
# Note the -sku: switch/prefix is optional
> dnx vs -- install Enterprise +mobile

# Install VS community with the .NET Core, ASP.NET and Azure workloads, 
# shows installation progress and waits for it to finish before returning
> dnx vs -- install +core +web +azure

# Install VS 18 Enterprise
> dnx vs -- install -v:18 -sku:ent

# Install the latest VS 17 (2022) Community
> dnx vs -- install --version 17
```
<!-- EXAMPLES_END -->
