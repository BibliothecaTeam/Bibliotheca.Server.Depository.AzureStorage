using System;
using System.Collections.Generic;
using Bibliotheca.Server.Depository.AzureStorage.Core.Parameters;
using Bibliotheca.Server.Depository.AzureStorage.Core.Services;
using Bibliotheca.Server.Depository.AzureStorage.Core.Validators;
using Bibliotheca.Server.Mvc.Middleware.Authorization;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;

namespace Bibliotheca.Server.Depository.AzureStorage.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        protected bool UseServiceDiscovery { get; set; } = true;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationParameters>(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(SecureTokenDefaults.AuthenticationScheme)
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new QueryStringOrHeaderApiVersionReader("api-version");
            });

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Azure storage depository API",
                    Description = "Microservice for azure storage depository feature for Bibliotheca.",
                    TermsOfService = "None"
                });
            });

            services.AddServiceDiscovery();

            services.AddScoped<IAzureStorageService, AzureStorageService>();
            services.AddScoped<ICommonValidator, CommonValidator>(); ;
            services.AddScoped<IProjectsService, ProjectsService>();
            services.AddScoped<IBranchesService, BranchesService>();
            services.AddScoped<IDocumentsService, DocumentsService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (UseServiceDiscovery)
            {
                var options = GetServiceDiscoveryOptions();
                app.RegisterService(options);
            }

            if(env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            else
            {
                loggerFactory.AddAzureWebAppDiagnostics();
            }

            app.UseExceptionHandler();

            app.UseCors("AllowAllOrigins");

            var secureTokenOptions = new SecureTokenOptions
            {
                SecureToken = Configuration["SecureToken"],
                AuthenticationScheme = SecureTokenDefaults.AuthenticationScheme,
                Realm = SecureTokenDefaults.Realm
            };
            app.UseSecureTokenAuthentication(secureTokenOptions);

            var jwtBearerOptions = new JwtBearerOptions
            {
                Authority = Configuration["OAuthAuthority"],
                Audience = Configuration["OAuthAudience"],
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                RequireHttpsMetadata = !env.IsDevelopment()
            };
            app.UseBearerAuthentication(jwtBearerOptions);

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();
        }

        private ServiceDiscoveryOptions GetServiceDiscoveryOptions()
        {
            var serviceDiscoveryConfiguration = Configuration.GetSection("ServiceDiscovery");

            var tags = new List<string>();
            var tagsSection = serviceDiscoveryConfiguration.GetSection("ServiceTags");
            tagsSection.Bind(tags);

            var options = new ServiceDiscoveryOptions();
            options.ServiceOptions.Id = serviceDiscoveryConfiguration["ServiceId"];
            options.ServiceOptions.Name = serviceDiscoveryConfiguration["ServiceName"];
            options.ServiceOptions.Address = serviceDiscoveryConfiguration["ServiceAddress"];
            options.ServiceOptions.Port = Convert.ToInt32(serviceDiscoveryConfiguration["ServicePort"]);
            options.ServiceOptions.HttpHealthCheck = serviceDiscoveryConfiguration["ServiceHttpHealthCheck"];
            options.ServiceOptions.Tags = tags;
            options.ServerOptions.Address = serviceDiscoveryConfiguration["ServerAddress"];

            return options;
        }
    }
}
