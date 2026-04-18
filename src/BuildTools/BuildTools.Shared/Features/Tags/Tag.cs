using IDFCR.Abstractions.Mapper;

namespace BuildTools.Shared.Features.Tags;

public class Tag : MapperBase<ITag>, ITag
{
    public static bool operator ==(Tag? tag, Tag? otherTag)
    {
        if (ReferenceEquals(tag, otherTag))
        {
            return true;
        }

        if (tag is null || otherTag is null)
        {
            return false;
        }

        return tag.Equals(otherTag);
    }

    public static bool operator !=(Tag? tag, Tag? otherTag) => !(tag == otherTag);

    public object? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }

    public override void Map(ITag source)
    {
        Id = source.Id;
        Name = source.Name;
        DisplayName = source.DisplayName;
    }

    /// <summary>
    /// Determines whether the current tag is equal to the specified tag, using a case-insensitive comparison for the
    /// tag name.
    /// </summary>
    /// <remarks>The comparison ignores case differences in the tag name.</remarks>
    /// <param name="otherTag">The tag to compare with the current tag.</param>
    /// <returns>true if the tags are considered equal; otherwise, false.</returns>
    public bool Equals(Tag? otherTag)
    {
        return otherTag is not null
            && Name.Equals(otherTag.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current Tag instance is equal to the specified object.
    /// </summary>
    /// <remarks>This method supports comparison with both Tag instances and string values. When comparing
    /// with a string, the comparison is case-insensitive.</remarks>
    /// <param name="obj">The object to compare with the current Tag. This can be another Tag instance or a string representing a tag
    /// name.</param>
    /// <returns>true if the specified object is a Tag with the same name as the current instance, or a string that matches the
    /// tag name (case-insensitive); otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Tag tag)
        {
            return Equals(tag);
        }

        if (obj is string tagName)
        {
            return Name.Equals(tagName, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
    }
}