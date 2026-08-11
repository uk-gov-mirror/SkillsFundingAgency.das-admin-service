using FluentValidation;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;
using System.Text.RegularExpressions;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class DigitalAccessReferenceSearchViewModelValidator : AbstractValidator<DigitalAccessReferenceSearchViewModel>
    {
        public DigitalAccessReferenceSearchViewModelValidator()
        {
            RuleFor(x => x.ReferenceNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Enter reference")
                .Matches(new Regex("^[a-zA-Z0-9]+$")).WithMessage("Digital access reference must be alphanumeric");
        }
    }
}
