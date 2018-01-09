namespace Alert.Service.Infrastructure.Bootstrapper
{
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Options;

    public static class Bootstrapper
    {
        private static IContainer _container;

        public static IContainer GetContainer()
        {
            return _container ?? BuildContainer();
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(ISingletonService).GetTypeInfo().Assembly;
            LoadRepositories(builder, assembly);
            LoadServices(builder, assembly);
            LoadQueueHandlers(builder, assembly);
            LoadBuilders(builder, assembly);
            LoadOptions(builder);

            _container = builder.Build();
            return _container;
        }

        private static void LoadOptions(ContainerBuilder builder)
        {
            var optionCollection = new ServiceCollection()
                .AddOptions()
                .Configure<AppOptions>(GetAppSection("AppOptions"))
                .Configure<Neo4JOptions>(GetAppSection("Neo4jOptions"));
            builder.Populate(optionCollection);
        }

        private static void LoadBuilders(ContainerBuilder builder, Assembly assembly)
        {
            var builderCollection = new ServiceCollection().Scan(scan =>
                scan.FromAssemblies(assembly)
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Builder")))
                    .AsMatchingInterface()
                    .WithSingletonLifetime());
            builder.Populate(builderCollection);
        }

        private static void LoadQueueHandlers(ContainerBuilder builder, Assembly assembly)
        {
            var queueHandlerCollection = new ServiceCollection().Scan(scan =>
                scan.FromAssemblies(assembly)
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("QueueHandler")))
                    .AsSelf()
                    .WithSingletonLifetime());
            builder.Populate(queueHandlerCollection);
        }

        private static void LoadServices(ContainerBuilder builder, Assembly assembly)
        {
            var serviceCollection = new ServiceCollection().Scan(scan =>
                scan.FromAssemblies(assembly)
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                    .AsImplementedInterfaces().WithSingletonLifetime());
            builder.Populate(serviceCollection);
        }

        private static void LoadRepositories(ContainerBuilder builder, Assembly assembly)
        {
            var repositoryCollection = new ServiceCollection().Scan(scan =>
                scan.FromAssemblies(assembly)
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                    .AsImplementedInterfaces().WithSingletonLifetime());
            builder.Populate(repositoryCollection);
        }

        private static IConfigurationSection GetAppSection(string sectionName)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", true, true);
            var config = builder.Build();
            return config.GetSection(sectionName);
        }
    }
}