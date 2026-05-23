using NUnit.Framework;

namespace IDFCR.Abstractions.Results.Tests;

[TestFixture]
internal class UnitResultTests
{
    [Test]
    public void Simple_SuccessPath()
    {
        var result = UnitResult.Success(UnitAction.Add);
        Assert.AreEqual(UnitAction.Add, result.Action);
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void ModifiedState_Path()
    {
        var result = UnitResult.Success(UnitAction.Get);
        Assert.That(result.IsSuccess, Is.True);
        //can't set a non-generic UnitResult's modified state because it has no actual state it is storing
        Assert.That(result.TrySetState(Guid.NewGuid()), Is.False);

        var id = Guid.NewGuid();
        var idResult = UnitResult.FromResult(id);
        //can set a generic unit result's modified state
        Assert.That(idResult.IsSuccess, Is.True);
        Assert.That(idResult.TrySetState(Guid.NewGuid()), Is.True);
        Assert.That(idResult.Result, Is.Not.EqualTo(id));
        Assert.That(idResult.OriginalState, Is.EqualTo(id));
    }
}
