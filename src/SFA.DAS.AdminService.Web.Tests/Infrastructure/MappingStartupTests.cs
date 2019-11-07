using AutoMapper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;

namespace SFA.DAS.AdminService.Web.Tests
{
    [TestFixture]
    public class AutomapperConfigurationTests
    {
        [Test]
        public void IsAutomapperConfigurationValid()
        {
            MappingStartup.AddMappings();

            Mapper.AssertConfigurationIsValid();
        }
    }
}
