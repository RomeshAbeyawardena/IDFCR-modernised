using System.Collections.Concurrent;
using System.Threading;
using NUnit.Framework;

namespace IDFCR.Utilities.Tests;

[TestFixture]
internal class SetOnceTests
{
    [Test]
    public void CreateInstance_WithoutValue_IsUnsetWithDefaultState()
    {
        var sut = SetOnce.CreateInstance<int>();

        Assert.Multiple(() =>
        {
            Assert.That(sut.IsSet, Is.False);
            Assert.That(sut.Value, Is.EqualTo(default(int)));
            Assert.That(sut.GetValueOrDefault(42), Is.EqualTo(42));
        });
    }

    [Test]
    public void CreateInstance_WithValue_IsSetWithProvidedValue()
    {
        var sut = SetOnce.CreateInstance("configured");

        Assert.Multiple(() =>
        {
            Assert.That(sut.IsSet, Is.True);
            Assert.That(sut.Value, Is.EqualTo("configured"));
            Assert.That(sut.GetValueOrDefault("fallback"), Is.EqualTo("configured"));
        });
    }

    [Test]
    public void SetValue_FirstSet_Succeeds()
    {
        var sut = SetOnce.CreateInstance<Guid>();
        var value = Guid.NewGuid();

        sut.SetValue(value);

        Assert.Multiple(() =>
        {
            Assert.That(sut.IsSet, Is.True);
            Assert.That(sut.Value, Is.EqualTo(value));
        });
    }

    [Test]
    public void SetValue_SecondSet_ThrowsInvalidOperationException()
    {
        var sut = SetOnce.CreateInstance<int>();
        sut.SetValue(1);

        var ex = Assert.Throws<InvalidOperationException>(() => sut.SetValue(2));

        Assert.Multiple(() =>
        {
            Assert.That(ex!.Message, Is.EqualTo("Cannot set the value more than once."));
            Assert.That(sut.Value, Is.EqualTo(1));
        });
    }

    [Test]
    public void SetValue_NullReference_DoesNotSet()
    {
        var sut = SetOnce.CreateInstance<string>();

        sut.SetValue(null);

        Assert.Multiple(() =>
        {
            Assert.That(sut.IsSet, Is.False);
            Assert.That(sut.Value, Is.Null);
            Assert.That(sut.GetValueOrDefault("fallback"), Is.EqualTo("fallback"));
        });
    }

    [Test]
    public void NonGenericSetValue_WithCompatibleType_SetsValue()
    {
        ISetOnce sut = SetOnce.CreateInstance<int>();

        sut.SetValue(123);

        Assert.Multiple(() =>
        {
            Assert.That(sut.IsSet, Is.True);
            Assert.That(sut.Value, Is.EqualTo(123));
            Assert.That(sut.GetValueOrDefault(77), Is.EqualTo(123));
        });
    }

    [Test]
    public void NonGenericSetValue_WithIncompatibleType_DoesNothing()
    {
        ISetOnce sut = SetOnce.CreateInstance<int>();

        sut.SetValue("123");

        Assert.Multiple(() =>
        {
            Assert.That(sut.IsSet, Is.False);
            Assert.That(sut.Value, Is.EqualTo(default(int)));
            Assert.That(sut.GetValueOrDefault(77), Is.EqualTo(77));
        });
    }

    [Test]
    public void NonGenericGetValueOrDefault_WithIncompatibleDefault_ReturnsTypedDefault()
    {
        ISetOnce sut = SetOnce.CreateInstance<int>();

        var result = sut.GetValueOrDefault("fallback");

        Assert.That(result, Is.EqualTo(default(int)));
    }

    [Test]
    public async Task SetValue_ConcurrentWriters_OnlyOneWriteSucceeds()
    {
        var sut = SetOnce.CreateInstance<int>();
        var gate = new ManualResetEventSlim(false);
        var exceptions = new ConcurrentBag<Exception>();
        var successfulValues = new ConcurrentBag<int>();

        const int writerCount = 64;
        var tasks = Enumerable.Range(1, writerCount)
            .Select(value => Task.Run(() =>
            {
                gate.Wait();

                try
                {
                    sut.SetValue(value);
                    if (sut.IsSet && sut.Value == value)
                    {
                        successfulValues.Add(value);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }))
            .ToArray();

        gate.Set();
        await Task.WhenAll(tasks);

        var invalidOperationCount = exceptions.Count(static ex => ex is InvalidOperationException);

        Assert.Multiple(() =>
        {
            Assert.That(sut.IsSet, Is.True);
            Assert.That(sut.Value, Is.InRange(1, writerCount));
            Assert.That(exceptions.Count, Is.EqualTo(writerCount - 1));
            Assert.That(invalidOperationCount, Is.EqualTo(writerCount - 1));
            Assert.That(successfulValues.Count, Is.GreaterThanOrEqualTo(1));
        });
    }
}
