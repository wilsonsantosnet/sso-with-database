using Common.API;
using Common.Domain.Base;
using Common.Domain.Model;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seed.CrossCuting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Sso.Server.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                 .AddEnvironmentVariables();

            Configuration = builder.Build();
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            var cns =
             Configuration
                .GetSection("ConfigConnectionString:Default").Value;

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddSigningCredential(GetRSAParameters())
                //.AddInMemoryApiResources(Config.GetApiResources())
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                //.AddInMemoryClients(Config.GetClients(Configuration.GetSection("ConfigSettings").Get<ConfigSettingsBase>()));
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(cns, sql => sql.MigrationsAssembly(migrationsAssembly));

                }).AddOperationalStore(options => {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(cns, sql => sql.MigrationsAssembly(migrationsAssembly));
                });
            
            //Configurations
            services.Configure<ConfigSettingsBase>(Configuration.GetSection("ConfigSettings"));
            services.Configure<ConfigConnectionStringBase>(Configuration.GetSection("ConfigConnectionString"));
            //Container DI
            services.AddScoped<CurrentUser>();
            services.AddTransient<IUserCredentialServices, UserCredentialServices>();
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = "857854978384-sv33ngtei50k8fn5ea37rcddo08n0ior.apps.googleusercontent.com";
                options.ClientSecret = "x1SWT89gyn5LLLyMNFxEx_Ss";
            });
            // Add cross-origin resource sharing services Configurations
            var sp = services.BuildServiceProvider();
            var configuration = sp.GetService<IOptions<ConfigSettingsBase>>();
            Cors.Enable(services, configuration.Value.ClientAuthorityEndPoint.ToArray());

            services.AddMvc();


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<ConfigSettingsBase> configSettingsBase)
        {

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            app.UseCors("AllowAnyOrigin");
            app.UseIdentityServer();
            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //    ClientId = "857854978384-sv33ngtei50k8fn5ea37rcddo08n0ior.apps.googleusercontent.com",
            //    ClientSecret = "x1SWT89gyn5LLLyMNFxEx_Ss"
            //});
            app.UseAuthentication();
            app.AddTokenMiddlewareCustom();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            this.InitializeDatabase(app, configSettingsBase.Value);
        }



        private X509Certificate2 GetRSAParameters()
        {
            var fileCert = Path.Combine(_env.ContentRootPath, "pfx", "ids4smbasic.pfx");
            if (!File.Exists(fileCert))
                throw new InvalidOperationException("Certificado não encontrado");

            var password = "vm123s456";
            return new X509Certificate2(fileCert, password, X509KeyStorageFlags.Exportable);
        }

        private void InitializeDatabase(IApplicationBuilder app, ConfigSettingsBase settings)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                foreach (var client in Config.GetClients(settings))
                {
                    if (context.Clients.Where(_ => _.ClientId == client.ClientId).IsNotAny())
                        context.Clients.Add(client.ToEntity());

                }
                context.SaveChanges();

                foreach (var ir in Config.GetIdentityResources())
                {
                    if (context.IdentityResources.Where(_ => _.Name == ir.Name).IsNotAny())
                        context.IdentityResources.Add(ir.ToEntity());

                }
                context.SaveChanges();

                foreach (var ar in Config.GetApiResources())
                {
                    if (context.ApiResources.Where(_ => _.Name == ar.Name).IsNotAny())
                        context.ApiResources.Add(ar.ToEntity());

                }
                context.SaveChanges();

            }
        }
    }
}
