using DeveloperTools.Pages.Tools.DataFormatter;

namespace DeveloperTools.Tests;

public class DataFormatterTests
{

    [Fact]
    public void FormatJson_ReturnsIndentedJson()
    {
        var formatter = new DataFormatter();
        string input = "{\"name\":\"Alice\"}";

        var result = formatter.FormatJson(input, out var error);

        Assert.Null(error);
        Assert.Equal("{\r\n  \"name\": \"Alice\"\r\n}", result);
    }

    [Fact]
    public void FormatJson_SortsKeysAlphabetically_WhenSortKeysIsTrue()
    {
        var formatter = new DataFormatter();
        string input = "{\"z\":1,\"a\":2,\"m\":3}";

        var result = formatter.FormatJson(input, out var error, true);

        Assert.Null(error);
        Assert.Equal("{\r\n  \"a\": 2,\r\n  \"m\": 3,\r\n  \"z\": 1\r\n}", result);
    }

    [Fact]
    public void FormatText_SplitsSentencesOnNewLines()
    {
        var formatter = new DataFormatter();
        string input = "hello world. how are you?";

        var result = formatter.FormatText(input, out var error);

        Assert.Null(error);
        Assert.Equal("Hello world.\r\nHow are you?", result);
    }

    [Fact]
    public void FormatStackTrace_PlacesEachCallOnNewLine()
    {
        var formatter = new DataFormatter();
        string input = "   at MethodA\r\n   at MethodB";

        var result = formatter.FormatStackTrace(input, out var error);

        Assert.Null(error);
        Assert.Equal("at MethodA\nat MethodB", result);
    }

    [Fact]
    public void FormatSql_UppercasesKeywordsAndAddsNewLines()
    {
        var formatter = new DataFormatter();
        string input = "select * from table where id=1 order by name";

        var result = formatter.FormatSql(input, out var error);

        Assert.Null(error);
        Assert.Equal("SELECT *\n FROM table\n WHERE id=1\n ORDER BY name", result);
    }
}

