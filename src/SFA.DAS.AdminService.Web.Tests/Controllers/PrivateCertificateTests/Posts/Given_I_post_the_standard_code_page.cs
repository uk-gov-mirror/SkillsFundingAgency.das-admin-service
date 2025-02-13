﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AdminService.Web.Controllers.Private;
using SFA.DAS.AdminService.Web.ViewModels.Private;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_standard_code_page : CertificatePostBase
    {
        private RedirectToActionResult _result;

        [SetUp]
        public void WhenValidModelContainsNoErrors()
        {
            var distributedCacheMock = new Mock<IDistributedCache>();

            var certificatePrivateStandardCodeController =
                new CertificatePrivateStandardCodeController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    new CacheService(distributedCacheMock.Object),
                    MockApiClient,
                    MockStandardServiceClient.Object, MockOrganisationsApiClient
                    );

            var vm = new CertificateStandardCodeListViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                SelectedStandardCode = "93",
                IsPrivatelyFunded = true,
                ReasonForChange = "Required reason for change"
            };

            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId"))
                .Returns("EPA00001");

            var result = certificatePrivateStandardCodeController.StandardCode(vm).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;
        }

        [Test]
        public void ThenShouldReturnRedirectToController()
        {
            _result.ControllerName.Should().Be("CertificateAmend");
        }

        [Test]
        public void ThenShouldReturnRedirectToAction()
        {
            _result.ActionName.Should().Be("Check");
        }
    }
}

