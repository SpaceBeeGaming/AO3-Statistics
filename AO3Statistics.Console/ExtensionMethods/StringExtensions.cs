using System.Diagnostics.CodeAnalysis;

namespace AO3Statistics.ConsoleApp.ExtensionMethods;

/// <summary>
/// Extension methods for strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
    /// </summary>
    /// <param name="value"><inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/></param>
    /// <returns><inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/></returns>
    public static bool IsNullOrWhitespace([NotNullWhen(false)] this string? value) => String.IsNullOrWhiteSpace(value);

    /// <summary>
    /// <inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/>
    /// </summary>
    /// <param name="value"><inheritdoc cref="string.IsNullOrWhiteSpace(string?)"/></param>
    /// <returns>The inverse of: <see cref="string.IsNullOrWhiteSpace(string?)"/>.</returns>
    public static bool IsNotNullOrWhitespace([NotNullWhen(true)] this string? value) => !String.IsNullOrWhiteSpace(value);

}
