﻿namespace AO3Statistics.ConsoleApp;

public sealed class UrlOptions
{
    public required Uri LoginUrl { get; init; }
    public required Uri LogOutUrl { get; init; }
    public required Uri StatsUrl { get; set; }
}

public sealed class UserOptions
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}

public sealed class XPathOptions
{
    public required string LoginFormAuthenticityTokenXPath { get; init; }
    public required string LogoutFormAuthenticityTokenXPath { get; init; }
    public required string UserStatisticsXPath { get; init; }
    public required string WorkStatisticsXPath { get; init; }
}
