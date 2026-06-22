using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public static class ValidationHelper
{
    public static async Task<IReadOnlyList<string>?> ValidateAsync<T>(
        IValidator<T> validator,
        T model,
        CancellationToken cancellationToken = default)
    {
        var result = await validator.ValidateAsync(model, cancellationToken);
        if (result.IsValid)
            return null;

        return result.Errors
            .Select(e => e.ErrorMessage)
            .ToList();
    }
}
