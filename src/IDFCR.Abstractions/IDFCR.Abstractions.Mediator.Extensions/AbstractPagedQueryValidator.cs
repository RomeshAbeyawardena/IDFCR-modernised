using FluentValidation;
using IDFCR.Abstractions.Results;

namespace IDFCR.Abstractions.Mediator.Extensions;

/// <summary>
/// Represents an abstract validator for paged queries, providing common validation rules for page size and page index. This class can be extended to create specific validators for different types of paged queries, allowing for consistent validation logic across various query implementations. The optional maximumPageSize parameter allows for enforcing an upper limit on the page size if specified.
/// </summary>
/// <typeparam name="T">The type of the paged query being validated.</typeparam>
public abstract class AbstractPagedQueryValidator<T> : AbstractValidator<T>
    where T : IPagedQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractPagedQueryValidator{T}"/> class with optional maximum page size validation. This constructor sets up validation rules for the PageSize and PageIndex properties of the paged query, ensuring that they meet the specified criteria. If a maximumPageSize is provided, it also enforces that the PageSize does not exceed this limit.
    /// </summary>
    /// <param name="maximumPageSize">The optional maximum page size.</param>
    protected AbstractPagedQueryValidator(int? maximumPageSize = null)
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .When(x => x.PageSize.HasValue);

        RuleFor(x => x.PageIndex)
            .Null()
            .When(x => !x.PageSize.HasValue)
            .WithMessage("A page index cannot be specified without a page size.");

        if (maximumPageSize.HasValue)
        {
            RuleFor(x => x.PageSize)
                .LessThanOrEqualTo(maximumPageSize.Value)
                .When(x => x.PageSize.HasValue);
        }
    }
}
