using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Application.Commands.GetUserAllActivityByCode;
using SFA.DAS.AdminService.Common.Models;
using System;

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

        public async Task<UserNotMatchedViewModel> GetUserNotMatchedViewModel(string reference)
        {
            GetUserAllActivityByCodeCommandResult response = await _mediator.Send(new GetUserAllActivityByCodeCommand { Code = reference });

            if (response == null)
                return null;

            var history = new List<UserAccessHistoryItem>();

            if (response.UserActions != null && response.UserActions.Count > 0)
            {
                foreach (var ua in response.UserActions.OrderByDescending(u => u.ActionTime))
                {
                    var parsedActionType = ActionType.NotMatched;
                    if (!string.IsNullOrEmpty(ua.ActionType) && Enum.TryParse<ActionType>(ua.ActionType, true, out var at))
                    {
                        parsedActionType = at;
                    }

                    var item = new UserAccessHistoryItem
                    {
                        FormattedActionTime = ua.ActionTime.ToUkDateTimeString(),
                        ActionType = parsedActionType,
                        ReferenceNumber = ua.ActionCode
                    };

                    if (ua.UserMatches != null && ua.UserMatches.Count > 0)
                    {
                        foreach (var um in ua.UserMatches.OrderBy(u => u.EventTime))
                        {
                                item.Attempts.Add(new UserAttempt
                                {
                                    FormattedEventTime = um.EventTime.ToUkDateTimeString(),
                                    Uln = um.Uln?.ToString() ?? Constants.DigitalAccessConstants.Unknown,
                                    CourseName = string.IsNullOrWhiteSpace(um.CourseName) ? Constants.DigitalAccessConstants.Unknown : um.CourseName,
                                    DateAwarded = um.DateAwarded?.ToString() ?? Constants.DigitalAccessConstants.Unknown,
                                    ProviderName = string.IsNullOrWhiteSpace(um.ProviderName) ? Constants.DigitalAccessConstants.Unknown : um.ProviderName
                                });
                        }
                    }

                    if (ua.AdminActions != null && ua.AdminActions.Count > 0)
                    {
                        var unlocked = ua.AdminActions.Find(a =>
                        {
                            if (!Enum.TryParse<AdminActionType>(a.Action, true, out var adminActionType)) return false;
                            return adminActionType == AdminActionType.Unlocked && a.ActionTime > ua.ActionTime;
                        });

                        if (unlocked != null)
                        {
                            item.IsUnlocked = true;
                            item.UnlockedBy = unlocked.Username;
                            item.FormattedUnlockedTime = unlocked.ActionTime.ToUkDateTimeString();
                        }
                    }

                    item.TagClass = item.IsUnlocked ? Constants.DigitalAccessConstants.TagClassUnlocked : Constants.DigitalAccessConstants.TagClassLocked;
                    item.TagText = item.IsUnlocked ? Constants.DigitalAccessConstants.TagTextUnlocked : Constants.DigitalAccessConstants.TagTextLocked;

                    history.Add(item);
                }
            }

            var vm = new UserNotMatchedViewModel
            {
                ReferenceNumber = reference,
                FirstName = response.UserActions?.FirstOrDefault()?.GivenNames,
                LastName = response.UserActions?.FirstOrDefault()?.FamilyName,
                History = history,
                IsUserLocked = response.IsLocked
            };

            return vm;
        }
    }
}
