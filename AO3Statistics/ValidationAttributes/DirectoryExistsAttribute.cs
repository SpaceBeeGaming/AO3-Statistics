using System.ComponentModel.DataAnnotations;

namespace AO3Statistics.ValidationAttributes;
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class DirectoryExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        List<string> memberNameList = [validationContext.MemberName ?? string.Empty];

        return value is string path
            ? Directory.Exists(path)
                ? ValidationResult.Success
                : new ValidationResult($"Path: '{Path.GetFullPath(path)}' doesn't exist", memberNameList)
            : new ValidationResult($"{validationContext.DisplayName} is not of type {typeof(string)}.", memberNameList);
    }
}
