# dotnet-visualstudio

A global tool for managing Visual Studio installations

[![Build Status](https://dev.azure.com/kzu/oss/_apis/build/status/dotnet-visualstudio?branchName=master)](https://dev.azure.com/kzu/oss/_build/latest?definitionId=32&branchName=master)

Installing or updating (same command for both):

```
dotnet tool update -g dotnet-visualstudio --no-cache --add-source https://www.kzu.io/index.json
```

Supported commands:

```
visualstudio list
```

Supports same parameters as `vswhere.exe`:

```
  -all           Finds all instances even if they are incomplete and may not launch.
  -prerelease    Also searches prereleases. By default, only releases are searched.
  -products=arg  One or more product IDs to find. Defaults to Community, Professional, and Enterprise.
                 Specify "*" by itself to search all product instances installed.
                 See https://aka.ms/vs/workloads for a list of product IDs.
  -requires=arg  One or more workload or component IDs required when finding instances.
                 All specified IDs must be installed unless -requiresAny is specified.
                 See https://aka.ms/vs/workloads for a list of workload and component IDs.
  -requiresAny   Find instances with any one or more workload or components IDs passed to -requires.
  -version=arg   A version range for instances to find. Example: [15.0,16.0) will find versions 15.*.
  -latest        Return only the newest version and last installed.
  -sort          Sorts the instances from newest version and last installed to oldest.
                 When used with "find", first instances are sorted then files are sorted lexigraphically.
  -legacy        Also searches Visual Studio 2015 and older products. Information is limited.
                 This option cannot be used with either -products or -requires.
  -format=arg    Return information about instances found in a format described below.
  -property=arg  The name of a property to return. Defaults to "value" format.
                 Use delimiters ".", "/", or "_" to separate object and property names.
                 Example: "properties.nickname" will return the "nickname" property under "properties".
  -include=arg   One or more extra properties to include, as described below.
  -find=arg      Returns matching file paths under the installation path. Defaults to "value" format.
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
