using DeveloperTools.Pages.Tools.RandomNumberGenerator;

namespace DeveloperTools.Tests;

public class RandomNumberGeneratorTests
{
    private const int TestIterations = 1000;

    [Fact]
    public void GenerateRandom_ReturnsNull_WhenMinGreaterThanMax()
    {
        var result = RandomNumberGenerator.GenerateRandom(10, 5, out var error);
        Assert.Null(result);
        Assert.Equal("Min must be less than or equal to Max", error);
    }

    [Fact]
    public void GenerateRandom_ReturnsMin_WhenMinEqualsMax()
    {
        var result = RandomNumberGenerator.GenerateRandom(7, 7, out var error);
        Assert.Equal(7, result);
        Assert.Null(error);
    }

    [Fact]
    public void GenerateRandom_WithinInclusiveRange()
    {
        var min = -3;
        var max = 3;

        for (int i = 0; i < TestIterations; i++)
        {
            var value = RandomNumberGenerator.GenerateRandom(min, max, out var error);
            Assert.Null(error);
            Assert.NotNull(value);
            Assert.InRange(value!.Value, min, max);
        }
    }
}
