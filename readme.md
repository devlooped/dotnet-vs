![Icon](https://raw.githubusercontent.com/devlooped/dotnet-vs/main/docs/img/icon-32.png) dotnet-vs
============

A global tool for running, managing and querying Visual Studio installations

[![Version](https://img.shields.io/nuget/v/dotnet-vs.svg?color=royalblue)](https://www.nuget.org/packages/dotnet-vs)
[![Downloads](https://img.shields.io/nuget/dt/dotnet-vs.svg?color=darkmagenta)](https://www.nuget.org/packages/dotnet-vs)
[![License](https://img.shields.io/github/license/devlooped/dotnet-vs.svg?color=blue)](https://github.com/devlooped/dotnet-vs/blob/master/license.txt)
[![CI Status](https://github.com/devlooped/dotnet-vs/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/dotnet-vs/actions?query=branch%3Amain+workflow%3Abuild+)
[![CI Version](https://img.shields.io/endpoint?label=nuget.ci&color=brightgreen&url=https://shields.kzu.io/vpre/dotnet-vs/main)](https://pkg.kzu.io/index.json)

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


## alias

Shows the list of saved aliases

```
Usage: vs alias [options]
```

All built-in commands support a `-save:[alias]` option that will cause 
the command to be saved with that alias. From that point on, it's possible 
to just run the command (including all saved arguments) by just running 
the alias instead.

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Save the first VS enterprise with the Xamarin/Mobile workload as the "mobile" alias
> vs -sku:ent -first +mobile -save:mobile

# Runs the saved alias with all the original arguments
> vs mobile
```
<!-- EXAMPLES_END -->

## client

Launches Visual Studio in client mode

```
Usage: vs client [options]
```

|Option|Description|
|-|-|
| `rel\|release` | Run release version |
| `pre\|preview` | Run preview version |
| `int\|internal` | Run internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | Run first matching instance. |
| `exp\|experimental` | Run experimental instance instead of regular. |
| `w\|workspaceId:` | The workspace ID to connect to |


## config

Opens the config folder.

```
Usage: vs config [options]
```

|Option|Description|
|-|-|
| `rel\|release` | open release version |
| `pre\|preview` | open preview version |
| `int\|internal` | open internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | open first matching instance. |
| `exp\|experimental` | open experimental instance instead of regular. |


## install

Installs a specific edition of Visual Studio.

```
Usage: vs install [options]
```

|Option|Description|
|-|-|
| `rel\|release` | install release version |
| `pre\|preview` | install preview version |
| `int\|internal` | install internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `add:` | A workload ID |


You can add specific workload IDs by using the supported [workload switches](#workload-id-switches) 
using the `+` prefix.

See the [documentation for the Visual Studio installer command line options](https://docs.microsoft.com/en-us/visualstudio/install/use-command-line-parameters-to-install-visual-studio?view=vs-2019#install-options) 
for the full list of arguments that can be provided.

Common options are `--passive`, `quiet` and `--wait`, for example.

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Installs VS enterprise with the Maui/Mobile workload
# Note the -sku: switch/prefix is optional
> vs install Enterprise +mobile

# Install VS community with the .NET Core, ASP.NET and Azure workloads, 
# shows installation progress and waits for it to finish before returning
> vs install +core +web +azure
```
<!-- EXAMPLES_END -->

## kill

Kills running devenv processes.

```
Usage: vs kill [options]
```

|Option|Description|
|-|-|
| `rel\|release` | kill release version |
| `pre\|preview` | kill preview version |
| `int\|internal` | kill internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | kill experimental instance instead of regular. |
| `first` | kill first matching instance. |
| `all` | kill all instances. |


Examples:

<!-- EXAMPLES_BEGIN -->
```
# Kill all running instances of Visual Studio
> vs kill all
```
<!-- EXAMPLES_END -->

## log

Opens the folder containing the Activity.log file.

```
Usage: vs log [options]
```

|Option|Description|
|-|-|
| `rel\|release` | open release version |
| `pre\|preview` | open preview version |
| `int\|internal` | open internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | open first matching instance. |
| `exp\|experimental` | open experimental instance instead of regular. |


## modify

Modifies an installation of Visual Studio.

```
Usage: vs modify [options]
```

|Option|Description|
|-|-|
| `rel\|release` | modify release version |
| `pre\|preview` | modify preview version |
| `int\|internal` | modify internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | modify first matching instance. |
| `add:` | A workload ID |
| `remove:` | A workload ID |


A shorthand notation is available for `add|remove [workload ID]` via the supported 
workload ID switches/aliases, using the `+` (for `add`) and `-` (for `remove`) prefixes.

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Add .NET Core Workload to installed Visual Studio Preview
> vs modify preview +core
```
<!-- EXAMPLES_END -->

## run

This is default command, so typically it does not need to be provided as an argument.

```
Usage: vs run [options]
```

|Option|Description|
|-|-|
| `rel\|release` | run release version |
| `pre\|preview` | run preview version |
| `int\|internal` | run internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | run experimental instance instead of regular. |
| `id:` | Run a specific instance by its ID |
| `f\|first` | If more than one instance matches the criteria, run the first one sorted by descending build version. |
| `v\|version:` | Run specific (semantic) version, such as 16.4 or 16.5.3. |
| `w\|wait` | Wait for the started Visual Studio to exit. |
| `nr\|nodereuse` | Disable MSBuild node reuse. Useful when testing analyzers, tasks and targets. Defaults to true when running experimental instance. |
| `default` | Set as the default version to run when no arguments are provided, or remove the current default (with --default-). |
| `requires:` | A workload ID |


All [workload switches](#workload-id-switches) are available too to filter the 
instance to run, including using the `+` prefix/alias syntax.

This command will remember the last VS that was located and run. So the next time you 
can just run the same instance by simply using `vs` (since `run` is the default command 
and can be omitted).

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Runs the first VS enterprise with the Maui workload
> vs -sku:ent -first +maui

# Runs VS 16.8
> vs -v:16.8

# Runs VS 16.9 preview
> vs -v:16.9 -pre

# Runs the last VS that was run
> vs
```
<!-- EXAMPLES_END -->

## update

Updates an installation of Visual Studio.

```
Usage: vs update [options]
```

|Option|Description|
|-|-|
| `rel\|release` | Update release version |
| `pre\|preview` | Update preview version |
| `int\|internal` | Update internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | Update first matching instance. |
| `all` | Update all instances. |


## where

Locates the installed version(s) of Visual Studio that satisfy the requested requirements, optionally retrieving installation properties from it.

```
Usage: vs where [options]
```

|Option|Description|
|-|-|
| `rel\|release` | show release version |
| `pre\|preview` | show preview version |
| `int\|internal` | show internal (aka 'dogfood') version |
| `sku:` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter:` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | show first matching instance. |
| `prop\|property:` | The name of a property to return |
| `list` | Shows result as a list |
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
workload ID switches, using the `+` prefix (see below).

See also [vswhere on GitHub](https://github.com/microsoft/vswhere).


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


<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/clarius.png "Clarius Org")](https://github.com/clarius)
[![Kirill Osenkov](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/KirillOsenkov.png "Kirill Osenkov")](https://github.com/KirillOsenkov)
[![MFB Technologies, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MFB-Technologies-Inc.png "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![Torutek](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/torutek-gh.png "Torutek")](https://github.com/torutek-gh)
[![DRIVE.NET, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/drivenet.png "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![Keith Pickford](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Keflon.png "Keith Pickford")](https://github.com/Keflon)
[![Thomas Bolon](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/tbolon.png "Thomas Bolon")](https://github.com/tbolon)
[![Kori Francis](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/kfrancis.png "Kori Francis")](https://github.com/kfrancis)
[![Toni Wenzel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/twenzel.png "Toni Wenzel")](https://github.com/twenzel)
[![Uno Platform](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/unoplatform.png "Uno Platform")](https://github.com/unoplatform)
[![Dan Siegel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/dansiegel.png "Dan Siegel")](https://github.com/dansiegel)
[![Reuben Swartz](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/rbnswartz.png "Reuben Swartz")](https://github.com/rbnswartz)
[![Jacob Foshee](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jfoshee.png "Jacob Foshee")](https://github.com/jfoshee)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Mrxx99.png "")](https://github.com/Mrxx99)
[![Eric Johnson](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/eajhnsn1.png "Eric Johnson")](https://github.com/eajhnsn1)
[![Ix Technologies B.V.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/IxTechnologies.png "Ix Technologies B.V.")](https://github.com/IxTechnologies)
[![David JENNI](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/davidjenni.png "David JENNI")](https://github.com/davidjenni)
[![Jonathan ](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Jonathan-Hickey.png "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Charley Wu](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/akunzai.png "Charley Wu")](https://github.com/akunzai)
[![Jakob Tikjøb Andersen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jakobt.png "Jakob Tikjøb Andersen")](https://github.com/jakobt)
[![Seann Alexander](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/seanalexander.png "Seann Alexander")](https://github.com/seanalexander)
[![Tino Hager](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/tinohager.png "Tino Hager")](https://github.com/tinohager)
[![Mark Seemann](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/ploeh.png "Mark Seemann")](https://github.com/ploeh)
[![Ken Bonny](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/KenBonny.png "Ken Bonny")](https://github.com/KenBonny)
[![Simon Cropp](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/SimonCropp.png "Simon Cropp")](https://github.com/SimonCropp)
[![agileworks-eu](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/agileworks-eu.png "agileworks-eu")](https://github.com/agileworks-eu)
[![sorahex](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/sorahex.png "sorahex")](https://github.com/sorahex)
[![Zheyu Shen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/arsdragonfly.png "Zheyu Shen")](https://github.com/arsdragonfly)
[![Vezel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/vezel-dev.png "Vezel")](https://github.com/vezel-dev)
[![ChilliCream](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/ChilliCream.png "ChilliCream")](https://github.com/ChilliCream)
[![4OTC](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/4OTC.png "4OTC")](https://github.com/4OTC)
[![Vincent Limo](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/v-limo.png "Vincent Limo")](https://github.com/v-limo)


<!-- sponsors.md -->

[![Sponsor this project](https://raw.githubusercontent.com/devlooped/sponsors/main/sponsor.png "Sponsor this project")](https://github.com/sponsors/devlooped)
&nbsp;

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
