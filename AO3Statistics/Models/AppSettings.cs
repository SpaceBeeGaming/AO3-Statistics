using System.ComponentModel.DataAnnotations;
using System.Globalization;

using AO3Statistics.Enums;
using AO3Statistics.ValidationAttributes;

using Microsoft.Extensions.Options;

namespace AO3Statistics.Models;

public sealed class OutputOptions
{
    [Required]
    [EnumIsDefined<OutputFormats>]
    public required OutputFormats OutputFormat { get; init; }

    [Required]
    [DirectoryExists]
    public required string FolderPath { get; init; }

    [Required]
    public required CultureInfo OutputCulture { get; init; }
}

public sealed class UrlOptions
{
    [Required]
    [URIIsValid("www.archiveofourown.org")]
    public required Uri LoginUrl { get; init; }

    [Required]
    [URIIsValid("www.archiveofourown.org")]
    public required Uri LogOutUrl { get; init; }

    [Required]
    [URIIsValid("www.archiveofourown.org")]
    public required Uri StatsUrl { get; set; }
}

public sealed class UserOptions
{
    [Required]
    public required string Username { get; init; }

    public required string Password { get; set; }

    [Required]
    public required bool PasswordIsProtected { get; init; }

    [Required]
    public required bool PasswordIsFromCommandLine { get; init; }
}

public sealed class XPathOptions
{
    [Required]
    [XPathIsValid]
    public required string LoginFormAuthenticityTokenXPath { get; init; }

    [Required]
    [XPathIsValid]
    public required string LogoutFormAuthenticityTokenXPath { get; init; }

    [Required]
    [XPathIsValid]
    public required string UserStatisticsXPath { get; init; }

    [Required]
    [XPathIsValid]
    public required string WorkStatisticsXPath { get; init; }
}

[OptionsValidator]
public partial class ValidateOutputOptions : IValidateOptions<OutputOptions> { }

[OptionsValidator]
public partial class ValidateUrlOptions : IValidateOptions<UrlOptions> { }

[OptionsValidator]
public partial class ValidateUserOptions : IValidateOptions<UserOptions> { }

[OptionsValidator]
public partial class ValidateXPathOptions : IValidateOptions<XPathOptions> { }