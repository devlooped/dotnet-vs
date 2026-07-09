using System;
using System.Collections.Generic;
using System.Linq;

namespace Devlooped;

/// <summary>
/// Pure token-rewriting functions that normalize legacy Mono.Options-style CLI
/// tokens into System.CommandLine-compatible tokens before parsing.
/// </summary>
static class TokenRewriter
{
    static readonly HashSet<string> ChannelShortcuts = new(StringComparer.OrdinalIgnoreCase)
    {
        "stable", "rel", "release", "insiders", "pre", "preview", "int", "internal", "main",
    };

    static readonly HashSet<string> SkuShortcuts = new(StringComparer.OrdinalIgnoreCase)
    {
        "e", "ent", "enterprise",
        "p", "pro", "professional",
        "c", "com", "community",
        "b", "build", "buildtools",
        "t", "test", "testagent",
    };

    static readonly HashSet<string> BoolShortcuts = new(StringComparer.OrdinalIgnoreCase)
    {
        "exp", "experimental", "first", "all", "list", "self",
    };

    /// <summary>
    /// Rewrites bare channel words (<c>stable</c>, <c>pre</c>, …) to <c>--stable</c> etc.
    /// </summary>
    public static string[] RewriteChannelShortcuts(IEnumerable<string> tokens) =>
        tokens.Select(t => ChannelShortcuts.Contains(t) ? "--" + t.ToLowerInvariant() : t).ToArray();

    /// <summary>
    /// Rewrites bare SKU words (<c>e</c>, <c>ent</c>, …) to <c>--sku=…</c>.
    /// </summary>
    public static string[] RewriteSkuShortcuts(IEnumerable<string> tokens) =>
        tokens.Select(t => SkuShortcuts.Contains(t) ? "--sku=" + t : t).ToArray();

    /// <summary>
    /// Rewrites bare bool flag words (<c>exp</c>, <c>first</c>, <c>all</c>, …) to <c>--flag</c>.
    /// </summary>
    public static string[] RewriteBoolShortcuts(IEnumerable<string> tokens) =>
        tokens.Select(t => BoolShortcuts.Contains(t) ? "--" + t.ToLowerInvariant() : t).ToArray();

