# Migrate dotnet-vs argument parsing to System.CommandLine

## Problem

The CLI currently uses **Mono.Options** wrapped in a custom abstraction (`IOptions`, `Options`, `OptionSet<T>`, per-option classes in `Options/`), with a `CommandDescriptor` (parsing/state) + `Command` (execution) split wired by `CommandFactory`. We want to migrate to **System.CommandLine 2.0 (stable, currently 2.0.9)**, restructuring commands natively around its model (`Command` + `SetAction(ParseResult)`), replacing custom help with built-in help, while preserving **full CLI backward compatibility**.

## Current-state inventory

**Commands** (from `CommandFactory`): `run` (default/fallback), `where`, `install`, `update`, `modify`, `kill`, `config`, `log`, `alias`, `client` + system (hidden) commands: `gen-readme`, `save`, `update-self`.

**Non-standard behaviors that MUST be preserved** (System.CommandLine won't do these natively):

1. **Bare-word shortcuts**: `vs run stable`, `pre`, `main`, `int` → channel; `e|ent|enterprise|p|pro|...` → `--sku=`  (`ChannelOption.Parse`, `SkuOption.Parse` overrides).
2. **Workload alias prefixes**: `+mobile` / `-mobile` → `--requires Microsoft.VisualStudio.Workload.NetCrossPlat`; `+Some.Id` passthrough (`WorkloadOptions.Parse` override; alias prefix differs per command: `+`/`-` for run/where/kill, `+` add / `-` remove semantics for install/modify).
3. **Mono.Options toggle-off syntax**: `--default-` clears the default (bool? `SetDefault`), also `-nr`/`nodereuse` etc.
4. **`--save=ALIAS [--global]` on any command** → reroutes to hidden `save` command with original command prepended to args (`CommandFactory.CreateCommandAsync`).
5. **Saved aliases via DotNetConfig**: unknown command name looked up in `[vs "alias"]` config section, args restored from `|`-joined saved string.
6. **`run` as default command**: unknown first token (or no args) falls back to `run` with the token prepended; unmatched tokens are **passed through to devenv.exe** (`ExtraArguments`).
7. **`update --self`** → reroutes to hidden `update-self` command.
8. **`--version` only as first arg**; `-?`/`/?`/`/h`/`/help`/`?` all mean help at top level; `-?|-h|--help` per command.
9. **Examples appended to help** from embedded `Docs/{command}.md` between `EXAMPLES_BEGIN/END` markers.
10. **`gen-readme`** introspects option metadata to generate markdown tables in readme (`MarkdownOptionsTextWriter`).
11. **Ctrl+C cancellation** disposes the executing command (`Program.CancelAsync`).
12. **Version update check** after successful command run (`VersionChecker`), suppressed with `DebugOption` (`--debug` rethrow behavior).

## Approach

Big-bang migration. Native System.CommandLine object model; full compat achieved via a **token normalization (preprocessing) pass** before `Parse`, mirroring what the Mono.Options `Parse` overrides do today, plus `CommandFactory`-equivalent routing kept as an args-rewriting front-end before invoking the root command.

### Target architecture

```
Program.Main
  └─ ArgumentPreprocessor  (alias expansion from DotNetConfig, --save rerouting,
                            update --self rerouting, default-command fallback,
                            legacy help tokens /h /? ? → --help)
  └─ VsRootCommand (RootCommand)
       ├─ run (default)   RunCommand : Command, SetAction(async (ParseResult, CancellationToken))
       ├─ where, install, update, modify, kill, config, log, alias, client
       └─ hidden: save, update-self, gen-readme
  Shared option sets as reusable static/factory classes:
       VisualStudioOptionsBinder (channel, sku, filter, exp, nickname, all, first)
       WorkloadTokenRewriter (+alias/-alias → --requires ID)
       ChannelShortcutRewriter (stable/pre/main/int → --stable etc.)
       SkuShortcutRewriter (e/ent/... → --sku=...)
```

- **Commands** become classes deriving from `System.CommandLine.Command`, declaring their `Option<T>`/`Argument<T>` fields and calling `SetAction`. Service dependencies (`WhereService`, `InstallerService`) injected via constructor. Descriptor classes are deleted; state moves into `ParseResult.GetValue(option)` reads inside actions.
- **Token rewriting per command**: a small `TokenRewriter` step applied to that command's raw tokens (channel shortcuts, sku shortcuts, workload `+`/`-` aliases, `--opt-` toggle-off → `--opt false`). Implemented as a pure `string[] → string[]` function per command so it's unit-testable in isolation. Root-level rewriting handles legacy help/version tokens and command fallback.
- **Pass-through args (`run`, `client`)**: unmatched tokens collected via a greedy `Argument<string[]>` (or `UnmatchedTokensAreErrors=false` + `ParseResult.UnmatchedTokens`) and forwarded to devenv.
- **Help**: built-in `--help` customized (HelpBuilder/HelpAction customization) to append the Examples section parsed from embedded `Docs/*.md`. Add `-?`, `/h`, `/?`, `?` as rewritten tokens.
- **gen-readme**: rewrite `MarkdownOptionsTextWriter` to walk `Command.Options` (`Option.Name`, `Aliases`, `Description`, `Hidden`) and emit the same markdown table format.
- **Version check + error handling**: keep `VersionChecker`, top-level try/catch (`--debug` rethrow), and Ctrl+C disposal semantics in `Program`, wrapped around `rootCommand.Parse(args).InvokeAsync(cancellationToken)`.

## Todos

1. **add-package-and-scaffold** — Add `System.CommandLine` (2.0.x stable) to VisualStudio.csproj. Create `VsRootCommand` skeleton with all subcommands registered (empty actions), builds side-by-side with existing code.
2. **token-rewriters** — Implement pure token-rewriting functions: channel shortcuts, sku shortcuts, workload `+`/`-` aliases (with per-command alias table + prefix config), `--opt-` toggle-off, legacy help tokens (`?`, `-?`, `/?`, `/h`, `/help`), `--version`-first-arg rule. Port `WorkloadOptionsTests` expectations first (test-driven).
3. **routing-front-end** — Implement `ArgumentPreprocessor` replicating `CommandFactory.CreateCommandAsync` routing: `--save` → save command (original command prepended), `update --self` → update-self, DotNetConfig saved-alias expansion, unknown-command → `run` fallback with token prepended. Unit test against `CommandFactoryTests` scenarios.
4. **shared-options** — Build shared VS-selection option set (channel, sku, filter/exp expression, nickname, all, first) as reusable `Option<T>` factories replacing `VisualStudioOptions`; adapt `VisualStudioPredicateBuilder`/`WhereService` to accept parsed values (e.g. an options record) instead of `IOptions`.
5. **migrate-core-commands** — Migrate `run` and `where` (largest/most complex: pass-through args, workloads, id/version/first/wait/nr/default options, prop/list options). Delete their descriptors.
6. **migrate-install-family** — Migrate `install`, `update`, `modify` (workload add/remove semantics, installer service passthrough args).
7. **migrate-simple-commands** — Migrate `kill`, `config`, `log`, `alias`, `client`.
8. **migrate-system-commands** — Migrate hidden `save`, `update-self`, `gen-readme`. Rewrite readme markdown generation from `Command.Options` metadata; regenerate readme and diff to confirm identical/equivalent output.
9. **help-and-examples** — Customize help: append Examples from embedded `Docs/*.md`, root help listing commands with descriptions matching current layout closely; hide system commands.
10. **program-rewire** — Rewrite `Program.RunAsync` around root command invocation: preprocessor → parse → InvokeAsync with CancellationToken (Ctrl+C disposal), top-level exception handling (`--debug`), `VersionChecker.ShowUpdateAsync` after success, error codes.
11. **remove-mono-options** — Delete `IOptions`, `Options`, `OptionSet<T>`, all `Options/*.cs` option classes, `CommandDescriptor`, all `*CommandDescriptor` classes, `CommandFactory`, `ShowUsageException`, `ITextWriter`/`DefaultTextWriter` (if unused). Remove `Mono.Options` PackageReference.
12. **migrate-tests** — Update/port test suite: `CommandFactoryTests` → routing/preprocessor tests, `VisualStudioOptionsTests` → shared option parse tests, `WorkloadOptionsTests` → rewriter tests, `ProgramTests` end-to-end. Add compat regression tests asserting the documented example invocations from `Docs/*.md` parse successfully.
13. **validate-and-document** — `dotnet build`, `dnx --yes retest`, `dotnet format whitespace`/`style`. Regenerate readme via `gen-readme` and verify. Update `changelog.md` and AGENTS.md with the new architecture.

### Dependencies

- 1 → (2,3,4) → 5 → 6,7,8 → 9 → 10 → 11 → 12 → 13 (2,3,4 parallelizable; 6,7,8 parallelizable after 5).

## Notes & risks

- **Compat trap — bare shortcut tokens vs subcommand tokens**: `vs stable` today = `run stable` (channel shortcut). The preprocessor must apply the default-command fallback *before* rewriting; rewriters run on the args following a resolved command name.
- **`ExtraArguments` pass-through**: several commands (run, client, install/update passthrough to installer) rely on collecting unrecognized tokens. System.CommandLine is strict by default — those commands need unmatched-token tolerance; carefully ensure option-like tokens intended for devenv (e.g. `/log`, `/rootSuffix`) are not swallowed. Consider `--` separator support as an additional (documented) escape hatch.
- **`--default-` / bool-toggle syntax** is Mono.Options-specific; rewrite `--x-` → internal "clear" flag option (e.g. hidden `--x:false`) to keep `bool?` semantics.
- **Help text/readme diffs**: built-in help output format will differ from current custom output; readme markdown tables should be regenerated and reviewed rather than diffed byte-for-byte.
- **`Chooser` interactive selection** and services (`WhereService`, `InstallerService`, `VersionChecker`) are untouched by parsing — only their input types change.
- The nested descriptor coupling in `GenerateReadmeCommandDescriptor` (dictionary of all descriptors) becomes a simple walk of `rootCommand.Subcommands`.
