using Microsoft.Extensions.DependencyInjection;

namespace IDCR.Abstractions.DependencyInjection.Tests.Fakes;

internal class FakeServiceCollection : List<ServiceDescriptor>, IServiceCollection
{

}
