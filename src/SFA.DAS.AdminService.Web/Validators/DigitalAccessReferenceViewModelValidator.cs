using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.Search;
using System.Text.RegularExpressions;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class DigitalAccessReferenceViewModelValidator : AbstractValidator<DigitalAccessReferenceViewModel>
    {
        public DigitalAccessReferenceViewModelValidator()
        {
            RuleFor(x => x.ReferenceNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Enter reference number")
                .Matches(new Regex("^[a-zA-Z0-9]+$")).WithMessage("Digital access reference numbers must be alphanumeric");
        }
    }
}
