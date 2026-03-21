using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.DependencyInjection.Tests.Fakes;

internal class FakeServiceCollection : List<ServiceDescriptor>, IServiceCollection
{

}
