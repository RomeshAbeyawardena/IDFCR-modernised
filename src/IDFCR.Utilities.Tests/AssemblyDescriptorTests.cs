using NUnit.Framework;

namespace IDFCR.Utilities.Tests;

internal enum Category
{
    Application,
    Infrastructure,
    Data
}

[Flags]
internal enum CategoryWithFlags
{
    None = 0,
    Application = 1,
    Infrastructure = 2,
    Data = 4
}

[TestFixture]
internal class AssemblyDescriptorTests
{
    private IAssemblyDescriptorBuilder<Category> categoryBuilder;

    [SetUp]
    public void Setup()
    {
        categoryBuilder = AssemblyDescriptorBuilder.Create<Category>();
    }

    [Test]
    public void Create_WhenAppendingAssembliesForMultipleCategories_ReturnsMatchingAssembliesPerCategory()
    {
        categoryBuilder
            .Append(Category.Application, typeof(AssemblyDescriptorTests).Assembly)
            .Append(Category.Infrastructure, typeof(System.IO.Stream).Assembly)
            .Append<System.Data.DataTable>(Category.Data);

        var descriptor = categoryBuilder.Build();

        Assert.Multiple(() =>
        {
            Assert.That(descriptor.GetAssemblies(Category.Application), Is.EqualTo(new[] { typeof(AssemblyDescriptorTests).Assembly }));
            Assert.That(descriptor.GetAssemblies(Category.Infrastructure), Is.EqualTo(new[] { typeof(System.IO.Stream).Assembly }));
            Assert.That(descriptor.GetAssemblies(Category.Data), Is.EqualTo(new[] { typeof(System.Data.DataTable).Assembly }));
        });
    }

    [Test]
    public void Build_WhenCategoryWasNotConfigured_ReturnsEmptySequence()
    {
        var descriptor = AssemblyDescriptorBuilder.Build<Category>(_ => { });

        Assert.That(descriptor.GetAssemblies(Category.Application), Is.Empty);
    }

    [Test]
    public void Build_WhenSameAssemblyIsAppendedMultipleTimes_DeduplicatesByCategory()
    {
        categoryBuilder
            .Append(Category.Application, typeof(AssemblyDescriptorTests).Assembly, typeof(AssemblyDescriptorTests).Assembly)
            .Append<System.IO.Stream>(Category.Application)
            .Append(Category.Application, typeof(System.IO.Stream).Assembly);

        var descriptor = categoryBuilder.Build();

        Assert.That(
            descriptor.GetAssemblies(Category.Application),
            Is.EqualTo(new[] { typeof(AssemblyDescriptorTests).Assembly, typeof(System.IO.Stream).Assembly }));
    }
}

[TestFixture]
internal class AssemblyDescriptorFlagsTests
{
    [Test]
    public void Build_WhenEnumHasFlags_ReturnsAssembliesForExactAndCompositeMatches()
    {
        var descriptor = AssemblyDescriptorBuilder.Build<CategoryWithFlags>(builder =>
        {
            builder.Append(CategoryWithFlags.Infrastructure, typeof(System.IO.Stream).Assembly);
            builder.Append(CategoryWithFlags.Data, typeof(System.Data.DataTable).Assembly);
            builder.Append(CategoryWithFlags.Infrastructure | CategoryWithFlags.Data, typeof(AssemblyDescriptorFlagsTests).Assembly);
        });

        Assert.Multiple(() =>
        {
            Assert.That(
                descriptor.GetAssemblies(CategoryWithFlags.Infrastructure),
                Is.EquivalentTo(new[] { typeof(System.IO.Stream).Assembly, typeof(AssemblyDescriptorFlagsTests).Assembly }));

            Assert.That(
                descriptor.GetAssemblies(CategoryWithFlags.Data),
                Is.EquivalentTo(new[] { typeof(System.Data.DataTable).Assembly, typeof(AssemblyDescriptorFlagsTests).Assembly }));
        });
    }

    [Test]
    public void Build_WhenCombinedFlagRequested_ReturnsAssembliesForCombinedEntriesOnly()
    {
        var descriptor = AssemblyDescriptorBuilder.Build<CategoryWithFlags>(builder =>
        {
            builder.Append(CategoryWithFlags.Infrastructure, typeof(System.IO.Stream).Assembly);
            builder.Append(CategoryWithFlags.Data, typeof(System.Data.DataTable).Assembly);
            builder.Append(CategoryWithFlags.Infrastructure | CategoryWithFlags.Data, typeof(AssemblyDescriptorFlagsTests).Assembly);
        });

        Assert.That(
            descriptor.GetAssemblies(CategoryWithFlags.Infrastructure | CategoryWithFlags.Data),
            Is.EqualTo(new[] { typeof(AssemblyDescriptorFlagsTests).Assembly }));
    }

    [Test]
    public void Build_WhenFlagValueHasNoMatchingConfiguredEntries_ReturnsEmptySequence()
    {
        var descriptor = AssemblyDescriptorBuilder.Build<CategoryWithFlags>(builder =>
        {
            builder.Append(CategoryWithFlags.Infrastructure, typeof(System.IO.Stream).Assembly);
            builder.Append(CategoryWithFlags.Data, typeof(System.Data.DataTable).Assembly);
        });

        Assert.That(descriptor.GetAssemblies(CategoryWithFlags.Application), Is.Empty);
    }
}
