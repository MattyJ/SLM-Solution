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
    public class ServiceFunctionControllerTests
    {
        private Mock<IServiceFunctionService> _mockServiceFunctionService;
        private Mock<IFunctionTypeRefDataService> _mockFunctionTypeRefDataService;
        private Mock<IServiceDomainService> _mockServiceDomainService;
        private Mock<IContextManager> _mockContextManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IRequestManager> _mockRequestManager;

        private Mock<IResponseManager> _mockResponseManager;
        private Mock<NameValueCollection> _mockNameValueCollection;

        private Mock<IParameterService> _mockParameterService;

        private List<ServiceDomain> _serviceDomains;
        private List<ServiceFunctionViewModel> _serviceFunctionViewModels;
        private List<BulkServiceFunctionViewModel> _bulkServiceFunctionViewModels;
        private List<FunctionTypeRefData> _functionTypeRefDatas;

        private List<ServiceFunctionListItem> _serviceFunctionListItems;

        private ServiceFunction _serviceFunction;
        private ServiceFunction _serviceFunctionUpdated;

        private ServiceDesk _serviceDesk;
        private ServiceDomain _serviceDomain;
        private DomainTypeRefData _domainType;

        private const int CustomerId = 555;
        private const int ServiceDeskId = 666;
        private const int ServiceDomainId = 777;
        private const int ServiceFunctionId = 888;
        private const int DomainTypeId = 999;
        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private ServiceFunctionController _target;

        private ControllerContextMocks _controllerContextMocks;

        [TestInitialize]
        public void Initialize()
        {
            Bootstrapper.SetupAutoMapper();

            _serviceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>();

            _functionTypeRefDatas = new List<FunctionTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<FunctionTypeRefData>(x =>
                {
                    x.Id = 1;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<FunctionTypeRefData>(x =>
                {
                    x.Id = 2;
                    x.Visible = false;
                }),
                UnitTestHelper.GenerateRandomData<FunctionTypeRefData>(x =>
                {
                    x.Id = 3;
                    x.Visible = false;
                }),
                UnitTestHelper.GenerateRandomData<FunctionTypeRefData>(x =>
                {
                    x.Id = 4;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<FunctionTypeRefData>(x =>
                {
                    x.Id = 5;
                    x.Visible = true;
                }),
            };

            _mockFunctionTypeRefDataService = new Mock<IFunctionTypeRefDataService>();
            _mockFunctionTypeRefDataService.Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId)).Returns(_functionTypeRefDatas);
            _mockFunctionTypeRefDataService.Setup(s => s.GetById(1)).Returns(_functionTypeRefDatas[0]);
            _mockFunctionTypeRefDataService.Setup(s => s.GetById(2)).Returns(_functionTypeRefDatas[1]);
            _mockFunctionTypeRefDataService.Setup(s => s.GetById(3)).Returns(_functionTypeRefDatas[2]);
            _mockFunctionTypeRefDataService.Setup(s => s.GetById(4)).Returns(_functionTypeRefDatas[3]);
            _mockFunctionTypeRefDataService.Setup(s => s.GetById(5)).Returns(_functionTypeRefDatas[4]);

            _serviceFunctionViewModels = new List<ServiceFunctionViewModel>
            {
                UnitTestHelper.GenerateRandomData<ServiceFunctionViewModel>(x =>
                {
                    x.ServiceDomainId = ServiceDomainId;
                }),
                UnitTestHelper.GenerateRandomData<ServiceFunctionViewModel>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionViewModel>(x =>
                {
                    x.ServiceDomainId = ServiceDomainId;
                }),
                UnitTestHelper.GenerateRandomData<ServiceFunctionViewModel>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionViewModel>(x =>
                {
                    x.ServiceDomainId = ServiceDomainId;
                })
            };

            _bulkServiceFunctionViewModels = new List<BulkServiceFunctionViewModel>
            {
                UnitTestHelper.GenerateRandomData<BulkServiceFunctionViewModel>(x =>
                {
                    x.ServiceDomainId = ServiceDomainId;
                    x.FunctionTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceFunctionViewModel>(),
                UnitTestHelper.GenerateRandomData<BulkServiceFunctionViewModel>(x =>
                {
                    x.ServiceDomainId = ServiceDomainId;
                    x.FunctionTypeId = 3;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceFunctionViewModel>(x =>
                {
                    x.FunctionTypeId = 4;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceFunctionViewModel>(x =>
                {
                    x.ServiceDomainId = ServiceDomainId;
                    x.FunctionTypeId = 0;
                })
            };

            _serviceFunctionListItems = new List<ServiceFunctionListItem>
            {
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
            };

            _serviceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>();
            _serviceFunction.ServiceComponents = new List<ServiceComponent>();

            _mockServiceFunctionService = new Mock<IServiceFunctionService>();
            _mockServiceFunctionService.Setup(s => s.GetById(ServiceFunctionId)).Returns(_serviceFunction);
            _mockServiceFunctionService.Setup(s => s.Update(It.IsAny<ServiceFunction>())).Callback<ServiceFunction>(c => _serviceFunctionUpdated = c);
            _mockServiceFunctionService.Setup(s => s.Create(It.IsAny<ServiceFunction>())).Callback<ServiceFunction>(c => _serviceFunctionUpdated = c);
            _mockServiceFunctionService.Setup(s => s.ServiceDomainFunctions(ServiceDomainId)).Returns(_serviceFunctionListItems.AsQueryable());
            _mockServiceFunctionService.Setup(s => s.CustomerServiceFunctions(CustomerId)).Returns(_serviceFunctionListItems.AsQueryable());

            _mockResponseManager = new Mock<IResponseManager>();
            _mockRequestManager = new Mock<IRequestManager>();
            _mockUserManager = new Mock<IUserManager>();
            _mockNameValueCollection = new Mock<NameValueCollection>();

            _mockUserManager.Setup(s => s.Name).Returns(UserName);

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);
            _mockContextManager.Setup(s => s.RequestManager).Returns(_mockRequestManager.Object);
            _mockRequestManager.Setup(s => s.Form).Returns(_mockNameValueCollection.Object);

            _mockParameterService = new Mock<IParameterService>();

            _serviceDesk = new ServiceDesk
            {
                Id = ServiceDeskId,
                DeskName = "MJJ Service Desk",
            };

            _domainType = new DomainTypeRefData
            {
                Id = DomainTypeId,
                DomainName = "Computing & Device Services",
                SortOrder = 5,
                Visible = true,
            };

            _serviceDomains = new List<ServiceDomain>
            {
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.ServiceDesk = _serviceDesk;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.ServiceDesk = _serviceDesk;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.ServiceDesk = _serviceDesk;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.Id = ServiceDomainId;
                    x.ServiceDesk = _serviceDesk;
                    x.AlternativeName = string.Empty;
                    x.DomainTypeId = DomainTypeId;
                    x.DomainType = _domainType;

                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.ServiceDesk = _serviceDesk;
                }),
            };

            _mockServiceDomainService = new Mock<IServiceDomainService>();
            _mockServiceDomainService.Setup(s => s.GetByCustomer(CustomerId)).Returns(_serviceDomains.AsQueryable());
            _mockServiceDomainService.Setup(s => s.GetById(ServiceDomainId)).Returns(_serviceDomain);

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            });

            _target = new ServiceFunctionController(_mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockParameterService.Object,
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
        public void ServiceFunctionController_Ctor_ServiceDomainServiceIsNull_ThrowsException()
        {
            new ServiceFunctionController(null,
                _mockServiceFunctionService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctionController_Ctor_ServiceFunctionServiceIsNull_ThrowsException()
        {
            new ServiceFunctionController(_mockServiceDomainService.Object,
                null,
                _mockFunctionTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctionController_Ctor_FunctionTypeRefDataServiceIsNull_ThrowsException()
        {
            new ServiceFunctionController(_mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                null,
                _mockParameterService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctionController_Ctor_ParameterServiceIsNull_ThrowsException()
        {
            new ServiceFunctionController(_mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockFunctionTypeRefDataService.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctionController_Ctor_ContextManagerIsNull_ThrowsException()
        {
            new ServiceFunctionController(_mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockParameterService.Object,
                null,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctionController_Ctor_AppUserContextIsNull_ThrowsException()
        {
            new ServiceFunctionController(_mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockContextManager.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ServiceFunctionController_Move_Get_PartialViewReturned()
        {
            var result = _target.Move() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_Get_ReturnedViewModelCorrectType()
        {
            var result = _target.Move() as PartialViewResult;
            Assert.IsNotNull(result);
            var vm = result.Model as MoveServiceFunctionViewModel;
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_AppUserContextCustomerIsNull_ReturnedViewModelServiceDomainsEmpty()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext());
            var result = _target.Move() as PartialViewResult;
            var vm = result.Model as MoveServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.AreEqual(0, vm.ServiceDomains.Count);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_CustomerIsSelected_GetByCustomerServiceIsCalled()
        {
            var result = _target.Move() as PartialViewResult;
            _mockServiceDomainService.Verify(v => v.GetByCustomer(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_CustomerIsSelected_ReturnsCorrectNumberOfServiceDomains()
        {
            var result = _target.Move() as PartialViewResult;
            var vm = result.Model as MoveServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.AreEqual(_serviceDomains.Count, vm.ServiceDomains.Count);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_ModelHasErrors_ReturnsErrorInResult()
        {
            const string error = "XXX";
            _target.ModelState.AddModelError(error, error);
            var result = _target.Move(new MoveServiceFunctionViewModel());
            dynamic data = result.Data;
            Assert.AreEqual(error, data.Message);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_ModelHasErrors_ReturnsSuccessFalse()
        {
            const string error = "XXX";
            _target.ModelState.AddModelError(error, error);
            var result = _target.Move(new MoveServiceFunctionViewModel());
            dynamic data = result.Data;
            Assert.AreEqual("False", data.Success);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_NoErrors_GetByIdIsCalled()
        {
            var result = _target.Move(new MoveServiceFunctionViewModel
            {
                ServiceDomainId = ServiceDomainId,
                ServiceFunctionId = ServiceFunctionId,
            });

            _mockServiceFunctionService.Verify(v => v.GetById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_NoErrors_UpdateIsCalled()
        {
            var result = _target.Move(new MoveServiceFunctionViewModel
            {
                ServiceDomainId = ServiceDomainId,
                ServiceFunctionId = ServiceFunctionId,
            });

            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_NoErrors_UpdatedBySet()
        {
            var result = _target.Move(new MoveServiceFunctionViewModel
            {
                ServiceDomainId = ServiceDomainId,
                ServiceFunctionId = ServiceFunctionId,
            });
            Assert.AreEqual(UserName, _serviceFunctionUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_NoErrors_UpdatedDateSet()
        {
            var now = DateTime.Now;
            var result = _target.Move(new MoveServiceFunctionViewModel
            {
                ServiceDomainId = ServiceDomainId,
                ServiceFunctionId = ServiceFunctionId,
            });
            Assert.AreEqual(now.Year, _serviceFunctionUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceFunctionUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceFunctionUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceFunctionUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceFunctionUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_NoErrors_ServiceDomainIdSetToNewValue()
        {
            var result = _target.Move(new MoveServiceFunctionViewModel
            {
                ServiceDomainId = ServiceDomainId,
                ServiceFunctionId = ServiceFunctionId,
            });
            Assert.AreEqual(ServiceDomainId, _serviceFunctionUpdated.ServiceDomainId);
        }

        [TestMethod]
        public void ServiceFunctionController_Move_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockServiceFunctionService.Setup(s => s.Update(It.IsAny<ServiceFunction>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            var result = _target.Move(new MoveServiceFunctionViewModel
            {
                ServiceDomainId = ServiceDomainId,
                ServiceFunctionId = ServiceFunctionId,
            });

            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }


        [TestMethod]
        public void ServiceFunctionController_Add_Get_ReturnsAddServiceFunctionViewModel()
        {
            _mockServiceDomainService.Setup(s => s.GetById(ServiceDomainId).ServiceFunctions.Count).Returns(2);

            var result = _target.Add(NavigationLevelNames.None, ServiceDomainId) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_LevelPassed_LevelIsAppendedToViewName()
        {
            var result = _target.Add(NavigationLevelNames.LevelOne, ServiceDomainId) as ViewResult;
            Assert.AreEqual("Add" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_NoCurrentCustomer_GetAllAndNotVisibleForCustomerNotCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext());
            _target.Add(NavigationLevelNames.LevelOne, ServiceDomainId);
            _mockFunctionTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(CustomerId), Times.Never);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_IdIsPassed_IdIsSetAsServiceDomainIdOnViewModel()
        {
            var result = _target.Add(NavigationLevelNames.None, ServiceDomainId) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;
            Assert.AreEqual(ServiceDomainId, vm.ServiceDomainId);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_LevelIsPassed_LevelIsSetAsEditLevelOnViewModel()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, ServiceDomainId) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelTwo, vm.EditLevel);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_CustomerIsPresentOnContext_GetAllAndNotVisibleForCustomerCalled()
        {
            _target.Add(NavigationLevelNames.LevelOne, ServiceDomainId);
            _mockFunctionTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_CustomerIsPresentOnContext_FunctionTypesReturnedOnViewModel()
        {
            var result = _target.Add(NavigationLevelNames.LevelOne, ServiceDomainId) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.AreEqual(5, vm.FunctionTypes.Count);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_CustomerIsPresentOnContext_FunctionTypesReturnedOnViewModelAreSelectListItems()
        {
            var result = _target.Add(NavigationLevelNames.LevelOne, ServiceDomainId) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;
            var list = vm.FunctionTypes as List<SelectListItem>;

            Assert.IsNotNull(vm);
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_ServiceDomainIdPassedIsZero_ViewModelSetsHasServiceDomainContextToFalse()
        {
            var result = _target.Add(NavigationLevelNames.LevelOne, 0) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.IsFalse(vm.HasServiceDomainContext);
        }

        [TestMethod]
        public void ServiceFunctionController_Add_ServiceDomainPassedIsZero_ViewModelSetsReturnUrlAsBackToServiceFunctionsIndex()
        {
            var result = _target.Add(NavigationLevelNames.LevelOne, 0) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.IsTrue(vm.ReturnUrl.StartsWith("/ServiceFunction/Index"));
        }

        [TestMethod]
        public void ServiceFunctionController_Add_ServiceDomainIdPassedIsGreaterThanZero_ViewModelSetsReturnUrlAsBackToEditServiceDomain()
        {
            var result = _target.Add(NavigationLevelNames.LevelOne, ServiceDomainId) as ViewResult;
            var vm = result.Model as AddServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.IsTrue(vm.ReturnUrl.StartsWith("/ServiceDomain/Edit"));
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_LevelOneIsSupplied_CheckLevelOneAppendedToViewName()
        {
            _mockServiceDomainService.Setup(s => s.GetById(ServiceDomainId).ServiceFunctions.Count).Returns(2);

            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceFunctionId) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_LevelTwoIsSupplied_CheckLevelTwoAppendedToViewName()
        {

            _mockServiceDomainService.Setup(s => s.GetById(ServiceDomainId).ServiceFunctions.Count).Returns(2);

            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceFunctionId) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelTwo, result.ViewName);
        }

        [TestMethod]
        public void ServiceFunctionController_Index_EditLevelPassed_EditLevelOnViewModel()
        {
            var result = _target.Index(NavigationLevelNames.LevelZero) as ViewResult;
            var vm = result.Model as ViewServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.AreEqual(NavigationLevelNames.LevelZero, vm.EditLevel);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Redirect_Post_NavigationLevelOne_RedirectToServiceDomainEdit()
        {
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDomain");
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Redirect_Post_NavigationLevelTwo_RedirectToServiceDomainEdit()
        {
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.LevelTwo,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDomain");
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }


        [TestMethod]
        public void ServiceFunctionController_Edit_Post_RedirectToServiceDomainEdit()
        {
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["controller"], "ServiceDomain");
            Assert.AreEqual(result.RouteValues["action"], "Edit");
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_NavigationLevelNoneValidEditCallsUpdateService()
        {
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_NavigationLevelOneValidEditCallsUpdateService()
        {
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_NavigationLevelTwoValidEditCallsUpdateService()
        {
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.LevelTwo,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_NavigationLevelNoneModalStateErrorRedirectsToEdit()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ViewName);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_NavigationLevelOneModalStateErrorRedirectsToEdit()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_NavigationLevelTwoModalStateErrorRedirectsToEdit()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.LevelTwo,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 1
                }
            };

            var result = _target.Edit(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Edit" + NavigationLevelNames.LevelTwo, result.ViewName);
        }


        [TestMethod]
        public void ServiceFunctionController_Edit_Post_ValidEditCallsUpdateServiceAndDoesNotSetTheDomainTypeToVisibleWhenLessThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockFunctionTypeRefDataService.Setup(s => s.GetNumberOfFunctionTypeReferences(It.IsAny<int>())).Returns(1);

            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 2
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetById(model.ServiceFunction.FunctionTypeId), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_ValidEditCallsUpdateServiceAndDoesNotSetTheDomainTypeToVisibleWhenEqualThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockFunctionTypeRefDataService.Setup(s => s.GetNumberOfFunctionTypeReferences(It.IsAny<int>())).Returns(2);

            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 2
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetById(model.ServiceFunction.FunctionTypeId), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_Edit_Post_ValidEditCallsUpdateServiceAndSetsTheDomainTypeToVisibleWhenGreaterThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockFunctionTypeRefDataService.Setup(s => s.GetNumberOfFunctionTypeReferences(It.IsAny<int>())).Returns(3);

            var model = new EditServiceFunctionViewModel
            {
                EditLevel = NavigationLevelNames.None,
                ServiceFunction = new ServiceFunctionViewModel
                {
                    Id = ServiceFunctionId,
                    AlternativeName = "New Function Name",
                    ServiceDomainId = ServiceDomainId,
                    FunctionTypeId = 2
                }
            };

            var result = _target.Edit(model);

            Assert.IsNotNull(result);
            _mockServiceFunctionService.Verify(v => v.Update(It.IsAny<ServiceFunction>()), Times.Once);
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetById(model.ServiceFunction.FunctionTypeId), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ModelStateNotValid_CheckMessageIsInResponse()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var result = _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), new List<BulkServiceFunctionViewModel>()) as JsonResult;
            dynamic data = result.Data;
            var errors = (Dictionary<string, Dictionary<string, object>>)data.Errors;
            Assert.IsTrue(errors.ContainsKey("XXX"));
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionsEmpty_EmptyResultReturned()
        {
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), new List<BulkServiceFunctionViewModel>());
            _mockServiceFunctionService.Verify(v => v.GetById(CustomerId), Times.Never);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ExistingServiceDomainNotFound_NoFunctionsCreated()
        {
            _mockServiceDomainService.Setup(s => s.GetByCustomer(CustomerId)).Returns(new List<ServiceDomain>().AsQueryable());
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            _mockServiceFunctionService.Verify(v => v.Create(It.IsAny<ServiceFunction>()), Times.Never);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ExistingServiceFound_ServiceFunctionCreatedForInputWithMatchingServiceDomain()
        {
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            _mockServiceFunctionService.Verify(v => v.Create(It.IsAny<ServiceFunction>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ExistingServiceFound_ServiceFunctionCreatedForInputForInputWithMatchingServiceDesksDomainTypeNotSetToVisibleWhenLt()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockFunctionTypeRefDataService.Setup(s => s.GetNumberOfFunctionTypeReferences(It.IsAny<int>())).Returns(1);

            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);

            _mockServiceFunctionService.Verify(v => v.Create(It.IsAny<ServiceFunction>()), Times.Exactly(2));
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockFunctionTypeRefDataService.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ExistingServiceFound_ServiceFunctionCreatedForInputForInputWithMatchingServiceDesksDomainTypeNotSetToVisibleWhenEq()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockFunctionTypeRefDataService.Setup(s => s.GetNumberOfFunctionTypeReferences(It.IsAny<int>())).Returns(2);

            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);

            _mockServiceFunctionService.Verify(v => v.Create(It.IsAny<ServiceFunction>()), Times.Exactly(2));
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockFunctionTypeRefDataService.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ExistingServiceFound_ServiceFunctionCreatedForInputForInputWithMatchingServiceDesksDomainTypeNotSetToVisibleWhenGt()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockFunctionTypeRefDataService.Setup(s => s.GetNumberOfFunctionTypeReferences(It.IsAny<int>())).Returns(2);

            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);

            _mockServiceFunctionService.Verify(v => v.Create(It.IsAny<ServiceFunction>()), Times.Exactly(2));
            _mockParameterService.Verify(x => x.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Exactly(2));
            _mockFunctionTypeRefDataService.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_OneOfTheInputFunctionDoesNotHaveTheFunctionTypeIdSet_FunctionIsSkipped()
        {
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            _mockServiceFunctionService.Verify(v => v.Create(It.IsAny<ServiceFunction>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionAdded_AlternativeNameSetOnCreatedEntity()
        {
            var expected = _bulkServiceFunctionViewModels.Last(x => x.FunctionTypeId != 0 && x.ServiceDomainId == ServiceDomainId);
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            Assert.AreEqual(expected.AlternativeName, _serviceFunctionUpdated.AlternativeName);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionAdded_FunctionTypeIdSetOnCreatedEntity()
        {
            var expected = _bulkServiceFunctionViewModels.Last(x => x.FunctionTypeId != 0 && x.ServiceDomainId == ServiceDomainId);
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            Assert.AreEqual(expected.FunctionTypeId, _serviceFunctionUpdated.FunctionTypeId);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionAdded_ServiceDomainIdSetOnCreatedEntity()
        {
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            Assert.AreEqual(ServiceDomainId, _serviceFunctionUpdated.ServiceDomainId);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionAdded_InsertedBySetOnCreatedEntity()
        {
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            Assert.AreEqual(UserName, _serviceFunctionUpdated.InsertedBy);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionAdded_InsertedDateSetOnCreatedEntity()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            Assert.AreEqual(now.Year, _serviceFunctionUpdated.InsertedDate.Year);
            Assert.AreEqual(now.Month, _serviceFunctionUpdated.InsertedDate.Month);
            Assert.AreEqual(now.Day, _serviceFunctionUpdated.InsertedDate.Day);
            Assert.AreEqual(now.Hour, _serviceFunctionUpdated.InsertedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceFunctionUpdated.InsertedDate.Minute);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionAdded_UpdatedBySetOnCreatedEntity()
        {
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            Assert.AreEqual(UserName, _serviceFunctionUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddFunctionGrid_ServiceFunctionAdded_UpdatedDateSetOnCreatedEntity()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddFunctionGrid(new DataSourceRequest(), _bulkServiceFunctionViewModels);
            Assert.AreEqual(now.Year, _serviceFunctionUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceFunctionUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceFunctionUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceFunctionUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceFunctionUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceFunctionController_Index_CustomerHasMoreThanOneServiceDomain_CanMoveServiceFunctionIsTrue()
        {
            var result = _target.Index(NavigationLevelNames.LevelZero) as ViewResult;
            var vm = result.Model as ViewServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.IsTrue(vm.CanMoveServiceFunction);
        }

        [TestMethod]
        public void ServiceFunctionController_Index_CustomerHasOneServiceDomain_CanMoveServiceFunctionIsFalse()
        {
            _mockServiceDomainService.Setup(s => s.GetByCustomer(CustomerId)).Returns(new List<ServiceDomain>().AsQueryable());
            var result = _target.Index(NavigationLevelNames.LevelZero) as ViewResult;
            var vm = result.Model as ViewServiceFunctionViewModel;

            Assert.IsNotNull(vm);
            Assert.IsFalse(vm.CanMoveServiceFunction);
        }

        [TestMethod]
        public void ServiceFunctionController_ReadAjaxServiceFunctionGrid_WithContextReturnsServiceFunctionViewModel()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceFunctionGrid(request, ServiceDomainId) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceFunctionViewModel>;

            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(model, typeof(List<ServiceFunctionViewModel>));

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_ReadAjaxServiceFunctionGrid_WithContextCallsServiceDomainFunctions()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceFunctionGrid(request, ServiceDomainId) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceFunctionViewModel>;

            Assert.IsNotNull(model);
            _mockServiceFunctionService.Verify(x => x.ServiceDomainFunctions(ServiceDomainId), Times.Once);
            Assert.AreEqual(_serviceFunctionListItems.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_ReadAjaxServiceFunctionGrid_WithOutServiceDomainIdReturnsServiceFunctionViewModel()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceFunctionGrid(request, null) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceFunctionViewModel>;

            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(model, typeof(List<ServiceFunctionViewModel>));

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_ReadAjaxServiceFunctionGrid_WithOutServiceDomainIdCallsCustomerServiceFunctions()
        {
            #region Arrange


            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _target.ReadAjaxServiceFunctionGrid(request, null) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceFunctionViewModel>;

            Assert.IsNotNull(model);
            _mockServiceFunctionService.Verify(x => x.CustomerServiceFunctions(CustomerId), Times.Once);
            Assert.AreEqual(_serviceFunctionListItems.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_ReadAjaxServiceFunctionGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            _mockServiceFunctionService.Setup(s => s.CustomerServiceFunctions(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            var result = _target.ReadAjaxServiceFunctionGrid(request, null) as JsonResult;

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_UpdateAjaxServiceFunctionGrid_CallsServiceFunctionGetByIdAndUpdate()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceFunctionViewModel
            {
                Id = ServiceFunctionId,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.UpdateAjaxServiceFunctionGrid(request, update);

            #endregion

            #region Assert

            _mockServiceFunctionService.Verify(x => x.GetById(ServiceFunctionId), Times.Once);
            _mockServiceFunctionService.Verify(x => x.Update(It.IsAny<ServiceFunction>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_UpdateAjaxServiceFunctionGrid_DeletedServiceFunctionAppendsHandledErrorMessage()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceFunctionViewModel
            {
                Id = 99,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.UpdateAjaxServiceFunctionGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", WebResources.ServiceFunctionCannotBeFound), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_UpdateAjaxServiceFunctionGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new ServiceFunctionViewModel
            {
                Id = ServiceFunctionId,
                AlternativeName = "An Alternative Name"
            };

            _mockServiceFunctionService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _target.UpdateAjaxServiceFunctionGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_DeleteAjaxServiceFunctionGrid_CallsServiceFunctionGetByIdAndDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceFunctionViewModel
            {
                Id = ServiceFunctionId,
                AlternativeName = "An Alternative Name",
            };

            #endregion

            #region Act

            _target.DeleteAjaxServiceFunctionGrid(request, update);

            #endregion

            #region Assert

            _mockServiceFunctionService.Verify(x => x.GetById(ServiceFunctionId), Times.Once);
            _mockServiceFunctionService.Verify(x => x.Delete(It.IsAny<ServiceFunction>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_DeleteAjaxServiceFunctionGrid_DeleteServiceFunctionWithComponentsDoesNotDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();
            var serviceComponents = new List<ServiceComponent>()
            {
                UnitTestHelper.GenerateRandomData<ServiceComponent>(),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(),
            };

            _serviceFunction.ServiceComponents = serviceComponents;

            var update = new ServiceFunctionViewModel
            {
                Id = ServiceFunctionId,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.DeleteAjaxServiceFunctionGrid(request, update);

            #endregion

            #region Assert

            _mockServiceFunctionService.Verify(x => x.Delete(It.IsAny<ServiceFunction>()), Times.Never);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_DeleteAjaxServiceFunctionGrid_DeletedServiceFunctionDoesNotDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new ServiceFunctionViewModel
            {
                Id = 99,
                AlternativeName = "An Alternative Name"
            };

            #endregion

            #region Act

            _target.DeleteAjaxServiceFunctionGrid(request, update);

            #endregion

            #region Assert

            _mockServiceFunctionService.Verify(x => x.Delete(It.IsAny<ServiceFunction>()), Times.Never);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionController_DeleteAjaxServiceFunctionGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new ServiceFunctionViewModel
            {
                Id = ServiceFunctionId,
                AlternativeName = "An Alternative Name"
            };

            _mockServiceFunctionService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _target.DeleteAjaxServiceFunctionGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }


        [TestMethod]
        public void ServiceFunctionController_DeleteServiceFunction_CallsServiceFunctionDelete()
        {
            #region Arrange

            #endregion

            #region Act

            _target.DeleteServiceFunction(1);

            #endregion

            #region Assert

            _mockServiceFunctionService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceFunctionService.Verify(x => x.Delete(It.IsAny<ServiceFunction>()), Times.Once);

            #endregion
        }

        #region Method Authorization Requirement Tests

        [TestMethod]
        public void ServiceFunctionController_MoveGet_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Move", (AuthorizeAttribute att) => att.Roles, new Type[] { }));
        }

        [TestMethod]
        public void ServiceFunctionController_MovePost_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Move", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(MoveServiceFunctionViewModel) }));
        }

        [TestMethod]
        public void ServiceFunctionController_Add_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Add", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceFunctionController_UpdateAjaxServiceFunctionGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxServiceFunctionGrid", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(DataSourceRequest), typeof(ServiceFunctionViewModel) }));
        }

        [TestMethod]
        public void ServiceFunctionController_DeleteAjaxServiceFunctionGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DeleteAjaxServiceFunctionGrid", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(DataSourceRequest), typeof(ServiceFunctionViewModel) }));
        }

        [TestMethod]
        public void ServiceFunctionController_CreateAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("CreateAjaxServiceAddFunctionGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceFunctionController_ReadAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("ReadAjaxServiceAddFunctionGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceFunctionController_UpdateAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxServiceAddFunctionGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceFunctionController_DestroyAjaxServiceAddDomainGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DestroyAjaxServiceAddFunctionGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceFunctionController_Index_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("Index", (AuthorizeAttribute att) => att.Roles));
        }

        #endregion
    }
}