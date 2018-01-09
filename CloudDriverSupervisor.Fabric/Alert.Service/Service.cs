namespace Alert.Service
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
    using Modules.Alert.MessageHandlers;
    using Modules.Violation.MessageHandlers;
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
            var getUserDrivingLimitQueueHandler =
                container.Resolve<GetUserDrivingLimitQueueHandler>();
            var postAlertQueueHandler =
                container.Resolve<PostAlertQueueHandler>();
            var getTransportationAlertListQueueHandler =
                container.Resolve<GetTransportationAlertListQueueHandler>();
            var getAlertListQueueHandler =
                container.Resolve<GetAlertListQueueHandler>();
            var getAlertQueueHandler =
                container.Resolve<GetAlertQueueHandler>();
            var patchAlertSeverityQueueHandler =
                container.Resolve<PatchAlertSeverityQueueHandler>();
            var getUserViolationListQueueHandler =
                container.Resolve<GetUserViolationListQueueHandler>();

            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var serviceBusConnectionString = configurationPackage.Settings.Sections["ConnectionStrings"]
                .Parameters["Microsoft.ServiceBus.ConnectionString"].Value;

            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getUserDrivingLimitQueueHandler, Context,
                            "Incoming-Get-User-Driving-Time-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Alert-Service-Incoming-Get-User-Driving-Time-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(postAlertQueueHandler, Context,
                            "Incoming-Post-Alert-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Alert-Service-Incoming-Post-Alert-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getTransportationAlertListQueueHandler, Context,
                            "Incoming-Get-Transportation-Alert-List-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Alert-Service-Get-Transportation-Alert-List-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getAlertListQueueHandler, Context,
                            "Incoming-Get-Alert-List-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Alert-Service-Get-Alert-List-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getAlertQueueHandler, Context,
                            "Incoming-Get-Alert-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Alert-Service-Get-Alert-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(patchAlertSeverityQueueHandler, Context,
                            "Incoming-Patch-Alert-Severity-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Alert-Service-Patch-Alert-Severity-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getUserViolationListQueueHandler, Context,
                            "Incoming-Get-User-Violation-List-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction
                        }, "Stateless-Alert-Service-Get-User-Violation-List-Queue-Listener"
                )
            };
        }
    }
}