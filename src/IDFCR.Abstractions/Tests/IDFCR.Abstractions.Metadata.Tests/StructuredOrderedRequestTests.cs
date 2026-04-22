using NUnit.Framework;

namespace IDFCR.Abstractions.Metadata.Tests;

public record MyStructuredOrderedRequest : StructuredOrderedRequestBase { }

public record MyRestrictedOrderedRequest : StructuredOrderedRequestBase
{
    public MyRestrictedOrderedRequest()
    {
        SupportedFields.AddRange(["environment", "deployedBy"]);
    }
}

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

    [Test]
    public void ParseFields_WithSupportedFields_ShouldSucceed_WhenAllFieldsAreAllowed()
    {
        const string json = """
            [
              { "field": "environment", "order": 1 },
              { "field": "deployedBy", "order": -1 }
            ]
            """;

        var request = new MyRestrictedOrderedRequest();

        Assert.DoesNotThrow(() => request.ParseFields(json));
        Assert.That(request.Fields.Count(), Is.EqualTo(2));
    }

    [Test]
    public void ParseFields_WithSupportedFields_ShouldThrowFieldValidationException_WhenUnsupportedFieldIsRequested()
    {
        // "version" is not in MyRestrictedOrderedRequest's SupportedFields
        var request = new MyRestrictedOrderedRequest();

        Assert.Throws<FieldValidationException>(() => request.ParseFields(SampleJson));
    }

    [Test]
    public void ParseFields_WithSupportedFields_ShouldBeCaseInsensitive()
    {
        const string json = """
            [
              { "field": "ENVIRONMENT", "order": 1 },
              { "field": "DeployedBy", "order": -1 }
            ]
            """;

        var request = new MyRestrictedOrderedRequest();

        Assert.DoesNotThrow(() => request.ParseFields(json));
    }
}
