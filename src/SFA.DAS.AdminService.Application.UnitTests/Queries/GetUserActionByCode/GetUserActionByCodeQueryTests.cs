using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Queries.GetUserActionByCode;

namespace SFA.DAS.AdminService.Application.UnitTests.Queries.GetUserActionByCode
{
    [TestFixture]
    public class GetUserActionByCodeQueryTests
    {
        [Test]
        public void Should_Set_Code_Property()
        {
            // Arrange
            var query = new GetUserActionByCodeQuery { Code = "code-1" };

            // Act
            var code = query.Code;

            // Assert
            code.Should().Be("code-1");
        }
    }
}
