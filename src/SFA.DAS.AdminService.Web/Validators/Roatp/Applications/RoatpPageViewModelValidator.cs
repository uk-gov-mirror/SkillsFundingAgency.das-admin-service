using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Applications;

namespace SFA.DAS.AdminService.Web.Validators.Roatp.Applications
{
    public class RoatpPageViewModelValidator : AbstractValidator<RoatpPageViewModel>
    {
        public RoatpPageViewModelValidator()
        {
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                if (string.IsNullOrWhiteSpace(vm.ReviewStatus))
                {
                    context.AddFailure("ReviewStatus", "Select an outcome for this page");
                }
                else if (vm.ReviewStatus == "Fail" && string.IsNullOrWhiteSpace(vm.ReviewComments))
                {
                    context.AddFailure("ReviewComments", "Enter why the page has a fail outcome");
                }
            });
        }
    }
}
