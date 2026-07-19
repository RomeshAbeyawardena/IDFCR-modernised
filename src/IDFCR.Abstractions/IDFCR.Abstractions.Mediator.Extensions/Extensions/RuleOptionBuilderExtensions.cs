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
    /// Validates that the property of type <typeparamref name="TProperty"/> implements <see cref="IIdentifiable"/> and that its ID is a valid GUID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being validated.</typeparam>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    /// <param name="builder">The rule builder.</param>
    /// <returns>The rule builder options.</returns>
    public static IRuleBuilderOptions<TEntity,TProperty> IsOfGuid<TEntity, TProperty>(IRuleBuilderInitial<TEntity, TProperty> builder)
        where TProperty : IIdentifiable
    {
        return builder
            .Must(x => x.Id.IsOfGuid(out _))
            .WithMessage("The provided ID for {propertyName} is not a valid GUID.");
    }
}
