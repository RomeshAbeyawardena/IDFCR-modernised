using IDFCR.Persistence.EntityFrameworkCore.Extensions;
using NUnit.Framework;

namespace IDFCR.Persistence.EntityFrameworkCore.Tests;

public record Fruit
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public int OrderIndex { get; init; }
}

[TestFixture]
public class OrderedTesting
{
    [Test]
    public void ApplyOrdering_Should_OrderBy_OrderIndexDesc_Then_NameAsc()
    {
        // Arrange
        List<Fruit> list =
        [
            new Fruit { OrderIndex = 1, Name = "Plum" },
            new Fruit { OrderIndex = 2, Name = "Cherry" },
            new Fruit { OrderIndex = 3, Name = "Apple" },
            new Fruit { OrderIndex = 5, Name = "Strawberry" },
            new Fruit { OrderIndex = 5, Name = "Banana" }, // tie on OrderIndex to validate secondary sort
            new Fruit { OrderIndex = 9, Name = "Pear" }
        ];

        // Act
        var orderedList = list
            .AsQueryable()
            .ApplyOrdering("OrderIndex desc, Name asc")
            .ToList();

        // Assert
        Assert.That(orderedList.Select(x => x.Name), Is.EqualTo(
        [
            "Pear",
            "Banana",
            "Strawberry",
            "Apple",
            "Cherry",
            "Plum"
        ]));
    }

    [Test]
    public void ApplyOrdering_Should_Use_DefaultSortDirection_When_DirectionIsOmitted()
    {
        // Arrange
        List<Fruit> list =
        [
            new Fruit { OrderIndex = 1, Name = "Plum" },
            new Fruit { OrderIndex = 9, Name = "Pear" },
            new Fruit { OrderIndex = 3, Name = "Apple" }
        ];

        // Act
        var orderedList = list
            .AsQueryable()
            .ApplyOrdering("OrderIndex", defaultSortDirection: "desc")
            .ToList();

        // Assert
        Assert.That(orderedList.Select(x => x.OrderIndex), Is.EqualTo([9, 3, 1]));
    }

    [Test]
    public void ApplyOrdering_Should_Return_Source_When_OrderBy_Is_Empty()
    {
        // Arrange
        List<Fruit> list =
        [
            new Fruit { OrderIndex = 2, Name = "Cherry" },
            new Fruit { OrderIndex = 1, Name = "Plum" }
        ];

        // Act
        var orderedList = list
            .AsQueryable()
            .ApplyOrdering("   ")
            .ToList();

        // Assert
        Assert.That(orderedList.Select(x => x.Name), Is.EqualTo(["Cherry", "Plum"]));
    }
}
