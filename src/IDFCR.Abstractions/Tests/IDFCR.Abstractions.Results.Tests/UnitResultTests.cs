using NUnit.Framework;
using IDFCR.Abstractions.Results.Extensions;
using System.Data;

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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IsSuccess, Is.True);
            //can't set a non-generic UnitResult's modified state because it has no actual state it is storing
            Assert.That(result.TrySetState(Guid.NewGuid()), Is.False);
        }

        var id = Guid.NewGuid();
        var idResult = UnitResult.FromResult(id);
        using (Assert.EnterMultipleScope())
        {
            //can set a generic unit result's modified state
            Assert.That(idResult.IsSuccess, Is.True);
            Assert.That(idResult.TrySetState(Guid.NewGuid()), Is.True);
            Assert.That(idResult.Result, Is.Not.EqualTo(id));
            Assert.That(idResult.OriginalState, Is.EqualTo(id));
        }
    }

    record Foo(int Id, string Key, string Value, DateTime? LastUpdatedTimestampUtc = null);

    [Test]
    public void ModifiedState_UnitResultPath()
    {
        //find result

        var originalResult = new Foo(1, "bar", "mars");

        var findResult = UnitResult.FromResult(originalResult);

        //update signal
        var upsertResultSignal = UnitResult.Success(UnitAction.Update);
        var updatedDate = new DateTime(2026, 05, 25, 11, 30, 0);

        var persistedEntity = new Foo(1, "bar", "mars", updatedDate);

        upsertResultSignal.AddMeta(Metadata.Meta.CurrentEntityState, persistedEntity);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(findResult.TrySetResultState(upsertResultSignal), Is.True);
            Assert.That(findResult.Result, Is.SameAs(persistedEntity));
        }

        findResult = UnitResult.NotFound<Foo>(1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(findResult.TrySetResultState(upsertResultSignal), Is.False);
            Assert.That(findResult.Result, Is.Null);
        }

        findResult = UnitResult.FromResult(new Foo(1, "bar", "mars"));
        upsertResultSignal = UnitResult.Failed<Foo>(new DataException("Update failed due to a data error"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(findResult.TrySetResultState(upsertResultSignal), Is.False);
            Assert.That(findResult.Result, Is.EqualTo(originalResult));
        }
    }
}
