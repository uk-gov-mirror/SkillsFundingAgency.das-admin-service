using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Common.Models;
using System.Security.Claims;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    // TODO: As part of the cleanup, rename this class. If this file grows larger, split the test cases into multiple test classes.
    [TestFixture]
    public class DigitalAccessTests : SearchControllerTestsBase
    {
        [SetUp]
        public void SetupDigitalAccessUser()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test@user"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "Test"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "User")
            }));

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        }

        [Test]
        public async Task CertificatePrintRequest_Get_OrchestratorReturnsViewModel_ReturnsViewWithModel()
        {
            // Arrange
            var reference = "REF123";
            var certId = Guid.NewGuid();
            var vm = new CertificatePrintRequestViewModel
            {
                ReferenceNumber = reference,
                CourseName = "Course Z",
                CertificateId = certId,
                CertificateType = CertificateType.Standard,
                FirstName = "Sam",
                LastName = "Smith",
                Uln = 321,
                StandardCode = 789
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetCertificatePrintRequestViewModel(reference, It.IsAny<string>()))
                .ReturnsAsync(vm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.CertificatePrintRequest(reference);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<CertificatePrintRequestViewModel>().Subject;
            model.ReferenceNumber.Should().Be(reference);
            model.CourseName.Should().Be("Course Z");
            model.CertificateId.Should().Be(certId);
            model.FirstName.Should().Be("Sam");
            model.LastName.Should().Be("Smith");
            model.Uln.Should().Be(321);
            model.StandardCode.Should().Be(789);
        }

        [Test]
        public async Task CertificatePrintRequest_Get_OrchestratorReturnsNull_RedirectsToSearch()
        {
            // Arrange
            var reference = "REF999";

            _digitalAccessOrchestratorMock.Setup(x => x.GetCertificatePrintRequestViewModel(reference, It.IsAny<string>()))
                .ReturnsAsync((SFA.DAS.AdminService.Web.ViewModels.DigitalAccess.CertificatePrintRequestViewModel)null);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.CertificatePrintRequest(reference);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.DigitalAccessReferenceSearchRouteGet);
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsReprint_RedirectsToCertificatePrintRequest()
        {
            // Arrange
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = "ABC123" };
            var resultVm = new DigitalAccessReferenceSearchViewModel
            {
                ReferenceNumber = vm.ReferenceNumber,
                ActionType = ActionType.Reprint
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync(resultVm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.CertificatePrintRequestRouteGet);
            redirect.RouteValues.Should().ContainKey("referenceNumber");
            redirect.RouteValues["referenceNumber"].Should().Be(vm.ReferenceNumber);
        }

        [Test]
        public void DigitalAccessReferenceSearch_Get_ReturnsViewWithNewViewModel()
        {
            // Arrange
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);

            // Act
            var result = controller.DigitalAccessReferenceSearch();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<DigitalAccessReferenceSearchViewModel>().Which.ReferenceNumber.Should().BeEmpty();
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_InvalidModel_RedirectsToGet()
        {
            // Arrange
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            controller.ModelState.AddModelError("ReferenceNumber", "Enter reference");
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = "ABC123" };

            // Act
            var result = await controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.DigitalAccessReferenceSearchRouteGet);
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsNull_AddsModelErrorAndReturnsView()
        {
            // Arrange
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = "ABC123" };
            _digitalAccessOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync((DigitalAccessReferenceSearchViewModel)null);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.DigitalAccessReferenceSearchRouteGet);
            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState["ReferenceNumber"].Errors[0].ErrorMessage.Should().Be("No records found with this reference number");
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsNotFound_RedirectsToUserNotFound()
        {
            // Arrange
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = "ABC123" };
            var resultVm = new DigitalAccessReferenceSearchViewModel
            {
                ReferenceNumber = vm.ReferenceNumber,
                ActionType = ActionType.NotFound
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync(resultVm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.UserNotFoundRouteGet);
            redirect.RouteValues.Should().ContainKey("referenceNumber");
            redirect.RouteValues["referenceNumber"].Should().Be(vm.ReferenceNumber);
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsContact_RedirectsToCertificateChangeRequest()
        {
            // Arrange
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = "ABC123" };
            var resultVm = new DigitalAccessReferenceSearchViewModel
            {
                ReferenceNumber = vm.ReferenceNumber,
                ActionType = ActionType.Contact
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync(resultVm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.CertificateChangeRequestRouteGet);
            redirect.RouteValues.Should().ContainKey("referenceNumber");
            redirect.RouteValues["referenceNumber"].Should().Be(vm.ReferenceNumber);
        }

        [Test]
        public async Task CertificateChangeRequest_Get_OrchestratorReturnsViewModel_ReturnsViewWithModel()
        {
            // Arrange
            var reference = "REF123";
            var vm = new CertificateChangeRequestViewModel
            {
                ReferenceNumber = reference,
                FirstName = "Amy",
                LastName = "Adams"
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetCertificateChangeRequestViewModel(reference, It.IsAny<string>()))
                .ReturnsAsync(vm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.CertificateChangeRequest(reference);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<CertificateChangeRequestViewModel>().Subject;
            model.ReferenceNumber.Should().Be(reference);
            model.FirstName.Should().Be("Amy");
            model.LastName.Should().Be("Adams");
        }

        [Test]
        public async Task CertificateChangeRequest_Get_OrchestratorReturnsNull_RedirectsToSearch()
        {
            // Arrange
            var reference = "REF999";

            _digitalAccessOrchestratorMock.Setup(x => x.GetCertificateChangeRequestViewModel(reference, It.IsAny<string>()))
                .ReturnsAsync((CertificateChangeRequestViewModel)null);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.CertificateChangeRequest(reference);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.DigitalAccessReferenceSearchRouteGet);
        }

        [Test]
        public async Task UserNotFound_Get_OrchestratorReturnsViewModel_ReturnsViewWithModel()
        {
            // Arrange
            var reference = "REF123";
            var vm = new UserNotFoundViewModel
            {
                ReferenceNumber = reference,
                FirstName = "Amy",
                LastName = "Adams"
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetUserNotFoundViewModel(reference, It.IsAny<string>()))
                .ReturnsAsync(vm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.UserNotFound(reference);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<UserNotFoundViewModel>().Subject;
            model.ReferenceNumber.Should().Be(reference);
            model.FirstName.Should().Be("Amy");
            model.LastName.Should().Be("Adams");
        }

        [Test]
        public async Task UserNotFound_Get_OrchestratorReturnsNull_RedirectsToSearch()
        {
            // Arrange
            var reference = "REF999";

            _digitalAccessOrchestratorMock.Setup(x => x.GetUserNotFoundViewModel(reference, It.IsAny<string>()))
                .ReturnsAsync((UserNotFoundViewModel)null);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.UserNotFound(reference);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.DigitalAccessReferenceSearchRouteGet);
        }

        [Test]
        public async Task UserNotMatched_Get_OrchestratorReturnsNull_RedirectsToSearch()
        {
            // Arrange
            var reference = "REF999";

            _digitalAccessOrchestratorMock.Setup(x => x.GetUserNotMatchedViewModel(reference))
                .ReturnsAsync((UserNotMatchedViewModel)null);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.UserNotMatched(reference);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.DigitalAccessReferenceSearchRouteGet);
            _digitalAccessOrchestratorMock.Verify(x => x.GetUserNotMatchedViewModel(reference), Moq.Times.Once);
        }

        [Test]
        public async Task UserNotMatched_Get_OrchestratorReturnsViewModel_ReturnsViewWithModel()
        {
            // Arrange
            var reference = "REF123";
            var vm = new UserNotMatchedViewModel
            {
                ReferenceNumber = reference,
                FirstName = "Amy",
                LastName = "Adams",
                IsUserLocked = true
            };

            vm.History.Add(new UserAccessHistoryItem
            {
                FormattedActionTime = "10:00",
                ActionType = ActionType.Reprint,
                ReferenceNumber = reference,
                IsUnlocked = false,
                UnlockedBy = string.Empty,
                FormattedUnlockedTime = string.Empty,
                TagClass = "tag",
                TagText = "text"
            });

            _digitalAccessOrchestratorMock.Setup(x => x.GetUserNotMatchedViewModel(reference))
                .ReturnsAsync(vm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.UserNotMatched(reference);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<UserNotMatchedViewModel>().Subject;
            model.ReferenceNumber.Should().Be(reference);
            model.FirstName.Should().Be("Amy");
            model.LastName.Should().Be("Adams");
            model.IsUserLocked.Should().BeTrue();
            model.History.Should().HaveCount(1);
            _digitalAccessOrchestratorMock.Verify(x => x.GetUserNotMatchedViewModel(reference), Moq.Times.Once);
        }

        [Test]
        public async Task RestoreAccess_Get_OrchestratorReturnsViewModel_ReturnsViewWithModel()
        {
            // Arrange
            var reference = "REF123";
            var vm = new RestoreAccessViewModel
            {
                ReferenceNumber = reference,
                UserId = Guid.NewGuid(),
                UserActionId = 42
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetRestoreAccessViewModel(reference))
                .ReturnsAsync(vm);

            // Act
            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);
            var result = await controller.RestoreAccess(reference);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<RestoreAccessViewModel>().Subject;
            model.ReferenceNumber.Should().Be(reference);
            model.UserActionId.Should().Be(42);
        }

        [Test]
        public async Task RestoreAccessPost_PostWithReference_CallsUnlockAndRedirects()
        {
            // Arrange
            var reference = "REF123";
            var userId = Guid.NewGuid();
            var userActionId = 42L;

            var restoreVm = new RestoreAccessViewModel
            {
                ReferenceNumber = reference,
                UserId = userId,
                UserActionId = userActionId
            };

            _digitalAccessOrchestratorMock.Setup(x => x.GetRestoreAccessViewModel(reference)).ReturnsAsync(restoreVm);
            _digitalAccessOrchestratorMock.Setup(x => x.UnlockUser(userId, It.IsAny<string>(), userActionId)).Returns(Task.CompletedTask).Verifiable();

            var controller = new DigitalAccessController(_httpContextAccessorMock.Object, _digitalAccessOrchestratorMock.Object);

            // Act
            var result = await controller.RestoreAccessPost(reference);

            // Assert
            _digitalAccessOrchestratorMock.Verify(x => x.UnlockUser(userId, It.IsAny<string>(), userActionId), Times.Once);

            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(DigitalAccessController.UserNotMatchedRouteGet);
            redirect.RouteValues.Should().ContainKey("referenceNumber");
            redirect.RouteValues["referenceNumber"].Should().Be(reference);
        }
    }
}
