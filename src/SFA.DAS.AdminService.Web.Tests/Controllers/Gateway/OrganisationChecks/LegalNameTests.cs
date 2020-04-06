﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Controllers.Roatp.Apply;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Services.Gateway;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Gateway.OrganisationChecks
{
    [TestFixture]
    public class LegalNameTests
    {
        private RoatpGatewayOrganisationChecksController _controller;
        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IRoatpGatewayPageViewModelValidator> _gatewayValidator;
        private Mock<IGatewayOrganisationChecksOrchestrator> _orchestrator;
        private Mock<ILogger<RoatpGatewayOrganisationChecksController>> _logger;

        private string username => "mark cain";
        private string givenName => "mark";
        private string surname => "cain";
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

            var context = new DefaultHttpContext{ User = user };
            _gatewayValidator.Setup(v => v.Validate(It.IsAny<LegalNamePageViewModel>()))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>()
                    }
                );
            _contextAccessor.Setup(_ => _.HttpContext).Returns(context);
            _controller = new RoatpGatewayOrganisationChecksController(_applyApiClient.Object,_contextAccessor.Object,_gatewayValidator.Object, _orchestrator.Object,_logger.Object);
        }

        [Test]
        public  void check_legal_name_request_is_sent()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-10";

            _orchestrator.Setup(x => x.GetLegalNameViewModel(new GetLegalNameRequest(applicationId, username)))
                .ReturnsAsync(new LegalNamePageViewModel())
                .Verifiable("view model not returned");

            var _result =  _controller.GetGatewayLegalNamePage(applicationId, pageId).Result;
            _orchestrator.Verify(x => x.GetLegalNameViewModel(It.IsAny<GetLegalNameRequest>()), Times.Once());
        }

        [Test]
        public void post_legal_name_happy_path()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-10";

            var vm = new LegalNamePageViewModel
            {
                Status = SectionReviewStatus.Pass,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()
            };

            vm.SourcesCheckedOn = DateTime.Now;

            var pageData = JsonConvert.SerializeObject(vm);

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluateLegalNamePage(vm).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _orchestrator.Verify(x => x.GetLegalNameViewModel(It.IsAny<GetLegalNameRequest>()), Times.Never());
        }

        [Test]
        public void post_legal_name_path_with_errors()
        {
            var applicationId = Guid.NewGuid();
            var pageId = "1-20";

            var vm = new LegalNamePageViewModel
            {
                Status = SectionReviewStatus.Fail,
                SourcesCheckedOn = DateTime.Now,
                ErrorMessages = new List<ValidationErrorDetail>()

            };

            _gatewayValidator.Setup(v => v.Validate(It.IsAny<LegalNamePageViewModel>()))
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

            _orchestrator.Setup(x => x.GetLegalNameViewModel(It.IsAny<GetLegalNameRequest>()))
                .ReturnsAsync(vm)
                .Verifiable("view model not returned");

            var pageData = JsonConvert.SerializeObject(vm);

            _applyApiClient.Setup(x =>
                x.SubmitGatewayPageAnswer(applicationId, pageId, vm.Status, username, It.IsAny<string>()));

            var result = _controller.EvaluateLegalNamePage(vm).Result;

            _applyApiClient.Verify(x => x.SubmitGatewayPageAnswer(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _orchestrator.Verify(x => x.GetLegalNameViewModel(It.IsAny<GetLegalNameRequest>()), Times.Never());
        }
    }
}
