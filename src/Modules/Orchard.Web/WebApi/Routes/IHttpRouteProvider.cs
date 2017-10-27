using Orchard.Web.Routes;
using System.Collections.Generic;

namespace Orchard.Web.WebApi.Routes
{
    public interface IHttpRouteProvider : IDependency
    {
        void GetRoutes(ICollection<RouteDescriptor> routes);
    }
}