    /// <summary>
    /// Rewrites bare filter expressions containing <c>=&gt;</c> to <c>--filter=…</c>,
    /// and normalizes single quotes to double quotes.
    /// </summary>
    public static string[] RewriteFilterExpressions(IEnumerable<string> tokens)
    {
        var result = new List<string>();
        foreach (var token in tokens)
        {
            if (token.Contains("=>") && !token.StartsWith("-") && !token.StartsWith("/"))
                result.Add("--filter=" + token.Replace('\'', '"').Trim());
            else if (token.StartsWith("--filter=", StringComparison.OrdinalIgnoreCase) ||
                     token.StartsWith("-filter=", StringComparison.OrdinalIgnoreCase) ||
                     token.StartsWith("/filter:", StringComparison.OrdinalIgnoreCase) ||
                     token.StartsWith("-filter:", StringComparison.OrdinalIgnoreCase))
            {
                var value = token[(token.IndexOfAny(['=', ':']) + 1)..].Replace('\'', '"').Trim();
                result.Add("--filter=" + value);
            }
            else if (token.Equals("--filter", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("-filter", StringComparison.OrdinalIgnoreCase) ||
                     token.Equals("/filter", StringComparison.OrdinalIgnoreCase))
            {
                result.Add("--filter");
            }
            else
                result.Add(token);
        }

        // Join --filter with following token if split, and normalize quotes.
        var joined = new List<string>();
        for (var i = 0; i < result.Count; i++)
        {
            if (result[i].Equals("--filter", StringComparison.OrdinalIgnoreCase) && i + 1 < result.Count)
            {
                joined.Add("--filter=" + result[i + 1].Replace('\'', '"').Trim());
                i++;
            }
            else
                joined.Add(result[i]);
        }

        return joined.ToArray();
    }

    /// <summary>
    /// Rewrites workload alias prefixes (e.g. <c>+mobile</c>, <c>-core</c>) into
    /// <c>--requires ID</c> / <c>--add ID</c> / <c>--remove ID</c> token pairs.
    /// Tokens that match a known switch prefix but not a workload alias are left as-is
    /// when they look like options (start with <c>--</c> after one prefix char check fails
    /// for non-alias bare switches like <c>-someswitch</c> that should remain unmatched).
    /// </summary>
    /// <param name="tokens">Input tokens.</param>
    /// <param name="argument">Target option name without dashes (e.g. <c>requires</c>, <c>add</c>, <c>remove</c>).</param>
    /// <param name="aliasPrefixes">Prefixes that introduce workload aliases, e.g. <c>+</c> or <c>+|-</c>.</param>
    /// <param name="argumentPrefix">Emitted option prefix, default <c>--</c>.</param>
    public static string[] RewriteWorkloadAliases(
        IEnumerable<string> tokens,
        string argument,
        string aliasPrefixes,
        string argumentPrefix = "--")
    {
        var prefixes = aliasPrefixes.Split('|', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        foreach (var token in tokens)
        {
            // Already a standard long option for this argument — keep as-is (may be --requires=ID or separate).
            if (token.StartsWith(argumentPrefix + argument, StringComparison.OrdinalIgnoreCase))
            {
                if (token.Contains('='))
                {
                    var parts = token.Split('=', 2);
                    result.Add(parts[0]);
                    result.Add(parts[1]);
                }
                else
                    result.Add(token);
                continue;
            }

            var rewritten = false;
            foreach (var prefix in prefixes)
            {
                if (!token.StartsWith(prefix, StringComparison.Ordinal) || token.Length <= prefix.Length)
                    continue;

                // Do not treat standard options (--foo, -h) as workload aliases.
                if (prefix == "-" && (token.StartsWith("--") || token.Length == 2))
                    continue;

                var value = token[prefix.Length..];

                // For "-" prefix: only rewrite when value is a known alias or looks like a workload/component ID.
                // Bare switches like -someswitch stay unmatched for other parsers / pass-through.
                if (prefix == "-" && !WorkloadAliases.Map.ContainsKey(value) && !LooksLikeWorkloadId(value))
                    continue;

                var workloadId = WorkloadAliases.Resolve(value);
                result.Add(argumentPrefix + argument);
                result.Add(workloadId);
                rewritten = true;
                break;
            }

            if (!rewritten)
                result.Add(token);
        }

        return result.ToArray();
    }

    static bool LooksLikeWorkloadId(string value) =>
        value.Contains('.') || value.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Mono.Options toggle-off: <c>--default-</c> → hidden clear flag <c>--clear-default</c>.
    /// Also rewrites trailing <c>-</c> on other bool options to <c>--opt false</c> form where useful.
    /// </summary>
    public static string[] RewriteToggleOff(IEnumerable<string> tokens)
    {
        var result = new List<string>();
        foreach (var token in tokens)
        {
            if (token.Equals("--default-", StringComparison.OrdinalIgnoreCase) ||
                token.Equals("-default-", StringComparison.OrdinalIgnoreCase))
            {
                result.Add("--clear-default");
            }
            else if (token.Length > 3 && token.StartsWith("--") && token.EndsWith("-") && !token.EndsWith("--"))
            {
                // --flag- → --flag false (SCL bool options accept explicit false)
                result.Add(token[..^1]);
                result.Add("false");
            }
            else
                result.Add(token);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Normalizes single-dash long options (<c>-sku:ent</c>, <c>-first</c>) to double-dash form
    /// for System.CommandLine, and slash options (<c>/filter:…</c>) similarly.
    /// Short single-letter options (<c>-v</c>, <c>-?</c>) are left alone.
    /// </summary>
    public static string[] RewriteLegacyOptionPrefixes(IEnumerable<string> tokens)
    {
        var result = new List<string>();
        foreach (var token in tokens)
        {
            if (token.Length >= 3 && token[0] == '/' && char.IsLetter(token[1]))
            {
                // /help → --help, /filter:expr → --filter=expr
                var body = token[1..];
                var sep = body.IndexOfAny([':', '=']);
                if (sep >= 0)
                    result.Add("--" + body[..sep] + "=" + body[(sep + 1)..]);
                else
                    result.Add("--" + body);
            }
            else if (token.Length >= 3 && token[0] == '-' && token[1] != '-' && char.IsLetter(token[1]))
            {
                // -sku:ent → --sku=ent, -first → --first, but -v stays (single letter handled by length)
                // Multi-char after single dash
                var body = token[1..];
                // Could be -v:18.7 (short+value) — single letter before separator
                var sep = body.IndexOfAny([':', '=']);
                if (sep == 1)
                {
                    // short option with value: -v:18.7 — keep as-is (SCL understands -v:value)
                    result.Add(token);
                }
                else if (sep > 1)
                {
                    result.Add("--" + body[..sep] + "=" + body[(sep + 1)..]);
                }
                else if (body.Length > 1)
                {
                    result.Add("--" + body);
                }
                else
                    result.Add(token);
            }
            else
                result.Add(token);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Full rewrite pipeline for Visual Studio selection tokens shared by most commands.
    /// </summary>
    public static string[] RewriteVisualStudioSelection(IEnumerable<string> tokens)
    {
        var t = tokens.ToArray();
        t = RewriteLegacyOptionPrefixes(t);
        t = RewriteToggleOff(t);
        t = RewriteChannelShortcuts(t);
        t = RewriteSkuShortcuts(t);
        t = RewriteBoolShortcuts(t);
        t = RewriteFilterExpressions(t);
        return t;
    }

    /// <summary>
    /// Rewrite pipeline for run/where/kill-style commands that use <c>+/-</c> workload requires.
    /// </summary>
    public static string[] RewriteRequiresWorkloads(IEnumerable<string> tokens)
    {
        var t = RewriteVisualStudioSelection(tokens);
        return RewriteWorkloadAliases(t, "requires", "+|-");
    }

    /// <summary>
    /// Rewrite pipeline for install (only <c>+</c> → add).
    /// </summary>
    public static string[] RewriteInstallWorkloads(IEnumerable<string> tokens)
    {
        var t = RewriteVisualStudioSelection(tokens);
        return RewriteWorkloadAliases(t, "add", "+");
    }

    /// <summary>
    /// Rewrite pipeline for modify (<c>+</c> → add, <c>-</c> → remove).
    /// </summary>
    public static string[] RewriteModifyWorkloads(IEnumerable<string> tokens)
    {
        var t = RewriteVisualStudioSelection(tokens);
        // Process remove (-) first carefully, then add (+)
        t = RewriteWorkloadAliases(t, "remove", "-");
        t = RewriteWorkloadAliases(t, "add", "+");
        return t;
    }
}
