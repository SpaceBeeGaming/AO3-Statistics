using System.Diagnostics.CodeAnalysis;

namespace StringExtensions;

public static class StringExtensions
{
    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? value) => String.IsNullOrWhiteSpace(value);

    public static bool IsNotNullOrWhitespace([NotNullWhen(true)] this string? value) => !String.IsNullOrWhiteSpace(value);

}
