using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AdminService.Web.Controllers;
using SFA.DAS.AdminService.Common.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class DigitalAccessTests : SearchControllerTestsBase
    {
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
    }
}
