using Common.Api;
using Common.Cripto;
using Common.Domain;
using Common.Domain.Interfaces;
using Common.Domain.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Score.Platform.Account.HangFire.Config
{
    public static partial class ConfigContainer
    {
        public static void RegisterOtherComponents(IServiceCollection services)
        {
            services.AddScoped<CurrentUser>();
            services.AddScoped<IRequest, Request>();
            services.AddScoped<ICripto, Cripto>();
        }
    }
}