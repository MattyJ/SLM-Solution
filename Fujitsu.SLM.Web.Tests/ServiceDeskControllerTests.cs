using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using KellermanSoftware.CompareNetObjects;
using Kendo.Mvc.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceDeskControllerTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IAppUserContext> _mockAppUserContext;

        private Mock<IServiceDeskService> _mockServiceDeskService;
        private Mock<IInputTypeRefDataService> _mockInputTypeRefDataService;
        private Mock<ITemplateService> _mockTemplateService;

        private IInputTypeRefDataService _inputTypeRefDataService;
        private IServiceDeskService _serviceDeskService;

        private ServiceDeskController _controller;
        private ServiceDeskController _controllerWithMockedServices;
        private AppContext _appContext;
        private Mock<IRequestManager> _mockRequestManager;
        private Mock<NameValueCollection> _mockNameValueCollection;

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;
        private Mock<IRepository<InputTypeRefData>> _mockInputTypeRefDataRepository;

        private List<ServiceDesk> _serviceDesks;
        private List<DeskInputType> _serviceDeskInputTypes;
        private List<InputTypeRefData> _inputTypeRefData;
        private ServiceDesk _serviceDesk;

        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";

        private const int CustomerId = 1;

        private ControllerContextMocks _controllerContextMocks;


        [TestInitialize]
        public void Initialize()
        {
            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            };

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserNameOne);

            _mockRequestManager = new Mock<IRequestManager>();
            _mockNameValueCollection = new Mock<NameValueCollection>();

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.RequestManager).Returns(_mockRequestManager.Object);
            _mockRequestManager.Setup(s => s.Form).Returns(_mockNameValueCollection.Object);

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            var dateTimeNow = DateTime.Now;

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id = 1,
                    DeskName = "Desk A",
                    DeskInputTypes = new List<DeskInputType>
                    {
                        UnitTestHelper.GenerateRandomData<DeskInputType>(),
                        UnitTestHelper.GenerateRandomData<DeskInputType>()
                    },
                     ServiceDomains = new List<ServiceDomain>(),
                    CustomerId = 1,
                    Customer = new Customer
                    {
                        Id =1,
                        CustomerName = "3663",
                        CustomerNotes = "Some 3663 Customer Notes.",
                        Active = true,
                        AssignedArchitect = UserNameOne,
                        InsertedBy = UserNameOne,
                        InsertedDate = dateTimeNow,
                        UpdatedBy = UserNameOne,
                        UpdatedDate = dateTimeNow
                    }
                },
                new ServiceDesk
                {
                    Id = 2,
                    DeskName = "Desk A",
                    DeskInputTypes = new List<DeskInputType>
                    {
                        UnitTestHelper.GenerateRandomData<DeskInputType>(),
                        UnitTestHelper.GenerateRandomData<DeskInputType>()
                    },
                    ServiceDomains = new List<ServiceDomain>(),
                    CustomerId = 2,
                    Customer = new Customer
                    {
                        Id =1,
                        CustomerName = "3663",
                        CustomerNotes = "Some 3663 Customer Notes.",
                        Active = true,
                        AssignedArchitect = UserNameOne,
                        InsertedBy = UserNameOne,
                        InsertedDate = dateTimeNow,
                        UpdatedBy = UserNameOne,
                        UpdatedDate = dateTimeNow
                    }
                },
                new ServiceDesk
                {
                    Id = 3,
                    DeskName = "Desk C",
                    DeskInputTypes = new List<DeskInputType>(),
                    ServiceDomains = new List<ServiceDomain>(),
                    CustomerId = 1,
                    Customer = new Customer
                    {
                        Id =1,
                        CustomerName = "3663",
                        CustomerNotes = "Some 3663 Customer Notes.",
                        Active = true,
                        AssignedArchitect = UserNameOne,
                        InsertedBy = UserNameOne,
                        InsertedDate = dateTimeNow,
                        UpdatedBy = UserNameOne,
                        UpdatedDate = dateTimeNow
                    }
                },
                new ServiceDesk
                {
                    Id = 4,
                    DeskName = "Desk D",
                    CustomerId = 4
                },
            };

            _inputTypeRefData = new List<InputTypeRefData>
            {
                new InputTypeRefData
                {
                    Id = 1,
                    InputTypeName = "Input A",
                    Default = true,
                    SortOrder = 5
                },
                new InputTypeRefData
                {
                    Id = 2,
                    InputTypeName = "Input B",
                    SortOrder = 5
                },
                new InputTypeRefData
                {
                    Id = 3,
                    InputTypeName = "Input C",
                    Default = true,
                    SortOrder = 5
                },
                new InputTypeRefData
                {
                    Id = 4,
                    InputTypeName = "Input D",
                    Default = true,
                    SortOrder = 5
                },
            };

            _serviceDeskInputTypes = new List<DeskInputType>
            {
                new DeskInputType
                {
                    Id = 1,
                    InputTypeRefData = _inputTypeRefData.Find(x => x.Id == 4),
                    ServiceDesk = _serviceDesks.Find(x => x.Id == 2)
                },
                new DeskInputType
                {
                    Id = 2,
                    InputTypeRefData = _inputTypeRefData.Find(x => x.Id == 2),
                    ServiceDesk = _serviceDesks.Find(x => x.Id == 1)
                },
                new DeskInputType
                {
                    Id = 3,
                    InputTypeRefData = _inputTypeRefData.Find(x => x.Id == 3),
                    ServiceDesk = _serviceDesks.Find(x => x.Id == 1)
                }
            };


            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);
            _mockInputTypeRefDataRepository = MockRepositoryHelper.Create(_inputTypeRefData, (entity, id) => entity.Id == (int)id);
            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_serviceDeskInputTypes, (entity, id) => entity.Id == (int)id);
            _serviceDeskService = new ServiceDeskService(
                _mockServiceDeskRepository.Object,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);

            _inputTypeRefDataService = new InputTypeRefDataService(
                _mockInputTypeRefDataRepository.Object, _mockDeskInputTypeRepository.Object, _mockUnitOfWork.Object);

            _mockTemplateService = new Mock<ITemplateService>();

            _controller = new ServiceDeskController(_serviceDeskService,
                _inputTypeRefDataService,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);

            _serviceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(x =>
            {
                x.DeskInputTypes = new List<DeskInputType>
                {
                    UnitTestHelper.GenerateRandomData<DeskInputType>(),
                    UnitTestHelper.GenerateRandomData<DeskInputType>()
                };
            });

            _mockServiceDeskService = new Mock<IServiceDeskService>();
            _mockServiceDeskService.Setup(s => s.GetById(1)).Returns(_serviceDesks[0]);
            _mockServiceDeskService.Setup(s => s.GetById(2)).Returns(_serviceDesks[1]);
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), 1)).Returns(_serviceDesks[0]);
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), 2)).Returns(_serviceDesk);
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), 3)).Returns(_serviceDesks[2]);
            _mockServiceDeskService.Setup(s => s.GetByCustomer(It.Is<int>(m => m == 666)))
                .Returns(_serviceDesks.Where(w => w.Id == 1).AsQueryable());
            _mockServiceDeskService.Setup(s => s.GetByCustomer(It.Is<int>(m => m != 666)))
                .Returns(_serviceDesks.AsQueryable());

            _mockInputTypeRefDataService = new Mock<IInputTypeRefDataService>();
            _mockInputTypeRefDataService.Setup(s => s.GetById(1)).Returns(_inputTypeRefData[0]);
            _mockInputTypeRefDataService.Setup(s => s.GetById(2)).Returns(_inputTypeRefData[1]);
            _mockInputTypeRefDataService.Setup(s => s.All()).Returns(_inputTypeRefData);

            _controllerWithMockedServices = new ServiceDeskController(_mockServiceDeskService.Object,
                _mockInputTypeRefDataService.Object,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);

            _controllerContextMocks = new ControllerContextMocks();
            _controllerWithMockedServices.ControllerContext = new ControllerContext(_controllerContextMocks.MockHttpContextBase.Object,
                new RouteData(),
                _controllerWithMockedServices);

            if (RouteTable.Routes.Count == 0)
            {
                RouteConfig.RegisterRoutes(RouteTable.Routes);
            }

            _controllerWithMockedServices.Url = new UrlHelper(new RequestContext(_controllerContextMocks.MockHttpContextBase.Object, new RouteData()));

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskController_Constructor_NoServiceDeskServiceThrowsException()
        {
            new ServiceDeskController(
                null,
                _mockInputTypeRefDataService.Object,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskController_Constructor_NoInputTypeServiceThrowsException()
        {
            new ServiceDeskController(
                _mockServiceDeskService.Object,
                null,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskController_Constructor_NoTemplateServiceThrowsException()
        {
            new ServiceDeskController(
                _mockServiceDeskService.Object,
                _mockInputTypeRefDataService.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskController_Constructor_NoContextManagerThrowsException()
        {
            new ServiceDeskController(
                _mockServiceDeskService.Object,
                _mockInputTypeRefDataService.Object,
                _mockTemplateService.Object,
                null,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskController_Constructor_NoAppUserContextThrowsException()
        {
            new ServiceDeskController(
                _mockServiceDeskService.Object,
                _mockInputTypeRefDataService.Object,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                null
                );
        }

        #endregion


        [TestMethod]
        public void ServiceDeskController_Index_Get_ReturnsViewResult()
        {
            var result = _controller.Index(NavigationLevelNames.LevelOne) as ViewResult;

            Assert.IsNotNull(result);
        }

        public void ServiceDeskController_FujitsuDomains_ReturnsView_IsPartialView()
        {
            var result = _controller.FujitsuDomains(0);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void ServiceDeskController_FujitsuDomains_ReturnsView_IsFujitsuDomainsView()
        {
            var result = _controller.FujitsuDomains(0) as PartialViewResult;
            Assert.AreEqual("_FujitsuDomains", result.ViewName);
        }

        public void ServiceDeskController_CustomerServices_ReturnsView_IsPartialView()
        {
            var result = _controller.CustomerServices(0);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void ServiceDeskController_CustomerServices_ReturnsView_IsCustomerServicesView()
        {
            var result = _controller.CustomerServices(0) as PartialViewResult;
            Assert.AreEqual("_CustomerServices", result.ViewName);
        }

        public void ServiceDeskController_ServiceOrganisation_Fujitsu_ReturnsView_IsPartialView()
        {
            var result = _controller.ServiceOrganisation(1, Constants.Diagram.FujitsuServiceOrganisation);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        public void ServiceDeskController_ServiceOrganisation_Customer_ReturnsView_IsPartialView()
        {
            var result = _controller.ServiceOrganisation(1, Constants.Diagram.CustomerServiceOrganisation);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        public void ServiceDeskController_ServiceOrganisation_CustomerThirdParty_ReturnsView_IsPartialView()
        {
            var result = _controller.ServiceOrganisation(1, Constants.Diagram.CustomerThirdPartyServiceOrganisation);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void ServiceDeskController_ReadAjaxServiceDeskGrid_ReturnsServiceDeskViewModel()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _controller.ReadAjaxServiceDeskGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceDeskViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(_serviceDesks.Count(x => x.CustomerId == CustomerId), model.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_CreateAjaxServiceDeskGrid_CallsServiceDeskServiceCreate()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new ServiceDeskViewModel()
            {
                Id = 5,
                DeskName = "Desk E",
                CustomerId = 1,
                CustomerName = "3663",
                DeskInputTypes = new List<InputTypeRefData>(),
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxServiceDeskGrid(request, insert);

            #endregion

            #region Assert

            _mockServiceDeskService.Verify(x => x.Create(It.IsAny<ServiceDesk>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_CreateAjaxServiceDeskGrid_CallsRepositoryInsertAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new ServiceDeskViewModel()
            {
                Id = 5,
                DeskName = "Desk E",
                CustomerId = 1,
                CustomerName = "3663",
                DeskInputTypes = new List<InputTypeRefData>(),
            };

            #endregion

            #region Act

            _controller.CreateAjaxServiceDeskGrid(request, insert);

            #endregion

            #region Assert

            _mockServiceDeskRepository.Verify(x => x.Insert(It.IsAny<ServiceDesk>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_CreateAjaxServiceDeskGrid_WithDeskInputTypeCallsRepositoryInsertAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new ServiceDeskViewModel()
            {
                Id = 5,
                DeskName = "Desk E",
                CustomerId = 1,
                CustomerName = "3663",
                DeskInputTypes = new List<InputTypeRefData>
                {
                    new InputTypeRefData
                    {
                        Id =1,
                    },
                },
            };

            #endregion

            #region Act

            _controller.CreateAjaxServiceDeskGrid(request, insert);

            #endregion

            #region Assert

            _mockServiceDeskRepository.Verify(x => x.Insert(It.IsAny<ServiceDesk>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_CreateAjaxServiceDeskGrid_WithDeskInputsLooksUpTypes()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new ServiceDeskViewModel()
            {
                Id = 5,
                DeskName = "Desk E",
                CustomerId = 1,
                CustomerName = "3663",
                DeskInputTypes = new List<InputTypeRefData>
                {
                    new InputTypeRefData
                    {
                        Id =1,
                    },
                    new InputTypeRefData
                    {
                        Id =2,
                    },
                },
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxServiceDeskGrid(request, insert);

            #endregion

            #region Assert

            _mockInputTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockServiceDeskService.Verify(x => x.Create(It.IsAny<ServiceDesk>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_DeleteAjaxServiceDeskGrid_CallsServiceDeskServiceDelete()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new ServiceDeskViewModel
            {
                Id = 1
            };

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxServiceDeskGrid(request, delete);

            #endregion

            #region Assert

            _mockServiceDeskService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceDeskService.Verify(x => x.Delete(It.IsAny<ServiceDesk>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_DeleteServiceDesk_CallsServiceDeskServiceDelete()
        {
            #region Arrange

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteServiceDesk(1);

            #endregion

            #region Assert

            _mockServiceDeskService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceDeskService.Verify(x => x.Delete(It.IsAny<ServiceDesk>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_GetCustomers_ReturnsListOfSelectListItems()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetServiceDesks();
            var selectListItems = result.Data as List<SelectListItem>;

            #endregion

            #region Assert

            Assert.IsNotNull(selectListItems);
            Assert.IsInstanceOfType(selectListItems, typeof(List<SelectListItem>));

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_GetCustomers_NoContext_ReturnsAllServiceDesksWithDefaultDropDown()
        {
            #region Arrange
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            #endregion

            #region Act

            var result = _controller.GetServiceDesks();
            var selectListItems = result.Data as List<SelectListItem>;

            #endregion

            #region Assert

            Assert.IsNotNull(selectListItems);
            Assert.IsTrue(selectListItems.Any(x => x.Text == WebResources.DefaultDropDownListText));
            Assert.AreEqual(_serviceDesks.Count + 1, selectListItems.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_GetCustomers_WithContext_ReturnsCustomerServiceDesksWithDefaultDropDown()
        {
            #region Arrange

            _appContext.CurrentCustomer = new CurrentCustomerViewModel()
            {
                Id = 1,
                CustomerName = "3663"
            };

            #endregion

            #region Act

            var result = _controller.GetServiceDesks();
            var selectListItems = result.Data as List<SelectListItem>;

            #endregion

            #region Assert

            Assert.IsNotNull(selectListItems);
            Assert.IsTrue(selectListItems.Any(x => x.Text == WebResources.DefaultDropDownListText));
            Assert.AreEqual(3, selectListItems.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskController_Add_CurrentCustomerContextNotPresent_ReturnsRedirectRouteResult()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelOne) as RedirectToRouteResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceDeskController_Add_CurrentCustomerContextNotPresentAndZeroLevel_RedirectToIndexServiceDeskLevelZero()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("ServiceDesk", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(NavigationLevelNames.LevelZero, result.RouteValues["level"]);
        }

        [TestMethod]
        public void ServiceDeskController_Add_CurrentCustomerContextNotPresentAndZeroLevel_RedirectToIndexServiceDeskLevelOne()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelOne) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("ServiceDesk", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(NavigationLevelNames.LevelOne, result.RouteValues["level"]);
        }

        [TestMethod]
        public void ServiceDeskController_Add_CurrentCustomerContextNotPresentAndZeroLevel_RedirectToIndexServiceDeskLevelTwo()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelTwo) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("ServiceDesk", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(NavigationLevelNames.LevelTwo, result.RouteValues["level"]);
        }

        [TestMethod]
        public void ServiceDeskController_Add_LevelZeroPassed_LevelZeroPlacedOnViewModel()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelZero, model.EditLevel);
        }

        [TestMethod]
        public void ServiceDeskController_Add_LevelOnePassed_LevelOnePlacedOnViewModel()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelOne) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelOne, model.EditLevel);
        }

        [TestMethod]
        public void ServiceDeskController_Add_LevelTwoPassed_LevelTwoPlacedOnViewModel()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelTwo, model.EditLevel);
        }

        [TestMethod]
        public void ServiceDeskController_Add_LevelZeroPassed_ViewModelSetsReturnUrlAsBackToServiceDesksIndexWithLevel()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.ReturnUrl.StartsWith("/ServiceDesk?level=LevelZero"));
        }

        [TestMethod]
        public void ServiceDeskController_Add_LevelOnePassed_ViewModelSetsReturnUrlAsBackToServiceDesksIndexWithLevel()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelOne) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.ReturnUrl.StartsWith("/ServiceDesk?level=LevelOne"));
        }

        [TestMethod]
        public void ServiceDeskController_Add_LevelTwoPassed_ViewModelSetsReturnUrlAsBackToServiceDesksIndexWithLevel()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;
            Assert.IsNotNull(model);
            Assert.IsTrue(model.ReturnUrl.StartsWith("/ServiceDesk?level=LevelTwo"));
        }

        [TestMethod]
        public void ServiceDeskController_Add_LevelZeroPassed_ReturnUrlLevelZeroPlacedOnViewModel()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelZero, model.EditLevel);
        }

        [TestMethod]
        public void ServiceDeskController_Add_ServiceDesk_ServiceDeskInputTypesPrePopulatedWithDefaultInputTypesFromReferenceData()
        {
            var result = _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero) as ViewResult;
            var model = result.Model as AddServiceDeskViewModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(_inputTypeRefData.Where(x => x.Default).ToList().Count, model.ServiceDesk.DeskInputTypes.Count);
        }

        [TestMethod]
        public void ServiceDeskController_Add_ServiceDesk_ServiceDeskInputTypeRefDataServiceCalledOnce()
        {
            _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero);

            _mockInputTypeRefDataService.Verify(v => v.All(), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Add_ServiceDesk_WithContextServiceDeskCreateCalledOnce()
        {
            var deskInputTypes = new int[4];
            _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero, _serviceDesk.DeskName, deskInputTypes);

            _mockServiceDeskService.Verify(v => v.Create(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Add_ServiceDesk_WithContextServiceDeskWithNoDeskInputsCreateCalledOnce()
        {
            var deskInputTypes = new int[0];
            _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero, _serviceDesk.DeskName, deskInputTypes);

            _mockServiceDeskService.Verify(v => v.Create(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Add_ServiceDesk_WithContextServiceDeskWithNullDeskInputsCreateCalledOnce()
        {
            _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero, _serviceDesk.DeskName, null);

            _mockServiceDeskService.Verify(v => v.Create(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Add_ServiceDesk_WithoutContextServiceDeskCreateIsNeverCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var deskInputTypes = new int[4];
            _controllerWithMockedServices.Add(NavigationLevelNames.LevelZero, _serviceDesk.DeskName, deskInputTypes);

            _mockServiceDeskService.Verify(v => v.Create(It.IsAny<ServiceDesk>()), Times.Never);
        }


        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextNotPresent_ReturnsRedirectRouteResult()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextNotPresentAndNoLevelIsEmptyString_RedirectToCustomerMyCustomers()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.None, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            var controller = result.RouteValues["controller"];
            var action = result.RouteValues["action"];
            Assert.AreEqual("Customer", controller);
            Assert.AreEqual("MyCustomers", action);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextNotPresentAndNoLevelIsNull_RedirectToCustomerMyCustomers()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext { CurrentCustomer = null });
            var result = _controllerWithMockedServices.Edit(null, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            var controller = result.RouteValues["controller"];
            var action = result.RouteValues["action"];
            Assert.AreEqual("Customer", controller);
            Assert.AreEqual("MyCustomers", action);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextPresentAndNoLevelIsNull_RedirectToServiceDecompositionEditCustomer()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as ServiceDesk);
            var result = _controllerWithMockedServices.Edit(null, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            var controller = result.RouteValues["controller"];
            var action = result.RouteValues["action"];
            Assert.AreEqual("ServiceDecomposition", controller);
            Assert.AreEqual("EditCustomer", action);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextPresentAndNoLevelIsEmptyString_RedirectToServiceDecompositionEditCustomer()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as ServiceDesk);
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.None, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            var controller = result.RouteValues["controller"];
            var action = result.RouteValues["action"];
            Assert.AreEqual("ServiceDecomposition", controller);
            Assert.AreEqual("EditCustomer", action);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextPresentAndNoLevelIsNull_IdInRouteArgs()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as ServiceDesk);
            var result = _controllerWithMockedServices.Edit(null, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.RouteValues.ContainsKey("Id"));
            Assert.IsFalse(result.RouteValues.ContainsKey("Level"));
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextPresentAndNoLevelIsNull_CustomerIdInRouteArgs()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as ServiceDesk);
            var result = _controllerWithMockedServices.Edit(null, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.RouteValues.ContainsKey("Id"));
            Assert.AreEqual(CustomerId, result.RouteValues["Id"]);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextPresentAndLevelIsProvided_RedirectToServiceDeskIndex()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as ServiceDesk);
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelTwo, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            var controller = result.RouteValues["controller"];
            var action = result.RouteValues["action"];
            Assert.AreEqual(controller, "ServiceDesk");
            Assert.AreEqual(action, "Index");
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CurrentCustomerContextPresentAndLevelIsProvided_LevelInRouteArgs()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as ServiceDesk);
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelTwo, 2) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            var level = result.RouteValues["Level"];
            Assert.AreEqual(level, NavigationLevelNames.LevelTwo);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CustomerHasOneServiceDesk_ViewModelCanMoveServiceDomainFalse()
        {
            _mockAppUserContext
                .Setup(s => s.Current)
                .Returns(new AppContext { CurrentCustomer = new CurrentCustomerViewModel { Id = 666 } });
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 2) as ViewResult;
            var model = result.Model as EditServiceDeskViewModel;
            Assert.IsFalse(model.CanMoveServiceDomain);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_EditLevelPassed_EditLevelPlacedOnViewModel()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 2) as ViewResult;
            var model = result.Model as EditServiceDeskViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelOne, model.EditLevel);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_ServiceDeskExists_ServiceDeskPlacedOnViewModel()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 2) as ViewResult;
            var model = result.Model as EditServiceDeskViewModel;

            var compare = new CompareLogic(new ComparisonConfig
            {
                IgnoreObjectTypes = true,
                MaxDifferences = 100,
                MembersToIgnore = new List<string> { "DeskInputTypes", "InsertedDate", "InsertedBy" }
            });

            var same = compare.Compare(_serviceDesk, model.ServiceDesk);

            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelZeroNoAppUserContextRedirectToCorrectControllerAction()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext
            {
                CurrentCustomer = null
            });

            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelZero, 1, "Test Service Desk", null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "Customer");
            Assert.AreEqual(result.RouteValues["action"], "MyCustomers");
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NoNavigationLevelRedirectToCorrectControllerAction()
        {
            var result = _controllerWithMockedServices.Edit(string.Empty, 1, "Test Service Desk", null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDecomposition");
            Assert.AreEqual(result.RouteValues["action"], "EditCustomer");
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NoNavigationLevelServiceDesksEmptiedServiceDeskServiceUpdateCalled()
        {
            var result = _controllerWithMockedServices.Edit(string.Empty, 1, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NoNavigationLevelServiceDesksAddDeskInputServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(string.Empty, 3, "Test Service Desk", new int[1] { 1 });

            Assert.IsNotNull(result);

            _mockInputTypeRefDataService.Verify(v => v.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NoNavigationLevelServiceDesksWithNoServiceDeskInputsServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(string.Empty, 3, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelZeroRedirectToCorrectControllerActionAndLevel()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelZero, 1, "Test Service Desk", null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDesk");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["Level"], NavigationLevelNames.LevelZero);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelZeroServiceDesksEmptiedServiceDeskServiceUpdateCalled()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelZero, 1, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelZeroServiceDesksAddDeskInputServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelZero, 3, "Test Service Desk", new int[1] { 1 });

            Assert.IsNotNull(result);

            _mockInputTypeRefDataService.Verify(v => v.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelZeroServiceDesksWithNoServiceDeskInputsServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelZero, 3, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelOneRedirectToCorrectControllerActionAndLevel()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 1, "Test Service Desk", null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDesk");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["Level"], NavigationLevelNames.LevelOne);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelOneServiceDesksEmptiedServiceDeskServiceUpdateCalled()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 1, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelOneServiceDesksAddDeskInputServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 3, "Test Service Desk", new int[1] { 1 });

            Assert.IsNotNull(result);

            _mockInputTypeRefDataService.Verify(v => v.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelOneServiceDesksWithNoServiceDeskInputsServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 3, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelTwoRedirectToCorrectControllerActionAndLevel()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelTwo, 1, "Test Service Desk", null) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDesk");
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["Level"], NavigationLevelNames.LevelTwo);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelTwoServiceDesksEmptiedServiceDeskServiceUpdateCalled()
        {
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelTwo, 1, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelTwoServiceDesksAddDeskInputServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelTwo, 3, "Test Service Desk", new int[1] { 1 });

            Assert.IsNotNull(result);

            _mockInputTypeRefDataService.Verify(v => v.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>(), It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelTwoServiceDesksWithNoServiceDeskInputsServiceDeskServiceUpdateCalled()
        {

            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelTwo, 3, "Test Service Desk", null);

            Assert.IsNotNull(result);
            _mockServiceDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NoNavigationLevelZeroModalStateErrorRedirectToServiceDeskEdit()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            var result = _controllerWithMockedServices.Edit(string.Empty, 1, "Test Service Desk", null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ViewName);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelZeroModalStateErrorRedirectToServiceDeskEdit()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelZero, 1, "Test Service Desk", null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelZero, result.ViewName);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelOneModalStateErrorRedirectToServiceDeskEdit()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelOne, 1, "Test Service Desk", null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceDeskController_Edit_Post_NavigationLevelTwoModalStateErrorRedirectToServiceDeskEdit()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            var result = _controllerWithMockedServices.Edit(NavigationLevelNames.LevelTwo, 1, "Test Service Desk", null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelTwo, result.ViewName);
        }



        #region Method Authorization Requirement Tests

        [TestMethod]
        public void ServiceDeskController_CreateAjaxServiceDeskGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateAjaxServiceDeskGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDeskController_DeleteAjaxServiceDeskGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxServiceDeskGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDeskController_Edit_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new[] { typeof(string), typeof(int) }));
        }

        #endregion
    }
}
