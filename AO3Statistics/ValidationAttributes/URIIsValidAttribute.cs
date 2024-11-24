using System.ComponentModel.DataAnnotations;

namespace AO3Statistics.ValidationAttributes;
internal sealed class URIIsValidAttribute(string host, string scheme = "https") : ValidationAttribute
{
    private readonly string host = host;
    private readonly string scheme = scheme;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        List<string> memberNameList = [validationContext.MemberName ?? string.Empty];

        return value is Uri uri
            ? uri.Host == host && uri.Scheme == scheme
                ? ValidationResult.Success
                : new ValidationResult($"{validationContext.DisplayName} has wrong scheme or host.", memberNameList)
            : new ValidationResult($"{validationContext.DisplayName} is not of type {typeof(Uri)}.", memberNameList);
    }
}
