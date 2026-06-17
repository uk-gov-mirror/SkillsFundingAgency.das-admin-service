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
    public class SearchControllerDigitalAccessReferenceTests : SearchControllerTestsBase
    {
        [Test]
        public void DigitalAccessReferenceSearch_Get_ReturnsViewWithNewViewModel()
        {
            // Act
            var result = _controller.DigitalAccessReferenceSearch();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<DigitalAccessReferenceViewModel>().Which.ReferenceNumber.Should().BeEmpty();
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_InvalidModel_RedirectsToGet()
        {
            // Arrange
            _controller.ModelState.AddModelError("ReferenceNumber", "Enter reference");
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = "ABC123" };

            // Act
            var result = await _controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(SearchController.DigitalAccessReferenceSearchRouteGet);
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsNull_AddsModelErrorAndReturnsView()
        {
            // Arrange
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = "ABC123" };
            _searchOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync((DigitalAccessReferenceViewModel)null);

            // Act
            var result = await _controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(SearchController.DigitalAccessReferenceSearchRouteGet);
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState["ReferenceNumber"].Errors[0].ErrorMessage.Should().Be("No records found with this reference number");
        }

        // TODO: This should be updated as part of upcoming tickets
        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsKnownAction_RedirectsToUserNotFound()
        {
            // Arrange
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = "ABC123" };
            var resultVm = new DigitalAccessReferenceViewModel
            {
                ReferenceNumber = vm.ReferenceNumber,
                Result = new UserActionResponse { ActionType = ActionType.NotFound }
            };

            _searchOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync(resultVm);

            // Act
            var result = await _controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(SearchController.UserNotFoundRouteGet);
            redirect.RouteValues.Should().ContainKey("referenceNumber");
            redirect.RouteValues["referenceNumber"].Should().Be(vm.ReferenceNumber);
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsOtherAction_ReturnsViewWithResultVm()
        {
            // Arrange
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = "ABC123" };
            var resultVm = new DigitalAccessReferenceViewModel
            {
                ReferenceNumber = vm.ReferenceNumber,
                Result = new UserActionResponse { ActionType = (ActionType)999 }
            };

            _searchOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync(resultVm);

            // Act
            var result = await _controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            view.Model.Should().Be(resultVm);
        }

        [Test]
        public async Task DigitalAccessReferenceSearch_Post_OrchestratorReturnsNotFound_RedirectsToUserNotFound()
        {
            // Arrange
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = "ABC123" };
            var resultVm = new DigitalAccessReferenceViewModel
            {
                ReferenceNumber = vm.ReferenceNumber,
                Result = new UserActionResponse { ActionType = ActionType.NotFound }
            };

            _searchOrchestratorMock.Setup(x => x.GetDigitalAccessReferenceViewModel(vm.ReferenceNumber, It.IsAny<string>()))
                .ReturnsAsync(resultVm);

            // Act
            var result = await _controller.DigitalAccessReferenceSearch(vm);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToRouteResult>().Subject;
            redirect.RouteName.Should().Be(SearchController.UserNotFoundRouteGet);
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

            _searchOrchestratorMock.Setup(x => x.GetUserNotFoundViewModel(reference, It.IsAny<string>()))
                .ReturnsAsync(vm);

            // Act
            var result = await _controller.UserNotFound(reference);

            // Assert
            var view = result.Should().BeOfType<ViewResult>().Subject;
            var model = view.Model.Should().BeOfType<UserNotFoundViewModel>().Subject;
            model.ReferenceNumber.Should().Be(reference);
            model.FirstName.Should().Be("Amy");
            model.LastName.Should().Be("Adams");
        }
    }
}
