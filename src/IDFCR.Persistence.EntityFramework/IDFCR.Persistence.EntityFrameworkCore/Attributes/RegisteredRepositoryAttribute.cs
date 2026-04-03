namespace IDFCR.Persistence.EntityFrameworkCore.Attributes;

/// <summary>
/// Represents an attribute that can be applied to a repository class to indicate that it should be registered in the dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RegisteredRepositoryAttribute : Attribute
{

}