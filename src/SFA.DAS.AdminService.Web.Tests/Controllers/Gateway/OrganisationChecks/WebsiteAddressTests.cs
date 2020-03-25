﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.OrganisationChecks
{
    [TestFixture]
    public class WebsiteAddressTests
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IRoatpGatewayPageViewModelValidator> _gatewayValidator;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayOrganisationChecksController>> _logger;

        private string username = "john smith";
        private string givenName = "john";
        private string surname = "smith";

        [SetUp]
        public void Setup()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _gatewayValidator = new Mock<IRoatpGatewayPageViewModelValidator>();
            _logger = new Mock<ILogger<RoatpGatewayOrganisationChecksController>>();
            _orchestrator = new Mock<IGatewayOrganisationChecksOrchestrator>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
             {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", username),
                new Claim(ClaimTypes.GivenName, givenName),
                new Claim(ClaimTypes.Surname, surname)
             }));

            var context = new DefaultHttpContext { User = user };
            _gatewayValidator.Setup(v => v.Validate(It.IsAny<LegalNamePageViewModel>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                }
                );
            _contextAccessor.Setup(_ => _.HttpContext).Returns(context);
            _controller = new RoatpGatewayOrganisationChecksController(_applyApiClient.Object, _contextAccessor.Object, _gatewayValidator.Object, _orchestrator.Object, _logger.Object);
        }

        [Test]
        public void check_website_request_is_sent()
        {
            var applicationId = Guid.NewGuid();

            _orchestrator.Setup(x => x.GetWebsiteViewModel(new GetWebsiteRequest(applicationId, username)))
                .ReturnsAsync(new WebsiteViewModel())
                .Verifiable("view model not returned");

            var _result = _controller.GetWebsitePage(applicationId).Result;
            _orchestrator.Verify(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()), Times.Once());
        }

        [Test]
        public void post_website_address_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.WebsiteAddress;

            var vm = new WebsiteViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            _applyApiClient.Setup(x => x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluateWebsitePage(vm).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _orchestrator.Verify(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()), Times.Never());
        }

        [Test]
        public void post_website_address_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = GatewayPageIds.WebsiteAddress;

            var vm = new WebsiteViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            _gatewayValidator.Setup(v => v.Validate(It.IsAny<WebsiteViewModel>()))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field = "OptionFail", ErrorMessage = "needs text"}
                        }
                }
                );

            vm.ApplicationId = applicationId;
            vm.PageId = vm.PageId;
            vm.SourcesCheckedOn = DateTime.Now;

            _orchestrator.Setup(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluateWebsitePage(vm).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetWebsiteViewModel(It.IsAny<GetWebsiteRequest>()), Times.Never());
        }

    }
}
