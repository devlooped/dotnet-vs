[![EULA](https://img.shields.io/badge/EULA-OSMF-blue?labelColor=black&color=C9FF30)](osmfeula.txt)
[![OSS](https://img.shields.io/github/license/devlooped/oss.svg?color=blue)](license.txt)
[![GitHub](https://img.shields.io/badge/-source-181717.svg?logo=GitHub)](https://github.com/devlooped/dotnet-vs)

<!-- include ../../readme.md#content -->
<!-- #content -->
A global tool for running, managing and querying Visual Studio installations

Supports switches in all the following forms: `-flag`, `--flag`, `/flag`,
`-flag=value`, `--flag=value`, `-flag:value`, `--flag:value`, and bare channel/SKU/workload shortcuts.

Supported commands:

## alias

Shows the list of saved aliases

```
Usage: dnx vs -- alias [options]
```

All built-in commands support a `-save:[alias]` option that will cause 
the command to be saved with that alias. From that point on, it's possible 
to just run the command (including all saved arguments) by just running 
the alias instead.

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Save the first VS enterprise with the Maui/Mobile workload as the "mobile" alias
> dnx vs -- -sku:ent -first +mobile -save:mobile

# Runs the saved alias with all the original arguments
> dnx vs -- mobile
```
<!-- EXAMPLES_END -->

## client

Launches Visual Studio in client mode

```
Usage: dnx vs -- client [options]
```

|Option|Description|
|-|-|
| `stable` | Run stable version |
| `insiders` | Run insiders version |
| `int\|internal` | Run internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | Run experimental instance instead of regular. |
| `first` | Run first matching instance. |
| `w\|workspaceId` | The workspace ID to connect to |


## config

Opens the config folder.

```
Usage: dnx vs -- config [options]
```

|Option|Description|
|-|-|
| `stable` | open stable version |
| `insiders` | open insiders version |
| `int\|internal` | open internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | open experimental instance instead of regular. |
| `first` | open first matching instance. |


## install

Installs a specific edition of Visual Studio.

```
Usage: dnx vs -- install [options]
```

|Option|Description|
|-|-|
| `stable` | install stable version |
| `insiders` | install insiders version |
| `int\|internal` | install internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `nick\|nickname` | Optional nickname to use |
| `v\|version` | Install specific (semantic) version, such as 18.7 or 18.7.3 |
| `add` | A workload ID |


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

## kill

Kills running devenv processes.

```
Usage: dnx vs -- kill [options]
```

|Option|Description|
|-|-|
| `stable` | kill stable version |
| `insiders` | kill insiders version |
| `int\|internal` | kill internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | kill experimental instance instead of regular. |
| `first` | kill first matching instance. |
| `all` | kill all instances. |


Examples:

<!-- EXAMPLES_BEGIN -->
```
# Kill all running instances of Visual Studio
> dnx vs -- kill all
```
<!-- EXAMPLES_END -->

## log

Opens the folder containing the Activity.log file.

```
Usage: dnx vs -- log [options]
```

|Option|Description|
|-|-|
| `stable` | open stable version |
| `insiders` | open insiders version |
| `int\|internal` | open internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | open experimental instance instead of regular. |
| `first` | open first matching instance. |


## modify

Modifies an installation of Visual Studio.

```
Usage: dnx vs -- modify [options]
```

|Option|Description|
|-|-|
| `stable` | modify stable version |
| `insiders` | modify insiders version |
| `int\|internal` | modify internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | modify first matching instance. |
| `add` | A workload ID |
| `remove` | A workload ID |


A shorthand notation is available for `add|remove [workload ID]` via the supported 
workload ID switches/aliases, using the `+` (for `add`) and `-` (for `remove`) prefixes.

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Add .NET Core Workload to installed Visual Studio Preview
> dnx vs -- modify insiders +core
```
<!-- EXAMPLES_END -->

## run

This is default command, so typically it does not need to be provided as an argument.

```
Usage: dnx vs -- run [options]
```

|Option|Description|
|-|-|
| `stable` | run stable version |
| `insiders` | run insiders version |
| `int\|internal` | run internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `exp\|experimental` | run experimental instance instead of regular. |
| `id` | Run a specific instance by its ID |
| `f\|first` | If more than one instance matches the criteria, run the first one sorted by descending build version. |
| `v\|version` | Run specific (semantic) version, such as 18.7 or 18.7.3 |
| `w\|wait` | Wait for the started Visual Studio to exit. |
| `nr\|nodereuse` | Disable MSBuild node reuse. Useful when testing analyzers, tasks and targets. Defaults to true when running experimental instance. |
| `default` | Set as the default version to run when no arguments are provided, or remove the current default (with --default-). |
| `requires` | A workload ID |


All [workload switches](#workload-id-switches) are available too to filter the 
instance to run, including using the `+` prefix/alias syntax.

This command will remember the last VS that was located and run. So the next time you 
can just run the same instance by simply using `dnx vs` (since `run` is the default command 
and can be omitted).

Examples:

<!-- EXAMPLES_BEGIN -->
```
# Runs the first VS enterprise with the Maui/Mobile workload
> dnx vs -- -sku:ent -first +mobile

# Runs VS 18.7
> dnx vs -- -v:18.7

# Runs VS 18 Insiders
> dnx vs -- -v:18 -insiders

# Runs the last VS that was run
> dnx vs
```
<!-- EXAMPLES_END -->

## update

Updates an installation of Visual Studio.

```
Usage: dnx vs -- update [options]
```

|Option|Description|
|-|-|
| `stable` | Update stable version |
| `insiders` | Update insiders version |
| `int\|internal` | Update internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | Update first matching instance. |
| `all` | Update all instances. |
| `v\|version` | Update specific (semantic) version, such as 18.7 or 18.7.3 |


Examples:

<!-- EXAMPLES_BEGIN -->
```
# Update the installed VS 18.7 instance
> dnx vs -- update -v:18.7

# Update all matching VS 17 instances
> dnx vs -- update --version 17 --all
```
<!-- EXAMPLES_END -->

## where

Locates the installed version(s) of Visual Studio that satisfy the requested requirements, optionally retrieving installation properties from it.

```
Usage: dnx vs -- where [options]
```

|Option|Description|
|-|-|
| `stable` | show stable version |
| `insiders` | show insiders version |
| `int\|internal` | show internal (aka 'dogfood') version |
| `sku` | Edition, one of `e\|ent\|enterprise`, `p\|pro\|professional`, `c\|com\|community`, `b\|build\|buildtools` or `t\|test\|testagent` |
| `filter` | Expression to filter VS instances. E.g. `x => x.InstanceId = '123'` |
| `first` | show first matching instance. |
| `prop\|property` | The name of a property to return |
| `list` | Shows result as a list |
| `requires` | A workload ID |


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
| `vc`      | Microsoft.VisualStudio.Workload.VCTools |
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

See the [full list of workload and component IDs](https://aka.ms/vs/workloads).

<!-- #content -->
<!-- ../../readme.md#content -->
<!-- include https://github.com/devlooped/.github/raw/main/osmf.md -->
## Open Source Maintenance Fee

To ensure the long-term sustainability of this project, users of this package who generate 
revenue must pay an [Open Source Maintenance Fee](https://opensourcemaintenancefee.org). 
While the source code is freely available under the terms of the [License](license.txt), 
this package and other aspects of the project require [adherence to the Maintenance Fee](osmfeula.txt).

To pay the Maintenance Fee, [become a Sponsor](https://github.com/sponsors/devlooped) at the proper 
OSMF tier. A single fee covers all of [Devlooped packages](https://www.nuget.org/profiles/Devlooped).

<!-- https://github.com/devlooped/.github/raw/main/osmf.md -->
<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://avatars.githubusercontent.com/u/71888636?v=4&s=39 "Clarius Org")](https://github.com/clarius)
[![MFB Technologies, Inc.](https://avatars.githubusercontent.com/u/87181630?v=4&s=39 "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![SandRock](https://avatars.githubusercontent.com/u/321868?u=99e50a714276c43ae820632f1da88cb71632ec97&v=4&s=39 "SandRock")](https://github.com/sandrock)
[![DRIVE.NET, Inc.](https://avatars.githubusercontent.com/u/15047123?v=4&s=39 "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![Keith Pickford](https://avatars.githubusercontent.com/u/16598898?u=64416b80caf7092a885f60bb31612270bffc9598&v=4&s=39 "Keith Pickford")](https://github.com/Keflon)
[![Thomas Bolon](https://avatars.githubusercontent.com/u/127185?u=7f50babfc888675e37feb80851a4e9708f573386&v=4&s=39 "Thomas Bolon")](https://github.com/tbolon)
[![Reuben Swartz](https://avatars.githubusercontent.com/u/724704?u=2076fe336f9f6ad678009f1595cbea434b0c5a41&v=4&s=39 "Reuben Swartz")](https://github.com/rbnswartz)
[![Jacob Foshee](https://avatars.githubusercontent.com/u/480334?v=4&s=39 "Jacob Foshee")](https://github.com/jfoshee)
[![](https://avatars.githubusercontent.com/u/33566379?u=bf62e2b46435a267fa246a64537870fd2449410f&v=4&s=39 "")](https://github.com/Mrxx99)
[![Eric Johnson](https://avatars.githubusercontent.com/u/26369281?u=41b560c2bc493149b32d384b960e0948c78767ab&v=4&s=39 "Eric Johnson")](https://github.com/eajhnsn1)
[![Jonathan ](https://avatars.githubusercontent.com/u/5510103?u=98dcfbef3f32de629d30f1f418a095bf09e14891&v=4&s=39 "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Ken Bonny](https://avatars.githubusercontent.com/u/6417376?u=569af445b6f387917029ffb5129e9cf9f6f68421&v=4&s=39 "Ken Bonny")](https://github.com/KenBonny)
[![Simon Cropp](https://avatars.githubusercontent.com/u/122666?v=4&s=39 "Simon Cropp")](https://github.com/SimonCropp)
[![agileworks-eu](https://avatars.githubusercontent.com/u/5989304?v=4&s=39 "agileworks-eu")](https://github.com/agileworks-eu)
[![Zheyu Shen](https://avatars.githubusercontent.com/u/4067473?v=4&s=39 "Zheyu Shen")](https://github.com/arsdragonfly)
[![Vezel](https://avatars.githubusercontent.com/u/87844133?v=4&s=39 "Vezel")](https://github.com/vezel-dev)
[![ChilliCream](https://avatars.githubusercontent.com/u/16239022?v=4&s=39 "ChilliCream")](https://github.com/ChilliCream)
[![4OTC](https://avatars.githubusercontent.com/u/68428092?v=4&s=39 "4OTC")](https://github.com/4OTC)
[![domischell](https://avatars.githubusercontent.com/u/66068846?u=0a5c5e2e7d90f15ea657bc660f175605935c5bea&v=4&s=39 "domischell")](https://github.com/DominicSchell)
[![Adrian Alonso](https://avatars.githubusercontent.com/u/2027083?u=129cf516d99f5cb2fd0f4a0787a069f3446b7522&v=4&s=39 "Adrian Alonso")](https://github.com/adalon)
[![torutek](https://avatars.githubusercontent.com/u/33917059?v=4&s=39 "torutek")](https://github.com/torutek)
[![Ryan McCaffery](https://avatars.githubusercontent.com/u/16667079?u=c0daa64bb5c1b572130e05ae2b6f609ecc912d4d&v=4&s=39 "Ryan McCaffery")](https://github.com/mccaffers)
[![Seika Logiciel](https://avatars.githubusercontent.com/u/2564602?v=4&s=39 "Seika Logiciel")](https://github.com/SeikaLogiciel)
[![Andrew Grant](https://avatars.githubusercontent.com/devlooped-user?s=39 "Andrew Grant")](https://github.com/wizardness)
[![eska-gmbh](https://avatars.githubusercontent.com/devlooped-team?s=39 "eska-gmbh")](https://github.com/eska-gmbh)
[![Geodata AS](https://avatars.githubusercontent.com/u/5946299?v=4&s=39 "Geodata AS")](https://github.com/geodata-no)
[![Jiri Slachta](https://avatars.githubusercontent.com/u/6891947?u=802cfeb13b070d04c53269fc662b0d58963480dd&v=4&s=39 "Jiri Slachta")](https://github.com/jslachta)


<!-- sponsors.md -->
[![Sponsor this project](https://avatars.githubusercontent.com/devlooped-sponsor?s=118 "Sponsor this project")](https://github.com/sponsors/devlooped)

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
