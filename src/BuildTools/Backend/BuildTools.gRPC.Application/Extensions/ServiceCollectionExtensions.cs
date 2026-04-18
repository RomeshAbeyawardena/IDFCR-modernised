using BuildTools.GRPC.Application.Features.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BuildTools.GRPC.Application.Extensions;

public static class GrpcServices
{
    public static IEnumerable<Type> Fetch()
    {
        var serviceTypes = typeof(GrpcServices).Assembly
           .GetTypes()
           .Where(t => t.IsClass && !t.IsAbstract &&
                       t.GetCustomAttribute<RegisteredServiceAttribute>() is not null);
        
        return serviceTypes;
    }
}
