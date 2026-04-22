using NUnit.Framework;

namespace IDFCR.Abstractions.Metadata.Tests;

public record MyStructuredOrderedRequest : StructuredOrderedRequestBase { }

[TestFixture]
public class StructuredOrderedRequestTests
{
    private const string SampleJson = """
        [
          { "field": "environment", "order": 1 },
          { "field": "version", "order": -1 },
          { "field": "deployedBy", "order": 1 }
        ]
        """;

    [Test]
    public void ParseFields_ShouldPopulateFields_WithCorrectCount()
    {
        var request = new MyStructuredOrderedRequest();
        request.ParseFields(SampleJson);

        Assert.That(request.Fields.Count(), Is.EqualTo(3));
    }

    [Test]
    public void ParseFields_ShouldMapFieldNamesAndDirectionsCorrectly()
    {
        var request = new MyStructuredOrderedRequest();
        request.ParseFields(SampleJson);

        var fields = request.Fields.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(fields[0].Field, Is.EqualTo("environment"));
            Assert.That(fields[0].Direction, Is.EqualTo(OrderDirection.Ascending));

            Assert.That(fields[1].Field, Is.EqualTo("version"));
            Assert.That(fields[1].Direction, Is.EqualTo(OrderDirection.Descending));

            Assert.That(fields[2].Field, Is.EqualTo("deployedBy"));
            Assert.That(fields[2].Direction, Is.EqualTo(OrderDirection.Ascending));
        });
    }

    [Test]
    public void ParseFields_WithEmptyJson_ShouldResultInEmptyFields()
    {
        var request = new MyStructuredOrderedRequest();
        request.ParseFields("[]");

        Assert.That(request.Fields, Is.Empty);
    }

    [Test]
    public void ToOrderedRequest_ShouldBuildCorrectOrderByString()
    {
        var request = new MyStructuredOrderedRequest();
        request.ParseFields(SampleJson);

        var ordered = request.ToOrderedRequest(null);

        Assert.That(ordered.OrderBy, Is.EqualTo("environment asc, version desc, deployedBy asc"));
    }
}
