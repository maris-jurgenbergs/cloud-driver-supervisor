namespace User.Service
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
    using Modules.Role.MessageHandlers;
    using Modules.User.MessageHandlers;
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
            var getUserRolesQueueHandler = container.Resolve<GetUserRolesQueueHandler>();
            var postUserRolesQueueHandler = container.Resolve<PostUserRolesQueueHandler>();
            var deleteUserRolesQueueHandler = container.Resolve<DeleteUserRolesQueueHandler>();
            var getUsersQueueHandler = container.Resolve<GetUsersQueueHandler>();
            var deleteUserQueueHandler = container.Resolve<DeleteUserQueueHandler>();
            var postUserQueueHandler = container.Resolve<PostUserQueueHandler>();

            var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var serviceBusConnectionString = configurationPackage.Settings.Sections["ConnectionStrings"]
                .Parameters["Microsoft.ServiceBus.ConnectionString"].Value;

            return new[]
            {
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getUserRolesQueueHandler, Context,
                            "Incoming-Get-User-Roles-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction,
                            MaxConcurrentCalls = 10
                        },
                    "Stateless-User-Service-Incoming-Get-User-Role-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(getUsersQueueHandler, Context,
                            "Incoming-Get-Users-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction,
                            MaxConcurrentCalls = 10
                        },
                    "Stateless-User-Service-Incoming-Get-Users-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(deleteUserQueueHandler, Context,
                            "Incoming-Delete-User-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction,
                            MaxConcurrentCalls = 10
                        },
                    "Stateless-User-Service-Incoming-Delete-User-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(postUserQueueHandler, Context,
                            "Incoming-Post-User-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction,
                            MaxConcurrentCalls = 10
                        },
                    "Stateless-User-Service-Incoming-Post-User-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(postUserRolesQueueHandler, Context,
                            "Incoming-Post-User-Roles-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction,
                            MaxConcurrentCalls = 10
                        },
                    "Stateless-User-Service-Incoming-Post-User-Roles-Queue-Listener"
                ),
                new ServiceInstanceListener(serviceContext =>
                        new ServiceBusQueueCommunicationListener(deleteUserRolesQueueHandler, Context,
                            "Incoming-Delete-User-Roles-Queue", serviceBusConnectionString,
                            serviceBusConnectionString)
                        {
                            AutoRenewTimeout = TimeSpan.FromSeconds(70),
                            LogAction = _logAction,
                            MaxConcurrentCalls = 10
                        },
                    "Stateless-User-Service-Incoming-Delete-User-Roles-Queue-Listener"
                )
            };
        }
    }
}