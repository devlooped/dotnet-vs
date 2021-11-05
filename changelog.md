# Changelog

## [v1.1.1](https://github.com/devlooped/dotnet-vs/tree/v1.1.1) (2021-11-05)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/v1.1.0...v1.1.1)

:sparkles: Implemented enhancements:

- Decrease package size by building 3.1 and using rollForward [\#114](https://github.com/devlooped/dotnet-vs/issues/114)

:bug: Fixed bugs:

- +core workload alias doesn't work for VS2022 [\#106](https://github.com/devlooped/dotnet-vs/issues/106)

:twisted_rightwards_arrows: Merged:

- Fix renamed/removed .NETCore workload for 2022 [\#115](https://github.com/devlooped/dotnet-vs/pull/115) (@kzu)

## [v1.1.0](https://github.com/devlooped/dotnet-vs/tree/v1.1.0) (2021-08-10)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/v1.0.0...v1.1.0)

:sparkles: Implemented enhancements:

- When installing non-stable channels, dev17/vs2022 should be picked [\#104](https://github.com/devlooped/dotnet-vs/issues/104)
- Visual Studio 2022 is not considered when using channel aliases [\#102](https://github.com/devlooped/dotnet-vs/issues/102)

:twisted_rightwards_arrows: Merged:

- When installing non-stable channel, pick dev17/vs2022 [\#105](https://github.com/devlooped/dotnet-vs/pull/105) (@kzu)
- Consider Visual Studio 2022 when filtering by channel alias [\#103](https://github.com/devlooped/dotnet-vs/pull/103) (@kzu)

## [v1.0.0](https://github.com/devlooped/dotnet-vs/tree/v1.0.0) (2021-02-01)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/v0.9.4...v1.0.0)

:sparkles: Implemented enhancements:

- When rendering supported commands, sort by command name [\#82](https://github.com/devlooped/dotnet-vs/issues/82)
- Check for updated versions of the tool  [\#80](https://github.com/devlooped/dotnet-vs/issues/80)
- Clean up workload ID prefixes for added consistency [\#79](https://github.com/devlooped/dotnet-vs/issues/79)
- Add `client` command doc and `-w:` shorthand for -workspaceId [\#78](https://github.com/devlooped/dotnet-vs/issues/78)
- Add missing `alias` command doc template and examples [\#77](https://github.com/devlooped/dotnet-vs/issues/77)
- Update docs and readme from generated readme command [\#76](https://github.com/devlooped/dotnet-vs/issues/76)
- Showing help for commands results in non-zero exit code [\#75](https://github.com/devlooped/dotnet-vs/issues/75)
- Allow rendering tool version with --version [\#72](https://github.com/devlooped/dotnet-vs/issues/72)

:bug: Fixed bugs:

- Running vs where -version \[...\] always shows an empty list [\#73](https://github.com/devlooped/dotnet-vs/issues/73)

:hammer: Other:

- ⭮ Rename top-level namespace to Devlooped ⭯ [\#70](https://github.com/devlooped/dotnet-vs/issues/70)

:twisted_rightwards_arrows: Merged:

- ⭮ Rename top-level namespace to Devlooped ⭯ [\#71](https://github.com/devlooped/dotnet-vs/pull/71) (@kzu)

## [v0.9.4](https://github.com/devlooped/dotnet-vs/tree/v0.9.4) (2021-01-04)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/v0.9.3...v0.9.4)

:bug: Fixed bugs:

- For install/modify commands, -?/-h does not show help [\#67](https://github.com/devlooped/dotnet-vs/issues/67)
- Install is passing wrongly -add instead of --add to the bootstrapper [\#66](https://github.com/devlooped/dotnet-vs/issues/66)

:twisted_rightwards_arrows: Merged:

- 🌐 Add building and pushing gh-pages to clarius.org via dotnet-file [\#65](https://github.com/devlooped/dotnet-vs/pull/65) (@kzu)

## [v0.9.3](https://github.com/devlooped/dotnet-vs/tree/v0.9.3) (2020-12-21)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/v0.9.2...v0.9.3)

:sparkles: Implemented enhancements:

- Unify vswhere -requires argument on + alias prefix [\#64](https://github.com/devlooped/dotnet-vs/issues/64)
- Add support for updating/modifying Build Tools and other installed builds [\#57](https://github.com/devlooped/dotnet-vs/issues/57)

## [v0.9.2](https://github.com/devlooped/dotnet-vs/tree/v0.9.2) (2020-12-09)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/v0.9.1...v0.9.2)

:sparkles: Implemented enhancements:

- Provide repository information in the package [\#59](https://github.com/devlooped/dotnet-vs/issues/59)

## [v0.9.1](https://github.com/devlooped/dotnet-vs/tree/v0.9.1) (2020-12-08)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/v0.9.0...v0.9.1)

:sparkles: Implemented enhancements:

- Target .net5 natively too [\#50](https://github.com/devlooped/dotnet-vs/issues/50)
- Add support for updating/modifying Build Tools and other installed builds [\#58](https://github.com/devlooped/dotnet-vs/pull/58) (@kzu)

## [v0.9.0](https://github.com/devlooped/dotnet-vs/tree/v0.9.0) (2020-12-08)

[Full Changelog](https://github.com/devlooped/dotnet-vs/compare/1d0071fac69235e83dc873c226c2c2748d49ff7b...v0.9.0)

:sparkles: Implemented enhancements:

- Support for Visual Studio Build Tools? [\#44](https://github.com/devlooped/dotnet-vs/issues/44)
- Allow running devenv commands from the context of a selected VS [\#8](https://github.com/devlooped/dotnet-vs/issues/8)

:hammer: Other:

- Target .NET Core 2.1 which is more broadly available [\#36](https://github.com/devlooped/dotnet-vs/issues/36)
- Some suggestions [\#32](https://github.com/devlooped/dotnet-vs/issues/32)
- Add alias -exp for /rootSuffix Exp when running VS [\#12](https://github.com/devlooped/dotnet-vs/issues/12)
- Switch storage of default VS settings to use dotnet-file [\#10](https://github.com/devlooped/dotnet-vs/issues/10)
- Add kill command [\#9](https://github.com/devlooped/dotnet-vs/issues/9)
- Allow modifying an existing instance of VS [\#7](https://github.com/devlooped/dotnet-vs/issues/7)
- Allow updating an instance automatically if needed [\#6](https://github.com/devlooped/dotnet-vs/issues/6)
- Make changing default version of VS explicit [\#5](https://github.com/devlooped/dotnet-vs/issues/5)
- Indent "Workload ID aliases:" header [\#4](https://github.com/devlooped/dotnet-vs/issues/4)
- vs install [\#2](https://github.com/devlooped/dotnet-vs/issues/2)
- vs run [\#1](https://github.com/devlooped/dotnet-vs/issues/1)

:twisted_rightwards_arrows: Merged:

- ⁙ Target .net5 natively too [\#55](https://github.com/devlooped/dotnet-vs/pull/55) (@kzu)
- 🖆 Apply kzu/oss template via dotnet-file [\#48](https://github.com/devlooped/dotnet-vs/pull/48) (@kzu)
- Add support for --first to all relevant commands [\#47](https://github.com/devlooped/dotnet-vs/pull/47) (@kzu)
- Add support for VS Build Tools and VS Test Agent [\#46](https://github.com/devlooped/dotnet-vs/pull/46) (@likemike91)
- Opening activity log folder and select file instead of opening the file [\#45](https://github.com/devlooped/dotnet-vs/pull/45) (@adalon)
- Multi-target 2.1 and 3.1 for maximum reach [\#42](https://github.com/devlooped/dotnet-vs/pull/42) (@kzu)
- Consider Run default command for saved aliases too [\#41](https://github.com/devlooped/dotnet-vs/pull/41) (@kzu)
- When running VS, /log must come last [\#40](https://github.com/devlooped/dotnet-vs/pull/40) (@kzu)
- Bump to latest dotnet-config and new API [\#39](https://github.com/devlooped/dotnet-vs/pull/39) (@kzu)
- Renamed channel master -\> main [\#38](https://github.com/devlooped/dotnet-vs/pull/38) (@adalon)
- Target .NET Core 2.1 which is more broadly available [\#37](https://github.com/devlooped/dotnet-vs/pull/37) (@kzu)
- Make --config automatically --passive [\#35](https://github.com/devlooped/dotnet-vs/pull/35) (@kzu)
- Replaced Command.CancelAsync with AsyncDisposable pattern [\#34](https://github.com/devlooped/dotnet-vs/pull/34) (@adalon)
- Added ClientCommand to launch VS in client mode [\#33](https://github.com/devlooped/dotnet-vs/pull/33) (@adalon)
- Added update improvement and vs update all [\#31](https://github.com/devlooped/dotnet-vs/pull/31) (@adalon)
- Misc improvements [\#30](https://github.com/devlooped/dotnet-vs/pull/30) (@adalon)
- Implemented save command using dotnet-config [\#29](https://github.com/devlooped/dotnet-vs/pull/29) (@adalon)
- Added SelfUpdateCommand [\#28](https://github.com/devlooped/dotnet-vs/pull/28) (@adalon)
- Improved where commands: [\#27](https://github.com/devlooped/dotnet-vs/pull/27) (@adalon)
- Added generate-readme command [\#26](https://github.com/devlooped/dotnet-vs/pull/26) (@adalon)
- Moved files around [\#24](https://github.com/devlooped/dotnet-vs/pull/24) (@adalon)
- Improved WhereCommand [\#23](https://github.com/devlooped/dotnet-vs/pull/23) (@adalon)
- Added ChooseMany support [\#22](https://github.com/devlooped/dotnet-vs/pull/22) (@adalon)
- Added unit test for run being the default command [\#21](https://github.com/devlooped/dotnet-vs/pull/21) (@kzu)
- Add back run as the default command, improve its selector [\#20](https://github.com/devlooped/dotnet-vs/pull/20) (@kzu)
- Added KillCommand [\#19](https://github.com/devlooped/dotnet-vs/pull/19) (@adalon)
- Add `vs log` command to open the ActivityLog.xml [\#18](https://github.com/devlooped/dotnet-vs/pull/18) (@kzu)
- Add support for `vs config` [\#17](https://github.com/devlooped/dotnet-vs/pull/17) (@kzu)
- Refactored commands and added ModifyCommand [\#16](https://github.com/devlooped/dotnet-vs/pull/16) (@adalon)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/github-changelog-generator/github-changelog-generator)*
