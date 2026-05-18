using IDFCR.Abstractions.Results.Extensions;
using NUnit.Framework;

namespace IDFCR.Abstractions.Results.Tests;

[TestFixture]
internal class ObjectExtensionTests
{
    [Test]
    public void Test()
    {
        object? obj = 5;
        Assert.IsTrue(obj.IsOf<int>(out var value));
        Assert.AreEqual(5, value);
        obj = null;
        Assert.IsFalse(obj.IsOf<int>(out _));
        obj = "string";
        Assert.IsFalse(obj.IsOf<int>(out _));
    }

    [Test]
    public void TestIsOfAndNotDefault()
    {
        object? obj = 5;
        Assert.IsTrue(obj.IsOfAndNotDefault<int>(out var value));
        Assert.AreEqual(5, value);
        obj = default;
        Assert.IsFalse(obj.IsOfAndNotDefault<int>(out _));
        obj = null;
        Assert.IsFalse(obj.IsOfAndNotDefault<int>(out _));
        obj = "string";
        Assert.IsFalse(obj.IsOfAndNotDefault<int>(out _));

        obj = Guid.Empty;
        Assert.IsFalse(obj.IsOfAndNotDefault<Guid>(out _));

        obj = Guid.NewGuid();
        Assert.True(obj.IsOfAndNotDefault<Guid>(out _));
    }
}
