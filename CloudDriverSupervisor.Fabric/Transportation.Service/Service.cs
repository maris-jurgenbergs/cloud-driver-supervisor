namespace Transportation.Service
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using Autofac;
    using Infrastructure.Bootstrapper;
    using Infrastructure.Logging.Interfaces;
    using Infrastructure.Neo;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Modules.CapturedLocation.Interfaces;
    using Modules.CapturedLocation.MessageHandlers;
    using Modules.Transportation.MessageHandlers;
    using ServiceFabric.ServiceBus.Services.CommunicationListeners;

    /// <summary>
    ///     The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class Service : StatelessService
    {
        private static Action<string> _logAction;

        public Service(StatelessServiceContext context)
            : base(context)
        {
            NeoConfig.ConfigureModel();
        }

        /// <summary>
        ///     Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            _logAction = log => ServiceEventSource.Current.Message(log);
            var container = Bootstrapper.GetContainer();
            var loggingService = container.Resolve<ILoggingService>();
            loggingService.SetLogAction(_logAction);
            var postTransportationQueueHandler = container.Resolve<PostTransportationQueueHandler>();
            var postCapturedLocationQueueHandler =
                container.Resolve<PostCapturedLocationQueueHandler>();
            var getTransportationListQueueHandler =
                container.Resolve<GetTransportationListQueueHandler>();
            var patchTransportationQueueHandler =
                container.Resolve<PatchTransportationQueueHandler>();
            var getUserTransportationListQueueHandler =
                container.Resolve<GetUserTransportationListQueueHandler>();
            var getTransportationDetailsQueueHandler =
                container.Resolve<GetTransportationDetailsQueueHandler>();
            container.Resolve<ICapturedLocationService>().StartTransactionCountdown();

            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var serviceBusConnectionString = configurationPackage.Settings.Sections["ConnectionStrings"]
                .Parameters["Microsoft.ServiceBus.ConnectionString"].Value;

            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(postTransportationQueueHandler, Context,
                            "Incoming-Post-Transportation-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Transportation-Service-Incoming-Post-Transportation-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(postCapturedLocationQueueHandler, Context,
                            "Incoming-Post-Captured-Locations-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction,
                            MaxConcurrentCalls = 20,
                            MessagePrefetchCount = 20
                        }, "Stateless-Transportation-Service-Incoming-Post-Captured-Locations-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getTransportationListQueueHandler, Context,
                            "Incoming-Get-Transportation-List-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Transportation-Service-Incoming-Get-Transportation-List-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(patchTransportationQueueHandler, Context,
                            "Incoming-Patch-Transportation-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Transportation-Service-Incoming-Patch-Transportation-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getUserTransportationListQueueHandler, Context,
                            "Incoming-Get-User-Transportation-List-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Transportation-Service-Incoming-Get-User-Transportation-List-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getTransportationDetailsQueueHandler, Context,
                            "Incoming-Get-Transportation-Details-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Transportation-Service-Incoming-Get-Transportation-Details-Queue-Listener"
                )
            };
        }
    }
}