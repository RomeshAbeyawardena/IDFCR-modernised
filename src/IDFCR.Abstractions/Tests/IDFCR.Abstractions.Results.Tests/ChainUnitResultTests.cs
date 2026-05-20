using IDFCR.Abstractions.Results.Extensions;
using NUnit.Framework;

namespace IDFCR.Abstractions.Results.Tests;

[TestFixture]
internal class ChainUnitResultTests
{
    [Test]
    public void Chain_BothSuccess_ShouldSucceed()
    {
        var first = UnitResult.Success(UnitAction.Add);
        var second = UnitResult.Success(UnitAction.Update);

        var chained = first.Chain(second);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.True);
            Assert.That(chained.Current, Is.SameAs(first));
            Assert.That(chained.Last, Is.SameAs(second));
            Assert.That(chained.Action, Is.EqualTo(first.Action));
        });
    }

    [Test]
    public void Chain_FirstFails_ShouldFail()
    {
        var ex = new InvalidOperationException("first failed");
        var first = UnitResult.Failed(ex, UnitAction.Update, FailureReason.ValidationError);
        var second = UnitResult.Success(UnitAction.Add);

        var chained = first.Chain(second);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.False);
            Assert.That(chained.Exception, Is.SameAs(ex));
            Assert.That(chained.FailureReason, Is.EqualTo(FailureReason.ValidationError));
            Assert.That(chained.Current, Is.SameAs(first));
            Assert.That(chained.Last, Is.SameAs(second));
        });
    }

    [Test]
    public void Chain_SecondFails_ShouldFail()
    {
        var ex = new InvalidOperationException("second failed");
        var first = UnitResult.Success(UnitAction.Update);
        var second = UnitResult.Failed(ex, UnitAction.Add, FailureReason.Conflict);

        var chained = first.Chain(second);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.False);
            Assert.That(chained.Exception, Is.SameAs(ex));
            Assert.That(chained.FailureReason, Is.EqualTo(FailureReason.Conflict));
            Assert.That(chained.Current, Is.SameAs(first));
            Assert.That(chained.Last, Is.SameAs(second));
        });
    }

    [Test]
    public void Chain_SecondFails_SetAsFailWhenAnyUnitsFailFalse_ShouldRemainSuccess()
    {
        var first = UnitResult.Success(UnitAction.Get);
        var ex = new InvalidOperationException("second failed");
        var second = UnitResult.Failed(ex, UnitAction.Update, FailureReason.Conflict);

        var chained = first.Chain(second, setAsFailWhenAnyUnitsFail: false);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.True);
            Assert.That(chained.Exception, Is.Null);
            Assert.That(chained.FailureReason, Is.Null);
            Assert.That(chained.Current, Is.SameAs(first));
            Assert.That(chained.Last, Is.SameAs(second));
        });
    }

    [Test]
    public void Chain_FirstFails_SetAsFailWhenAnyUnitsFailFalse_ShouldStillFail()
    {
        var ex = new InvalidOperationException("first failed");
        var first = UnitResult.Failed(ex, UnitAction.Delete, FailureReason.ValidationError);
        var second = UnitResult.Success(UnitAction.Update);

        var chained = first.Chain(second, setAsFailWhenAnyUnitsFail: false);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.False);
            Assert.That(chained.Exception, Is.SameAs(ex));
            Assert.That(chained.FailureReason, Is.EqualTo(FailureReason.ValidationError));
            Assert.That(chained.Current, Is.SameAs(first));
            Assert.That(chained.Last, Is.SameAs(second));
        });
    }

    [Test]
    public void Chain_Typed_SecondFails_SetAsFailWhenAnyUnitsFailFalse_ShouldRemainSuccessAndKeepCurrentTypedValue()
    {
        var first = UnitResult.FromResult("value", UnitAction.Get, isSuccess: true);
        var ex = new InvalidOperationException("second failed");
        var second = UnitResult.Failed(ex, UnitAction.Update, FailureReason.InternalError);

        var chained = first.Chain(second, setAsFailWhenAnyUnitsFail: false);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.True);
            Assert.That(chained.Current.Result, Is.EqualTo("value"));
            Assert.That(chained.Exception, Is.Null);
            Assert.That(chained.FailureReason, Is.Null);
            Assert.That(chained.Current, Is.SameAs(first));
            Assert.That(chained.Last, Is.SameAs(second));
        });
    }

    [Test]
    public void Chain_Typed_FirstSuccessSecondFails_ShouldFailAndKeepCurrentTypedValue()
    {
        var first = UnitResult.FromResult("value", UnitAction.Get, isSuccess: true);
        var second = UnitResult.Failed(new InvalidOperationException("second failed"), UnitAction.Update, FailureReason.InternalError);

        var chained = first.Chain(second);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.False);
            Assert.That(chained.Current.Result, Is.EqualTo("value"));
            Assert.That(chained.Last, Is.SameAs(second));
            Assert.That(chained.Current, Is.SameAs(first));
        });

        IEnumerable<IUnitResult> chainedList = [.. chained.Enumerate()];
        Assert.That(chainedList, Has.Exactly(2).Items);
    }

    [Test]
    public void Chain_MultipleResults_ShouldCreateChainInOrder()
    {
        var first = UnitResult.Success(UnitAction.Add);
        var second = UnitResult.Success(UnitAction.Update);
        var third = UnitResult.Success(UnitAction.Delete);

        var chained = first.Chain([second, third]);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.True);
            Assert.That(chained.Current, Is.AssignableTo<IChainedUnitResult>());

            var results = chained.Enumerate();
            Assert.That(results, Is.EqualTo([first, second, third]));
        });

        var nested = (IChainedUnitResult)chained.Current;
        Assert.Multiple(() =>
        {
            Assert.That(nested.Current, Is.SameAs(first));
            Assert.That(nested.Last, Is.SameAs(second));
        });
    }

    [Test]
    public void Chain_MultipleResults_WithSingleItem_ShouldCreateTwoItemChainInOrder()
    {
        var first = UnitResult.Success(UnitAction.Add);
        var second = UnitResult.Success(UnitAction.Update);

        var chained = first.Chain([second]);

        Assert.Multiple(() =>
        {
            Assert.That(chained.Current, Is.SameAs(first));
            Assert.That(chained.Last, Is.SameAs(second));
            Assert.That(chained.Enumerate(), Is.EqualTo(new IUnitResult[] { first, second }));
        });
    }

    [Test]
    public void Chain_MultipleResults_WhenLaterResultFails_ShouldUseFirstFailure()
    {
        var first = UnitResult.Success(UnitAction.Get);
        var secondException = new InvalidOperationException("second failed");
        var second = UnitResult.Failed(secondException, UnitAction.Update, FailureReason.ValidationError);
        var third = UnitResult.Success(UnitAction.Delete);

        var chained = first.Chain([second, third]);

        Assert.Multiple(() =>
        {
            Assert.That(chained.IsSuccess, Is.False);
            Assert.That(chained.Exception, Is.SameAs(secondException));
            Assert.That(chained.FailureReason, Is.EqualTo(FailureReason.ValidationError));
            Assert.That(chained.GetFirstFailure(), Is.SameAs(second));
            Assert.That(chained.Enumerate(), Is.EqualTo(new IUnitResult[] { first, second, third }));
        });
    }

    [Test]
    public void Chain_MultipleResults_EmptyEnumerable_ShouldThrow()
    {
        var first = UnitResult.Success(UnitAction.Add);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => first.Chain([]));

        Assert.That(exception!.ParamName, Is.EqualTo("results"));
    }
}
