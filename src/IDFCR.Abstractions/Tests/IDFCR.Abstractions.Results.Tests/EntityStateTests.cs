using IDFCR.Abstractions.Metadata;
using IDFCR.Abstractions.Results.Extensions;
using NUnit.Framework;

namespace IDFCR.Abstractions.Results.Tests;

record TestEntity : IIdentifiable
{
    public object? Id { get; init; }
}

[TestFixture]
internal class EntityStateTests
{
    [Test]
    public void GetEntityState_WhenIdentifiableIsNull_ReturnsNewAndSuccessfulFalseState()
    {
        IIdentifiable? identifiable = null;

        var entityState = identifiable.GetEntityState(out var output);
        var typedResult = output.Result as IUnitResult<bool>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entityState, Is.EqualTo(EntityState.New));
            Assert.That(output.Id, Is.Null);
            Assert.That(output.Result.IsSuccess, Is.True);
            Assert.That(typedResult, Is.Not.Null);
            Assert.That(typedResult!.Result, Is.False);
        }
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void GetEntityState_WhenIdIsNullOrWhitespace_ReturnsNewAndSuccessfulFalseState(string? id)
    {
        TestEntity testEntity = new()
        {
            Id = id
        };

        var entityState = testEntity.GetEntityState(out var output);
        var typedResult = output.Result as IUnitResult<bool>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entityState, Is.EqualTo(EntityState.New));
            Assert.That(output.Id, Is.Null);
            Assert.That(output.Result.IsSuccess, Is.True);
            Assert.That(typedResult, Is.Not.Null);
            Assert.That(typedResult!.Result, Is.False);
        }
    }

    [Test]
    public void GetEntityState_WhenIdIsValidGuidString_ReturnsUpdateAndParsedGuid()
    {
        var testId = Guid.NewGuid();
        TestEntity testEntity = new()
        {
            Id = testId.ToString()
        };

        var entityState = testEntity.GetEntityState(out var output);
        var typedResult = output.Result as IUnitResult<bool>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entityState, Is.EqualTo(EntityState.Update));
            Assert.That(output.Id, Is.EqualTo(testId));
            Assert.That(output.Result.IsSuccess, Is.True);
            Assert.That(typedResult, Is.Not.Null);
            Assert.That(typedResult!.Result, Is.True);
        }
    }

    [Test]
    public void GetEntityState_WhenIdIsGuidEmptyString_ReturnsUpdate()
    {
        TestEntity testEntity = new()
        {
            Id = Guid.Empty.ToString()
        };

        var entityState = testEntity.GetEntityState(out var output);
        var typedResult = output.Result as IUnitResult<bool>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entityState, Is.EqualTo(EntityState.Update));
            Assert.That(output.Id, Is.EqualTo(Guid.Empty));
            Assert.That(output.Result.IsSuccess, Is.True);
            Assert.That(typedResult, Is.Not.Null);
            Assert.That(typedResult!.Result, Is.True);
        }
    }

    [Test]
    public void GetEntityState_WhenIdIsGuidObject_ReturnsUpdateAndSameGuid()
    {
        var testId = Guid.NewGuid();
        TestEntity testEntity = new()
        {
            Id = testId
        };

        var entityState = testEntity.GetEntityState(out var output);
        var typedResult = output.Result as IUnitResult<bool>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entityState, Is.EqualTo(EntityState.Update));
            Assert.That(output.Id, Is.EqualTo(testId));
            Assert.That(output.Result.IsSuccess, Is.True);
            Assert.That(typedResult, Is.Not.Null);
            Assert.That(typedResult!.Result, Is.True);
        }
    }

    [Test]
    public void GetEntityState_WhenIdIsGuidEmpty_ReturnsUpdate()
    {
        TestEntity testEntity = new()
        {
            Id = Guid.Empty
        };

        var entityState = testEntity.GetEntityState(out var output);
        var typedResult = output.Result as IUnitResult<bool>;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entityState, Is.EqualTo(EntityState.Update));
            Assert.That(output.Id, Is.EqualTo(Guid.Empty));
            Assert.That(output.Result.IsSuccess, Is.True);
            Assert.That(typedResult, Is.Not.Null);
            Assert.That(typedResult!.Result, Is.True);
        }
    }

    [TestCase("not-a-guid")]
    [TestCase(123)]
    public void GetEntityState_WhenIdIsNonEmptyButInvalid_ReturnsInvalidAndFailedResult(object id)
    {
        TestEntity testEntity = new()
        {
            Id = id
        };

        var entityState = testEntity.GetEntityState(out var output);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(entityState, Is.EqualTo(EntityState.Invalid));
            Assert.That(output.Id, Is.Null);
            Assert.That(output.Result.IsSuccess, Is.False);
            Assert.That(output.Result.Exception, Is.TypeOf<ArgumentException>());
            Assert.That(output.Result.Exception!.Message, Is.EqualTo("Invalid Id."));
        }
    }
}
