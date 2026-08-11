using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Queries.GetUserAllActivityByCode;

namespace SFA.DAS.AdminService.Application.UnitTests.Queries.GetUserAllActivityByCode
{
    [TestFixture]
    public class GetUserAllActivityByCodeQueryTests
    {
        [Test]
        public void Should_Set_Code_Property()
        {
            var query = new GetUserAllActivityByCodeQuery { Code = "code-1" };
            query.Code.Should().Be("code-1");
        }
    }
}
