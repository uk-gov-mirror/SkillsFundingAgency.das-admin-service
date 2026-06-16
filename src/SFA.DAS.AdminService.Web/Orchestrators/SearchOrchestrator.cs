using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System.Linq;
using SFA.DAS.AdminService.Common.Models;
using UserActionResponse = SFA.DAS.AdminService.Web.ViewModels.Search.UserActionResponse;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using System;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public class SearchOrchestrator : ISearchOrchestrator
    {
        private readonly IMediator _mediator;

        public SearchOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<DigitalAccessReferenceViewModel> FindUserActionByReference(string reference, string username)
        {
            var result = await _mediator.Send(new CheckUserActionByCodeCommand { Code = reference, Username = username });

            if (result == null)
                return new DigitalAccessReferenceViewModel { ReferenceNumber = reference };

            var vm = new DigitalAccessReferenceViewModel
            {
                ReferenceNumber = reference,
                Result = new UserActionResponse
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    ActionType = result.ActionType,
                    ActionTime = result.ActionTime,
                    ActionStatus = result.ActionStatus,
                    Uln = result.Uln,
                    FamilyName = result.FamilyName,
                    GivenNames = result.GivenNames,
                    CertificateId = result.CertificateId,
                    CertificateType = result.CertificateType,
                    CourseName = result.CourseName,
                    AdminActions = result.AdminActions?.Select(a => new AdminAction { Username = a.Username, ActionTime = a.ActionTime, Action = Enum.Parse<AdminActionType>(a.Action, true) }).ToList()
                }
            };

            return vm;
        }
    }
}
