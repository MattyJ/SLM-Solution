using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class EditServiceComponentLevel2ViewModelValidatorTests
    {
        private EditServiceComponentLevel2ViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new EditServiceComponentLevel2ViewModelValidator();
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SdoTypeNotSelected_IsValid()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel(),
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SdoTypeNotSelectedSdoNotesSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationNotes = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.ServiceComponentServiceDeliveryOrganisationTypeIdMustBeSupplied, result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SdoTypeSelectedSdoNotesNotSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverGroup = new EditResolverResolverGroupViewModel()

            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SdoTypeSelectedSdoNotesSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "Notes are now valid with a type selected",
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }


        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SduTypeNotSelectedSduNotesSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "Notes are now valid with a type selected",
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitNotes = "XXX"
                },
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.ServiceComponentServiceDeliveryUnitTypeIdMustBeSupplied, result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SduTypeSelectedSduNotesNotSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "Notes are now valid with a type selected",
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = 1
                },
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SduTypeSelectedSduNotesSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "Notes are now valid with a type selected",
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = 1,
                    ServiceDeliveryUnitNotes = "Notes are now valid with a type selected"
                },
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }


        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SdoAndSduSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "Notes are now valid with a type selected",
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = 1,
                    ServiceDeliveryUnitNotes = "Notes are now valid with a type selected"
                },
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SdoTypeNotSelectedSduSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel(),
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = 1
                },
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.ServiceComponentServiceDeliveryOrganisationTypeIdMustBeSupplied, result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void EditServiceComponentLevel2ViewModelValidator_Validate_SdoTypeNotSelectedResolverSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel(),
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverGroup = new EditResolverResolverGroupViewModel()
                {
                    ResolverGroupTypeId = 1
                }
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.ServiceComponentServiceDeliveryOrganisationTypeIdMustBeSupplied, result.Errors[0].ErrorMessage);
        }

    }
}