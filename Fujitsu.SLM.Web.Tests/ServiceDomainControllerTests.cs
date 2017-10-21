using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
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
    public class ServiceDomainControllerTests
    {
        private Mock<IServiceDomainService> _mockServiceDomainService;
        private Mock<IDomainTypeRefDataService> _mockDomainTypeRefDataService;
        private Mock<IServiceDeskService> _mockServiceDeskService;
        private Mock<IParameterService> _mockParameterService;
        private Mock<ITemplateService> _mockTemplateService;
        private Mock<IContextManager> _mockContextManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IRequestManager> _mockRequestManager;

        private Mock<IResponseManager> _mockResponseManager;
        private Mock<NameValueCollection> _mockNameValueCollection;

        private List<ServiceDesk> _serviceDesks;
        private List<ServiceDomainViewModel> _serviceDomainViewModels;
        private List<BulkServiceDomainViewModel> _bulkServiceDomainViewModels;
        private List<DomainTypeRefData> _domainTypeRefDatas;

        private ServiceDomain _serviceDomain;
        private ServiceDomain _serviceDomainUpdated;

        private ServiceDesk _serviceDesk;

        private List<ServiceDomainListItem> _serviceDomainListItems;

        private const int CustomerId = 666;
        private const int ServiceDeskId = 777;
        private const int ServiceDomainId = 888;
        private const string UserName = "mark.hart@uk.fujitsu.com";
        private ServiceDomainController _target;

        private ControllerContextMocks _controllerContextMocks;

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

            _serviceDomainViewModels = new List<ServiceDomainViewModel>
            {
                UnitTestHelper.GenerateRandomData<ServiceDomainViewModel>(x =>
                {
                    x.ServiceDeskId = ServiceDeskId;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomainViewModel>(),
                UnitTestHelper.GenerateRandomData<ServiceDomainViewModel>(x =>
                {
                    x.ServiceDeskId = ServiceDeskId;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomainViewModel>(),
                UnitTestHelper.GenerateRandomData<ServiceDomainViewModel>(x =>
                {
                    x.ServiceDeskId = ServiceDeskId;
                })
            };

            _bulkServiceDomainViewModels = new List<BulkServiceDomainViewModel>
            {
                UnitTestHelper.GenerateRandomData<BulkServiceDomainViewModel>(x =>
                {
                    x.ServiceDeskId = ServiceDeskId;
                    x.DomainTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceDomainViewModel>(x =>
                {
                    x.DomainTypeId = 2;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceDomainViewModel>(x =>
                {
                    x.ServiceDeskId = ServiceDeskId;
                    x.DomainTypeId = 3;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceDomainViewModel>(x =>
                {
                    x.DomainTypeId = 4;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceDomainViewModel>(x =>
                {
                    x.ServiceDeskId = ServiceDeskId;
                    x.DomainTypeId = 0;
                })
            };

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

            _mockServiceDomainService = new Mock<IServiceDomainService>();
            _mockServiceDomainService.Setup(s => s.GetByCustomerAndId(CustomerId, ServiceDomainId)).Returns(_serviceDomain);
            _mockServiceDomainService.Setup(s => s.GetById(ServiceDomainId)).Returns(_serviceDomain);
            _mockServiceDomainService.Setup(s => s.Update(It.IsAny<ServiceDomain>())).Callback<ServiceDomain>(c => _serviceDomainUpdated = c);
            _mockServiceDomainService.Setup(s => s.Create(It.IsAny<ServiceDomain>())).Callback<ServiceDomain>(c => _serviceDomainUpdated = c);
            _mockServiceDomainService.Setup(s => s.ServiceDeskDomains(ServiceDeskId)).Returns(_serviceDomainListItems.AsQueryable());
            _mockServiceDomainService.Setup(s => s.CustomerServiceDomains(CustomerId)).Returns(_serviceDomainListItems.AsQueryable());

            _mockParameterService = new Mock<IParameterService>();

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

            _target = new ServiceDomainController(_mockServiceDomainService.Object,
                _mockServiceDeskService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockTemplateService.Object,
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
            _target.Url = new UrlHelper(new RequestContext(_controllerContextMocks.MockHttpContextBase.Object, new RouteData()));
        }

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainController_Ctor_ServiceDomainServiceIsNull_ThrowsException()
        {
            new ServiceDomainController(null,
                _mockServiceDeskService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainController_Ctor_ServiceDeskServiceIsNull_ThrowsException()
        {
            new ServiceDomainController(_mockServiceDomainService.Object,
                null,
                _mockDomainTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainController_Ctor_DomainTypeRefDataServiceIsNull_ThrowsException()
        {
            new ServiceDomainController(_mockServiceDomainService.Object,
                _mockServiceDeskService.Object,
                null,
                _mockParameterService.Object,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainController_Ctor_ParameterServiceIsNull_ThrowsException()
        {
            new ServiceDomainController(_mockServiceDomainService.Object,
                _mockServiceDeskService.Object,
                _mockDomainTypeRefDataService.Object,
                null,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainController_Ctor_TemplateServiceIsNull_ThrowsException()
        {
            new ServiceDomainController(_mockServiceDomainService.Object,
                _mockServiceDeskService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockParameterService.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainController_Ctor_ContextManagerIsNull_ThrowsException()
        {
            new ServiceDomainController(_mockServiceDomainService.Object,
                _mockServiceDeskService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockTemplateService.Object,
                null,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainController_Ctor_AppUserContextIsNull_ThrowsException()
        {
            new ServiceDomainController(_mockServiceDomainService.Object,
                _mockServiceDeskService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockTemplateService.Object,
                _mockContextManager.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ServiceDomainController_Move_Get_PartialViewReturned()
        {
            var result = _target.Move() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceDomainController_Move_Get_ReturnedViewModelCorrectType()
        {
            var result = _target.Move() as PartialViewResult;
            Assert.IsNotNull(result);
            var vm = result.Model as MoveServiceDomainViewModel;
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void ServiceDomainController_Move_AppUserContextCustomerIsNull_ReturnedViewModelServiceDesksEmpty()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext());
            var result = _target.Move() as PartialViewResult;
            var vm = result.Model as MoveServiceDomainViewModel;
            Assert.AreEqual(0, vm.ServiceDesks.Count);
        }

        [TestMethod]
        public void ServiceDomainController_Move_CustomerIsSelected_GetByCustomerServiceIsCalled()
        {
            var result = _target.Move() as PartialViewResult;
            _mockServiceDeskService.Verify(v => v.GetByCustomer(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_Move_CustomerIsSelected_ReturnsCorrectNumberOfServiceDesks()
        {
            var result = _target.Move() as PartialViewResult;
            var vm = result.Model as MoveServiceDomainViewModel;
            Assert.AreEqual(5, vm.ServiceDesks.Count);
        }

        [TestMethod]
        public void ServiceDomainController_Move_ModelHasErrors_ReturnsErrorInResult()
        {
            const string error = "XXX";
            _target.ModelState.AddModelError(error, error);
            var result = _target.Move(new MoveServiceDomainViewModel());
            dynamic data = result.Data;
            Assert.AreEqual(error, data.Message);
        }

        [TestMethod]
        public void ServiceDomainController_Move_ModelHasErrors_ReturnsSuccessFalse()
        {
            const string error = "XXX";
            _target.ModelState.AddModelError(error, error);
            var result = _target.Move(new MoveServiceDomainViewModel());
            dynamic data = result.Data;
            Assert.AreEqual("False", data.Success);
        }

        [TestMethod]
        public void ServiceDomainController_Move_NoErrors_GetByCustomerAndIdIsCalled()
        {
            _target.Move(new MoveServiceDomainViewModel
            {
                ServiceDeskId = ServiceDeskId,
                ServiceDomainId = ServiceDomainId
            });
            _mockServiceDomainService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_Move_NoErrors_UpdateIsCalled()
        {
            _target.Move(new MoveServiceDomainViewModel
            {
                ServiceDeskId = ServiceDeskId,
                ServiceDomainId = ServiceDomainId
            });
            _mockServiceDomainService.Verify(v => v.Update(It.IsAny<ServiceDomain>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_Move_NoErrors_UpdatedBySet()
        {
            _target.Move(new MoveServiceDomainViewModel
            {
                ServiceDeskId = ServiceDeskId,
                ServiceDomainId = ServiceDomainId
            });
            Assert.AreEqual(UserName, _serviceDomainUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceDomainController_Move_NoErrors_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.Move(new MoveServiceDomainViewModel
            {
                ServiceDeskId = ServiceDeskId,
                ServiceDomainId = ServiceDomainId
            });
            Assert.AreEqual(now.Year, _serviceDomainUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceDomainUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceDomainUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceDomainUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceDomainUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceDomainController_Move_NoErrors_ServiceDeskIdSetToNewValue()
        {
            _target.Move(new MoveServiceDomainViewModel
            {
                ServiceDeskId = ServiceDeskId,
                ServiceDomainId = ServiceDomainId
            });
            Assert.AreEqual(ServiceDeskId, _serviceDomainUpdated.ServiceDeskId);
        }

        [TestMethod]
        public void ServiceDomainController_Move_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockServiceDomainService.Setup(s => s.Update(It.IsAny<ServiceDomain>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            _target.Move(new MoveServiceDomainViewModel
            {
                ServiceDomainId = ServiceDomainId,
                ServiceDeskId = ServiceDeskId,
            });

            _mockServiceDomainService.Verify(v => v.Update(It.IsAny<ServiceDomain>()), Times.Once);

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_Add_Get_ReturnsAddServiceDomainViewModel()
        {
            var result = _target.Add(NavigationLevelNames.None, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void ServiceDomainController_Add_LevelPassed_LevelIsAppendedToViewName()
        {
            var result = _target.Add(NavigationLevelNames.LevelOne, ServiceDeskId) as ViewResult;
            Assert.AreEqual("Add" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceDomainController_Add_NoCurrentCustomer_GetAllAndNotVisibleForCustomerNotCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext());
            _target.Add(NavigationLevelNames.LevelOne, ServiceDeskId);
            _mockDomainTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(CustomerId), Times.Never);
        }

        [TestMethod]
        public void ServiceDomainController_Add_IdIsPassed_IdIsSetAsServiceDeskIdOnViewModel()
        {
            var result = _target.Add(NavigationLevelNames.None, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            Assert.AreEqual(ServiceDeskId, vm.ServiceDeskId);
        }

        [TestMethod]
        public void ServiceDomainController_Add_LevelIsPassed_LevelIsSetAsEditLevelOnViewModel()
        {
            var result = _target.Add(NavigationLevelNames.LevelZero, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelZero, vm.EditLevel);
        }

        [TestMethod]
        public void ServiceDomainController_Add_CustomerIsPresentOnContext_GetAllAndNotVisibleForCustomerCalled()
        {
            _target.Add(NavigationLevelNames.LevelZero, ServiceDeskId);
            _mockDomainTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_Add_CustomerIsPresentOnContext_DomainTypesReturnedOnViewModel()
        {
            var result = _target.Add(NavigationLevelNames.LevelZero, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            Assert.AreEqual(5, vm.DomainTypes.Count);
        }

        [TestMethod]
        public void ServiceDomainController_Add_CustomerIsPresentOnContext_DomainTypesReturnedOnViewModelAreSelectListItems()
        {
            var result = _target.Add(NavigationLevelNames.LevelZero, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            var list = vm.DomainTypes as List<SelectListItem>;
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void ServiceDomainController_Add_ServiceDeskIdPassedIsZero_ViewModelSetsHasServiceDeskContextToFalse()
        {
            var result = _target.Add(NavigationLevelNames.LevelZero, 0) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            Assert.IsFalse(vm.HasServiceDeskContext);
        }

        [TestMethod]
        public void ServiceDomainController_Add_ServiceDeskIdPassedIsZero_ViewModelSetsReturnUrlAsBackToServiceDomainsIndex()
        {
            var result = _target.Add(NavigationLevelNames.LevelZero, 0) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            Assert.IsTrue(vm.ReturnUrl.StartsWith("/ServiceDomain/Index"));
        }

        [TestMethod]
        public void ServiceDomainController_Add_ServiceDeskIdPassedIsGreaterThanZero_ViewModelSetsReturnUrlAsBackToEditServiceDesk()
        {
            var result = _target.Add(NavigationLevelNames.LevelZero, ServiceDeskId) as ViewResult;
            var vm = result.Model as AddServiceDomainViewModel;
            Assert.IsTrue(vm.ReturnUrl.StartsWith("/ServiceDesk/Edit"));
        }

        [TestMethod]
        public void ServiceDomainController_Edit_LevelZeroIsSupplied_CheckLevelZeroAppendedToViewName()
        {
            _mockServiceDeskService.Setup(s => s.GetById(ServiceDeskId).ServiceDomains.Count).Returns(2);
            var result = _target.Edit(NavigationLevelNames.LevelZero, ServiceDomainId) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelZero, result.ViewName);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_LevelOneIsSupplied_CheckLevelOneAppendedToViewName()
        {
            _mockServiceDeskService.Setup(s => s.GetById(ServiceDeskId).ServiceDomains.Count).Returns(2);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceDomainId) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_LevelTwoIsSupplied_CheckLevelTwoAppendedToViewName()
        {
            _mockServiceDeskService.Setup(s => s.GetById(ServiceDeskId).ServiceDomains.Count).Returns(2);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceDomainId) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_NavigationLevelZeroNoAppUserContextRedirectToServiceDomainIndex()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext
            {
                CurrentCustomer = null
            });

            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDomain");
            Assert.AreEqual(result.RouteValues["action"], "Index");
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_NavigationLevelZeroRedirectToServiceDeskEdit()
        {
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDesk");
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_NavigationLevelOneRedirectToServiceDeskEdit()
        {
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDesk");
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_NavigationLevelTwoRedirectToServiceDeskEdit()
        {
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.LevelTwo,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDesk");
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_NavigationLevelZeroModalStateErrorRedirectToServiceDomainEdit()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelZero, result.ViewName);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_NavigationLevelOneModalStateErrorRedirectToServiceDomainEdit()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_NavigationLevelTwoModalStateErrorRedirectToServiceDomainEdit()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.LevelTwo,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelTwo, result.ViewName);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_RedirectToServiceDeskEdit()
        {
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDesk");
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_ValidEditCallsUpdateService()
        {
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceDomainService.Verify(x => x.Update(It.IsAny<ServiceDomain>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_ValidEditCallsUpdateServiceAndChecksTheDomainType()
        {
            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 1
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceDomainService.Verify(x => x.Update(It.IsAny<ServiceDomain>()), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetById(model.ServiceDomain.DomainTypeId), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_ValidEditCallsUpdateServiceAndDoesNotSetTheDomainTypeToVisibleWhenLessThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(It.IsAny<int>())).Returns(1);

            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 2
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);

            _mockServiceDomainService.Verify(x => x.Update(It.IsAny<ServiceDomain>()), Times.Once);
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetById(model.ServiceDomain.DomainTypeId), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_ValidEditCallsUpdateServiceAndSetsTheDomainTypeToVisibleWhenEqualThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(It.IsAny<int>())).Returns(2);

            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 2
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);

            _mockServiceDomainService.Verify(x => x.Update(It.IsAny<ServiceDomain>()), Times.Once);
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetById(model.ServiceDomain.DomainTypeId), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_Edit_Post_ValidEditCallsUpdateServiceAndSetsTheDomainTypeToVisibleWhenGreaterThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(1);
            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(It.IsAny<int>())).Returns(3);

            var model = new EditServiceDomainViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceDomain = new ServiceDomainViewModel
                {
                    Id = ServiceDomainId,
                    AlternativeName = "New Domain Name",
                    ServiceDeskId = ServiceDeskId,
                    DomainTypeId = 2
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);

            _mockServiceDomainService.Verify(x => x.Update(It.IsAny<ServiceDomain>()), Times.Once);
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetById(model.ServiceDomain.DomainTypeId), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ModelStateNotValid_CheckMessageIsInResponse()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var result = _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), new List<BulkServiceDomainViewModel>()) as JsonResult;
            dynamic data = result.Data;
            var errors = (Dictionary<string, Dictionary<string, object>>)data.Errors;
            Assert.IsTrue(errors.ContainsKey("XXX"));
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainsEmpty_EmptyResultReturned()
        {
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), new List<BulkServiceDomainViewModel>());
            _mockServiceDeskService.Verify(v => v.GetByCustomerAndId(CustomerId, It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ExistingServiceDeskNotFound_NoDomainsCreated()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomer(CustomerId)).Returns(new List<ServiceDesk>().AsQueryable());
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            _mockServiceDomainService.Verify(v => v.Create(It.IsAny<ServiceDomain>()), Times.Never);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ExistingServiceFound_ServiceDomainCreatedForInputWithMatchingServiceDesks()
        {
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            _mockServiceDomainService.Verify(v => v.Create(It.IsAny<ServiceDomain>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ExistingServiceFound_ServiceDomainCreatedForInputWithMatchingServiceDesksDomainTypeNotSetToVisibleWhenLt()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(It.IsAny<int>())).Returns(1);

            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            _mockServiceDomainService.Verify(v => v.Create(It.IsAny<ServiceDomain>()), Times.Exactly(2));
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockDomainTypeRefDataService.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ExistingServiceFound_ServiceDomainCreatedForInputWithMatchingServiceDesksDomainTypeSetToVisibleWhenEq()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(It.IsAny<int>())).Returns(2);

            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            _mockServiceDomainService.Verify(v => v.Create(It.IsAny<ServiceDomain>()), Times.Exactly(2));
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockDomainTypeRefDataService.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ExistingServiceFound_ServiceDomainCreatedForInputWithMatchingServiceDesksDomainTypeSetToVisibleWhenGt()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(It.IsAny<int>())).Returns(3);

            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            _mockServiceDomainService.Verify(v => v.Create(It.IsAny<ServiceDomain>()), Times.Exactly(2));
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockDomainTypeRefDataService.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_OneOfTheInputDomainsDoesNotHaveTheDomainTypeIdSet_DomainIsSkipped()
        {
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            _mockServiceDomainService.Verify(v => v.Create(It.IsAny<ServiceDomain>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainAdded_AlternativeNameSetOnCreatedEntity()
        {
            var expected = _bulkServiceDomainViewModels.Last(x => x.DomainTypeId != 0 && x.ServiceDeskId == ServiceDeskId);
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            Assert.AreEqual(expected.AlternativeName, _serviceDomainUpdated.AlternativeName);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainAdded_DomainTypeIdSetOnCreatedEntity()
        {
            var expected = _bulkServiceDomainViewModels.Last(x => x.DomainTypeId != 0 && x.ServiceDeskId == ServiceDeskId);
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            Assert.AreEqual(expected.DomainTypeId, _serviceDomainUpdated.DomainTypeId);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainAdded_ServiceDeskIdSetOnCreatedEntity()
        {
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            Assert.AreEqual(ServiceDeskId, _serviceDomainUpdated.ServiceDeskId);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainAdded_InsertedBySetOnCreatedEntity()
        {
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            Assert.AreEqual(UserName, _serviceDomainUpdated.InsertedBy);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainAdded_InsertedDateSetOnCreatedEntity()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            Assert.AreEqual(now.Year, _serviceDomainUpdated.InsertedDate.Year);
            Assert.AreEqual(now.Month, _serviceDomainUpdated.InsertedDate.Month);
            Assert.AreEqual(now.Day, _serviceDomainUpdated.InsertedDate.Day);
            Assert.AreEqual(now.Hour, _serviceDomainUpdated.InsertedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceDomainUpdated.InsertedDate.Minute);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainAdded_UpdatedBySetOnCreatedEntity()
        {
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            Assert.AreEqual(UserName, _serviceDomainUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_ServiceDomainAdded_UpdatedDateSetOnCreatedEntity()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddDomainGrid(new DataSourceRequest(), _bulkServiceDomainViewModels);
            Assert.AreEqual(now.Year, _serviceDomainUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceDomainUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceDomainUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceDomainUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceDomainUpdated.UpdatedDate.Minute);
        }



        [TestMethod]
        public void ServiceDomainController_Index_CustomerHasMoreThanOneServiceDesk_CanMoveServiceDomainIsTrue()
        {
            var result = _target.Index(NavigationLevelNames.LevelZero) as ViewResult;
            var vm = result.Model as ViewServiceDomainViewModel;
            Assert.IsTrue(vm.CanMoveServiceDomain);
        }

        [TestMethod]
        public void ServiceDomainController_Index_CustomerHasOneServiceDesk_CanMoveServiceDomainIsFalse()
        {
            _mockServiceDeskService.Setup(s => s.GetByCustomer(CustomerId)).Returns(new List<ServiceDesk>().AsQueryable());
            var result = _target.Index(NavigationLevelNames.LevelZero) as ViewResult;
            var vm = result.Model as ViewServiceDomainViewModel;
            Assert.IsFalse(vm.CanMoveServiceDomain);
        }

        [TestMethod]
        public void ServiceDomainController_Index_EditLevelPassed_EditLevelOnViewModel()
        {
            var result = _target.Index(NavigationLevelNames.LevelZero) as ViewResult;
            var vm = result.Model as ViewServiceDomainViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelZero, vm.EditLevel);
        }

        [TestMethod]
        public void ServiceDomainController_ReadAjaxServiceDomainGrid_WithContextReturnsServiceDomainViewModel()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceDomainGrid(request, ServiceDeskId) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceDomainViewModel>;

            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(model, typeof(List<ServiceDomainViewModel>));

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_ReadAjaxServiceDomainGrid_WithContextCallsServiceDeskDomains()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceDomainGrid(request, ServiceDeskId) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceDomainViewModel>;

            Assert.IsNotNull(model);
            _mockServiceDomainService.Verify(x => x.ServiceDeskDomains(ServiceDeskId), Times.Once);
            Assert.AreEqual(_serviceDomainListItems.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_ReadAjaxServiceDomainGrid_WithOutServiceDeskIdReturnsServiceDomainViewModel()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceDomainGrid(request, null) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceDomainViewModel>;

            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(model, typeof(List<ServiceDomainViewModel>));

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_ReadAjaxServiceDomainGrid_WithOutServiceDeskIdCallsCustomerServiceDomains()
        {
            #region Arrange


            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceDomainGrid(request, null) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceDomainViewModel>;

            Assert.IsNotNull(model);
            _mockServiceDomainService.Verify(x => x.CustomerServiceDomains(CustomerId), Times.Once);
            Assert.AreEqual(_serviceDomainListItems.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_ReadAjaxServiceDomainGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            _mockServiceDomainService.Setup(s => s.CustomerServiceDomains(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            var result = _target.ReadAjaxServiceDomainGrid(request, null) as JsonResult;

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_UpdateAjaxServiceDomainGrid_CallsServiceDomainGetByIdAndUpdate()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceDomainViewModel
            {
                Id = ServiceDomainId,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.UpdateAjaxServiceDomainGrid(request, update);

            #endregion

            #region Assert

            _mockServiceDomainService.Verify(x => x.GetById(ServiceDomainId), Times.Once);
            _mockServiceDomainService.Verify(x => x.Update(It.IsAny<ServiceDomain>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_UpdateAjaxServiceDomainGrid_DeletedServiceDomainAppendsHandledErrorMessage()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceDomainViewModel
            {
                Id = 99,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.UpdateAjaxServiceDomainGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", WebResources.ServiceDomainCannotBeFound), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_UpdateAjaxServiceDomainGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new ServiceDomainViewModel
            {
                Id = ServiceDomainId,
                AlternativeName = "An Alternative Name"
            };

            _mockServiceDomainService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _target.UpdateAjaxServiceDomainGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_DeleteAjaxServiceDomainGrid_CallsServiceDomainGetByIdAndDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceDomainViewModel
            {
                Id = ServiceDomainId,
                AlternativeName = "An Alternative Name",
            };

            #endregion

            #region Act

            _target.DeleteAjaxServiceDomainGrid(request, update);

            #endregion

            #region Assert

            _mockServiceDomainService.Verify(x => x.GetById(ServiceDomainId), Times.Once);
            _mockServiceDomainService.Verify(x => x.Delete(It.IsAny<ServiceDomain>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_DeleteAjaxServiceDomainGrid_DeleteServiceDomainWithFunctionDoesNotDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();
            var serviceFunctions = new List<ServiceFunction>
            {
                UnitTestHelper.GenerateRandomData<ServiceFunction>(),
                UnitTestHelper.GenerateRandomData<ServiceFunction>(),
            };

            _serviceDomain.ServiceFunctions = serviceFunctions;

            var update = new ServiceDomainViewModel
            {
                Id = ServiceDomainId,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.DeleteAjaxServiceDomainGrid(request, update);

            #endregion

            #region Assert

            _mockServiceDomainService.Verify(x => x.Delete(It.IsAny<ServiceDomain>()), Times.Never);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_DeleteAjaxServiceDomainGrid_DeletedServiceDomainDoesNotDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceDomainViewModel
            {
                Id = 99,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.DeleteAjaxServiceDomainGrid(request, update);

            #endregion

            #region Assert

            _mockServiceDomainService.Verify(x => x.Delete(It.IsAny<ServiceDomain>()), Times.Never);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_DeleteAjaxServiceDomainGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new ServiceDomainViewModel
            {
                Id = ServiceDomainId,
                AlternativeName = "An Alternative Name"
            };

            _mockServiceDomainService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _target.DeleteAjaxServiceDomainGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainController_DeleteServiceDomain_CallsServiceDomainDelete()
        {
            #region Arrange

            #endregion

            #region Act

            _target.DeleteServiceDomain(1);

            #endregion

            #region Assert

            _mockServiceDomainService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceDomainService.Verify(x => x.Delete(It.IsAny<ServiceDomain>()), Times.Once);

            #endregion
        }


        #region Method Authorization Requirement Tests

        [TestMethod]
        public void ServiceDomainController_Edit_HttpGet_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(string), typeof(int) }));
        }

        [TestMethod]
        public void ServiceDomainController_Edit_HttpPost_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(EditServiceDomainViewModel) }));
        }

        [TestMethod]
        public void ServiceDomainController_MoveGet_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Move", (AuthorizeAttribute att) => att.Roles, new Type[] { }));
        }

        [TestMethod]
        public void ServiceDomainController_MovePost_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Move", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(MoveServiceDomainViewModel) }));
        }

        [TestMethod]
        public void ServiceDomainController_Add_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Add", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDomainController_Index_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("Index", (AuthorizeAttribute att) => att.Roles));
        }
        [TestMethod]
        public void ServiceDomainController_UpdateAjaxServiceDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxServiceDomainGrid", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(DataSourceRequest), typeof(ServiceDomainViewModel) }));
        }

        [TestMethod]
        public void ServiceDomainController_DeleteAjaxServiceDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DeleteAjaxServiceDomainGrid", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(DataSourceRequest), typeof(ServiceDomainViewModel) }));
        }

        [TestMethod]
        public void ServiceDomainController_CreateAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("CreateAjaxServiceAddDomainGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDomainController_ReadAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("ReadAjaxServiceAddDomainGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDomainController_UpdateAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxServiceAddDomainGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDomainController_DestroyAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DestroyAjaxServiceAddDomainGrid", (AuthorizeAttribute att) => att.Roles));
        }

        #endregion
    }
}