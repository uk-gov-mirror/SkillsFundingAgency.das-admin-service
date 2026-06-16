using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.Search;

namespace SFA.DAS.AdminService.Web.UnitTests.Validators
{
    [TestFixture]
    public class DigitalAccessReferenceViewModelValidatorTests
    {
        private DigitalAccessReferenceViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new DigitalAccessReferenceViewModelValidator();
        }

        [Test]
        public void ReferenceNumber_Null_HasError()
        {
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = null };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.ReferenceNumber).WithErrorMessage("Enter reference number");
        }

        [Test]
        public void ReferenceNumber_Empty_HasError()
        {
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = string.Empty };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.ReferenceNumber).WithErrorMessage("Enter reference number");
        }

        [Test]
        public void ReferenceNumber_NonAlphanumeric_HasError()
        {
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = "ABC-123" };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.ReferenceNumber).WithErrorMessage("Digital access reference numbers must be alphanumeric");
        }

        [Test]
        public void ReferenceNumber_Alphanumeric_NoError()
        {
            var vm = new DigitalAccessReferenceViewModel { ReferenceNumber = "ABC123" };
            var result = _validator.TestValidate(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.ReferenceNumber);
        }
    }
}
