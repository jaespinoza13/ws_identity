
using Application.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Validations;
public class ValidaCanalAttribute : ValidationAttribute
{
    private readonly IParametersInMemory _memoryCache;
    public ValidaCanalAttribute(IParametersInMemory memory)
    {
        _memoryCache = memory;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var canal = _memoryCache.FindParametro(value!.ToString()!);
        
        if (String.IsNullOrEmpty(canal.str_valor_ini) || canal.str_valor_ini == value!.ToString())
        {
            return new ValidationResult("Canal inválido");
        }
        return ValidationResult.Success!;
    }
}