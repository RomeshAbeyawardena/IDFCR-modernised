# Validation

IDFCR integrates with FluentValidation through a MediatR pipeline behavior. Validators run automatically before your handler and, on failure, produce a typed result rather than propagating a raw exception.

**Package:** `IDFCR.Abstractions.Mediator.Extensions` (pipeline), `FluentValidation` (validators — your project)

---

## How validation works

1. A `ValidationPipeline<TRequest, TResponse>` behavior runs before the handler.
2. It collects all `IValidator<TRequest>` implementations from the DI container.
3. If any validator reports errors, it throws `FluentValidation.ValidationException`.
4. `GenericDefaultExceptionPipeline` catches that exception and converts it to a result with `FailureReason.ValidationError`.
5. The handler never runs; the caller receives the failed result.

This means you do not need to write validation logic inside your handler. Keep the handler focused on business logic.

---

## Enabling the validation pipeline

```csharp
services
    .ConfigureExceptionBehaviourManager(b => b.SetFluentValidationBehaviours())
    .AddMediatorServicesAndPipelines(
        configuration,
        configureOptions: o => o.UseFluentValidation(),
        assemblies: typeof(Program).Assembly);
```

`SetFluentValidationBehaviours()` registers `ValidationException` → `FailureReason.ValidationError` with the exception behaviour manager so the pipeline knows how to map it.

---

## Writing a validator

Write a standard FluentValidation `AbstractValidator<T>` for your request type. Register it in the same assembly you pass to `AddMediatorServicesAndPipelines`; it is discovered automatically.

```csharp
using FluentValidation;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Reference)
            .NotEmpty()
            .WithMessage("Reference is required.")
            .MaximumLength(100)
            .WithMessage("Reference must not exceed 100 characters.");
    }
}
```

---

## Paged query validation

IDFCR provides `AbstractPagedQueryValidator<T>` as a base for validators on paged requests. It adds ready-made rules for `PageSize` and `PageIndex`:

- `PageSize` must be greater than zero (when provided).
- `PageIndex` must be null when `PageSize` is null.
- `PageIndex` must be greater than or equal to zero (when provided).
- Optionally enforces a maximum page size.

```csharp
public sealed class GetOrdersQueryValidator : AbstractPagedQueryValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator() : base(maximumPageSize: 100)
    {
        // Additional rules go here
        RuleFor(x => x.Status).NotEmpty().When(x => x.Status is not null);
    }
}
```

`T` must implement `IPagedQuery` (which `PagedUnitResultRequestBase<T>` already does).

---

## Validation failures in results

When validation fails, the caller receives an `IUnitResult<T>` where:

- `IsSuccess == false`
- `FailureReason == FailureReason.ValidationError`
- `Exception` is the `FluentValidation.ValidationException` (containing the individual `ValidationFailure` entries)

If you call `.AsHttp()` on the result, it maps to **422 Unprocessable Entity**.

---

## Validators with dependency injection

Validators can depend on services resolved from the DI container. Register them as transient services (or let the assembly scanning do it automatically).

```csharp
public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(IOrderRepository orders)
    {
        RuleFor(x => x.Reference)
            .MustAsync(async (reference, ct) =>
                !await orders.ExistsAsync(reference, ct))
            .WithMessage("An order with this reference already exists.");
    }
}
```

---

## Tips

- Keep validators focused on structural and business-rule validation. Do not perform side effects (writes, state changes) inside validators.
- Use `When(...)` and `Unless(...)` to conditionally apply rules rather than branching inside the validation method.
- For complex cross-field rules, write a custom `AbstractValidator` rather than embedding logic in the handler.
- If you need validation errors as a structured list (not the full exception), read `result.Exception` and cast it to `ValidationException` to access `.Errors`.

---

## Further reading

- [Handlers and pipelines](handlers-and-pipelines.md) — the full pipeline execution model.
- [Results and flow](results-and-flow.md) — how `FailureReason` maps to HTTP status codes.
