![Icon](https://raw.githubusercontent.com/devlooped/dotnet-vs/main/docs/img/icon-32.png) dnx vs
============

[![Version](https://img.shields.io/nuget/v/vs.svg?color=royalblue)](https://www.nuget.org/packages/vs)
[![Downloads](https://img.shields.io/nuget/dt/vs.svg?color=darkmagenta)](https://www.nuget.org/packages/vs)
[![License](https://img.shields.io/github/license/devlooped/dotnet-vs.svg?color=blue)](https://github.com/devlooped/dotnet-vs/blob/master/license.txt)
[![CI Status](https://github.com/devlooped/dotnet-vs/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/dotnet-vs/actions?query=branch%3Amain+workflow%3Abuild+)
[![CI Version](https://img.shields.io/endpoint?label=nuget.ci&color=brightgreen&url=https://shields.kzu.app/vpre/vs/main)](https://pkg.kzu.app/index.json)

Run with:

```
dnx vs -- [command] [options]
```

To get the CI version:

```
dnx vs --prerelease --source https://pkg.kzu.app/index.json -- [command] [options]
```

<!-- #content -->
A global tool for running, managing and querying Visual Studio installations

Command line parsing is done with [System.CommandLine](https://www.nuget.org/packages/System.CommandLine),
with a compatibility layer that still accepts common legacy forms such as `-flag`, `--flag`, `/flag`,
`-flag=value`, `--flag=value`, `-flag:value`, `--flag:value`, and bare channel/SKU/workload shortcuts.

Supported commands:

{Commands}

## Workload ID switches

For commands that receive workload ID switches (i.e. `dnx vs -- where -requires [WORKLOAD_ID]` or 
`dnx vs -- install --add [WORKLOAD_ID]`), the following aliases are available:

|  Alias    | Workload ID |
|-----------|----------------------------|
| `mobile`  | Microsoft.VisualStudio.Workload.NetCrossPlat |
| `xamarin` | Microsoft.VisualStudio.Workload.NetCrossPlat |
| `maui`    | Microsoft.VisualStudio.Workload.NetCrossPlat |
| `core`    | Microsoft.NetCore.Component.DevelopmentTools |
| `azure`   | Microsoft.VisualStudio.Workload.Azure |
| `data`    | Microsoft.VisualStudio.Workload.Data |
| `desktop` | Microsoft.VisualStudio.Workload.ManagedDesktop |
| `unity`   | Microsoft.VisualStudio.Workload.ManagedGame |
| `native`  | Microsoft.VisualStudio.Workload.NativeDesktop |
| `web`     | Microsoft.VisualStudio.Workload.NetWeb |
| `node`    | Microsoft.VisualStudio.Workload.Node |
| `office`  | Microsoft.VisualStudio.Workload.Office |
| `py`      | Microsoft.VisualStudio.Workload.Python |
| `python`  | Microsoft.VisualStudio.Workload.Python |
| `uwp`     | Microsoft.VisualStudio.Workload.Universal |
| `vsx`     | Microsoft.VisualStudio.Workload.VisualStudioExtension |

The aliases are converted to the appropriate switch automatically, such as into 
`-requires [ID]` or `--add [ID]`. Additionally, depending on the command being run, 
the aliases might use a `+` prefix (like `+mobile`), which might make for a more 
intuitive command line, such as `dnx vs -- install +mobile -sku:enterprise` or `dnx vs -- +mobile` 
(runs the VS with the mobile workload installed). The *modify* command uses `+` and `-` 
prefix to add or remove workloads respectively, for example.

<!-- #content -->
---
<!-- include https://github.com/devlooped/.github/raw/main/osmf.md -->
## Open Source Maintenance Fee

To ensure the long-term sustainability of this project, users of this package who generate 
revenue must pay an [Open Source Maintenance Fee](https://opensourcemaintenancefee.org). 
While the source code is freely available under the terms of the [License](license.txt), 
this package and other aspects of the project require [adherence to the Maintenance Fee](osmfeula.txt).

To pay the Maintenance Fee, [become a Sponsor](https://github.com/sponsors/devlooped) at the proper 
OSMF tier. A single fee covers all of [Devlooped packages](https://www.nuget.org/profiles/Devlooped).

<!-- https://github.com/devlooped/.github/raw/main/osmf.md -->
---
<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

*[get mentioned here too](https://github.com/sponsors/devlooped)!*
