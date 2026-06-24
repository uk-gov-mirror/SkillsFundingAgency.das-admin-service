using System;
using FluentAssertions;
using MediatR;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.GetUserAllActivityByCode;

namespace SFA.DAS.AdminService.Application.UnitTests.Commands.GetUserAllActivityByCode
{
    [TestFixture]
    public class GetUserAllActivityByCodeCommandTests
    {
        [Test]
        public void Should_Set_Code_Property()
        {
            var command = new GetUserAllActivityByCodeCommand { Code = "code-1" };
            command.Code.Should().Be("code-1");
        }
    }
}
