using Fujitsu.SLM.Constants;
using Fujitsu.SLM.DataImportProcessors.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.TemplateProcessors.Interface;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
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
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class TemplateControllerTests
    {
        private Mock<ITemplateService> _mockTemplateService;

        private Mock<IServiceDecompositionTemplateDataImportProcessor> _mockServiceDecompositionTemplateDataImportProcessor;
        private Mock<IServiceDecompositionDesignDataImportProcessor> _mockServiceDecompositionDesignDataImportProcessor;
        private Mock<ITemplateProcessor> _mockTemplateProcessor;

        private Mock<IServiceDomainService> _mockServiceDomainService;
        private Mock<IDomainTypeRefDataService> _mockDomainTypeRefDataService;
        private Mock<IServiceDeskService> _mockServiceDeskService;
        private Mock<IContextManager> _mockContextManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IRequestManager> _mockRequestManager;


        private Mock<IResponseManager> _mockResponseManager;
        private Mock<NameValueCollection> _mockNameValueCollection;

        private List<ServiceDesk> _serviceDesks;
        private List<DomainTypeRefData> _domainTypeRefDatas;

        private ServiceDomain _serviceDomain;

        private ServiceDesk _serviceDesk;

        private List<ServiceDomainListItem> _serviceDomainListItems;
        private List<TemplateListItem> _templateListItems;

        private const int CustomerId = 666;
        private const int ServiceDeskId = 777;
        private const int ServiceDomainId = 888;
        private const int TemplateId = 1;
        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private TemplateController _target;

        private ControllerContextMocks _controllerContextMocks;

        private const string SORT = "SORT";
        private const string SLM = "SLM";

        [TestInitialize]
        public void Initialize()
        {
            Bootstrapper.SetupAutoMapper();

            _serviceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();

            _domainTypeRefDatas = new List<DomainTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<DomainTypeRefData>(x =>
                {
                    x.Id = 1;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<DomainTypeRefData>(x =>
                {
                    x.Id = 2;
                    x.Visible = false;
                }),
                UnitTestHelper.GenerateRandomData<DomainTypeRefData>(x =>
                {
                    x.Id = 3;
                    x.Visible = false;
                }),
                UnitTestHelper.GenerateRandomData<DomainTypeRefData>(x =>
                {
                    x.Id = 4;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<DomainTypeRefData>(x =>
                {
                    x.Id = 5;
                    x.Visible = true;
                })
            };

            _mockDomainTypeRefDataService = new Mock<IDomainTypeRefDataService>();
            _mockDomainTypeRefDataService.Setup(s => s.GetById(1)).Returns(_domainTypeRefDatas[0]);
            _mockDomainTypeRefDataService.Setup(s => s.GetById(2)).Returns(_domainTypeRefDatas[1]);
            _mockDomainTypeRefDataService.Setup(s => s.GetById(3)).Returns(_domainTypeRefDatas[2]);
            _mockDomainTypeRefDataService.Setup(s => s.GetById(4)).Returns(_domainTypeRefDatas[3]);
            _mockDomainTypeRefDataService.Setup(s => s.GetById(5)).Returns(_domainTypeRefDatas[4]);
            _mockDomainTypeRefDataService.Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId))
                .Returns(_domainTypeRefDatas);

            _serviceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>();
            _serviceDomain.ServiceFunctions = new List<ServiceFunction>();

            _serviceDomainListItems = new List<ServiceDomainListItem>
            {
                UnitTestHelper.GenerateRandomData<ServiceDomainListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceDomainListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceDomainListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceDomainListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceDomainListItem>(),
            };

            _templateListItems = new List<TemplateListItem>
            {
                UnitTestHelper.GenerateRandomData<TemplateListItem>(x =>
                    {
                        x.Id = TemplateId;
                        x.TemplateType = TemplateTypeNames.SORT;
                    }),
                UnitTestHelper.GenerateRandomData<TemplateListItem>(),
                UnitTestHelper.GenerateRandomData<TemplateListItem>(),
                UnitTestHelper.GenerateRandomData<TemplateListItem>()
            };

            _mockServiceDomainService = new Mock<IServiceDomainService>();
            _mockServiceDomainService.Setup(s => s.GetByCustomerAndId(CustomerId, ServiceDomainId))
                .Returns(_serviceDomain);
            _mockServiceDomainService.Setup(s => s.GetById(ServiceDomainId)).Returns(_serviceDomain);
            _mockServiceDomainService.Setup(s => s.ServiceDeskDomains(ServiceDeskId))
                .Returns(_serviceDomainListItems.AsQueryable());
            _mockServiceDomainService.Setup(s => s.CustomerServiceDomains(CustomerId))
                .Returns(_serviceDomainListItems.AsQueryable());

            _mockResponseManager = new Mock<IResponseManager>();
            _mockRequestManager = new Mock<IRequestManager>();

            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserName);

            _mockNameValueCollection = new Mock<NameValueCollection>();

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);
            _mockContextManager.Setup(s => s.RequestManager).Returns(_mockRequestManager.Object);
            _mockRequestManager.Setup(s => s.Form).Returns(_mockNameValueCollection.Object);

            _serviceDesks = new List<ServiceDesk>
            {
                UnitTestHelper.GenerateRandomData<ServiceDesk>(),
                UnitTestHelper.GenerateRandomData<ServiceDesk>(),
                UnitTestHelper.GenerateRandomData<ServiceDesk>(x =>
                {
                    x.Id = ServiceDeskId;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDesk>(),
                UnitTestHelper.GenerateRandomData<ServiceDesk>()
            };

            _mockServiceDeskService = new Mock<IServiceDeskService>();
            _mockServiceDeskService.Setup(s => s.GetByCustomer(CustomerId)).Returns(_serviceDesks.AsQueryable());
            _mockServiceDeskService.Setup(s => s.GetByCustomerAndId(CustomerId, ServiceDeskId)).Returns(_serviceDesk);

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            });

            _mockTemplateService = new Mock<ITemplateService>();
            _mockTemplateService.Setup(s => s.GetById(TemplateId))
                .Returns(new Template { Id = TemplateId, Filename = "example.xlxs" });
            _mockTemplateService.Setup(s => s.AllTemplates())
                .Returns(_templateListItems);

            _mockServiceDecompositionTemplateDataImportProcessor = new Mock<IServiceDecompositionTemplateDataImportProcessor>();
            _mockServiceDecompositionDesignDataImportProcessor = new Mock<IServiceDecompositionDesignDataImportProcessor>();
            _mockTemplateProcessor = new Mock<ITemplateProcessor>();


            _target = new TemplateController(_mockTemplateService.Object,
                _mockServiceDeskService.Object,
                _mockServiceDecompositionTemplateDataImportProcessor.Object,
                _mockServiceDecompositionDesignDataImportProcessor.Object,
                _mockTemplateProcessor.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);

            _controllerContextMocks = new ControllerContextMocks();
            _target.ControllerContext = new ControllerContext(_controllerContextMocks.MockHttpContextBase.Object,
                new RouteData(),
                _target);
            if (RouteTable.Routes.Count == 0)
            {
                RouteConfig.RegisterRoutes(RouteTable.Routes);
            }
            _target.Url =
                new UrlHelper(new RequestContext(_controllerContextMocks.MockHttpContextBase.Object, new RouteData()));
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateController_Constructor_TemplateServiceIsNull_ThrowsException()
        {
            new TemplateController(null,
                _mockServiceDeskService.Object,
                _mockServiceDecompositionTemplateDataImportProcessor.Object,
                _mockServiceDecompositionDesignDataImportProcessor.Object,
                _mockTemplateProcessor.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateController_Constructor_ServiceDeskServiceIsNull_ThrowsException()
        {
            new TemplateController(_mockTemplateService.Object,
                null,
                _mockServiceDecompositionTemplateDataImportProcessor.Object,
                _mockServiceDecompositionDesignDataImportProcessor.Object,
                _mockTemplateProcessor.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateController_Constructor_ServiceDecompositionTemplateDataImportProcessorIsNull_ThrowsException
            ()
        {
            new TemplateController(_mockTemplateService.Object,
                _mockServiceDeskService.Object,
                null,
                _mockServiceDecompositionDesignDataImportProcessor.Object,
                _mockTemplateProcessor.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateController_Constructor_TransformTemplateDomainsIsNull_ThrowsException()
        {
            new TemplateController(_mockTemplateService.Object,
                _mockServiceDeskService.Object,
                _mockServiceDecompositionTemplateDataImportProcessor.Object,
                _mockServiceDecompositionDesignDataImportProcessor.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateController_Constructor_ContextManagerIsNull_ThrowsException()
        {
            new TemplateController(_mockTemplateService.Object,
                _mockServiceDeskService.Object,
                _mockServiceDecompositionTemplateDataImportProcessor.Object,
                _mockServiceDecompositionDesignDataImportProcessor.Object,
                _mockTemplateProcessor.Object,
                null,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateController_Constructor_AppUserContextIsNull_ThrowsException()
        {
            new TemplateController(_mockTemplateService.Object,
                _mockServiceDeskService.Object,
                _mockServiceDecompositionTemplateDataImportProcessor.Object,
                _mockServiceDecompositionDesignDataImportProcessor.Object,
                _mockTemplateProcessor.Object,
                _mockContextManager.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void TemplateController_Index_Get_ReturnsViewResult()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Administrator);
            var result = _target.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TemplateController_ReadAjaxTemplateGrid_ReturnsTemplateViewModel()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxTemplateGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<TemplateViewModel>;

            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(model, typeof(List<TemplateViewModel>));

            #endregion
        }

        [TestMethod]
        public void TemplateController_ReadAjaxTemplateGrid_CallsServiceDeskDomains()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxTemplateGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<TemplateViewModel>;

            Assert.IsNotNull(model);
            _mockTemplateService.Verify(x => x.AllTemplates(), Times.Once);
            Assert.AreEqual(_templateListItems.Count, model.Count);

            #endregion
        }


        [TestMethod]
        public void TemplateController_ReadAjaxTemplateGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            _mockTemplateService.Setup(s => s.AllTemplates()).Throws(new ApplicationException("Oh no!!"));

            #endregion

            var result = _target.ReadAjaxTemplateGrid(request) as JsonResult;

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TemplateController_DeleteAjaxTemplateGrid_CallsTemplateServiceGetByIdAndDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var entity = new TemplateViewModel
            {
                Id = TemplateId
            };

            #endregion

            #region Act

            _target.DeleteAjaxTemplateGrid(request, entity);

            #endregion

            #region Assert

            _mockTemplateService.Verify(x => x.GetById(TemplateId), Times.Once);
            _mockTemplateService.Verify(x => x.Delete(It.IsAny<Template>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TemplateController_DeleteAjaxTemplateGrid_CallsTemplateServiceGetByIdAndDoesNotCallDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var entity = new TemplateViewModel
            {
                Id = 99
            };

            #endregion

            #region Act

            _target.DeleteAjaxTemplateGrid(request, entity);

            #endregion

            #region Assert

            _mockTemplateService.Verify(x => x.GetById(99), Times.Once);
            _mockTemplateService.Verify(x => x.Delete(It.IsAny<Template>()), Times.Never);

            #endregion
        }

        [TestMethod]
        public void TemplateController_DeleteAjaxTemplateGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new TemplateViewModel
            {
                Id = TemplateId
            };

            _mockTemplateService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _target.DeleteAjaxTemplateGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TemplateController_Add_Get_ReturnsAddSortServiceDomainViewModel()
        {
            var result = _target.AddSLMTemplate(NavigationLevelNames.None, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddTemplateServiceDomainViewModel;
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void TemplateController_Add_LevelPassed_LevelZeroIsAppendedToViewName()
        {
            var result = _target.AddServiceConfiguratorTemplate(NavigationLevelNames.LevelZero, ServiceDeskId) as ViewResult;
            Assert.AreEqual("Add" + NavigationLevelNames.LevelZero, result.ViewName);
        }

        [TestMethod]
        public void TemplateController_Add_LevelPassed_LevelOneIsAppendedToViewName()
        {
            var result = _target.AddSLMTemplate(NavigationLevelNames.LevelOne, ServiceDeskId) as ViewResult;
            Assert.AreEqual("Add" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void TemplateController_Add_LevelPassed_LevelTwoIsAppendedToViewName()
        {
            var result = _target.AddServiceConfiguratorTemplate(NavigationLevelNames.LevelTwo, ServiceDeskId) as ViewResult;
            Assert.AreEqual("Add" + NavigationLevelNames.LevelTwo, result.ViewName);
        }

        [TestMethod]
        public void TemplateController_Add_IdIsPassed_IdIsSetAsServiceDeskIdOnViewModel()
        {
            var result = _target.AddSLMTemplate(NavigationLevelNames.None, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddTemplateServiceDomainViewModel;
            Assert.AreEqual(ServiceDeskId, vm.ServiceDeskId);
        }

        [TestMethod]
        public void TemplateController_Add_LevelIsPassed_LevelIsSetAsEditLevelOnViewModel()
        {
            var result = _target.AddSLMTemplate(NavigationLevelNames.LevelZero, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddTemplateServiceDomainViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelZero, vm.EditLevel);
        }

        [TestMethod]
        public void TemplateController_Add_CustomerIsPresentOnContext_ServiceDeskTypesReturnedOnViewModelAreSelectListItems()
        {
            var result = _target.AddServiceConfiguratorTemplate(NavigationLevelNames.LevelZero, 0) as ViewResult;
            var vm = result.Model as AddTemplateServiceDomainViewModel;
            var list = vm.ServiceDesks as List<SelectListItem>;
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void TemplateController_Add_ServiceDeskIdPassedIsZero_ViewModelSetsHasServiceDeskContextToFalse()
        {
            var result = _target.AddServiceConfiguratorTemplate(NavigationLevelNames.LevelZero, 0) as ViewResult;
            var vm = result.Model as AddTemplateServiceDomainViewModel;
            Assert.IsFalse(vm.HasServiceDeskContext);
        }

        [TestMethod]
        public void TemplateController_AddSort_ServiceDeskIdPassedIsZero_ViewModelSetsReturnUrlAsBackToServiceDomainsIndex()
        {
            var result = _target.AddSLMTemplate(NavigationLevelNames.LevelZero, 0) as ViewResult;
            var vm = result.Model as AddTemplateServiceDomainViewModel;
            Assert.IsTrue(vm.ReturnUrl.StartsWith("/ServiceDomain/Index"));
        }

        [TestMethod]
        public void TemplateController_AddSort_ServiceDeskIdPassedIsGreaterThanZero_ViewModelSetsReturnUrlAsBackToEditServiceDesk()
        {
            var result = _target.AddSLMTemplate(NavigationLevelNames.LevelZero, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddTemplateServiceDomainViewModel;
            Assert.IsTrue(vm.ReturnUrl.StartsWith("/ServiceDesk/Edit"));
        }

        [TestMethod]
        public void TemplateController_Template_Get_ReturnsViewResult()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Administrator);
            var result = _target.Template(1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TemplateController_Template_Get_ReturnsTemplateViewModel()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Administrator);
            var result = _target.Template(1) as ViewResult;
            var model = result.Model as TemplateViewModel;
            Assert.IsInstanceOfType(model, typeof(TemplateViewModel));
        }
    }
}