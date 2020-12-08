<img src="https://simpleicons.org/icons/visualstudio.svg" alt="drawing" width="32"/> dotnet-vs
============

A global tool for running, managing and querying Visual Studio installations

[![Build Status](https://dev.azure.com/kzu/oss/_apis/build/status/dotnet-vs?branchName=main)](https://dev.azure.com/kzu/oss/_build/latest?definitionId=32&branchName=main)
[![Version](https://img.shields.io/nuget/v/dotnet-vs.svg?color=royalblue)](https://www.nuget.org/packages/dotnet-vs)
[![Downloads](https://img.shields.io/nuget/dt/dotnet-vs.svg?color=darkmagenta)](https://www.nuget.org/packages/dotnet-vs)

Installing or updating (same command for both):

```
dotnet tool update -g dotnet-vs
```

To get the CI version:

```
dotnet tool update -g dotnet-vs --no-cache --add-source https://pkg.kzu.io/index.json
```

Command line parsing is done with [Mono.Options](https://www.nuget.org/packages/mono.options) so 
all the following variants for arguments are supported: `-flag`, `--flag`, `/flag`, `-flag=value`, `--flag=value`, 
`/flag=value`, `-flag:value`, `--flag:value`, `/flag:value`, `-flag value`, `--flag value`, `/flag value`.

Supported commands:

{Commands}

## Workload ID switches

For commands that receive workload ID switches (i.e. `vs where -requires [WORKLOAD_ID]` or 
`vs install --add [WORKLOAD_ID]`), the following aliases are available:

|  Switch     | Workload ID |
|-------------|----------------------------|
| `--mobile`  | Microsoft.VisualStudio.Workload.NetCrossPlat |
| `--core`    | Microsoft.VisualStudio.Workload.NetCoreTools |
| `--azure`   | Microsoft.VisualStudio.Workload.Azure |
| `--data`    | Microsoft.VisualStudio.Workload.Data |
| `--desktop` | Microsoft.VisualStudio.Workload.ManagedDesktop |
| `--unity`   | Microsoft.VisualStudio.Workload.ManagedGame |
| `--native`  | Microsoft.VisualStudio.Workload.NativeDesktop |
| `--xamarin` | Microsoft.VisualStudio.Workload.NetCrossPlat |
| `--web`     | Microsoft.VisualStudio.Workload.NetWeb |
| `--node`    | Microsoft.VisualStudio.Workload.Node |
| `--office`  | Microsoft.VisualStudio.Workload.Office |
| `--py`      | Microsoft.VisualStudio.Workload.Python |
| `--python`  | Microsoft.VisualStudio.Workload.Python |
| `--uwp`     | Microsoft.VisualStudio.Workload.Universal |
| `--vsx`     | Microsoft.VisualStudio.Workload.VisualStudioExtension |

The switches are converted to the appropriate argument automatically, such as into 
`-requires [ID]` or `--add [ID]`. Additionally, the switches can also be specified 
with a `+` (like `+mobile`), which might make for a more intuitive command line, 
such as `vs install +mobile -sku:enterprise` or `vs +mobile` (runs the VS with the 
mobile workload installed).
