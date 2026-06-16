using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.AdminService.Infrastructure.Configuration;
using SFA.DAS.AdminService.Infrastructure.ApiClients.DigitalCertificates;
using SFA.DAS.Http.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AdminService.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class AddServiceRegistrationExtensions
    {
        public static IServiceCollection AddOuterApi(this IServiceCollection services, DigitalCertificatesOuterApiConfiguration configuration)
        {
            services.AddScoped<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddScoped<Http.MessageHandlers.LoggingMessageHandler>();
            services.AddScoped<Http.MessageHandlers.ApimHeadersHandler>();

            services
                .AddRestEaseClient<IDigitalCertificatesOuterApi>(configuration.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            services.AddTransient<IApimClientConfiguration>((_) => configuration);

            return services;
        }
    }
}
