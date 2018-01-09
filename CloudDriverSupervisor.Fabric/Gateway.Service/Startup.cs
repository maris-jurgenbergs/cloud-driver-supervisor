namespace Gateway.Service
{
    using System.IO;
    using AzureAd;
    using AzureAd.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;
    using Modules.AutoMapper;
    using Modules.AutoMapper.Interfaces;
    using Modules.ServiceBus;
    using Modules.ServiceBus.Interfaces;
    using SignalR;
    using SignalR.Hubs;
    using SignalR.Interfaces;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        private const string AzureMetadataAddress =
            "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

        private readonly bool _testInProcess;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true);

            Configuration = builder.Build();
            _testInProcess = Configuration.GetSection("AppOptions").GetValue<bool>("TestInProcess");
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var section = Configuration.GetSection("AppOptions");
            services.Configure<AppOptions>(section);

            //TODO: implement autofac or check if possible to use assembly scan in aspnetcore DI container
            services.AddTransient<IMessageBrokerService, MessageBrokerService>();
            services.AddSingleton<IAutoMapperService, AutoMapperService>();
            services.AddSingleton<ITransportationHubOrchestrator, TransportationHubOrchestrator>();
            services.AddSingleton<IAzureActiveDirectoryService, AzureActiveDirectoryService>();
            if (!_testInProcess)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "CDS API Gateway", Version = "v1" });

                    var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                        "Gateway.Service.xml");
                    c.IncludeXmlComments(filePath);
                });
            }

            var retriever = new OpenIdConnectConfigurationRetriever();
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(AzureMetadataAddress, retriever);
            var config = configManager.GetConfigurationAsync().Result;

            var validIssuer = Configuration["Authentication:AzureAd:AADInstance"]
                              + Configuration["Authentication:AzureAd:TenantId"] + "/";
            var validAudienceMobile = Configuration["Authentication:AzureAd:AudienceMobile"];
            var validAudienceWeb = Configuration["Authentication:AzureAd:AudienceWeb"];
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = validIssuer,
                        ValidateIssuer = true,
                        ValidAudiences = new[]
                        {
                            validAudienceMobile, validAudienceWeb
                        },
                        ValidateAudience = true,
                        IssuerSigningKeys = config.SigningKeys
                    };
                });

            services.AddSignalR().AddRedis(options =>
            {
                options.Options.Ssl = true;
                options.Options.EndPoints.Add(Configuration["Redis:Endpoint"]);
                options.Options.Password = Configuration["Redis:Password"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!_testInProcess)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            }

            //TODO: add in config file
            app.UseCors(builder => builder.WithOrigins("http://localhost:5000", "https://cdsw.azurewebsites.net")
                .AllowAnyHeader().AllowAnyMethod());
            app.UseSignalR(routes => routes.MapHub<TransportationHub>("transportation"));
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            var transportationHubOrchestrator = app.ApplicationServices.GetService<ITransportationHubOrchestrator>();
            transportationHubOrchestrator.SubscribeToProcessedTransportations();
        }
    }
}