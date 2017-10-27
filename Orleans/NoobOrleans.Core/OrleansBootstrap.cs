using Microsoft.Extensions.DependencyInjection;
using System;

namespace NoobOrleans.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class OrleansBootstrap
    {

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.AddScoped((serviceProvider) =>
            //{
            //    return repository;
            //});
            //services.AddSingleton<IInjectedSingletonService,InjectedSingletonService>();
            //services.AddScoped<IInjectedScopedService, InjectedScopedService>();

            // explicitly register a grain class to assert that it will NOT use the registration, 
            // as by design this is not supported.
            //services.AddTransient(
            //    sp => new ExplicitlyRegisteredSimpleDIGrain(
            //        sp.GetRequiredService<IInjectedService>(),
            //        "some value",
            //        5));
            IServiceProvider serviceProvider;
            serviceProvider = services.BuildServiceProvider();
            //serviceProvider = new AutofacServiceProvider(container);
            return serviceProvider;
        }
    }
}
