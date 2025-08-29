using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DeveloperTools.Pages.Tools.DataDifference
{
    public static class DataDifference
    {
        public enum DiffOp { Equal, Insert, Delete }
        public enum DiffKind { Equal, Added, Removed }
        public readonly record struct DiffLine(string Text, DiffKind Kind);

        public static (string result, List<DiffLine> lines) GetTextDiff(
            string left,
            string right,
            bool ignoreCase,
            bool ignoreWhitespace,
            bool trimLines)
        {
            var leftLinesRaw = (left ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var rightLinesRaw = (right ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            string Normalize(string s)
            {
                if (s == null) return string.Empty;
                var v = s;
                if (trimLines)
                {
                    v = v.Trim();
                }
                if (ignoreWhitespace)
                {
                    v = Regex.Replace(v, @"\s+", string.Empty);
                }
                if (ignoreCase) v = v.ToLowerInvariant();
                return v;
            }

            var leftNorm = leftLinesRaw.Select(Normalize).ToArray();
            var rightNorm = rightLinesRaw.Select(Normalize).ToArray();

            var ops = GetLcsOps(leftNorm, rightNorm);
            var sb = new StringBuilder();
            var lines = new List<DiffLine>();
            int i = 0, j = 0;
            foreach (var op in ops)
            {
                switch (op)
                {
                    case DiffOp.Equal:
                        i++; j++;
                        break;
                    case DiffOp.Delete:
                        if (i < leftLinesRaw.Length)
                        {
                            sb.AppendLine($"- {leftLinesRaw[i]}");
                            lines.Add(new DiffLine($"- {leftLinesRaw[i]}", DiffKind.Removed));
                        }
                        i++;
                        break;
                    case DiffOp.Insert:
                        if (j < rightLinesRaw.Length)
                        {
                            sb.AppendLine($"+ {rightLinesRaw[j]}");
                            lines.Add(new DiffLine($"+ {rightLinesRaw[j]}", DiffKind.Added));
                        }
                        j++;
                        break;
                }
            }
            return (sb.ToString().TrimEnd(), lines);
        }

        public static (string result, List<DiffLine> lines) GetJsonDiff(
            JsonElement left,
            JsonElement right,
            bool pretty,
            bool sortProps)
        {
            var sb = new StringBuilder();
            var lines = new List<DiffLine>();

            void Add(string prefix, string path, string value, DiffKind kind)
            {
                var text = $"{prefix} {path}: {value}";
                sb.AppendLine(text);
                lines.Add(new DiffLine(text, kind));
            }

            string ValToString(JsonElement el)
            {
                try
                {
                    if (pretty)
                    {
                        using var doc = JsonDocument.Parse(el.GetRawText());
                        return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
                    }
                    return el.GetRawText();
                }
                catch
                {
                    return el.ToString();
                }
            }

            void Diff(JsonElement l, JsonElement r, string path)
            {
                if (l.ValueKind == JsonValueKind.Object && r.ValueKind == JsonValueKind.Object)
                {
                    IEnumerable<JsonProperty> EnumProps(JsonElement obj)
                    {
                        var props = obj.EnumerateObject();
                        return sortProps ? props.OrderBy(p => p.Name, StringComparer.Ordinal) : props;
                    }

                    var lDict = EnumProps(l).ToDictionary(p => p.Name, p => p.Value);
                    var rDict = EnumProps(r).ToDictionary(p => p.Name, p => p.Value);
                    var keys = new SortedSet<string>(lDict.Keys.Concat(rDict.Keys), StringComparer.Ordinal);
                    foreach (var key in keys)
                    {
                        var sub = string.IsNullOrEmpty(path) ? key : $"{path}.{key}";
                        var hasL = lDict.TryGetValue(key, out var lv);
                        var hasR = rDict.TryGetValue(key, out var rv);
                        if (hasL && hasR)
                        {
                            if (!JsonElementDeepEquals(lv, rv))
                            {
                                Diff(lv, rv, sub);
                            }
                        }
                        else if (hasL)
                        {
                            Add("-", sub, ValToString(lv), DiffKind.Removed);
                        }
                        else
                        {
                            Add("+", sub, ValToString(rv), DiffKind.Added);
                        }
                    }
                }
                else if (l.ValueKind == JsonValueKind.Array && r.ValueKind == JsonValueKind.Array)
                {
                    var la = l.EnumerateArray().ToArray();
                    var ra = r.EnumerateArray().ToArray();
                    var max = Math.Max(la.Length, ra.Length);
                    for (int idx = 0; idx < max; idx++)
                    {
                        var sub = string.IsNullOrEmpty(path) ? $"[{idx}]" : $"{path}[{idx}]";
                        var hasL = idx < la.Length;
                        var hasR = idx < ra.Length;
                        if (hasL && hasR)
                        {
                            if (!JsonElementDeepEquals(la[idx], ra[idx]))
                            {
                                Diff(la[idx], ra[idx], sub);
                            }
                        }
                        else if (hasL)
                        {
                            Add("-", sub, ValToString(la[idx]), DiffKind.Removed);
                        }
                        else
                        {
                            Add("+", sub, ValToString(ra[idx]), DiffKind.Added);
                        }
                    }
                }
                else if (!JsonElementDeepEquals(l, r))
                {
                    Add("-", path, ValToString(l), DiffKind.Removed);
                    Add("+", path, ValToString(r), DiffKind.Added);
                }
            }

            Diff(left, right, string.Empty);
            return (sb.ToString().TrimEnd(), lines);
        }

        private static bool JsonElementDeepEquals(JsonElement a, JsonElement b)
        {
            if (a.ValueKind != b.ValueKind) return false;
            switch (a.ValueKind)
            {
                case JsonValueKind.Object:
                    var aProps = a.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
                    var bProps = b.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
                    if (aProps.Count != bProps.Count) return false;
                    foreach (var key in aProps.Keys)
                    {
                        if (!bProps.ContainsKey(key) || !JsonElementDeepEquals(aProps[key], bProps[key])) return false;
                    }
                    return true;
                case JsonValueKind.Array:
                    var aArr = a.EnumerateArray().ToArray();
                    var bArr = b.EnumerateArray().ToArray();
                    if (aArr.Length != bArr.Length) return false;
                    for (int i = 0; i < aArr.Length; i++)
                    {
                        if (!JsonElementDeepEquals(aArr[i], bArr[i])) return false;
                    }
                    return true;
                default:
                    try { return a.GetRawText() == b.GetRawText(); } catch { return a.ToString() == b.ToString(); }
            }
        }

        private static List<DiffOp> GetLcsOps(string[] a, string[] b)
        {
            int n = a.Length, m = b.Length;
            var dp = new int[n + 1, m + 1];
            for (int i = n - 1; i >= 0; i--)
            {
                for (int j = m - 1; j >= 0; j--)
                {
                    if (a[i] == b[j]) dp[i, j] = dp[i + 1, j + 1] + 1;
                    else dp[i, j] = Math.Max(dp[i + 1, j], dp[i, j + 1]);
                }
            }
            var ops = new List<DiffOp>();
            int x = 0, y = 0;
            while (x < n && y < m)
            {
                if (a[x] == b[y]) { ops.Add(DiffOp.Equal); x++; y++; }
                else if (dp[x + 1, y] >= dp[x, y + 1]) { ops.Add(DiffOp.Delete); x++; }
                else { ops.Add(DiffOp.Insert); y++; }
            }
            while (x < n) { ops.Add(DiffOp.Delete); x++; }
            while (y < m) { ops.Add(DiffOp.Insert); y++; }
            return ops;
        }
    }
}
