using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public class DigitalAccessOrchestrator : IDigitalAccessOrchestrator
    {
        private readonly IMediator _mediator;

        public DigitalAccessOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<DigitalAccessReferenceSearchViewModel> GetDigitalAccessReferenceViewModel(string reference, string username)
        {
            var result = await _mediator.Send(new CheckUserActionByCodeCommand { Code = reference, Username = username });

            if (result == null)
                return null;

            var vm = new DigitalAccessReferenceSearchViewModel
            {
                ReferenceNumber = reference,
                ActionType = result.ActionType
            };

            return vm;
        }

        public async Task<UserNotFoundViewModel> GetUserNotFoundViewModel(string reference, string username)
        {
            var result = await _mediator.Send(new CheckUserActionByCodeCommand { Code = reference, Username = username });

            if (result == null)
                return new UserNotFoundViewModel { ReferenceNumber = reference };

            return new UserNotFoundViewModel
            {
                ReferenceNumber = reference,
                FirstName = result.GivenNames,
                LastName = result.FamilyName
            };
        }
    }
}
