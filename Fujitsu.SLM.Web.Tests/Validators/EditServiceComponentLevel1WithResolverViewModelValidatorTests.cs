using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class EditServiceComponentLevel1WithResolverViewModelValidatorTests
    {
        private EditServiceComponentLevel1WithResolverViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new EditServiceComponentLevel1WithResolverViewModelValidator();
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithResolverViewModelValidator_Validate_SdoTypeNotSelectedSdoNotesSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationNotes = "XXX"
                }
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.ServiceComponentServiceDeliveryOrganisationTypeIdMustBeSupplied, result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithResolverViewModelValidator_Validate_SdoTypeSelectedSdoNotesNotSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1
                }
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithResolverViewModelValidator_Validate_SdoTypeSelectedSdoNotesSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel()
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "Notes are now valid with a type selected",
                }

            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithResolverViewModelValidator_Validate_SduTypeNotSelectedSduNotesSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitNotes = "XXX"
                },
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "XXX"
                }
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.ServiceComponentServiceDeliveryUnitTypeIdMustBeSupplied, result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithResolverViewModelValidator_Validate_SduTypeSelectedSduNotesNotSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = 1
                },
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "XXX"
                }
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithResolverViewModelValidator_Validate_SduTypeSelectedSduNotesSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = 1,
                    ServiceDeliveryUnitNotes = "Notes are now valid with a type selected"
                },
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "XXX"
                }
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        
        [TestMethod]
        public void EditServiceComponentLevel1WithResolverViewModelValidator_Validate_SdoAndSduSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitTypeId = 1,
                    ServiceDeliveryUnitNotes = "Notes are now valid with a type selected"
                },
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel()
                {
                    ServiceDeliveryOrganisationTypeId = 1,
                    ServiceDeliveryOrganisationNotes = "Notes are now valid with a type selected",
                }

            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }


    }
}
