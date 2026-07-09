using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;

namespace SFA.DAS.AdminService.Web.UnitTests.Validators
{
    [TestFixture]
    public class DigitalAccessReferenceSearchViewModelValidatorTests
    {
        private DigitalAccessReferenceSearchViewModelValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new DigitalAccessReferenceSearchViewModelValidator();
        }

        [Test]
        public void ReferenceNumber_Null_HasError()
        {
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = null };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.ReferenceNumber).WithErrorMessage("Enter reference");
        }

        [Test]
        public void ReferenceNumber_Empty_HasError()
        {
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = string.Empty };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.ReferenceNumber).WithErrorMessage("Enter reference");
        }

        [Test]
        public void ReferenceNumber_NonAlphanumeric_HasError()
        {
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = "ABC-123" };
            var result = _validator.TestValidate(vm);
            result.ShouldHaveValidationErrorFor(x => x.ReferenceNumber).WithErrorMessage("Digital access reference must be alphanumeric");
        }

        [Test]
        public void ReferenceNumber_Alphanumeric_NoError()
        {
            var vm = new DigitalAccessReferenceSearchViewModel { ReferenceNumber = "ABC123" };
            var result = _validator.TestValidate(vm);
            result.ShouldNotHaveValidationErrorFor(x => x.ReferenceNumber);
        }
    }
}
