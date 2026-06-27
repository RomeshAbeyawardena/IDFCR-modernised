
using NUnit.Framework;

namespace IDFCR.Utilities.Tests;

[TestFixture]
internal class SwitchTests
{
    private static readonly string[] Values = ["foo", "bar", "roo", "moo", "two"];
    private static string GetValue(int index)
    {
        return Values[index];
    }
    private const string InvalidOptionMessage = "Invalid option.";
    private ISwitch<int, string> sut;
    [SetUp]
    public void SetUp()
    {
        sut = SwitchBuilder.Build<int, string>(config =>
        {
            for (int i = 0; i < 10; i++)
            {
                config.CaseWhen(i, GetValue);
            }

            config.Else((key) => InvalidOptionMessage);
        });
    }

    [Test]
    public void Test()
    {
        for (int i = 0; i < 10; i++)
        {
            var m = Values[0];
            Assert.That(sut.ThenValue(0), Is.EqualTo(m));
        }

        Assert.That(sut.ThenValue(-1), Is.EqualTo(InvalidOptionMessage));
    }
}
