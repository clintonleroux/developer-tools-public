using DeveloperTools.Shared;

namespace DeveloperTools.Tests;

public class NumberedTextAreaTests
{
    [Fact]
    public void CountLines_WithEmptyText_ReturnsMinimumThreeRows()
    {
        // This test verifies that empty text results in minimum 3 rows for better usability
        var component = new NumberedTextArea();
        var countLinesMethod = typeof(NumberedTextArea).GetMethod("CountLines", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        var result = (int)countLinesMethod!.Invoke(component, new object[] { "" });
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void CountLines_WithSingleLine_ReturnsMinimumThreeRows()
    {
        // Single line should still return 3 rows minimum
        var component = new NumberedTextArea();
        var countLinesMethod = typeof(NumberedTextArea).GetMethod("CountLines", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        var result = (int)countLinesMethod!.Invoke(component, new object[] { "Single line without breaks" });
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void CountLines_WithMultipleLines_ReturnsActualLineCount()
    {
        // Multiple lines should return actual count when greater than minimum
        var component = new NumberedTextArea();
        var countLinesMethod = typeof(NumberedTextArea).GetMethod("CountLines", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        var result = (int)countLinesMethod!.Invoke(component, new object[] { "Line 1\nLine 2\nLine 3\nLine 4\nLine 5" });
        
        Assert.Equal(5, result);
    }
}