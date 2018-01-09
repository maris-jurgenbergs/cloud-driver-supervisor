namespace Mobile.Business.Bootstrapper
{
    using System.Reflection;
    using ApiClient;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Modules.Tracking.ServiceConnections;

    public static class Bootstrapper
    {
        private static IContainer _container;

        public static IContainer GetContainer()
        {
            return _container ?? (_container = BuildContainer());
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            var assembly = typeof(ApiService).GetTypeInfo().Assembly;
            var serviceCollection = new ServiceCollection().Scan(scan =>
                scan.FromAssemblies(assembly)
                    .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                    .AsMatchingInterface()
                    .WithSingletonLifetime()
                    .AddClasses(classes => classes.AssignableTo<ITransientService>())
                    .AsMatchingInterface()
                    .WithTransientLifetime());
            builder.Populate(serviceCollection);

            builder.RegisterType<TrackingServiceConnection>().SingleInstance();
            return builder.Build();
        }
    }
}