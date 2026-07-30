# Testing

IDFCR provides a test utilities package and follows conventional .NET testing patterns throughout. This page covers how to test handlers, results, repositories, and interceptors.

**Package:** `IDFCR.TestUtilities`  
**Test frameworks used in the IDFCR codebase:** NUnit 4.6.1, Moq 4.20.72, MELT 1.1.0, EF Core InMemory

---

## IDFCR.TestUtilities

`IDFCR.TestUtilities` provides reusable helpers that you can reference from your test projects. The package includes:

- In-memory stream doubles for CLI command testing.
- Shared test infrastructure primitives.
- Utility base classes for reducing test setup boilerplate.

Add it to your test project:

```xml
<PackageReference Include="IDFCR.TestUtilities" Version="2.0.7.1" />
```

---

## Testing a handler

Handlers are plain classes with a `Handle` method. You can test them directly without MediatR by constructing them with mocked or real dependencies.

```csharp
[Test]
public async Task CreateOrderCommand_Returns_Success_When_Reference_Is_Valid()
{
    // Arrange
    var repository = new Mock<IOrderRepository>();
    repository
        .Setup(r => r.UpsertAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(UnitResult.FromResult(Guid.NewGuid(), UnitAction.Add));

    var handler = new CreateOrderCommandHandler(repository.Object);
    var command = new CreateOrderCommand("REF-001");

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Action, Is.EqualTo(UnitAction.Add));
    Assert.That(result.HasValue, Is.True);
}

[Test]
public async Task CreateOrderCommand_Returns_ValidationError_When_Reference_Is_Empty()
{
    var handler = new CreateOrderCommandHandler(Mock.Of<IOrderRepository>());

    var result = await handler.Handle(new CreateOrderCommand(""), CancellationToken.None);

    Assert.That(result.IsSuccess, Is.False);
    Assert.That(result.FailureReason, Is.EqualTo(FailureReason.ValidationError));
}
```

---

## Testing with in-memory EF Core

For integration-style tests that exercise repositories, use `Microsoft.EntityFrameworkCore.InMemory`:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
```

```csharp
[Test]
public async Task OrderRepository_FindAsync_Returns_NotFound_For_Unknown_Id()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    await using var db = new AppDbContext(options);
    var repository = new OrderRepository(db, Mock.Of<IEntityInterceptorFactory>());

    var result = await repository.FindAsync(Guid.NewGuid(), CancellationToken.None);

    Assert.That(result.IsSuccess, Is.False);
    Assert.That(result.FailureReason, Is.EqualTo(FailureReason.NotFound));
}
```

---

## Testing validation

Test your FluentValidation validators independently:

```csharp
[Test]
public async Task CreateOrderCommandValidator_Fails_When_Reference_Is_Empty()
{
    var validator = new CreateOrderCommandValidator();

    var result = await validator.ValidateAsync(new CreateOrderCommand(""));

    Assert.That(result.IsValid, Is.False);
    Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(
        e => e.PropertyName == nameof(CreateOrderCommand.Reference)));
}
```

---

## Testing the full pipeline (MediatR integration)

For tests that exercise the validation pipeline and exception pipeline together, build a `ServiceProvider` with MediatR registered:

```csharp
[Test]
public async Task Pipeline_Returns_ValidationError_Result_When_Validator_Fails()
{
    var services = new ServiceCollection();
    services
        .ConfigureExceptionBehaviourManager(b => b.SetFluentValidationBehaviours())
        .AddMediatorServicesAndPipelines(
            configuration: null,
            configureOptions: o => o.UseFluentValidation(),
            assemblies: typeof(CreateOrderCommandHandler).Assembly);

    await using var provider = services.BuildServiceProvider();
    var mediator = provider.GetRequiredService<IMediator>();

    var result = await mediator.Send(new CreateOrderCommand(""), CancellationToken.None);

    Assert.That(result.IsSuccess, Is.False);
    Assert.That(result.FailureReason, Is.EqualTo(FailureReason.ValidationError));
}
```

---

## Testing interceptors

Test interceptors by constructing the factory and calling `InterceptAsync` with a test context:

```csharp
[Test]
public async Task AuditCreatedTimestampInterceptor_Sets_CreatedAt_On_Insert()
{
    var timeProvider = new ManualTimeProvider();
    timeProvider.SetCurrentTime(DateTimeOffset.UtcNow);

    var interceptor = new AuditCreatedTimestampEntityInterceptor(timeProvider);
    var entity = new Order(); // implements IAuditCreatedTimestamp

    var context = new TestEntityInterceptContext(
        entity,
        EntityContextBehavior.Insert,
        EntityContextBehaviorStage.After);

    await interceptor.InterceptAsync(context, CancellationToken.None);

    Assert.That(entity.CreatedAt, Is.EqualTo(timeProvider.GetUtcNow()));
}
```

---

## Testing CLI commands

CLI commands are testable because `IManagedStream` is an interface:

```csharp
[Test]
public async Task CreateOrderCliCommand_Writes_Success_Message()
{
    var mediator = new Mock<IMediator>();
    mediator
        .Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(UnitResult.FromResult(new OrderDto(Guid.NewGuid(), "REF-001"), UnitAction.Add));

    var command = new CreateOrderCliCommand(mediator.Object);
    var output = new StringWriteableStream();  // from IDFCR.TestUtilities or a custom double

    var parameters = new ArgumentParameters { ["reference"] = "REF-001" };
    var returnResult = await command.ExecuteAsync(parameters, output, CancellationToken.None);

    Assert.That(returnResult, Is.EqualTo(ReturnResult.Success));
    Assert.That(output.ToString(), Contains.Substring("Created order"));
}
```

---

## Testing result contracts

Result behaviour is covered by the in-repository tests in `IDFCR.Abstractions.Results.Tests`. For your own code, test the shape of the results your handlers return rather than the internal mechanics of `UnitResult`:

```csharp
// Good — tests handler intent
Assert.That(result.IsSuccess, Is.True);
Assert.That(result.Action, Is.EqualTo(UnitAction.Add));
Assert.That(result.Result!.Reference, Is.EqualTo("REF-001"));

// Avoid — tests framework internals
Assert.That(result, Is.InstanceOf<DefaultUnitResult<OrderDto>>());
```

---

## Running tests

```bash
# Run all tests
dotnet test IDFCR.slnx

# Run a specific test project
dotnet test src/IDFCR.Abstractions/Tests/IDFCR.Abstractions.Results.Tests

# Run with coverage
dotnet test IDFCR.slnx --collect:"XPlat Code Coverage"
```

---

## Further reading

- [Getting started](getting-started.md) — DI registration patterns that tests can replicate.
- [Handlers and pipelines](handlers-and-pipelines.md) — pipeline behaviour to test.
- [Validation](validation.md) — testing validators independently.
