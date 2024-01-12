using System.ComponentModel.DataAnnotations;

using AO3Statistics.ConsoleApp.Enums;
using AO3Statistics.ConsoleApp.ValidationAttributes;

namespace AO3Statistics.ConsoleApp.Models;

public sealed class OutputOptions
{
    [Required]
    [EnumIsDefined<OutputFormats>]
    public required OutputFormats OutputFormat { get; init; }

    [Required]
    [DirectoryExists]
    public required string FolderPath { get; init; }
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

    [Required]
    public required string Password { get; init; }

    [Required]
    public required bool IsProtected { get; init; }
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
