<h1 id="dotnet-vs"><img src="https://raw.githubusercontent.com/kzu/dotnet-vs/main/docs/img/icon.svg" alt="icon" height="32" width="32" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">  dotnet-vs</h1>

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


## install

Installs a specific edition of Visual Studio

```
Usage: vs install [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | install preview version |
| `int\|internal` | install internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `add:` | A workload ID |


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

## run

This is default command, so typically it does not need to be provided as an argument.

```
Usage: vs run [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | run preview version |
| `int\|internal` | run internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | run experimental instance instead of regular. |
| `id:` | Run a specific instance by its ID |
| `f\|first` | If more than one instance matches the criteria, run the first one sorted by descending build version. |
| `v\|version:` | Run specific (semantic) version, such as 16.4 or 16.5.3. |
| `w\|wait` | Wait for the started Visual Studio to exit. |
| `nr\|nodereuse` | Disable MSBuild node reuse. Useful when testing analyzers, tasks and targets. Defaults to true when running experimental instance. |
| `default` | Set as the default version to run when no arguments are provided, or remove the current default (with --default-). |
| `requires:` | A workload ID |


All [workload switches](#workload-id-switches) are available too to filter the instance to run.

This command will remember the last VS that was located and run. So the next time you 
can just run the same instance by simply using `vs` (since `run` is the default command 
and can be omitted).

Examples:

```
// Runs the first VS enterprise with the Xamarin/Mobile workload
vs -sku:ent -first +mobile

// Runs VS 16.4
vs -v:16.4

// Runs VS 16.5 preview
vs -v:16.4 -pre

// Runs the last VS that was run
vs
```

## where

Locates the installed version(s) of Visual Studio that satisfy the requested requirements, optionally retrieving installation properties from it.

```
Usage: vs where [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | show preview version |
| `int\|internal` | show internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | show first matching instance. |
| `all` | show all instances. |
| `prop\|property:` | The name of a property to return |
| `requires:` | A workload ID |


Supports same options as `vswhere.exe`:

```
  -all           Finds all instances even if they are incomplete and may not launch.
  -prerelease    Also searches prereleases. By default, only releases are searched.
  -products arg  One or more product IDs to find. Defaults to Community, Professional, and Enterprise.
                 Specify "*" by itself to search all product instances installed.
                 See https://aka.ms/vs/workloads for a list of product IDs.
  -requires arg  One or more workload or component IDs required when finding instances.
                 All specified IDs must be installed unless -requiresAny is specified.
                 See https://aka.ms/vs/workloads for a list of workload and component IDs.
  -requiresAny   Find instances with any one or more workload or components IDs passed to -requires.
  -version arg   A version range for instances to find. Example: [15.0,16.0) will find versions 15.*.
  -latest        Return only the newest version and last installed.
  -sort          Sorts the instances from newest version and last installed to oldest.
                 When used with "find", first instances are sorted then files are sorted lexigraphically.
  -legacy        Also searches Visual Studio 2015 and older products. Information is limited.
                 This option cannot be used with either -products or -requires.
  -format arg    Return information about instances found in a format described below.
  -property arg  The name of a property to return. Defaults to "value" format.
                 Use delimiters ".", "/", or "_" to separate object and property names.
                 Example: "properties.nickname" will return the "nickname" property under "properties".
  -include arg   One or more extra properties to include, as described below.
  -find arg      Returns matching file paths under the installation path. Defaults to "value" format.
                 The following patterns are supported:
                 ?  Matches any one character except "\".
                 *  Matches zero or more characters except "\".
                 ** Searches the current directory and subdirectories for the remaining search pattern.
  -nologo        Do not show logo information. Some formats noted below will not show a logo anyway.
  -utf8          Use UTF-8 encoding (recommended for JSON).
  -?, -h, -help  Display this help message.


Extra properties:
  packages       Return an array of packages installed in this instance.
                 Supported only by the "json" and "xml" formats.

Formats:
  json           An array of JSON objects for each instance (no logo).
  text           Colon-delimited properties in separate blocks for each instance (default).
  value          A single property specified by the -property parameter (no logo).
  xml            An XML data set containing instances (no logo).
```

A shorthand notation is available for `-requires [workload ID]` via the supported 
workload ID switches (see below).

See also [vswhere on GitHub](https://github.com/microsoft/vswhere).

## update

Updates an installation of Visual Studio

```
Usage: vs update [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | Update preview version |
| `int\|internal` | Update internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | Update first matching instance. |


Examples:

```
```

## modify

Modifies an installation of Visual Studio

```
Usage: vs modify [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | modify preview version |
| `int\|internal` | modify internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | Modify first matching instance. |
| `add:` | A workload ID |
| `remove:` | A workload ID |


Examples:

```
```

## config

Opens the config folder

```
Usage: vs config [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | open preview version |
| `int\|internal` | open internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | Use first matching VS instance. |
| `exp\|experimental` | open experimental instance instead of regular. |


Examples:

```
```

## log

Opens the folder containing the Activity.log file

```
Usage: vs log [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | open preview version |
| `int\|internal` | open internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | Use first matching VS instance. |
| `exp\|experimental` | open experimental instance instead of regular. |


Examples:

```
```

## kill

Kills running devenv processes

```
Usage: vs kill [options]
```

|Option|Description|
|-|-|
| `pre\|preview` | kill preview version |
| `int\|internal` | kill internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community` `b\|build\|buildtools` or `t\|test\|testagent`  |
| `filter:` | An expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | kill experimental instance instead of regular. |
| `first` | kill first matching instance. |
| `all` | kill all instances. |


Examples:

```
```


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


## Sponsors

<h3 style="vertical-align: text-top" id="by-clarius">
<img src="https://raw.githubusercontent.com/devlooped/oss/main/assets/images/sponsors.svg" alt="sponsors" height="36" width="36" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">&nbsp;&nbsp;by&nbsp;<a href="https://github.com/clarius">@clarius</a>&nbsp;<img src="https://raw.githubusercontent.com/clarius/branding/main/logo/logo.svg" alt="sponsors" height="36" width="36" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">
</h3>

*[get mentioned here too](https://github.com/sponsors/devlooped)*
