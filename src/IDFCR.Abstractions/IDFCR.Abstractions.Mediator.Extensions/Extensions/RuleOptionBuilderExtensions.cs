using FluentValidation;
using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results.Extensions;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Defines extension methods for FluentValidation's rule builder to add custom validation rules for identifiable entities.
/// </summary>
public static class RuleOptionBuilderExtensions
{
    /// <summary>
    /// Validates that the provided value is a valid GUID. This extension method can be used in FluentValidation rules to ensure that the value being validated can be parsed as a GUID. If the value is not a valid GUID, a validation error message will be generated indicating that the provided ID is not valid.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being validated.</typeparam>
    /// <param name="builder">The rule builder.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<TEntity, object?> MustBeOfGuid<TEntity>(
        this IRuleBuilderInitial<TEntity, object?> builder)
    {
        return builder
            .Must(x => x.IsOfGuid(out _))
            .WithMessage("The provided ID for {propertyName} is not a valid GUID.");
    }

    /// <summary>
    /// Validates that the provided value is a valid GUID, with an additional condition specified by the 'and' parameter. This extension method can be used in FluentValidation rules to ensure that the value being validated can be parsed as a GUID and meets the specified condition. If the value is not a valid GUID or does not meet the condition, a validation error message will be generated indicating that the provided ID is not valid.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being validated.</typeparam>
    /// <param name="builder">The rule builder.</param>
    /// <param name="and">The additional condition to be met.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<TEntity, object?> MustBeOfGuid<TEntity>(
        this IRuleBuilderInitial<TEntity, object?> builder, And and)
    {
        return builder
            .Must(x => x.IsOfGuid(and, out _))
            .WithMessage("The provided ID for {propertyName} is not a valid GUID.");
    }
}
