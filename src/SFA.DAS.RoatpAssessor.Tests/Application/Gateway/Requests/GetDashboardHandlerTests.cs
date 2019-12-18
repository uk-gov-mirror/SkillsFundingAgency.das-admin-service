using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.RoatpAssessor.Application.Gateway.Queries;
using RoatpAssessorTypes = SFA.DAS.AssessorService.Api.Types.Models.Roatp.Assessor;

namespace SFA.DAS.RoatpAssessor.Tests.Application.Gateway.Requests
{
    [TestFixture]
    public class GetDashboardHandlerTests
    {
        [TestCase(nameof(RoatpAssessorTypes.Application.OrganisationName), false, "a", "b", "c", "d","e")]
        [TestCase(nameof(RoatpAssessorTypes.Application.OrganisationName), true, "e", "d", "c", "b", "a")]
        [TestCase(nameof(RoatpAssessorTypes.Application.UKPRN), false, "e", "a", "d", "c", "b")]
        [TestCase(nameof(RoatpAssessorTypes.Application.UKPRN), true, "b", "c", "d", "a", "e")]
        [TestCase(nameof(RoatpAssessorTypes.Application.ReferenceNumber), false, "d", "b", "a", "e", "c")]
        [TestCase(nameof(RoatpAssessorTypes.Application.ReferenceNumber), true, "c", "e", "a", "b", "d")]
        [TestCase(nameof(RoatpAssessorTypes.Application.ApplicationRouteId), false, "b", "c", "a", "d", "e")]
        [TestCase(nameof(RoatpAssessorTypes.Application.ApplicationRouteId), true, "e", "a", "d", "b", "c")]
        [TestCase(nameof(RoatpAssessorTypes.Application.ApplicationSubmittedOn), false, "c", "e", "b", "a", "d")]
        [TestCase(nameof(RoatpAssessorTypes.Application.ApplicationSubmittedOn), true, "d", "a", "b", "e", "c")]
        public async Task ShouldOrderNewApplications(string sortBy, bool sortDescending, string first, string second, string third, string fourth, string fifth)
        {
            var applyApiClientMock = new Mock<IRoatpApplyApiClient>();
            applyApiClientMock.Setup(a => a.GetGatewayCounts())
                .ReturnsAsync(new GatewayCounts());

            var roatpApiClientMock = new Mock<IRoatpApiClient>();
            roatpApiClientMock.Setup(x => x.GetProviderTypes()).ReturnsAsync(new List<ProviderType>
            {
                new ProviderType { Id = 1, Description = "Main provider" },
                new ProviderType { Id = 2, Description = "Employer provider" },
                new ProviderType { Id = 3, Description = "Supporting provider" }
            });

            applyApiClientMock.Setup(a => a.GetSubmittedApplications())
                .ReturnsAsync(new List<RoatpAssessorTypes.Application>
                {
                    new RoatpAssessorTypes.Application{OrganisationName = "b", UKPRN = "5", ReferenceNumber = "refb", ApplicationRouteId = "1", ApplicationSubmittedOn = DateTime.Parse("2003-01-01")},
                    new RoatpAssessorTypes.Application{OrganisationName = "e", UKPRN = "1", ReferenceNumber = "refd", ApplicationRouteId = "3", ApplicationSubmittedOn = DateTime.Parse("2002-01-01")},
                    new RoatpAssessorTypes.Application{OrganisationName = "a", UKPRN = "2", ReferenceNumber = "refc", ApplicationRouteId = "2", ApplicationSubmittedOn = DateTime.Parse("2004-01-01")},
                    new RoatpAssessorTypes.Application{OrganisationName = "c", UKPRN = "4", ReferenceNumber = "refe", ApplicationRouteId = "1", ApplicationSubmittedOn = DateTime.Parse("2001-01-01")},
                    new RoatpAssessorTypes.Application{OrganisationName = "d", UKPRN = "3", ReferenceNumber = "refa", ApplicationRouteId = "2", ApplicationSubmittedOn = DateTime.Parse("2005-01-01")}
                });

            var handler = new GetDashboardHandler(applyApiClientMock.Object, roatpApiClientMock.Object);

            var request = new GetDashboardRequest(DashboardTab.New, sortBy, sortDescending);

            var response = await handler.Handle(request, new CancellationToken());

            response.NewApplications[0].OrganisationName.Should().Be(first);
            response.NewApplications[1].OrganisationName.Should().Be(second);
            response.NewApplications[2].OrganisationName.Should().Be(third);
            response.NewApplications[3].OrganisationName.Should().Be(fourth);
            response.NewApplications[4].OrganisationName.Should().Be(fifth);
        }
    }
}
