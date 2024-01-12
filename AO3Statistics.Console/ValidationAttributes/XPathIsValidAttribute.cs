using System.ComponentModel.DataAnnotations;
using System.Xml.XPath;

namespace AO3Statistics.ConsoleApp.ValidationAttributes;
public sealed class XPathIsValidAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        List<string> memberNameList = [validationContext.MemberName ?? string.Empty];

        if (value is string xPath)
        {
            try
            {
                XPathExpression.Compile(xPath);
                return ValidationResult.Success;
            }
            catch (XPathException)
            {
                return new ValidationResult($"{xPath} is malformed.", memberNameList);
            }
        }

        return new ValidationResult($"{validationContext.DisplayName} is not of type {typeof(string)}.", memberNameList);

    }
}
