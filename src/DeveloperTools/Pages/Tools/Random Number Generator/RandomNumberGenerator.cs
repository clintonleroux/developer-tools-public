namespace DeveloperTools.Pages.Tools.RandomNumberGenerator;

public static class RandomNumberGenerator
{
    public static int? GenerateRandom(int min, int max, out string? errorMessage)
    {
        if (min > max)
        {
            errorMessage = "Min must be less than or equal to Max";
            return null;
        }

        errorMessage = null;

        if (min == max)
        {
            return min;
        }

        var value = (int)Random.Shared.NextInt64(min, (long)max + 1);
        return value;
    }

    public static int? GenerateRandomDigits(int digits, out string? errorMessage)
    {
        if (digits < 1)
        {
            errorMessage = "Digits must be greater than or equal to 1";
            return null;
        }

        if (digits > 10)
        {
            errorMessage = "Digits too large for Int32; maximum is 10";
            return null;
        }

        int min = digits == 1 ? 0 : (int)Math.Pow(10, digits - 1);
        long theoreticalMax = (long)Math.Pow(10, digits) - 1L;
        int max = theoreticalMax > int.MaxValue ? int.MaxValue : (int)theoreticalMax;

        return GenerateRandom(min, max, out errorMessage);
    }
}
