## install

{Description}

```
{Usage}
```

{Options}

You can add specific workload IDs by using the supported [workload switches](#workload-id-switches).

See the [documentation for the Visual Studio installer command line options](https://docs.microsoft.com/en-us/visualstudio/install/use-command-line-parameters-to-install-visual-studio?view=vs-2019#install-options) 
for the full list of arguments that can be provided.

Common options are `--passive`, `quiet` and `--wait`, for example.

Examples:

```
// Installs VS enterprise with the Xamarin/Mobile workload
vs install -sku:enterprise +mobile

// Install VS community with the .NET Core, ASP.NET and Azure workloads, 
// shows installation progress and waits for it to finish before returning
vs install +core +web +azure
```
