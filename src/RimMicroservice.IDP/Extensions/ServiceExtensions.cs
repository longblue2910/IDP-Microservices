using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RimMicroservices.IDP.Common;
using RimMicroservices.IDP.Entities;
using RimMicroservices.IDP.Persistence;
using Serilog;
using System.Runtime;

namespace RimMicroservices.IDP.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
            IConfiguration configuration)
        {
            var smtpSettings = configuration.GetSection(nameof(SMTPEmailSetting))
                .Get<SMTPEmailSetting>();
            services.AddSingleton(smtpSettings);

            return services;
        }

        internal static void AddAppConfigurations(this ConfigureHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();
            });
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
        }

        public static void ConfigureSerilog(this ConfigureHostBuilder host)
        {
            host.UseSerilog((context, config) =>
            {
                var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");
                var environmentName = context.HostingEnvironment.EnvironmentName ?? "Development";

                var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
                var username = context.Configuration.GetValue<string>("ElasticConfiguration:Username");
                var password = context.Configuration.GetValue<string>("ElasticConfiguration:Password");

                config
                    .WriteTo.Debug()
                    .WriteTo.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                    .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        IndexFormat = $"rimlogs-{applicationName}-{environmentName}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfReplicas = 1,
                        NumberOfShards = 2,
                        ModifyConnectionSettings = x => x.BasicAuthentication(username, password)
                    })
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Environment", environmentName)
                    .Enrich.WithProperty("Application", applicationName)
                    .ReadFrom.Configuration(context.Configuration);
            });
        }

        public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddIdentityServer(options =>
             {
                 options.EmitStaticAudienceClaim = true;
                 options.Events.RaiseErrorEvents = true;
                 options.Events.RaiseInformationEvents = false;
                 options.Events.RaiseFailureEvents = false;
                 options.Events.RaiseSuccessEvents = false;
             })
            // not recommended for production
            .AddDeveloperSigningCredential()
            //.AddInMemoryIdentityResources(Config.IdentityResources)
            //.AddInMemoryApiScopes(Config.ApiScopes)
            //.AddInMemoryClients(Config.Clients)
            //.AddInMemoryApiResources(Config.ApiResources)
            //.AddTestUsers(TestUsers.Users)
            .AddConfigurationStore(otp =>
            {
                otp.ConfigureDbContext = c => c.UseSqlServer(
                    connectionString,
                    builder => builder.MigrationsAssembly("RimMicroservices.IDP"));
            }).AddOperationalStore(otp =>
            {
                otp.ConfigureDbContext = c => c.UseSqlServer(
                    connectionString,
                    builder => builder.MigrationsAssembly("RimMicroservices.IDP"));
            })
            .AddAspNetIdentity<User>()
            .AddProfileService<IdentityProfileService>();
        }

        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionstring = configuration.GetConnectionString("DefaultConnection");
            services
                .AddDbContext<RIMIdentityContext>(options => options.UseSqlServer(connectionstring))
                .AddIdentity<User, IdentityRole>(otp =>
                {
                    otp.Password.RequireDigit = false;
                    otp.Password.RequiredLength = 6;
                    otp.Password.RequireUppercase = false;
                    otp.Password.RequireLowercase = false;
                    otp.User.RequireUniqueEmail = true;
                    otp.Lockout.AllowedForNewUsers = true;
                    otp.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    otp.Lockout.MaxFailedAccessAttempts = 3;
                }).AddEntityFrameworkStores<RIMIdentityContext>()
                .AddDefaultTokenProviders();
        }
    }
}
