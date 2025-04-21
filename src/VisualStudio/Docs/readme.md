<h1 id="dotnet-vs"><img src="https://raw.githubusercontent.com/devlooped/dotnet-vs/main/docs/img/icon.svg" alt="icon" height="32" width="32" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">  dotnet-vs</h1>

A global tool for running, managing and querying Visual Studio installations

[![Version](https://img.shields.io/nuget/v/dotnet-vs.svg?color=royalblue)](https://www.nuget.org/packages/dotnet-vs)
[![Downloads](https://img.shields.io/nuget/dt/dotnet-vs.svg?color=darkmagenta)](https://www.nuget.org/packages/dotnet-vs)
[![License](https://img.shields.io/github/license/devlooped/dotnet-vs.svg?color=blue)](https://github.com/devlooped/dotnet-vs/blob/master/license.txt)
[![CI Status](https://github.com/devlooped/dotnet-vs/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/dotnet-vs/actions?query=branch%3Amain+workflow%3Abuild+)
[![CI Version](https://img.shields.io/endpoint?label=nuget.ci&color=brightgreen&url=https://shields.kzu.app/vpre/dotnet-vs/main)](https://pkg.kzu.app/index.json)

Installing or updating (same command for both):

```
dotnet tool update -g dotnet-vs
```

To get the CI version:

```
dotnet tool update -g dotnet-vs --no-cache --add-source https://pkg.kzu.app/index.json
```

Command line parsing is done with [Mono.Options](https://www.nuget.org/packages/mono.options) so 
all the following variants for arguments are supported: `-flag`, `--flag`, `/flag`, `-flag=value`, `--flag=value`, 
`/flag=value`, `-flag:value`, `--flag:value`, `/flag:value`, `-flag value`, `--flag value`, `/flag value`.

Supported commands:

{Commands}

## Workload ID switches

For commands that receive workload ID switches (i.e. `vs where -requires [WORKLOAD_ID]` or 
`vs install --add [WORKLOAD_ID]`), the following aliases are available:

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
intuitive command line, such as `vs install +mobile -sku:enterprise` or `vs +mobile` 
(runs the VS with the mobile workload installed). The *modify* command uses `+` and `-` 
prefix to add or remove workloads respectively, for example.


## Sponsors

<h3 style="vertical-align: text-top" id="by-clarius">
<img src="https://raw.githubusercontent.com/devlooped/oss/main/assets/images/sponsors.svg" alt="sponsors" height="36" width="36" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">&nbsp;&nbsp;by&nbsp;<a href="https://github.com/clarius">@clarius</a>&nbsp;<img src="https://raw.githubusercontent.com/clarius/branding/main/logo/logo.svg" alt="sponsors" height="36" width="36" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">
</h3>

*[get mentioned here too](https://github.com/sponsors/devlooped)!*
