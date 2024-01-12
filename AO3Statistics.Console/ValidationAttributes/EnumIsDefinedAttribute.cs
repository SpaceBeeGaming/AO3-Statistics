using System.ComponentModel.DataAnnotations;

namespace AO3Statistics.ConsoleApp.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class EnumIsDefinedAttribute<TEnum> : ValidationAttribute where TEnum : struct, Enum
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        List<string> memberNameList = [validationContext.MemberName ?? string.Empty];

        return value is TEnum enumValue
            ? Enum.IsDefined(enumValue)
                ? ValidationResult.Success
                : new ValidationResult($"{enumValue} is not defined.", memberNameList)
            : new ValidationResult($"{validationContext.DisplayName} is not of type {typeof(TEnum)}.", memberNameList);
    }
}
