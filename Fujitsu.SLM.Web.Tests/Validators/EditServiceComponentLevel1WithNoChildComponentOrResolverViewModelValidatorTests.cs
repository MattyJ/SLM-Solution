using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidatorTests
    {
        private EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator();
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoTypeNotSelected_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel(),
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoTypeNotSelectedSdoNotesSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoTypeSelectedSdoNotesNotSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoTypeSelectedSdoNotesSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SduTypeNotSelectedSduNotesSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SduTypeSelectedSduNotesNotSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SduTypeSelectedSduNotesSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel
                {
                    ServiceDeliveryOrganisationTypeId = 1,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoAndSduSupplied_IsValid()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                InputMode = 0,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoTypeNotSelectedSduSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
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
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoTypeNotSelectedSduNotesSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel(),
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel
                {
                    ServiceDeliveryUnitNotes = "Some notes"
                },
                ResolverGroup = new EditResolverResolverGroupViewModel()
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.ServiceComponentServiceDeliveryUnitTypeIdMustBeSupplied, result.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator_Validate_SdoTypeNotSelectedResolverGroupSupplied_IsValidationError()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                ComponentName = new EditServiceComponentNameViewModel
                {
                    ComponentName = "XXX"
                },
                ServiceActivities = new ServiceActivityViewModel
                {
                    ServiceActivities = "- Activity One"
                },
                InputMode = 0,
                ResolverServiceDeliveryOrganisation = new EditResolverServiceDeliveryOrganisationViewModel(),
                ResolverServiceDeliveryUnit = new EditResolverServiceDeliveryUnitViewModel(),
                ResolverGroup = new EditResolverResolverGroupViewModel
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