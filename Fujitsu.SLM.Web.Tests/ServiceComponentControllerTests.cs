using CloneExtensions;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.ModelHelpers;
using Fujitsu.SLM.Services.Entities;
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
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;
using EnumExtensions = Fujitsu.SLM.Enumerations.EnumExtensions;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    public class ServiceComponentControllerTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IResponseManager> _mockResponseManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IServiceComponentService> _mockServiceComponentService;
        private Mock<IServiceFunctionService> _mockServiceFunctionService;
        private Mock<IServiceComponentHelper> _mockServiceComponentHelper;
        private Mock<IServiceDeliveryOrganisationTypeRefDataService> _mockServiceDeliveryOrganisationTypeRefDataService;
        private Mock<IServiceDeliveryUnitTypeRefDataService> _mockServiceDeliveryUnitTypeRefDataService;
        private Mock<IResolverGroupTypeRefDataService> _mockResolverGroupTypeRefDataService;
        private Mock<IParameterService> _mockParameterService;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private const string ServiceComponentName1 = "ServiceComponentName1";
        private const string ServiceComponentName2 = "ServiceComponentName2";
        private const string ServiceComponentName3 = "ServiceComponentName3";
        private const string ServiceComponentParentName1 = "ServiceComponentParentName1";
        private const string ServiceComponentChildName1 = "ServiceComponentChildName1";
        private const string ServiceComponentParentName2 = "ServiceComponentParentName2";
        private const string ServiceComponentChildName2 = "ServiceComponentChildName2";
        private const int CustomerId = 22;
        private const int ServiceFunctionId = 44;
        private const int ServiceFunctionIdDestination = 2244;
        private const int ServiceComponentIdNotPresent = 77;
        private const int ServiceComponentIdExistingDestination = 3456;
        private const int ServiceComponentIdWithNoDependents = 87;
        private const int ServiceComponentIdWithChildComponentDependent = 88;
        private const int ServiceComponentIdWithResolverDependent = 99;
        private const int ServiceComponentIdLevel2WithResolverDependent = 111;
        private const int ServiceComponentIdChildServiceComponentIdWithParent = 112;
        private AppContext _appContext;
        private ServiceFunction _serviceFunction1;
        private ServiceFunction _serviceFunction2;
        private ServiceComponent _serviceComponentUpdated;
        private IEnumerable<ServiceComponent> _serviceComponentBulkCreated;
        private IEnumerable<ServiceComponent> _serviceComponentBulkUpdated;
        private ServiceComponentController _target;
        private List<BulkServiceComponentViewModel> _bulkServiceComponentLevel1ViewModels;
        private List<BulkServiceComponentViewModel> _bulkServiceComponentLevel2ViewModels;
        private List<ServiceComponent> _serviceComponents;
        private List<ServiceComponentListItem> _serviceComponentListItems;
        private List<ServiceFunctionListItem> _serviceFunctionListItems;
        private ControllerContextMocks _controllerContextMocks;
        private List<FunctionTypeRefData> _functionTypeRefData;
        private List<ServiceDeliveryOrganisationTypeRefData> _serviceDeliveryOrganisationTypeRefDatas;
        private List<ServiceDeliveryUnitTypeRefData> _serviceDeliveryUnitTypeRefDatas;
        private List<ResolverGroupTypeRefData> _resolverGroupTypeRefDatas;

        [TestInitialize]
        public void Initialize()
        {
            #region Data Collections

            _functionTypeRefData = new List<FunctionTypeRefData>();

            var fncA = new FunctionTypeRefData
            {
                Id = 1,
                FunctionName = "Function A",
                SortOrder = 5,
                Visible = true
            };
            var fncB = new FunctionTypeRefData
            {
                Id = 2,
                FunctionName = "Function B",
                SortOrder = 5,
                Visible = true
            };

            _functionTypeRefData.Add(fncA);
            _functionTypeRefData.Add(fncB);


            _serviceFunction1 = UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
            {
                x.FunctionType = _functionTypeRefData[0];
            });

            _serviceFunction2 = UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
            {
                x.Id = ServiceFunctionId;
                x.FunctionType = _functionTypeRefData[1];
            });

            _serviceComponents = new List<ServiceComponent>
            {
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 0
                {
                    x.Id = ServiceComponentIdWithNoDependents;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.ServiceFunction = _serviceFunction1;
                    x.ServiceFunctionId = _serviceFunction1.Id;
                    x.Resolver = null;
                    x.ComponentName = ServiceComponentName1;
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.FunctionType = UnitTestHelper.GenerateRandomData<FunctionTypeRefData>();
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 1
                {
                    x.Id = ServiceComponentIdWithChildComponentDependent;
                    x.ComponentName = ServiceComponentParentName1;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.ServiceFunction = _serviceFunction1;
                    x.Resolver = null;
                    x.ChildServiceComponents = new List<ServiceComponent>
                    {
                        UnitTestHelper.GenerateRandomData<ServiceComponent>(y =>
                        {
                            y.ComponentName = ServiceComponentChildName1;
                            y.ComponentLevel = (int) ServiceComponentLevel.Level2;
                            y.ParentServiceComponent = x;
                            y.ParentServiceComponentId = x.Id;
                            y.ServiceFunction = _serviceFunction1;
                            y.Resolver = UnitTestHelper.GenerateRandomData<Resolver>();
                        }),
                    };
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.FunctionType = UnitTestHelper.GenerateRandomData<FunctionTypeRefData>();
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 2
                {
                    x.Id = ServiceComponentIdWithResolverDependent;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.ServiceFunction = _serviceFunction1;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(y =>
                    {
                        y.Id = ServiceComponentIdWithResolverDependent;
                        y.ServiceDeliveryOrganisationType =
                            UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>();
                        y.ServiceDeliveryUnitType =
                            UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>();
                        y.ResolverGroupType =
                            UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>();
                    });
                    x.ChildServiceComponents = null;
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.FunctionType = UnitTestHelper.GenerateRandomData<FunctionTypeRefData>();
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 3
                {
                    x.Id = ServiceComponentIdLevel2WithResolverDependent;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level2;
                    x.ServiceFunction = _serviceFunction1;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>();
                    x.ChildServiceComponents = null;
                    x.ParentServiceComponent = UnitTestHelper.GenerateRandomData<ServiceComponent>(y =>
                    {
                        y.Id = 1;
                        y.ComponentLevel = (int) ServiceComponentLevel.Level2;
                        y.ComponentName = "Parent Component";
                    });
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.FunctionType = UnitTestHelper.GenerateRandomData<FunctionTypeRefData>();
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 4
                {
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.ServiceFunction = _serviceFunction2;
                    x.ComponentName = ServiceComponentName2;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>();
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.FunctionType = UnitTestHelper.GenerateRandomData<FunctionTypeRefData>();
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 5
                {
                    x.ComponentName = ServiceComponentParentName2;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.ServiceFunction = _serviceFunction2;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>();
                    x.ChildServiceComponents = new List<ServiceComponent>
                    {
                        UnitTestHelper.GenerateRandomData<ServiceComponent>(y =>
                        {
                            y.ComponentName = ServiceComponentChildName2;
                            y.ComponentLevel = (int) ServiceComponentLevel.Level2;
                            y.ParentServiceComponent = x;
                            y.ServiceFunction = _serviceFunction2;
                        }),
                    };
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.FunctionType = UnitTestHelper.GenerateRandomData<FunctionTypeRefData>();
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 6
                {
                    x.Id = ServiceComponentIdExistingDestination;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.ServiceFunction = _serviceFunction2;
                    x.ServiceFunctionId = _serviceFunction2.Id;
                    x.ComponentName = ServiceComponentName3;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>();
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.FunctionType = UnitTestHelper.GenerateRandomData<FunctionTypeRefData>();
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                new ServiceComponent
                {
                    Id = ServiceComponentIdChildServiceComponentIdWithParent,
                    ComponentLevel = (int) ServiceComponentLevel.Level2,
                    ServiceFunction = _serviceFunction1,
                    ServiceFunctionId = _serviceFunction1.Id,
                    Resolver = null,
                    ComponentName = "Level 2 Child Component",
                    ParentServiceComponentId = 999,
                    ParentServiceComponent = new ServiceComponent {Id = 999, ComponentName = "Parent Component"},
                }
            };


            _serviceComponentListItems = new List<ServiceComponentListItem>
            {
                UnitTestHelper.GenerateRandomData<ServiceComponentListItem>(x =>
                {
                    x.ServiceFunctionId = _serviceComponents[0].ServiceFunctionId;
                    x.ServiceComponent = _serviceComponents[0];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponentListItem>(x =>
                {
                    x.ServiceFunctionId = _serviceComponents[1].ServiceFunctionId;
                    x.ServiceComponent = _serviceComponents[1];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponentListItem>(x =>
                {
                    x.ServiceFunctionId = _serviceComponents[2].ServiceFunctionId;
                    x.ServiceComponent = _serviceComponents[2];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponentListItem>(x =>
                {
                    x.ServiceFunctionId = _serviceComponents[3].ServiceFunctionId;
                    x.ServiceComponent = _serviceComponents[3];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponentListItem>(x =>
                {
                    x.ServiceFunctionId = _serviceComponents[4].ServiceFunctionId;
                    x.ServiceComponent = _serviceComponents[4];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponentListItem>(x =>
                {
                    x.ServiceFunctionId = _serviceComponents[5].ServiceFunctionId;
                    x.ServiceComponent = _serviceComponents[5];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponentListItem>(x =>
                {
                    x.ServiceFunctionId = _serviceComponents[6].ServiceFunctionId;
                    x.ServiceComponent = _serviceComponents[6];
                })
            };

            _bulkServiceComponentLevel1ViewModels = new List<BulkServiceComponentViewModel>
            {
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ServiceFunctionId = 0;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ComponentName = string.Empty;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ComponentName = null;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>()
            };

            _bulkServiceComponentLevel2ViewModels = new List<BulkServiceComponentViewModel>
            {
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ServiceComponentLevel1Id = _serviceComponents
                        .Single(y => y.ComponentName == ServiceComponentName1).Id;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ServiceComponentLevel1Id = 0;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ServiceComponentLevel1Id = _serviceComponents
                        .Single(y => y.ComponentName == ServiceComponentName2).Id;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ComponentName = string.Empty;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ServiceComponentLevel1Id = _serviceComponents
                        .Single(y => y.ComponentName == ServiceComponentName3).Id;
                }),
                UnitTestHelper.GenerateRandomData<BulkServiceComponentViewModel>(x =>
                {
                    x.ComponentName = null;
                })
            };

            _serviceDeliveryOrganisationTypeRefDatas = new List<ServiceDeliveryOrganisationTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>()
            };

            _serviceDeliveryUnitTypeRefDatas = new List<ServiceDeliveryUnitTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>()
            };

            _resolverGroupTypeRefDatas = new List<ResolverGroupTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>()
            };

            _serviceFunctionListItems = new List<ServiceFunctionListItem>
            {
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>(),
                UnitTestHelper.GenerateRandomData<ServiceFunctionListItem>()
            };

            _serviceFunctionListItems[0].Id = ServiceFunctionId;
            _serviceFunctionListItems[0].FunctionName = "Service Function A";

            #endregion

            Bootstrapper.SetupAutoMapper();

            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserName);

            _mockResponseManager = new Mock<IResponseManager>();

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            };

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockServiceComponentService = new Mock<IServiceComponentService>();
            _mockServiceComponentService.Setup(s => s.GetByCustomer(CustomerId))
                .Returns(_serviceComponents.AsQueryable());
            _mockServiceComponentService.Setup(s => s.GetByCustomerWithHierarchy(CustomerId))
                .Returns(_serviceComponentListItems.AsQueryable());
            _mockServiceComponentService.Setup(s => s.Update(It.IsAny<ServiceComponent>()))
                .Callback<ServiceComponent>(c => _serviceComponentUpdated = c);
            _mockServiceComponentService.Setup(s => s.Create(It.IsAny<IEnumerable<ServiceComponent>>()))
                .Callback<IEnumerable<ServiceComponent>>(c => _serviceComponentBulkCreated = c);
            _mockServiceComponentService.Setup(s => s.Update(It.IsAny<IEnumerable<ServiceComponent>>()))
                .Callback<IEnumerable<ServiceComponent>>(c => _serviceComponentBulkUpdated = c);

            _mockServiceFunctionService = new Mock<IServiceFunctionService>();
            _mockServiceFunctionService.Setup(s => s.CustomerServiceFunctions(CustomerId))
                .Returns(_serviceFunctionListItems.AsQueryable());

            _mockServiceComponentHelper = new Mock<IServiceComponentHelper>();

            _mockServiceDeliveryOrganisationTypeRefDataService = new Mock<IServiceDeliveryOrganisationTypeRefDataService>();
            _mockServiceDeliveryOrganisationTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _serviceDeliveryOrganisationTypeRefDatas.SingleOrDefault(x => x.Id == id));
            _mockServiceDeliveryOrganisationTypeRefDataService.Setup(s => s.All())
                .Returns(_serviceDeliveryOrganisationTypeRefDatas.AsQueryable());

            _mockServiceDeliveryUnitTypeRefDataService = new Mock<IServiceDeliveryUnitTypeRefDataService>();
            _mockServiceDeliveryUnitTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _serviceDeliveryUnitTypeRefDatas.SingleOrDefault(x => x.Id == id));
            _mockServiceDeliveryUnitTypeRefDataService.Setup(s => s.All())
                .Returns(_serviceDeliveryUnitTypeRefDatas.AsQueryable());

            _mockResolverGroupTypeRefDataService = new Mock<IResolverGroupTypeRefDataService>();
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _resolverGroupTypeRefDatas.SingleOrDefault(x => x.Id == id));
            _mockResolverGroupTypeRefDataService.Setup(s => s.All())
                .Returns(_resolverGroupTypeRefDatas.AsQueryable());
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.GetNumberOfResolverGroupTypeReferences(It.IsAny<int>()))
                .Returns(0);

            _mockParameterService = new Mock<IParameterService>();
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(3);

            _target = new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);

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
        public void ServiceComponentController_Ctor_ContextManagerNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(null,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_AppUserContextNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                null,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_ServiceComponentServiceNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                null,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_ServiceFunctionServiceNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                null,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_ServiceDeliveryOrganisationTypeServiceNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                null,
                _mockResolverGroupTypeRefDataService.Object,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_ServiceDeliveryUnitTypeServiceNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                null,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_ResolverGroupTypeRefDataServiceNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                null,
                _mockParameterService.Object,
                _mockServiceComponentHelper.Object);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_ParameterServiceNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                null,
                _mockServiceComponentHelper.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentController_Ctor_ServiceComponentHelperNull_ThrowsArgumentNullException()
        {
            new ServiceComponentController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockServiceComponentService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockParameterService.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ServiceComponentController_Index_LevelProvided_ViewIsLevelName()
        {
            var result = _target.Index(NavigationLevelNames.LevelOne) as ViewResult;
            Assert.AreEqual(NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_Index_MoreThan1CustomerServiceFunction_CanMoveTrue()
        {
            var result = _target.Index(NavigationLevelNames.LevelOne) as ViewResult;
            var model = result.Model as ViewServiceComponentViewModel;
            Assert.IsTrue(model.CanMoveServiceComponent);
        }

        [TestMethod]
        public void ServiceComponentController_Index_NoCustomerServiceFunction_CanMoveFalse()
        {
            _mockServiceFunctionService.Setup(s => s.CustomerServiceFunctions(CustomerId))
                .Returns(new List<ServiceFunctionListItem>().AsQueryable());
            var result = _target.Index(NavigationLevelNames.LevelOne) as ViewResult;
            var model = result.Model as ViewServiceComponentViewModel;
            Assert.IsFalse(model.CanMoveServiceComponent);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_CurrentCustomerNull_ResultIsNull()
        {
            SetCustomerContextNull();
            var result = _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), ServiceFunctionId) as JsonResult;
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_CurrentCustomerZero_ResultIsNull()
        {
            SetCustomerContextZero();
            var result = _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), ServiceFunctionId) as JsonResult;
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetGetByCustomerWithHierarchyException();
            _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), ServiceFunctionId);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerWithHierarchyException();
            _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), ServiceFunctionId);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_DataReturned_ServiceComponentServiceGetByCustomerUsed()
        {
            _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), ServiceFunctionId);
            _mockServiceComponentService.Verify(v => v.GetByCustomerWithHierarchy(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_ServiceFunctionIdNotSupplied_AllComponentsReturned()
        {
            var result = _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), null) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;

            var data = kendoDataSource.Data as List<ServiceComponentViewModel>;
            Assert.AreEqual(8, data.Count);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_Check1_Level2ComponentsFollowParents()
        {
            var result = _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), null) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var data = kendoDataSource.Data as List<ServiceComponentViewModel>;
            var parentIndex = data.FindIndex(x => x.ComponentName == ServiceComponentParentName1);
            var childIndex = data.FindIndex(x => x.ComponentName == ServiceComponentChildName1);
            Assert.AreEqual(parentIndex + 1, childIndex);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_Check2_Level2ComponentsFollowParents()
        {
            var result = _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), null) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var data = kendoDataSource.Data as List<ServiceComponentViewModel>;
            var parentIndex = data.FindIndex(x => x.ComponentName == ServiceComponentParentName2);
            var childIndex = data.FindIndex(x => x.ComponentName == ServiceComponentChildName2);
            Assert.AreEqual(parentIndex + 1, childIndex);
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_ServiceFunctionIdSupplied_OnlySuppliedFunctionIdComponentsReturned()
        {
            var result = _target.ReadAjaxServiceComponentsGrid(new DataSourceRequest(), ServiceFunctionId) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var data = kendoDataSource.Data as List<ServiceComponentViewModel>;
            Assert.IsFalse(data.Any(x => x.ServiceFunctionId != ServiceFunctionId));
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_CurrentCustomerNull_NoDeleteTakesPlace()
        {
            SetCustomerContextNull();
            var result = _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel()) as JsonResult;
            _mockServiceComponentService.Verify(v => v.Delete(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_CurrentCustomerZero_NoDeleteTakesPlace()
        {
            SetCustomerContextZero();
            var result = _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel()) as JsonResult;
            _mockServiceComponentService.Verify(v => v.Delete(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetGetByCustomerException();
            _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerException();
            _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_ServiceComponentCannotBeFound_ErrorAddedToModelState()
        {
            var result = _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel() { Id = 666 }) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ServiceComponentCannotBeFound));
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_ServiceComponentCannotBeFound_NoDeleteTakesPlace()
        {
            _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel() { Id = 666 });
            _mockServiceComponentService.Verify(v => v.Delete(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_ServiceComponentFoundButHasDependents_ErrorAddedToModelState()
        {
            var result = _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel() { Id = ServiceComponentIdWithChildComponentDependent }) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ServiceComponentCannotBeDeletedDueToDependents));
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_ServiceComponentFoundButHasDependents_NoDeleteTakesPlace()
        {
            _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel() { Id = ServiceComponentIdWithChildComponentDependent });
            _mockServiceComponentService.Verify(v => v.Delete(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_ServiceComponentFoundAndCanBeDeleted_DeleteTakesPlace()
        {
            SetCanDeleteTrue();
            _target.DeleteAjaxServiceComponentsGrid(new DataSourceRequest(), new ServiceComponentViewModel() { Id = ServiceComponentIdWithNoDependents });
            _mockServiceComponentService.Verify(v => v.Delete(It.IsAny<ServiceComponent>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_CustomerContextIsNull_EmptyViewModelReturned()
        {
            SetCustomerContextNull();
            var result = _target.MoveLevel1() as PartialViewResult;
            var model = result.Model as MoveServiceComponentLevel1ViewModel;
            Assert.AreEqual(0, model.ServiceFunctions.Count);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_CustomerContextIsZero_EmptyViewModelReturned()
        {
            SetCustomerContextZero();
            var result = _target.MoveLevel1() as PartialViewResult;
            var model = result.Model as MoveServiceComponentLevel1ViewModel;
            Assert.AreEqual(0, model.ServiceFunctions.Count);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_HasCustomerContext_SelectListPopulated()
        {
            var result = _target.MoveLevel1() as PartialViewResult;
            var model = result.Model as MoveServiceComponentLevel1ViewModel;
            Assert.AreEqual(5, model.ServiceFunctions.Count);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_CustomerContextIsNull_EmptyViewModelReturned()
        {
            SetCustomerContextNull();
            var result = _target.MoveLevel2() as PartialViewResult;
            var model = result.Model as MoveServiceComponentLevel2ViewModel;
            Assert.AreEqual(0, model.ServiceComponents.Count);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_CustomerContextIsZero_EmptyViewModelReturned()
        {
            SetCustomerContextZero();
            var result = _target.MoveLevel2() as PartialViewResult;
            var model = result.Model as MoveServiceComponentLevel2ViewModel;
            Assert.AreEqual(0, model.ServiceComponents.Count);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_HasCustomerContext_SelectListPopulatedWithOnlyLevel1AndNoChildren()
        {
            var expected = _serviceComponents
                .Where(w => w.ComponentLevel == 1 &&
                    w.Resolver == null)
                .Select(s => s.Id.ToString())
                .ToList();

            var result = _target.MoveLevel2() as PartialViewResult;

            var model = result.Model as MoveServiceComponentLevel2ViewModel;
            var actual = model.ServiceComponents.Select(s => s.Value).ToList();

            Assert.AreEqual(87, int.Parse(expected[0]));
            Assert.AreEqual(88, int.Parse(expected[1]));
            Assert.AreEqual(87, int.Parse(actual[0]));
            Assert.AreEqual(88, int.Parse(actual[1]));
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ModelStateInvalid_ErrorMessageAddedToResponseHeader()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, "XXX"), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ModelStateInvalid_SetsStatusCodeTo500()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_CustomerContextNull_NoMoveOccurs()
        {
            SetCustomerContextNull();
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionId
            });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_CustomerContextZero_NoMoveOccurs()
        {
            SetCustomerContextZero();
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionId
            });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentCannotBeFound_SetsStatusCodeTo500()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = 666,
                DestinationServiceFunctionId = ServiceFunctionId
            });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentCannotBeFound_AppendsErrorMessageToHeader()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = 666,
                DestinationServiceFunctionId = ServiceFunctionId
            });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, WebResources.ServiceComponentCannotBeFound), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentFound_UpdateIsCalled()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionId
            });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentWithChildFound_ServiceFunctionIdUpdated()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });
            Assert.AreEqual(ServiceFunctionIdDestination, _serviceComponentUpdated.ServiceFunctionId);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentWithChildFound_UpdatedBySet()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });
            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentWithChildFound_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });
            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentWithChildFound_ChildServiceFunctionIdUpdated()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });

            Assert.AreEqual(ServiceFunctionIdDestination, _serviceComponentUpdated.ChildServiceComponents.First().ServiceFunctionId);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentWithChildFound_ChildUpdatedBySet()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });

            Assert.AreEqual(UserName, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentWithChildFound_ChildUpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });

            Assert.AreEqual(now.Year, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Hour);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ComponentWithChildFound_OnlyExpectedFieldsUpdated()
        {

            var initializers = new Dictionary<Type, Func<object, object>> {
                    { typeof(ICollection<ServiceComponent>), (s) => new List<ServiceComponent>() }
                };

            var expected = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithChildComponentDependent)
                .GetClone(initializers);

            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });

            var compare = new CompareLogic(new ComparisonConfig
            {
                MembersToIgnore = new List<string>
                {
                    "ChildServiceComponents",
                    "UpdatedBy",
                    "UpdatedDate",
                    "ServiceFunctionId"
                },
                MaxDifferences = 100
            });
            var same = compare.Compare(expected, _serviceComponentUpdated);
            Assert.IsTrue(same.AreEqual);
        }

        public void ServiceComponentController_MoveLevel1_ComponentWithNoChildFound_ServiceFunctionIdUpdated()
        {
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceFunctionId = ServiceFunctionIdDestination
            });
            Assert.AreEqual(ServiceFunctionIdDestination, _serviceComponentUpdated.ServiceFunctionId);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetGetByCustomerException();
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerException();
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_ModelStateInvalid_ErrorMessageAddedToResponseHeader()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, "XXX"), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_ModelStateInvalid_SetsStatusCodeTo500()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_CustomerContextNull_NoMoveOccurs()
        {
            SetCustomerContextNull();
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_CustomerContextZero_NoMoveOccurs()
        {
            SetCustomerContextZero();
            _target.MoveLevel1(new MoveServiceComponentLevel1ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithChildComponentDependent,
                DestinationServiceFunctionId = ServiceComponentIdExistingDestination
            });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_ExistingServiceComponentCannotBeFound_SetsStatusCodeTo500()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdNotPresent,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_ExistingServiceComponentCannotBeFound_AppendsErrorMessageToHeader()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdNotPresent,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, WebResources.ServiceComponentCannotBeFound), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_ExistingServiceComponentCannotBeFound_NoUpdateTakesPalce()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdNotPresent,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_DestinationServiceComponentCannotBeFound_SetsStatusCodeTo500()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdNotPresent
            });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_DestinationComponentCannotBeFound_AppendsErrorMessageToHeader()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdNotPresent
            });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, WebResources.ServiceComponentCannotBeFound), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_DestinationServiceComponentCannotBeFound_NoUpdateTakesPalce()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdNotPresent
            });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }


        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_ServiceComponentIsAttachedToDestinationServiceComponent()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });

            Assert.IsNotNull(_serviceComponentUpdated.ChildServiceComponents);
            Assert.IsTrue(_serviceComponentUpdated.ChildServiceComponents.ToList().TrueForAll(x => x.ParentServiceComponentId == ServiceComponentIdExistingDestination));
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_ServiceFunctionUpdatedOnMovingServiceComponentToSameAsParent()
        {
            var current = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithNoDependents)
                .GetClone();
            var expected = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdExistingDestination)
                .GetClone();
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });

            Assert.AreNotEqual(current.ServiceFunctionId, _serviceComponentUpdated.ChildServiceComponents.First().ServiceFunctionId);
            Assert.AreEqual(expected.ServiceFunctionId, _serviceComponentUpdated.ChildServiceComponents.First().ServiceFunctionId);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_UpdatedBySet()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });
            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });
            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_ChildUpdatedBySet()
        {
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });

            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
            Assert.IsTrue(_serviceComponentUpdated.ChildServiceComponents.ToList().TrueForAll(x => x.UpdatedBy == UserName));

        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_ChildUpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });


            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.IsTrue(_serviceComponentUpdated.ChildServiceComponents.ToList().TrueForAll(x => x.UpdatedDate.Year == now.Year));
            Assert.IsTrue(_serviceComponentUpdated.ChildServiceComponents.ToList().TrueForAll(x => x.UpdatedDate.Month == now.Month));
            Assert.IsTrue(_serviceComponentUpdated.ChildServiceComponents.ToList().TrueForAll(x => x.UpdatedDate.Day == now.Day));
            Assert.IsTrue(_serviceComponentUpdated.ChildServiceComponents.ToList().TrueForAll(x => x.UpdatedDate.Hour == now.Hour));
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_OnlyExpectedFieldsUpdatedOnParent()
        {
            var expected = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdExistingDestination)
                .GetClone();

            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });

            var compare = new CompareLogic(new ComparisonConfig
            {
                MembersToIgnore = new List<string>
                {
                    "UpdatedBy",
                    "UpdatedDate",
                    "ChildServiceComponents"
                },
                MaxDifferences = 100
            });
            var same = compare.Compare(expected, _serviceComponentUpdated);
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_MoveValid_OnlyExpectedFieldsUpdatedOnChild()
        {
            var expected = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithNoDependents)
                .GetClone();

            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel
            {
                ServiceComponentId = ServiceComponentIdWithNoDependents,
                DestinationServiceComponentId = ServiceComponentIdExistingDestination
            });
            var compare = new CompareLogic(new ComparisonConfig
            {
                MembersToIgnore = new List<string>
                {
                    "UpdatedBy",
                    "UpdatedDate",
                    "ServiceFunctionId",
                    "ParentServiceComponentId",
                },
                MaxDifferences = 100
            });

            var same = compare.Compare(expected, _serviceComponentUpdated.ChildServiceComponents.First());
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetGetByCustomerException();
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerException();
            _target.MoveLevel2(new MoveServiceComponentLevel2ViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel1_LevelPassed_IsUsedToFormReturningViewName()
        {
            var result = _target.AddLevel1(NavigationLevelNames.LevelTwo, ServiceFunctionId) as ViewResult;
            Assert.AreEqual("AddLevelTwoL1", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel1_IdNotPassed_HasServiceFunctionContextFalse()
        {
            var result = _target.AddLevel1(NavigationLevelNames.LevelTwo, null) as ViewResult;
            var model = result.Model as AddServiceComponentLevel1ViewModel;
            Assert.IsFalse(model.HasServiceFunctionContext);
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel1_IdIsPassed_HasServiceFunctionContextTrue()
        {
            var result = _target.AddLevel1(NavigationLevelNames.LevelTwo, ServiceFunctionId) as ViewResult;
            var model = result.Model as AddServiceComponentLevel1ViewModel;
            Assert.IsTrue(model.HasServiceFunctionContext);
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel1_IdIsPassed_ReturnUrlIsServiceFunctionEdit()
        {
            var result = _target.AddLevel1(NavigationLevelNames.LevelTwo, ServiceFunctionId) as ViewResult;
            var model = result.Model as AddServiceComponentLevel1ViewModel;
            Assert.AreEqual("/ServiceFunction/Edit/" + ServiceFunctionId + "?level=LevelTwo", model.ReturnUrl);
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel1_IdNotPassed_ReturnUrlIsServiceComponentIndex()
        {
            var result = _target.AddLevel1(NavigationLevelNames.LevelTwo, null) as ViewResult;
            var model = result.Model as AddServiceComponentLevel1ViewModel;
            Assert.AreEqual("/ServiceComponent?level=LevelTwo", model.ReturnUrl);
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel1_IdNotPassed_ServiceFunctionsAddedToModel()
        {
            var result = _target.AddLevel1(NavigationLevelNames.LevelTwo, null) as ViewResult;
            var model = result.Model as AddServiceComponentLevel1ViewModel;
            Assert.AreEqual(5, model.ServiceFunctions.Count);
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel2_LevelPassed_IsUsedToFormReturningViewName()
        {
            _mockServiceComponentService.Setup(s => s.GetById(ServiceComponentIdWithChildComponentDependent))
                .Returns(_serviceComponents[1]);

            var result = _target.AddLevel2(NavigationLevelNames.LevelTwo, ServiceComponentIdWithChildComponentDependent) as ViewResult;
            Assert.AreEqual("AddLevelTwoL2", result.ViewName);
        }


        [TestMethod]
        public void ServiceComponentController_AddLevel2_IdIsPassed_ReturnUrlIsServiceComponentEdit()
        {
            _mockServiceComponentService.Setup(s => s.GetById(ServiceComponentIdWithChildComponentDependent))
                .Returns(_serviceComponents[1]);

            var result = _target.AddLevel2(NavigationLevelNames.LevelTwo, ServiceComponentIdWithChildComponentDependent) as ViewResult;
            var model = result.Model as AddServiceComponentLevel2ViewModel;
            Assert.AreEqual("/ServiceComponent/Edit/88?level=LevelTwo", model.ReturnUrl);
        }


        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceAddComponentGrid_DoesNothing_ReturnsEmptyJsonResult()
        {
            var result = _target.ReadAjaxServiceAddComponentGrid(new DataSourceRequest()) as JsonResult;
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void ServiceComponentController_UpdateAjaxServiceAddComponentGridd_DoesNothing_ReturnsEmptyJsonResult()
        {
            var result = _target.UpdateAjaxServiceAddComponentGrid(new DataSourceRequest()) as JsonResult;
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void ServiceComponentController_DestroyAjaxServiceAddComponentGrid_DoesNothing_ReturnsEmptyJsonResult()
        {
            var result = _target.DestroyAjaxServiceAddComponentGrid(new DataSourceRequest()) as JsonResult;
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_CustomerContextNull_NoCreateTakesPlace()
        {
            SetCustomerContextNull();
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_CustomerContextZero_NoCreateTakesPlace()
        {
            SetCustomerContextZero();
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_NoComponentsPassed_NoCreateTakesPlace()
        {
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), null);
            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_NoComponentsPassed_DataSourceResultStillReturned()
        {
            var result = _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), null) as JsonResult;
            var data = result.Data as DataSourceResult;
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsHaveServiceFunctionIdZeroOrNullEmptyComponentName_ComponentsIgnored()
        {
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            Assert.AreEqual(4, _serviceComponentBulkCreated.Count());
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.ServiceFunctionId == 0));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => string.IsNullOrEmpty(x.ComponentName)));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_ComponentLevelSetTo1()
        {
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.ComponentLevel != (int)ServiceComponentLevel.Level1));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_ComponentNameSet()
        {
            var expected = _bulkServiceComponentLevel1ViewModels
                .Where(w => w.ServiceFunctionId != 0 && !string.IsNullOrEmpty(w.ComponentName))
                .Select(s => s.ComponentName)
                .ToList();
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            var result = _serviceComponentBulkCreated
                .Select(s => s.ComponentName)
                .ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_ServiceFunctionIdSet()
        {
            var expected = _bulkServiceComponentLevel1ViewModels
                .Where(w => w.ServiceFunctionId.HasValue &&
                    w.ServiceFunctionId != 0 &&
                    !string.IsNullOrEmpty(w.ComponentName))
                .Select(s => s.ServiceFunctionId.Value)
                .ToList();
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            var result = _serviceComponentBulkCreated
                .Select(s => s.ServiceFunctionId)
                .ToList();
            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_InsertedBySet()
        {
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.InsertedBy != UserName));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.InsertedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.InsertedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.InsertedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.InsertedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.InsertedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_UpdatedBySet()
        {
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.UpdatedBy != UserName));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkCreated.Any(x => x.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ComponentsCreated_CreateBulkCalled()
        {
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetCreateBulkException();
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel1Grid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetCreateBulkException();
            _target.CreateAjaxServiceAddComponentLevel1Grid(new DataSourceRequest(), _bulkServiceComponentLevel1ViewModels);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_CustomerContextNull_NoCreateTakesPlace()
        {
            SetCustomerContextNull();
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_CustomerContextZero_NoCreateTakesPlace()
        {
            SetCustomerContextZero();
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_NoComponentsPassed_NoUpdateTakesPlace()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), null);

            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_NoComponentsPassed_DataSourceResultStillReturned()
        {
            var result = _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), null) as JsonResult;
            var data = result.Data as DataSourceResult;
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ParentComponentCannotBeFound_ModelStateErrorReturned()
        {
            _bulkServiceComponentLevel2ViewModels[0].ServiceComponentLevel1Id = 999;

            var result = _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;

            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ServiceComponentCannotBeFound));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ParentComponentCannotBeFound_NoUpdateTakesPlace()
        {
            _bulkServiceComponentLevel2ViewModels[0].ServiceComponentLevel1Id = 999;
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ValidData_UpdateTakesPlace()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_ComponentLevelSetTo2()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            Assert.IsTrue(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.Select(y => y.ComponentLevel).ToList().TrueForAll(z => z == (int)ServiceComponentLevel.Level2)));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_ComponentNameSet()
        {
            var expected = _bulkServiceComponentLevel2ViewModels
                .Where(w => w.ServiceComponentLevel1Id != 0 && !string.IsNullOrEmpty(w.ComponentName))
                .Select(s => s.ComponentName)
                .ToList();

            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);

            var result = _serviceComponentBulkUpdated
                .Select(s => s.ChildServiceComponents)
                .First().Select(component => component.ComponentName).ToList();

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_ServiceFunctionIdSet()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);

            var expected = _serviceComponentBulkUpdated
                .Select(s => s.ServiceFunctionId)
                .ToList();

            var result = _serviceComponentBulkUpdated
                .Select(s => s.ChildServiceComponents)
                .First().Select(component => component.ServiceFunctionId).Distinct().ToList();

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_ParentComponentIdSet()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);

            var expected = _serviceComponentBulkUpdated
                .Select(s => s.Id)
                .ToList();

            var result = _serviceComponentBulkUpdated
                .Select(s => s.ChildServiceComponents)
                .First().Select(component => component.ParentServiceComponentId.Value).Distinct().ToList();

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_InsertedBySet()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);

            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.InsertedBy != UserName)));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);

            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.InsertedDate.Year != now.Year)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.InsertedDate.Month != now.Month)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.InsertedDate.Day != now.Day)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.InsertedDate.Hour != now.Hour)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.InsertedDate.Minute != now.Minute)));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_UpdatedBySet()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);

            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.UpdatedBy != UserName)));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);

            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.UpdatedDate.Year != now.Year)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.UpdatedDate.Month != now.Month)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.UpdatedDate.Day != now.Day)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.UpdatedDate.Hour != now.Hour)));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.ChildServiceComponents.ToList().TrueForAll(y => y.UpdatedDate.Minute != now.Minute)));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_UpdatedByOnParentSet()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.UpdatedBy != UserName));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_UpdatedDateOnParentSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ComponentsCreated_CreateBulkCalled()
        {
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetGetByCustomerException();
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_CreateAjaxServiceAddComponentLevel2Grid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerException();
            _target.CreateAjaxServiceAddComponentLevel2Grid(new DataSourceRequest(), _bulkServiceComponentLevel2ViewModels);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_GetServiceComponentLevels_ReturnsData_CorrectNumberReturned()
        {
            var expected = EnumExtensions.AsSelectListItems<ServiceComponentLevel>();
            var result = _target.GetServiceComponentLevels();
            var data = result.Data as List<SelectListItem>;
            Assert.AreEqual(expected.Count, data.Count);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NoComponentFoundWithSuppliedId_ReturnsToComponentIndex()
        {
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdNotPresent) as RedirectToRouteResult;
            Assert.AreEqual("LevelOne", result.RouteValues["level"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_EditStateLevel1WithChildComponent_ReturnsEditServiceComponentLevel1WithChildComponentViewModel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithChildComponent);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithNoDependents) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditServiceComponentLevel1WithChildComponentViewModel));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_EditStateLevel1WithChildComponent_SetsEditLevel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithChildComponent);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithNoDependents) as ViewResult;
            var data = result.Model as EditServiceComponentViewModel;
            Assert.AreEqual("LevelOne", data.EditLevel);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_EditStateLevel1WithNoChildComponentOrResolver_ReturnsEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithNoDependents) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_EditStateLevel1WithNoChildComponentOrResolver_SetsEditLevel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var data = result.Model as EditServiceComponentViewModel;
            Assert.AreEqual("LevelOne", data.EditLevel);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_EditStateLevel1WithResolver_ReturnsEditServiceComponentLevel1WithResolverViewModel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditServiceComponentLevel1WithResolverViewModel));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_EditStateLevel1WithResolver_SetsEditLevel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var data = result.Model as EditServiceComponentViewModel;
            Assert.AreEqual("LevelOne", data.EditLevel);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOneEditStateLevel2_ReturnsEditServiceComponentLevel2ViewModel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);

            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdChildServiceComponentIdWithParent) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditServiceComponentLevel2ViewModel));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelTwoEditStateLevel2_ReturnsEditServiceComponentLevel2ViewModel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);

            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceComponentIdChildServiceComponentIdWithParent) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditServiceComponentLevel2ViewModel));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOneEditStateLevel2_SetsEditLevel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdChildServiceComponentIdWithParent) as ViewResult;
            var data = result.Model as EditServiceComponentViewModel;
            Assert.AreEqual("LevelOne", data.EditLevel);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelTwoEditStateLevel2_SetsEditLevel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);
            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceComponentIdChildServiceComponentIdWithParent) as ViewResult;
            var data = result.Model as EditServiceComponentViewModel;
            Assert.AreEqual("LevelTwo", data.EditLevel);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOneEditStateNone_ReturnsToComponentIndex()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.None);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as RedirectToRouteResult;
            Assert.AreEqual("LevelOne", result.RouteValues["level"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelTwoEditStateNone_ReturnsToComponentIndex()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.None);
            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceComponentIdWithResolverDependent) as RedirectToRouteResult;
            Assert.AreEqual("LevelTwo", result.RouteValues["level"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOneServiceComponentWithResolver_ReturnUrlIsComponentIndex()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentViewModel;
            Assert.IsTrue(model.ReturnUrl.StartsWith("/ServiceFunction/Edit/"));
            Assert.IsTrue(model.ReturnUrl.EndsWith("?level=LevelOne"));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelTwoServiceComponentWithResolver_ReturnUrlIsComponentIndex()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceComponentIdWithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentViewModel;
            Assert.IsTrue(model.ReturnUrl.StartsWith("/ServiceFunction/Edit/"));
            Assert.IsTrue(model.ReturnUrl.EndsWith("?level=LevelTwo"));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOneServiceComponentWithResolver_ReturnsToServiceFunction()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentViewModel;
            Assert.IsTrue(model.ReturnUrl.StartsWith("/ServiceFunction"));
            Assert.IsTrue(model.ReturnUrl.EndsWith("?level=LevelOne"));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelTwoServiceComponentWithResolver_ReturnsToFunction()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceComponentIdWithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentViewModel;
            Assert.IsTrue(model.ReturnUrl.StartsWith("/ServiceFunction"));
            Assert.IsTrue(model.ReturnUrl.EndsWith("?level=LevelTwo"));
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOnelServiceComponentWithResolver_EditUrlIsSetCorrectly()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentViewModel;
            string url = $"/Resolver/Edit/{ServiceComponentIdWithResolverDependent}?level=LevelOne";
            Assert.AreEqual(url, model.EditUrl);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelTwoServiceComponentWithResolver_EditUrlIsSetCorrectly()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceComponentIdWithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentViewModel;
            string url = $"/Resolver/Edit/{ServiceComponentIdWithResolverDependent}?level=LevelTwo";
            Assert.AreEqual(url, model.EditUrl);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOneServiceComponentIdLevel2WithResolver_CanEditIsTrue()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);

            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdLevel2WithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentLevel2ViewModel;

            Assert.IsTrue(model.CanEdit);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelTwoServiceComponentIdLevel2WithResolver_CanEditIsTrue()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);

            var result = _target.Edit(NavigationLevelNames.LevelTwo, ServiceComponentIdLevel2WithResolverDependent) as ViewResult;
            var model = result.Model as EditServiceComponentLevel2ViewModel;

            Assert.IsTrue(model.CanEdit);
        }

        [TestMethod]
        public void ServiceComponentController_EditGet_NavigationLevelOneServiceComponentWithNoDependents_AppendedToViewName()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithChildComponent);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithNoDependents) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_NavigationLevelOneLevelOneWithNoChildComponentOrResolver_ModelHasErrors_ReturnsToView()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var result = _target.EditLevel1WithNoChildComponentOrResolver(new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne
            }) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_NavigationLevelTwoLevelOneWithNoChildComponentOrResolver_ModelHasErrors_ReturnsToView()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var result = _target.EditLevel1WithNoChildComponentOrResolver(new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                EditLevel = NavigationLevelNames.LevelTwo
            }) as ViewResult;
            Assert.AreEqual("EditLevelTwo", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_NavigationLevelOneEditLevelOneWithNoChildComponentOrResolver_ModelHasErrors_NoDbActivity()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.EditLevel1WithNoChildComponentOrResolver(new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel());
            _mockServiceComponentService.Verify(v => v.GetByCustomerWithHierarchy(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentNotFound_MessageAddedToModelState()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                Id = ServiceComponentIdNotPresent
            };
            var result = _target.EditLevel1WithNoChildComponentOrResolver(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentEditCannotBeFound));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentNotFound_ReturnsToView()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithNoChildComponentOrResolver(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentNotFound_NoUpdateOccurs()
        {
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithNoChildComponentOrResolver(vm) as ViewResult;
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_StateNotLevel1WithNoChildComponentOrResolver_MessageAddedToModelState()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithNoChildComponentOrResolver(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentIncorrectState));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_StateNotLevel1WithNoChildComponentOrResolver_ReturnsToView()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithNoChildComponentOrResolver(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_StateNotLevel1WithNoChildComponentOrResolver_NoUpdateOccurs()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = new EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentFound_ComponentNameIsSet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.AreEqual(vm.ComponentName.ComponentName, _serviceComponentUpdated.ComponentName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentFound_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentFound_UpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0SDOTypeIdHasValue_ResolverGroupTypeIdSet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.AreEqual(vm.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId, _serviceComponentUpdated.Resolver.ServiceDeliveryOrganisationType.Id);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0SDOTypeIdHasValue_ResolverTypeNotesSet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.AreEqual(vm.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationNotes, _serviceComponentUpdated.Resolver.ServiceDeliveryOrganisationNotes);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0SDOTypeIdHasValue_InsertedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0SDOTypeIdHasValue_InsertedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);
            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0SDOTypeIdHasValue_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0SDOTypeIdHasValue_UpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ServiceDeliveryUnitTypeIdHasValue_ServiceDeliveryUnitTypeIdSet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(vm.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId, _serviceComponentUpdated.Resolver.ServiceDeliveryUnitType.Id);
        }


        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ServiceDeliveryUnitTypeIdHasValue_ServiceDeliveryUnitNotesSet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.AreEqual(vm.ResolverServiceDeliveryUnit.ServiceDeliveryUnitNotes, _serviceComponentUpdated.Resolver.ServiceDeliveryUnitNotes);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ServiceDeliveryUnitTypeIdHasValue_InsertedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.InsertedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ServiceDeliveryUnitTypeIdHasValue_InsertedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ServiceDeliveryUnitTypeIdHasValue_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ServiceDeliveryUnitTypeIdHasValue_UpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ResolverNotNullOrEmpty_InsertedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.InsertedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ResolverNotNullOrEmpty_InsertedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ResolverNotNullOrEmpty_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ResolverNotNullOrEmpty_UpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0_OnlyExpectedFieldsChanged()
        {
            var before = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithNoDependents)
                .GetClone();
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            var compare = new CompareLogic(new ComparisonConfig
            {
                MembersToIgnore = new List<string>
                {
                    "ComponentName",
                    "DiagramOrder",
                    "ServiceActivities",
                    "Resolver",
                    "InsertedBy",
                    "InsertedDate",
                    "UpdatedBy",
                    "UpdatedDate"
                },
                MaxDifferences = 100
            });
            var same = compare.Compare(before, _serviceComponentUpdated);
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0SdoTypeIdZero_NoSdoAdded()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            vm.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId = 0;
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.IsNull(_serviceComponentUpdated.Resolver);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ServiceDeliveryUnitTypeIdZero_NoServiceDeliveryUnitAdded()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            vm.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId = 0;
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.IsNull(_serviceComponentUpdated.Resolver.ServiceDeliveryUnitType);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode0ResolverGroupNameNull_NoResolverGroupAdded()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            vm.ResolverGroup.ResolverGroupTypeId = 0;
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            Assert.IsNull(_serviceComponentUpdated.Resolver.ResolverGroupType);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1ChildComponentNameSupplied_ComponentNameSet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(vm.ChildComponent.ComponentName, _serviceComponentUpdated.ChildServiceComponents.First().ComponentName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1ChildComponentNameSupplied_ServiceFunctionSetToParent()
        {
            var before = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithNoDependents)
                .GetClone();
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(before.ServiceFunctionId, _serviceComponentUpdated.ChildServiceComponents.First().ServiceFunctionId);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1ChildComponentNameSupplied_Level2Set()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual((int)ServiceComponentLevel.Level2, _serviceComponentUpdated.ChildServiceComponents.First().ComponentLevel);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1ChildComponentNameSupplied_InsertedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(UserName, _serviceComponentUpdated.ChildServiceComponents.First().InsertedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1ChildComponentNameSupplied_InsertedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1ChildComponentNameSupplied_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(UserName, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1ChildComponentNameSupplied_UpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            Assert.AreEqual(now.Year, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.ChildServiceComponents.First().UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_InputMode1_OnlyExpectedFieldsChanged()
        {
            var before = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithNoDependents)
                .GetClone();
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(1);

            _target.EditLevel1WithNoChildComponentOrResolver(vm);

            var compare = new CompareLogic(new ComparisonConfig
            {
                MembersToIgnore = new List<string>
                {
                    "ComponentName",
                    "ServiceActivities",
                    "DiagramOrder",
                    "ChildServiceComponents",
                    "InsertedBy",
                    "InsertedDate",
                    "UpdatedBy",
                    "UpdatedDate"
                },
                MaxDifferences = 100
            });
            var same = compare.Compare(before, _serviceComponentUpdated);
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentFound_UpdatedIsCalled()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            _target.EditLevel1WithNoChildComponentOrResolver(vm);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentFound_RedirectsToEdit()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            var result = _target.EditLevel1WithNoChildComponentOrResolver(vm) as RedirectToRouteResult;
            Assert.AreEqual("LevelOne", result.RouteValues["level"]);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
            Assert.AreEqual("ServiceFunction", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_ComponentFoundNoEditLevel_RedirectsToFunctionEdit()
        {
            var expectedServiceFunctionId = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithNoDependents)
                .ServiceFunctionId;
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(0);
            vm.EditLevel = NavigationLevelNames.None;
            var result = _target.EditLevel1WithNoChildComponentOrResolver(vm) as RedirectToRouteResult;
            Assert.AreEqual(expectedServiceFunctionId, result.RouteValues["Id"]);
            Assert.AreEqual(string.Empty, result.RouteValues["level"]);
            Assert.AreEqual("ServiceFunction", result.RouteValues["controller"]);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ModelHasErrors_ReturnsToView()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var result = _target.EditLevel1WithChildComponent(new EditServiceComponentLevel1WithChildComponentViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne
            }) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ModelHasErrors_NoDbActivity()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.EditLevel1WithChildComponent(new EditServiceComponentLevel1WithChildComponentViewModel());
            _mockServiceComponentService.Verify(v => v.GetByCustomerWithHierarchy(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentNotFound_MessageAddedToModelState()
        {
            var vm = new EditServiceComponentLevel1WithChildComponentViewModel
            {
                Id = ServiceComponentIdNotPresent
            };
            var result = _target.EditLevel1WithChildComponent(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentEditCannotBeFound));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentNotFound_ReturnsToView()
        {
            var vm = new EditServiceComponentLevel1WithChildComponentViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithChildComponent(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentNotFound_NoUpdateOccurs()
        {
            var vm = new EditServiceComponentLevel1WithChildComponentViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithChildComponent(vm) as ViewResult;
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_StateNotLevel1WithChildComponent_MessageAddedToModelState()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel1WithChildComponentViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithChildComponent(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentIncorrectState));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_StateNotLevel1WithChildComponent_ReturnsToView()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel1WithChildComponentViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithChildComponent(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_StateNotLevel1WithChildComponent_NoUpdateOccurs()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel1WithChildComponentViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            _target.EditLevel1WithChildComponent(vm);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }


        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentFound_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = GetEditServiceComponentLevel1WithChildComponentViewModel();
            _target.EditLevel1WithChildComponent(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentFound_UpdatedDateSet()
        {
            var now = DateTime.Now;

            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = GetEditServiceComponentLevel1WithChildComponentViewModel();

            _target.EditLevel1WithChildComponent(vm);
            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentFound_ComponentNameIsSet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = GetEditServiceComponentLevel1WithChildComponentViewModel();

            _target.EditLevel1WithChildComponent(vm);

            Assert.AreEqual(vm.ComponentName.ComponentName, _serviceComponentUpdated.ComponentName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentFound_UpdatedIsCalled()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = GetEditServiceComponentLevel1WithChildComponentViewModel();

            _target.EditLevel1WithChildComponent(vm);

            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentFound_RedirectsToEdit()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = GetEditServiceComponentLevel1WithChildComponentViewModel();

            var result = _target.EditLevel1WithChildComponent(vm) as RedirectToRouteResult;

            Assert.AreEqual("LevelOne", result.RouteValues["level"]);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
            Assert.AreEqual("ServiceFunction", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentFoundNoEditLevel_RedirectsToFunctionEdit()
        {
            var expectedServiceFunctionId = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithChildComponentDependent)
                .ServiceFunctionId;
            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = GetEditServiceComponentLevel1WithChildComponentViewModel();
            vm.EditLevel = NavigationLevelNames.None;
            var result = _target.EditLevel1WithChildComponent(vm) as RedirectToRouteResult;
            Assert.AreEqual(expectedServiceFunctionId, result.RouteValues["Id"]);
            Assert.AreEqual(string.Empty, result.RouteValues["level"]);
            Assert.AreEqual("ServiceFunction", result.RouteValues["controller"]);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_ComponentFound_OnlyExpectedFieldsChanged()
        {
            var initializers = new Dictionary<Type, Func<object, object>> {
                    { typeof(ICollection<ServiceComponent>), (s) => new List<ServiceComponent>() }
                };

            var before = _serviceComponents
                .Single(x => x.Id == ServiceComponentIdWithChildComponentDependent).GetClone(initializers);

            SetGetEditState(ServiceComponentEditState.Level1WithChildComponent);
            var vm = GetEditServiceComponentLevel1WithChildComponentViewModel();
            _target.EditLevel1WithChildComponent(vm);

            var compare = new CompareLogic(new ComparisonConfig
            {
                MaxStructDepth = 4,
                MembersToIgnore = new List<string>
                {
                    "ChildServiceComponents",
                    "ComponentName",
                    "DiagramOrder",
                    "ServiceActivities",
                    "UpdatedBy",
                    "UpdatedDate"
                },
                MaxDifferences = 100
            });


            var same = compare.Compare(before, _serviceComponentUpdated);
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_ModelHasErrors_ReturnsToView()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var result = _target.EditLevel1WithResolver(new EditServiceComponentLevel1WithResolverViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne
            }) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_ModelHasErrors_NoDbActivity()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.EditLevel1WithResolver(new EditServiceComponentLevel1WithResolverViewModel());
            _mockServiceComponentService.Verify(v => v.GetByCustomerWithHierarchy(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_ComponentNotFound_MessageAddedToModelState()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                Id = ServiceComponentIdNotPresent
            };
            var result = _target.EditLevel1WithResolver(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentEditCannotBeFound));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_ComponentNotFound_ReturnsToView()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithResolver(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_ComponentNotFound_NoUpdateOccurs()
        {
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            _target.EditLevel1WithResolver(vm);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_StateNotLevel1WithResolver_MessageAddedToModelState()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithResolver(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentIncorrectState));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_StateNotLevel1WithResolver_ReturnsToView()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel1WithResolver(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_StateNotLevel1WithResolver_NoUpdateOccurs()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel1WithResolverViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            _target.EditLevel1WithResolver(vm);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_ComponentFound_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithResolver);
            var vm = GetEditServiceComponentLevel1WithResolverViewModel();
            _target.EditLevel1WithResolver(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_ComponentFound_UpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level1WithResolver);
            var vm = GetEditServiceComponentLevel1WithResolverViewModel();
            _target.EditLevel1WithResolver(vm);
            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ModelHasErrors_ReturnsToView()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            var result = _target.EditLevel2(new EditServiceComponentLevel2ViewModel
            {
                EditLevel = NavigationLevelNames.LevelOne
            }) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ModelHasErrors_NoDbActivity()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.EditLevel2(new EditServiceComponentLevel2ViewModel());
            _mockServiceComponentService.Verify(v => v.GetByCustomerWithHierarchy(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ComponentNotFound_MessageAddedToModelState()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                Id = ServiceComponentIdNotPresent
            };
            var result = _target.EditLevel2(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentEditCannotBeFound));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ComponentNotFound_ReturnsToView()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel2(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ComponentNotFound_NoUpdateOccurs()
        {
            var vm = new EditServiceComponentLevel2ViewModel
            {
                Id = ServiceComponentIdNotPresent,
                EditLevel = NavigationLevelNames.LevelOne
            };
            _target.EditLevel2(vm);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_StateNotLevel2_MessageAddedToModelState()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel2ViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel2(vm) as ViewResult;
            Assert.IsTrue(result.ViewData.ModelState.Keys.Contains(ModelStateErrorNames.ServiceComponentIncorrectState));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_StateNotLevel2_ReturnsToView()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel2ViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            var result = _target.EditLevel2(vm) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_StateNotLevel2_NoUpdateOccurs()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithNoChildComponentOrResolver);
            var vm = new EditServiceComponentLevel2ViewModel
            {
                Id = ServiceComponentIdWithNoDependents,
                EditLevel = NavigationLevelNames.LevelOne
            };
            _target.EditLevel2(vm);
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ComponentLevel2MoveToLevel1_ComponentLevelSet()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditServiceComponentLevel2ViewModel();
            vm.ComponentNameLevel.ComponentLevel = ServiceComponentLevel.Level1.ToString();
            _target.EditLevel2(vm);
            Assert.AreEqual(1, _serviceComponentUpdated.ComponentLevel);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ComponentLevel2MoveToLevel1_ParentComponentPropertySetToNull()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditServiceComponentLevel2ViewModel();
            vm.ComponentNameLevel.ComponentLevel = ServiceComponentLevel.Level1.ToString();
            _target.EditLevel2(vm);
            Assert.IsNull(_serviceComponentUpdated.ParentServiceComponent);
        }


        [TestMethod]
        public void ServiceComponentController_EditLevel2_ComponentFound_UpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditServiceComponentLevel2ViewModel();
            _target.EditLevel2(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_ComponentFound_UpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditServiceComponentLevel2ViewModel();
            _target.EditLevel2(vm);
            Assert.AreEqual(now.Year, _serviceComponentUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceComponentController_DeleteServiceComponent_CallsServiceComponentDelete()
        {
            #region Arrange

            #endregion

            #region Act

            _target.DeleteServiceComponent(1);

            #endregion

            #region Assert

            _mockServiceComponentService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockServiceComponentService.Verify(x => x.Delete(It.IsAny<ServiceComponent>()), Times.Once);

            #endregion
        }

        #region Role Checks

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceComponentsGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("ReadAjaxServiceComponentsGrid", (AuthorizeAttribute att) => att.Roles, new[] { typeof(DataSourceRequest), typeof(int?) }));
        }

        [TestMethod]
        public void ServiceComponentController_UpdateAjaxServiceComponentsGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxServiceComponentsGrid", (AuthorizeAttribute att) => att.Roles, new[] { typeof(DataSourceRequest), typeof(ServiceComponentViewModel) }));
        }

        [TestMethod]
        public void ServiceComponentController_DeleteAjaxServiceComponentsGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DeleteAjaxServiceComponentsGrid", (AuthorizeAttribute att) => att.Roles, new[] { typeof(DataSourceRequest), typeof(ServiceComponentViewModel) }));
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1Get_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("MoveLevel1", (AuthorizeAttribute att) => att.Roles, new Type[] { }));
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel1Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("MoveLevel1", (AuthorizeAttribute att) => att.Roles, new[] { typeof(MoveServiceComponentLevel1ViewModel) }));
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2Get_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("MoveLevel2", (AuthorizeAttribute att) => att.Roles, new Type[] { }));
        }

        [TestMethod]
        public void ServiceComponentController_MoveLevel2Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("MoveLevel2", (AuthorizeAttribute att) => att.Roles, new[] { typeof(MoveServiceComponentLevel2ViewModel) }));
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxServiceAddComponentGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("ReadAjaxServiceAddComponentGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_UpdateAjaxServiceAddComponentGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxServiceAddComponentGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_DestroyAjaxServiceAddComponentGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DestroyAjaxServiceAddComponentGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel1_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("AddLevel1", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_AddLevel2_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("AddLevel2", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_GetServiceComponentLevels_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("GetServiceComponentLevels", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_Edit_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new[] { typeof(string), typeof(int) }));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithChildComponent_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("EditLevel1WithChildComponent", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithNoChildComponentOrResolver_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("EditLevel1WithNoChildComponentOrResolver", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel1WithResolver_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("EditLevel1WithResolver", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_EditLevel2_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("EditLevel2", (AuthorizeAttribute att) => att.Roles));
        }

        #endregion

        #region Helpers

        private void SetCustomerContextNull()
        {
            _mockAppUserContext.Setup(s => s.Current)
                .Returns(new AppContext { CurrentCustomer = null });
        }

        private void SetCustomerContextZero()
        {
            _mockAppUserContext.Setup(s => s.Current)
                .Returns(new AppContext { CurrentCustomer = new CurrentCustomerViewModel { Id = 0 } });
        }

        private void SetGetByCustomerException()
        {
            _mockServiceComponentService
                .Setup(x => x.GetByCustomer(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
        }

        private void SetGetByCustomerWithHierarchyException()
        {
            _mockServiceComponentService
                .Setup(x => x.GetByCustomerWithHierarchy(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
        }

        private void SetCreateBulkException()
        {
            _mockServiceComponentService
                .Setup(x => x.Create(It.IsAny<IEnumerable<ServiceComponent>>()))
                .Throws(new Exception("Oh no!!"));
        }

        private void SetCanDeleteTrue()
        {
            _mockServiceComponentHelper
                .Setup(s => s.CanDelete(It.IsAny<ServiceComponent>()))
                .Returns(true);
        }

        private void SetGetEditState(ServiceComponentEditState state)
        {
            _mockServiceComponentHelper
                .Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(state);
        }

        private EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel GetEditServiceComponentLevel1WithNoChildComponentOrResolverViewModel(int inputMode)
        {
            return UnitTestHelper.GenerateRandomData<EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel>(
                x =>
                {
                    x.Id = ServiceComponentIdWithNoDependents;
                    x.EditLevel = NavigationLevelNames.LevelOne;
                    x.ChildComponent = UnitTestHelper.GenerateRandomData<EditServiceComponentChildViewModel>();
                    x.ComponentName = UnitTestHelper.GenerateRandomData<EditServiceComponentNameViewModel>();
                    x.DiagramOrder = UnitTestHelper.GenerateRandomData<EditServiceComponentDiagramOrderViewModel>();
                    x.ServiceActivities = UnitTestHelper.GenerateRandomData<ServiceActivityViewModel>();
                    x.InputMode = inputMode;
                    x.ResolverServiceDeliveryOrganisation = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryOrganisationViewModel>(y =>
                    {
                        y.ServiceDeliveryOrganisationTypeId = _serviceDeliveryOrganisationTypeRefDatas[0].Id;
                    });
                    x.ResolverServiceDeliveryUnit = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryUnitViewModel>(y =>
                    {
                        y.ServiceDeliveryUnitTypeId = _serviceDeliveryUnitTypeRefDatas[1].Id;
                    });
                    x.ResolverGroup = UnitTestHelper.GenerateRandomData<EditResolverResolverGroupViewModel>(y =>
                    {
                        y.ResolverGroupTypeId = _resolverGroupTypeRefDatas[2].Id;
                    }); ;
                });
        }

        private EditServiceComponentLevel1WithChildComponentViewModel GetEditServiceComponentLevel1WithChildComponentViewModel()
        {
            return UnitTestHelper.GenerateRandomData<EditServiceComponentLevel1WithChildComponentViewModel>(
                x =>
                {
                    x.Id = ServiceComponentIdWithChildComponentDependent;
                    x.EditLevel = NavigationLevelNames.LevelOne;
                    x.ServiceComponent = UnitTestHelper.GenerateRandomData<ServiceComponentViewModel>();
                    x.ComponentName = UnitTestHelper.GenerateRandomData<EditServiceComponentNameViewModel>();
                    x.DiagramOrder = UnitTestHelper.GenerateRandomData<EditServiceComponentDiagramOrderViewModel>();
                    x.ServiceActivities = UnitTestHelper.GenerateRandomData<ServiceActivityViewModel>();
                });
        }

        private EditServiceComponentLevel1WithResolverViewModel GetEditServiceComponentLevel1WithResolverViewModel()
        {
            return UnitTestHelper.GenerateRandomData<EditServiceComponentLevel1WithResolverViewModel>(
                x =>
                {
                    x.Id = ServiceComponentIdWithResolverDependent;
                    x.EditLevel = NavigationLevelNames.LevelOne;
                    x.ComponentName = UnitTestHelper.GenerateRandomData<EditServiceComponentNameViewModel>();
                    x.DiagramOrder = UnitTestHelper.GenerateRandomData<EditServiceComponentDiagramOrderViewModel>();
                    x.ServiceActivities = UnitTestHelper.GenerateRandomData<ServiceActivityViewModel>();
                    x.ResolverGroup = UnitTestHelper.GenerateRandomData<EditResolverResolverGroupViewModel>();
                    x.ResolverServiceDeliveryOrganisation = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryOrganisationViewModel>();
                    x.ResolverServiceDeliveryUnit = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryUnitViewModel>();
                    x.ResolverGroup = new EditResolverResolverGroupViewModel
                    {
                        ResolverGroupTypeId = _resolverGroupTypeRefDatas[0].Id
                    };
                });
        }

        public EditServiceComponentLevel2ViewModel GetEditServiceComponentLevel2ViewModel()
        {
            return UnitTestHelper.GenerateRandomData<EditServiceComponentLevel2ViewModel>(
                x =>
                {
                    x.Id = ServiceComponentIdLevel2WithResolverDependent;
                    x.EditLevel = NavigationLevelNames.LevelOne;
                    x.ComponentNameLevel = UnitTestHelper.GenerateRandomData<EditServiceComponentNameLevelViewModel>();
                    x.ServiceActivities = UnitTestHelper.GenerateRandomData<ServiceActivityViewModel>();
                    x.ResolverServiceDeliveryOrganisation = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryOrganisationViewModel>();
                    x.ResolverServiceDeliveryUnit = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryUnitViewModel>();
                    x.ResolverGroup = new EditResolverResolverGroupViewModel
                    {
                        ResolverGroupTypeId = _resolverGroupTypeRefDatas[0].Id
                    };
                });
        }

        #endregion
    }
}