using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.TestUtilities.Fakes;

/// <summary>
/// Represents a fake implementation of the IServiceCollection interface, which is used for testing purposes. This class inherits from <see cref="List{ServiceDescriptor}"/>  and implements the IServiceCollection interface, allowing it to be used as a mock service collection in unit tests or other testing scenarios where a real service collection is not necessary or practical. By using this fake service collection, developers can easily simulate the behavior of a real service collection without needing to set up a full dependency injection container, making it easier to test components that rely on dependency injection in isolation. This can help improve the efficiency and effectiveness of unit tests and other testing efforts within applications and systems that utilize dependency injection for managing dependencies and services.
/// </summary>
public class FakeServiceCollection : List<ServiceDescriptor>, IServiceCollection
{

}
