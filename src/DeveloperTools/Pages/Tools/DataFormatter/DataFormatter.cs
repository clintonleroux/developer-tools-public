namespace DeveloperTools.Pages.Tools.DataFormatter;

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using YamlDotNet.RepresentationModel;

public class DataFormatter
{
    private static readonly Regex SqlKeywordsRegex = new("(select|from|where|group by|order by|having|inner join|left join|right join|join|union|and|or|LIKE)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex SqlKeywordsWithWhitespaceRegex = new("\\s+(SELECT|FROM|WHERE|GROUP BY|ORDER BY|HAVING|INNER JOIN|LEFT JOIN|RIGHT JOIN|JOIN|UNION|AND|OR|LIKE)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex StackTraceRegex = new("\\s+at\\s", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string? FormatData(string data, FormatType formatType, out string? errorMessage)
    {
        return formatType switch
        {
            FormatType.Sql => this.FormatSql(data, out errorMessage),
            FormatType.Xml => this.FormatXml(data, out errorMessage),
            FormatType.Yaml => this.FormatYaml(data, out errorMessage),
            FormatType.Text => this.FormatText(data, out errorMessage),
            FormatType.StackTrace => this.FormatStackTrace(data, out errorMessage),
            _ => this.ThrowUnknownFormat(out errorMessage),
        };
    }

    public string? FormatJson(string json, out string? errorMessage, bool sortKeys = false, bool minify = false, int indentSize = 2)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = !minify,
                IndentSize = indentSize
            };
            using var document = JsonDocument.Parse(json);
            errorMessage = null;

            return sortKeys ? JsonSerializer.Serialize(SortElement(document.RootElement), options) : JsonSerializer.Serialize(document.RootElement, options);
        }
        catch (JsonException ex)
        {
            errorMessage = $"Invalid JSON at line {ex.LineNumber}";
            return null;
        }
    }

    public string? FormatStackTrace(string stackTrace, out string? errorMessage)
    {
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(stackTrace))
        {
            return stackTrace;
        }

        var cleaned = stackTrace.Replace("\r", string.Empty).Replace("\n", string.Empty);
        var formatted = StackTraceRegex.Replace(cleaned, "\nat ");
        return formatted.TrimStart();
    }

    public string? FormatText(string text, out string? errorMessage)
    {
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        var sentences = Regex.Split(text.Trim(), @"(?<=[.!?])\s+");
        var sb = new StringBuilder();
        foreach (var sentence in sentences)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                continue;
            }

            var trimmed = sentence.Trim();
            var capitalized = char.ToUpper(trimmed[0]) + trimmed[1..];
            sb.AppendLine(capitalized);
        }

        return sb.ToString().TrimEnd();
    }

    public string? FormatSql(string sql, out string? errorMessage)
    {
        errorMessage = null;
        if (string.IsNullOrWhiteSpace(sql))
        {
            return sql;
        }

        var uppercased = SqlKeywordsRegex.Replace(sql, m => m.Value.ToUpper());
        var formatted = SqlKeywordsWithWhitespaceRegex.Replace(uppercased, match => "\n" + match.Value);

        return formatted.Trim();
    }

    public string? FormatXml(string xml, out string? errorMessage)
    {
        try
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(xml);

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = false,
            };

            using var stringWriter = new StringWriter();
            using (var writer = XmlWriter.Create(stringWriter, settings))
            {
                doc.Save(writer);
            }
            errorMessage = null;
            return stringWriter.ToString().Trim();
        }
        catch (XmlException ex)
        {
            errorMessage = $"Invalid XML at line {ex.LineNumber}, position {ex.LinePosition}";
            return null;
        }
    }

    public string? FormatYaml(string yaml, out string? errorMessage)
    {
        try
        {
            var yamlStream = new YamlStream();
            using (var reader = new StringReader(yaml))
            {
                yamlStream.Load(reader);
            }

            using var sw = new StringWriter();
            yamlStream.Save(sw, assignAnchors: false);

            errorMessage = null;
            return sw.ToString().Trim();
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            errorMessage = $"Invalid YAML: {ex.Message}";
            return null;
        }
    }

    private string? ThrowUnknownFormat(out string? errorMessage)
    {
        errorMessage = "Unknown format type";
        return null;
    }

    private static object? SortElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                {
                    var dict = new SortedDictionary<string, object?>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        dict[prop.Name] = SortElement(prop.Value);
                    }
                    return dict;
                }
            case JsonValueKind.Array:
                {
                    var list = new List<object?>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(SortElement(item));
                    }
                    return list;
                }
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                {
                    if (element.TryGetInt64(out var l)) { return l; }
                    if (element.TryGetDouble(out var d)) { return d; }
                    return element.GetRawText();
                }
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            default:
                return element.GetRawText();
        }
    }

    public enum FormatType
    {
        Json,
        Sql,
        Xml,
        Yaml,
        Text,
        StackTrace,
    }
}
