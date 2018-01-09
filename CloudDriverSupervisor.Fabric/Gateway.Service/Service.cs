namespace Gateway.Service
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Net;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Service : StatelessService
    {
        public Service(StatelessServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        ///     Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");

            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var environment = configurationPackage.Settings.Sections["Environment"]
                            .Parameters["ASPNETCORE_ENVIRONMENT"].Value;
                        var sslCertificateName = configurationPackage.Settings.Sections["Environment"]
                            .Parameters["SSL_CERTIFICATE_NAME"].Value;
                        var sslCertificatePassword = configurationPackage.Settings.Sections["Environment"]
                            .Parameters["SSL_CERTIFICATE_PW"].Value;
                        return new WebHostBuilder()
                            .UseKestrel(options =>
                            {
                                options.Listen(IPAddress.Any, 8419,
                                    listenOptions =>
                                        listenOptions.UseHttps(sslCertificateName, sslCertificatePassword));
                            })
                            .ConfigureServices(services => services.AddSingleton(serviceContext))
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseStartup<Startup>()
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url)
                            .UseEnvironment(environment)
                            .Build();
                    }))
            };
        }
    }
}