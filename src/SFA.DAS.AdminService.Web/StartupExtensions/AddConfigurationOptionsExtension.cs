using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.AdminService.Infrastructure.Configuration;

namespace SFA.DAS.AdminService.Web.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class AddConfigurationOptionsExtension
    {
        public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<DigitalCertificatesOuterApiConfiguration>(configuration.GetSection(nameof(DigitalCertificatesOuterApiConfiguration)));
            services.AddSingleton(cfg => cfg.GetRequiredService<IOptions<DigitalCertificatesOuterApiConfiguration>>().Value);
        }
    }
}
