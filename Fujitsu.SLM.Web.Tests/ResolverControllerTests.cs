using CloneExtensions;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
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
using Kendo.Mvc.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    public class ResolverControllerTests
    {
        private Mock<IServiceComponentService> _mockServiceComponentService;
        private Mock<IOperationalProcessTypeRefDataService> _mockOperationalProcessTypeRefDataService;
        private Mock<IServiceFunctionService> _mockServiceFunctionService;
        private Mock<IResolverService> _mockResolverService;
        private Mock<IServiceDeskService> _mockDeskService;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IResponseManager> _mockResponseManager;
        private Mock<IContextManager> _mockContextManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IServiceDeliveryOrganisationTypeRefDataService> _mockServiceDeliveryOrganisationTypeRefDataService;
        private Mock<IServiceDeliveryUnitTypeRefDataService> _mockServiceDeliveryUnitTypeRefDataService;
        private Mock<IResolverGroupTypeRefDataService> _mockResolverGroupTypeRefDataService;
        private Mock<IResolverHelper> _mockResolverHelper;
        private Mock<IServiceComponentHelper> _mockServiceComponentHelper;
        private Mock<IRepository<OperationalProcessType>> _mockOperationalProcessTypeRepository;
        private Mock<IParameterService> _mockParameterService;

        private List<BulkResolverViewModel> _bulkResolverViewModels;
        private List<BulkResolverLevelZeroViewModel> _bulkResolverLevelZeroViewModels;

        private List<ServiceDeliveryOrganisationTypeRefData> _serviceDeliveryOrganisationTypeRefDatas;
        private List<ServiceDeliveryUnitTypeRefData> _serviceDeliveryUnitTypeRefDatas;
        private List<ResolverGroupTypeRefData> _resolverGroupTypeRefDatas;
        private List<ServiceComponent> _serviceComponents;
        private List<ServiceComponentListItem> _serviceComponentListItems;
        private List<ResolverListItem> _resolverListItems;
        private AppContext _appContext;
        private ControllerContextMocks _controllerContextMocks;
        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private const int ServiceComponentIdWithNoDependents = 87;
        private const int ServiceComponentIdWithResolverDependent = 99;
        private const int ServiceComponentId1 = 23;
        private const int ServiceComponentId2 = 24;
        private const int ServiceComponentId3 = 25;
        private const int ServiceComponentIdNotPresent = 958;
        private const int CustomerId = 22;
        private ServiceComponent _serviceComponentUpdated;
        private IEnumerable<ServiceComponent> _serviceComponentBulkUpdated;
        private ServiceDesk _serviceDeskUpdated;
        private IEnumerable<ServiceDesk> _serviceDeskBulkUpdated;

        private ResolverController _target;
        private List<OperationalProcessTypeRefData> _operationalProcessTypeRefDatas;

        [TestInitialize]
        public void Initalize()
        {
            #region Data

            _bulkResolverViewModels = new List<BulkResolverViewModel>
            {
                UnitTestHelper.GenerateRandomData<BulkResolverViewModel>(x =>
                {
                    x.ServiceComponentId = ServiceComponentId1;
                    x.ResolverGroupTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<BulkResolverViewModel>(x =>
                {
                    x.ResolverGroupTypeId = 2;
                }),
                UnitTestHelper.GenerateRandomData<BulkResolverViewModel>(x =>
                {
                    x.ServiceComponentId = ServiceComponentId3;
                    x.ResolverGroupTypeId = 3;
                }),
                UnitTestHelper.GenerateRandomData<BulkResolverViewModel>(x =>
                {
                    x.ResolverGroupTypeId = 4;
                }) // Make sure this stays last in the list
            };

            _bulkResolverLevelZeroViewModels = new List<BulkResolverLevelZeroViewModel>
            {
                UnitTestHelper.GenerateRandomData<BulkResolverLevelZeroViewModel>(),
                UnitTestHelper.GenerateRandomData<BulkResolverLevelZeroViewModel>(),
                UnitTestHelper.GenerateRandomData<BulkResolverLevelZeroViewModel>()
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
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = 1;
                    x.ResolverGroupTypeName = "Resolver Group One";
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = 2;
                    x.ResolverGroupTypeName = "Resolver Group Two";
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = 3;
                    x.ResolverGroupTypeName = "Resolver Group Three";
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = 4;
                    x.ResolverGroupTypeName = "Resolver Group Four";
                    x.Visible = false;
                }),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = 5;
                    x.ResolverGroupTypeName = "Resolver Group Five";
                    x.Visible = false;
                })
            };

            _operationalProcessTypeRefDatas = new List<OperationalProcessTypeRefData> {
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Id = 1;
                    x.OperationalProcessTypeName = "Operational Process Type One";
                    x.Visible = false;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Id = 2;
                    x.OperationalProcessTypeName = "Operational Process Type Two";
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Id = 3;
                    x.OperationalProcessTypeName = "Operational Process Type Three";
                    x.Visible = true;
                }),
            };


            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            };

            _serviceComponents = new List<ServiceComponent>
            {
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 0
                {
                    x.Id = ServiceComponentIdWithNoDependents;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.Resolver = null;
                    x.ChildServiceComponents = new List<ServiceComponent>();
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 1
                {
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.Resolver = null;
                    x.ChildServiceComponents = new List<ServiceComponent>();
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 2
                {
                    x.Id = ServiceComponentIdWithResolverDependent;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(y =>
                    {
                        y.Id = ServiceComponentIdWithResolverDependent;
                    });
                    x.Resolver.OperationalProcessTypes = new List<OperationalProcessType>();
                    x.ChildServiceComponents = new List<ServiceComponent>();

                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 3
                {
                    x.Id = ServiceComponentId1;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.Resolver = null;
                    x.ChildServiceComponents = new List<ServiceComponent>();
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 4
                {
                    x.Id = ServiceComponentId2;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level2;
                    x.Resolver = null;
                    x.ChildServiceComponents = new List<ServiceComponent>();
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x => // 5
                {
                    x.Id = ServiceComponentId3;
                    x.ComponentLevel = (int) ServiceComponentLevel.Level1;
                    x.Resolver = null;
                    x.ChildServiceComponents = new List<ServiceComponent>();
                    x.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(y =>
                    {
                        y.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(z =>
                        {
                            z.ServiceDeskId = 1;
                        });
                    });
                })
            };

            var childComponent = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.ComponentLevel = (int)ServiceComponentLevel.Level2;
                x.ComponentName = "Child Component";
                x.Resolver = null;
                x.ChildServiceComponents = new List<ServiceComponent>();

            });

            _serviceComponents[1].ChildServiceComponents.Add(childComponent);

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
                })
            };

            _resolverListItems = new List<ResolverListItem>
            {
                UnitTestHelper.GenerateRandomData<ResolverListItem>(),
                UnitTestHelper.GenerateRandomData<ResolverListItem>(),
                UnitTestHelper.GenerateRandomData<ResolverListItem>(x =>
                {
                    x.ServiceComponentId = ServiceComponentIdWithResolverDependent;
                }),
                UnitTestHelper.GenerateRandomData<ResolverListItem>(),
                UnitTestHelper.GenerateRandomData<ResolverListItem>()
            };


            #endregion

            Bootstrapper.SetupAutoMapper();

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockOperationalProcessTypeRefDataService = new Mock<IOperationalProcessTypeRefDataService>();
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetById(1)).Returns(_operationalProcessTypeRefDatas[0]);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetById(2)).Returns(_operationalProcessTypeRefDatas[1]);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetById(3)).Returns(_operationalProcessTypeRefDatas[2]);

            _mockServiceDeliveryOrganisationTypeRefDataService = new Mock<IServiceDeliveryOrganisationTypeRefDataService>();
            _mockServiceDeliveryOrganisationTypeRefDataService.Setup(s => s.All()).Returns(_serviceDeliveryOrganisationTypeRefDatas.AsQueryable());

            _mockServiceDeliveryUnitTypeRefDataService = new Mock<IServiceDeliveryUnitTypeRefDataService>();
            _mockServiceDeliveryUnitTypeRefDataService.Setup(s => s.All()).Returns(_serviceDeliveryUnitTypeRefDatas.AsQueryable());
            _mockServiceDeliveryUnitTypeRefDataService.Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId)).Returns(_serviceDeliveryUnitTypeRefDatas.AsQueryable());

            _mockResolverGroupTypeRefDataService = new Mock<IResolverGroupTypeRefDataService>();
            _mockResolverGroupTypeRefDataService.Setup(s => s.All()).Returns(_resolverGroupTypeRefDatas.AsQueryable());
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetById(1)).Returns(_resolverGroupTypeRefDatas[0]);
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetById(2)).Returns(_resolverGroupTypeRefDatas[1]);
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetById(3)).Returns(_resolverGroupTypeRefDatas[2]);
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetById(4)).Returns(_resolverGroupTypeRefDatas[3]);
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetById(5)).Returns(_resolverGroupTypeRefDatas[4]);

            _mockServiceComponentService = new Mock<IServiceComponentService>();
            _mockServiceComponentService.Setup(s => s.GetByCustomer(CustomerId))
                .Returns(_serviceComponents.AsQueryable());
            _mockServiceComponentService.Setup(s => s.GetByCustomerWithHierarchy(CustomerId))
                .Returns(_serviceComponentListItems.AsQueryable());
            _mockServiceComponentService.Setup(s => s.GetResolverByCustomerWithHierarchy(CustomerId))
                .Returns(_resolverListItems.AsQueryable());
            _mockServiceComponentService.Setup(s => s.Update(It.IsAny<ServiceComponent>()))
                .Callback<ServiceComponent>(c => _serviceComponentUpdated = c);
            _mockServiceComponentService.Setup(s => s.Update(It.IsAny<IEnumerable<ServiceComponent>>()))
                .Callback<IEnumerable<ServiceComponent>>(c => _serviceComponentBulkUpdated = c);

            _mockServiceFunctionService = new Mock<IServiceFunctionService>();

            _mockResolverService = new Mock<IResolverService>();
            _mockResolverService.Setup(s => s.GetListByCustomer(CustomerId)).Returns(_resolverListItems.AsQueryable());
            _mockResolverService.Setup(s => s.GetById(1)).Returns(new Resolver
            {
                Id = 1,
                ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypeRefDatas[0],
                ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas[0]
            });

            _mockDeskService = new Mock<IServiceDeskService>();
            _mockDeskService.Setup(s => s.Update(It.IsAny<ServiceDesk>())).Callback<ServiceDesk>(c => _serviceDeskUpdated = c);
            _mockDeskService.Setup(s => s.Update(It.IsAny<IEnumerable<ServiceDesk>>())).Callback<IEnumerable<ServiceDesk>>(c => _serviceDeskBulkUpdated = c);

            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserName);

            _mockResponseManager = new Mock<IResponseManager>();

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

            _mockResolverHelper = new Mock<IResolverHelper>();

            _mockServiceComponentHelper = new Mock<IServiceComponentHelper>();

            _mockOperationalProcessTypeRepository = new Mock<IRepository<OperationalProcessType>>();

            _mockParameterService = new Mock<IParameterService>();

            _target = new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
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
        public void ResolverController_Ctor_ServiceComponentServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(null,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_OperationalProcessTypeRefDataServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                null,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ServiceDeliveryOrganisationTypeRefDataServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                null,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ServiceDeliveryUnitTypeRefDataServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                null,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ResolverGroupTypeServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                null,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ResolverServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                null,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_DeskServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                null,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ServiceFunctionServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                null,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ParameterServiceNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                null,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_OperationalProcessTypeRepositorNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                null,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ResolverHelperNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                null,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ServiceComponentHelperNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_ContextManagerNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                null,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverController_Ctor_AppUserContextNull_ThrowsArgumentNullException()
        {
            new ResolverController(_mockServiceComponentService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockDeskService.Object,
                _mockServiceFunctionService.Object,
                _mockParameterService.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockResolverHelper.Object,
                _mockServiceComponentHelper.Object,
                _mockContextManager.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ResolverController_Index_LevelProvided_ViewIsLevelName()
        {
            var result = _target.Index(NavigationLevelNames.LevelOne) as ViewResult;
            Assert.AreEqual(NavigationLevelNames.LevelOne, result.ViewName);
        }

        [TestMethod]
        public void ResolverController_Index_MoreThan1CustomerServiceComponent_CanMoveTrue()
        {
            var result = _target.Index(NavigationLevelNames.LevelOne) as ViewResult;
            var model = result.Model as ViewResolverViewModel;
            Assert.IsTrue(model.CanMoveResolver);
        }

        [TestMethod]
        public void ResolverController_Index_NoCustomerServiceComponent_CanMoveFalse()
        {
            _mockServiceComponentService.Setup(s => s.GetByCustomerWithHierarchy(CustomerId))
                .Returns(new List<ServiceComponentListItem>().AsQueryable());
            var result = _target.Index(NavigationLevelNames.LevelOne) as ViewResult;
            var model = result.Model as ViewResolverViewModel;
            Assert.IsFalse(model.CanMoveResolver);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_WithContext_CallsGetResolverByCustomerWithHierarchy()
        {
            var request = new DataSourceRequest();
            _target.ReadAjaxResolverGrid(request, ServiceComponentIdWithResolverDependent);
            _mockServiceComponentService.Verify(x => x.GetResolverByCustomerWithHierarchy(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_WithContext_ReturnsFilteredData()
        {
            var request = new DataSourceRequest();
            var result = _target.ReadAjaxResolverGrid(request, ServiceComponentIdWithResolverDependent) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ResolverViewModel>;
            Assert.IsFalse(model.Any(x => x.ServiceComponentId != ServiceComponentIdWithResolverDependent));
            Assert.AreEqual(1, model.Count);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_WithoutContext_ReturnsAllResolverRecords()
        {
            var request = new DataSourceRequest();
            var result = _target.ReadAjaxResolverGrid(request, null) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ResolverViewModel>;
            Assert.AreEqual(5, model.Count);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverLevelZeroGrid_ReturnsAllResolverRecords()
        {
            var request = new DataSourceRequest();
            var result = _target.ReadAjaxResolverLevelZeroGrid(request) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ResolverLevelZeroViewModel>;
            Assert.AreEqual(5, model.Count);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_WithoutContext_CallsGetResolverByCustomerWithHierarchy()
        {
            var request = new DataSourceRequest();
            _target.ReadAjaxResolverGrid(request, null);
            _mockServiceComponentService.Verify(x => x.GetResolverByCustomerWithHierarchy(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverLevelZeroGrid_CallsGetListByCustomer()
        {
            var request = new DataSourceRequest();
            _target.ReadAjaxResolverLevelZeroGrid(request);
            _mockResolverService.Verify(x => x.GetListByCustomer(CustomerId), Times.Once);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_Exception_AppendsErrorMessageToHeader()
        {
            SetGetResolverByCustomerWithHierarchyException();
            var request = new DataSourceRequest();
            _target.ReadAjaxResolverGrid(request, null);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverLevelZeroGrid_Exception_AppendsErrorMessageToHeader()
        {
            SetGetListByCustomerException();
            var request = new DataSourceRequest();
            _target.ReadAjaxResolverLevelZeroGrid(request);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_Exception_SetsStatusCodeTo500()
        {
            SetGetResolverByCustomerWithHierarchyException();
            var request = new DataSourceRequest();
            _target.ReadAjaxResolverGrid(request, null);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverLevelZeroGrid_Exception_SetsStatusCodeTo500()
        {
            SetGetListByCustomerException();
            var request = new DataSourceRequest();
            _target.ReadAjaxResolverLevelZeroGrid(request);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_Add_LevelPassed_IsUsedToFormReturningViewName()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, ServiceComponentId2) as ViewResult;
            Assert.AreEqual("AddLevelTwo", result.ViewName);
        }

        [TestMethod]
        public void ResolverController_Add_IdNotPassed_HasServiceComponentContextFalse()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, null) as ViewResult;
            var model = result.Model as AddResolverViewModel;
            Assert.IsFalse(model.HasServiceComponentContext);
        }

        [TestMethod]
        public void ResolverController_Add_IdIsPassed_HasServiceComponentContextTrue()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, ServiceComponentId2) as ViewResult;
            var model = result.Model as AddResolverViewModel;
            Assert.IsTrue(model.HasServiceComponentContext);
        }

        [TestMethod]
        public void ResolverController_Add_IdIsPassed_ReturnUrlIsServiceComponentEdit()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, ServiceComponentId2) as ViewResult;
            var model = result.Model as AddResolverViewModel;
            Assert.AreEqual("/ServiceComponent/Edit/" + ServiceComponentId2 + "?level=LevelTwo", model.ReturnUrl);
        }

        [TestMethod]
        public void ResolverController_Add_IdNotPassed_ReturnUrlIsServiceComponentIndex()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, null) as ViewResult;
            var model = result.Model as AddResolverViewModel;
            Assert.AreEqual("/Resolver?level=LevelTwo", model.ReturnUrl);
        }

        [TestMethod]
        public void ResolverController_Add_IdNotPassed_ServiceDeliveryUnitTypesAddedToModel()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, null) as ViewResult;
            var model = result.Model as AddResolverViewModel;
            Assert.AreEqual(5, model.ServiceDeliveryUnitTypes.Count);
        }

        [TestMethod]
        public void ResolverController_Add_IdNotPassed_ResolverGroupTypesAddedToModel()
        {
            var result = _target.Add(NavigationLevelNames.LevelTwo, null) as ViewResult;
            var model = result.Model as AddResolverViewModel;
            Assert.AreEqual(5, model.ServiceDeliveryOrganisationTypes.Count);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_CustomerContextNull_NoCreateTakesPlace()
        {
            SetCustomerContextNull();
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_CustomerContextZero_NoCreateTakesPlace()
        {
            SetCustomerContextZero();
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_NoResolversPassed_NoCreateTakesPlace()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), null);
            _mockServiceComponentService.Verify(v => v.Create(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_NoResolversPassed_NoCreateTakesPlace()
        {
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), null);
            _mockResolverService.Verify(v => v.Create(It.IsAny<Resolver>(), false), Times.Never);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_NoResolversPassed_DataSourceResultStillReturned()
        {
            var result = _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), null) as JsonResult;
            var data = result.Data as DataSourceResult;
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_NoResolversPassed_DataSourceResultStillReturned()
        {
            var result = _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), null) as JsonResult;
            var data = result.Data as DataSourceResult;
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolversHaveDuplicateComponent_ErrorAddedToModelState()
        {
            var duplicate = _bulkResolverViewModels.Single(x => x.ServiceComponentId == ServiceComponentId1).GetClone();
            _bulkResolverViewModels.Add(duplicate);
            var result = _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ServiceComponentAddDuplicateParent));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverPassedWithNoResolverDetails_IgnoredFromProcessing()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Id == ServiceComponentId2));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverPassedWithUnknownComponent_IgnoredFromProcessing()
        {
            var unknownId = _bulkResolverViewModels.Last().Id;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Id == unknownId));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryOrganisationPassed_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryOrganisationPassed_InsertedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryOrganisationPassed_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryOrganisationPassed_UpdatedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedBy != UserName));
        }


        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryUnitPassed_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryUnitPassed_InsertedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryUnitPassed_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ServiceDeliveryUnitPassed_UpdatedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedBy != UserName));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverGroupPassed_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolverGroupPassed_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Year != now.Year));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Month != now.Month));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Day != now.Day));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Minute != now.Minute));
        }


        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_SResolverGroupPassed_InsertedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_SResolverGroupPassed_InsertedBySet()
        {
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverGroupPassed_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolverGroupPassed_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverGroupPassed_UpdatedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedBy != UserName));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolverGroupPassed_UpdatedBySet()
        {
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedBy != UserName));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverPassed_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolverPassed_InsertedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Year != now.Year));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Month != now.Month));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Day != now.Day));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.InsertedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverPassed_InsertedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolverPassed_InsertedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.InsertedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverPassed_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolverPassed_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolverPassed_UpdatedBySet()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedBy != UserName));
            Assert.IsFalse(_serviceComponentBulkUpdated.Any(x => x.Resolver.UpdatedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolverPassed_UpdatedBySet()
        {
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedBy != UserName));
            Assert.IsFalse(_serviceDeskBulkUpdated.Any(x => x.UpdatedBy != UserName));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ResolversPassed_BulkUpdateCalled()
        {
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            _mockServiceComponentService.Verify(x => x.Update(It.IsAny<IEnumerable<ServiceComponent>>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_ResolversPassed_BulkUpdateCalled()
        {
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            _mockDeskService.Verify(x => x.Update(It.IsAny<IEnumerable<ServiceDesk>>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetGetByCustomerByException();
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverZevelZeroGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            _mockDeskService
               .Setup(x => x.GetByCustomer(It.IsAny<int>()))
               .Throws(new Exception("Oh no!!"));
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerByException();
            _target.CreateAjaxAddResolverGrid(new DataSourceRequest(), _bulkResolverViewModels);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverZevelZeroGrid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            _mockDeskService
              .Setup(x => x.GetByCustomer(It.IsAny<int>()))
              .Throws(new Exception("Oh no!!"));
            _target.CreateAjaxAddResolverLevelZeroGrid(new DataSourceRequest(), _bulkResolverLevelZeroViewModels);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_CurrentCustomerNull_NoDeleteTakesPlace()
        {
            SetCustomerContextNull();
            var result = _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel()) as JsonResult;
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLevelZeroGrid_CurrentCustomerNull_NoDeleteTakesPlace()
        {
            SetCustomerContextNull();
            var result = _target.DeleteAjaxResolverLevelZeroGrid(new DataSourceRequest(), new ResolverViewModel()) as JsonResult;
            _mockDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_CurrentCustomerZero_NoDeleteTakesPlace()
        {
            SetCustomerContextZero();
            var result = _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel()) as JsonResult;
            _mockServiceComponentService.Verify(v => v.Delete(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLevleZeroGrid_CurrentCustomerZero_NoDeleteTakesPlace()
        {
            SetCustomerContextZero();
            var result = _target.DeleteAjaxResolverLevelZeroGrid(new DataSourceRequest(), new ResolverViewModel()) as JsonResult;
            _mockDeskService.Verify(v => v.Delete(It.IsAny<ServiceDesk>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            SetGetByCustomerException();
            _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLvelZeroGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            _mockResolverService
                .Setup(x => x.GetById(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
            _target.DeleteAjaxResolverLevelZeroGrid(new DataSourceRequest(), new ResolverViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerException();
            _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLevelZeroGrid_ExceptionOccurs_AppendsErrorMessageToHeader()
        {
            _mockResolverService
                .Setup(x => x.GetById(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
            _target.DeleteAjaxResolverLevelZeroGrid(new DataSourceRequest(), new ResolverViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_ServiceComponentCannotBeFound_ErrorAddedToModelState()
        {
            var result = _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel() { ServiceComponentId = 666 }) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ServiceComponentCannotBeFound));
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_ServiceComponentCannotBeFound_NoDeleteTakesPlace()
        {
            _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel() { ServiceComponentId = 666 });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_ServiceComponentFoundButHasDependents_ErrorAddedToModelState()
        {
            var result = _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel() { ServiceComponentId = ServiceComponentIdWithResolverDependent }) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ResolverCannotBeDeletedDueToDependents));
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLevelZeroGrid_ServiceComponentFoundButHasDependents_ErrorAddedToModelState()
        {
            var result = _target.DeleteAjaxResolverLevelZeroGrid(new DataSourceRequest(), new ResolverViewModel() { ServiceComponentId = ServiceComponentIdWithResolverDependent }) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ResolverCannotBeDeletedDueToDependents));
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_ServiceComponentFoundButHasDependents_NoDeleteTakesPlace()
        {
            _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel() { ServiceComponentId = ServiceComponentIdWithResolverDependent });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLevelZeroGrid_ServiceComponentFoundButHasDependents_NoDeleteTakesPlace()
        {
            _target.DeleteAjaxResolverLevelZeroGrid(new DataSourceRequest(), new ResolverViewModel() { ServiceComponentId = ServiceComponentIdWithResolverDependent });
            _mockDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_ServiceComponentFoundAndCanBeDeleted_DeleteTakesPlace()
        {
            SetResolverGroupHelperCanDelete(true);
            _target.DeleteAjaxResolverGrid(new DataSourceRequest(), new ResolverViewModel() { ServiceComponentId = ServiceComponentIdWithResolverDependent });
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLevelZeroGrid_ServiceComponentFoundAndCanBeDeleted_DeleteTakesPlace()
        {
            SetResolverGroupHelperCanDelete(true);
            _mockResolverService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new Resolver { Id = 1 });
            _mockDeskService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new ServiceDesk { Id = 1 });
            _target.DeleteAjaxResolverLevelZeroGrid(new DataSourceRequest(), new ResolverViewModel());
            _mockDeskService.Verify(v => v.Update(It.IsAny<ServiceDesk>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_EditGet_NoComponentFoundWithSuppliedId_ReturnsToComponentIndex()
        {
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdNotPresent) as RedirectToRouteResult;
            Assert.AreEqual("LevelOne", result.RouteValues["level"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ResolverController_EditGet_EditStateLevelZeroWithResolver_ReturnUrlIsCorrect()
        {
            var result = _target.Edit(NavigationLevelNames.LevelZero, 1) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditResolverLevelZeroViewModel));
            var model = data as EditResolverLevelZeroViewModel;
            Assert.AreEqual("/Resolver?level=LevelZero", model.ReturnUrl);
        }

        [TestMethod]
        public void ResolverController_EditGet_EditStateLevel1WithResolver_ReturnsEditResolverViewModelViewModel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditResolverViewModel));
        }

        [TestMethod]
        public void ResolverController_EditGet_EditStateLevel1WithResolver_SetsEditLevel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>())).Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var data = result.Model as EditServiceComponentViewModel;
            Assert.AreEqual("LevelOne", data.EditLevel);
        }


        [TestMethod]
        public void ResolverController_EditGet_EditStateLevel2_ReturnsEditServiceComponentLevel2ViewModel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var data = result.Model;
            Assert.IsInstanceOfType(data, typeof(EditResolverViewModel));
        }

        [TestMethod]
        public void ResolverController_EditGet_EditStateLevel2_SetsEditLevel()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(ServiceComponentEditState.Level2);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            var data = result.Model as EditServiceComponentViewModel;
            Assert.AreEqual("LevelOne", data.EditLevel);
        }

        [TestMethod]
        public void ResolverController_EditGet_EditStateNone_ReturnsToComponentIndex()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>())).Returns(ServiceComponentEditState.None);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as RedirectToRouteResult;
            Assert.AreEqual("LevelOne", result.RouteValues["level"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ResolverController_EditGet_LevelPassed_AppendedToViewName()
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>())).Returns(ServiceComponentEditState.Level1WithResolver);
            var result = _target.Edit(NavigationLevelNames.LevelOne, ServiceComponentIdWithResolverDependent) as ViewResult;
            Assert.AreEqual("EditLevelOne", result.ViewName);
        }


        [TestMethod]
        public void ResolverController_EditPost_Level1WithResolverOperationalProcessAddded_TypeIsNotSetToVisibleWhenLessThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(3);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(1)).Returns(1);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(3)).Returns(1);

            SetGetEditState(ServiceComponentEditState.Level1WithResolver);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelOne.ToString());
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
            _mockOperationalProcessTypeRefDataService.Verify(x => x.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_EditPost_Level1WithResolverOperationalProcessAddded_TypeIsSetToVisibleWhenEqualThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(1)).Returns(1);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(3)).Returns(1);

            SetGetEditState(ServiceComponentEditState.Level1WithResolver);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelOne.ToString());
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
            _mockOperationalProcessTypeRefDataService.Verify(x => x.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_EditPost_Level1WithResolverOperationalProcessAddded_TypeIsSetToVisibleWhenGreaterThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(1)).Returns(2);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(3)).Returns(1);

            SetGetEditState(ServiceComponentEditState.Level1WithResolver);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelOne.ToString());
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
            _mockOperationalProcessTypeRefDataService.Verify(x => x.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_EditPost_Level2OperationalProcessAddded_TypeIsNotSetToVisibleWhenLessThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(3);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(1)).Returns(1);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(3)).Returns(1);

            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
            _mockOperationalProcessTypeRefDataService.Verify(x => x.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_EditPost_Level2OperationalProcessAddded_TypeIsSetToVisibleWhenEqualThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(1)).Returns(1);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(3)).Returns(1);

            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
            _mockOperationalProcessTypeRefDataService.Verify(x => x.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_EditPost_Level2OperationalProcessAddded_TypeIsSetToVisibleWhenGreaterThanThreshold()
        {
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(It.Is<string>(m => m == ParameterNames.CustomerSpecificTypeThreshold))).Returns(2);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(1)).Returns(2);
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetNumberOfOperationalProcessTypeReferences(3)).Returns(1);

            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
            _mockOperationalProcessTypeRefDataService.Verify(x => x.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_EditPost_OperationalProcessesNullOnModel_OperationalProcessesTypesRemoved()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            vm.OperationalProcesses = null;
            _target.Edit(vm);
            Assert.IsTrue(!_serviceComponentUpdated.Resolver.OperationalProcessTypes.Any());
        }

        [TestMethod]
        public void ResolverController_EditPost_OperationalProcessesNullOnModel_ResolverGroupUpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            vm.OperationalProcesses = null;
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
        }

        [TestMethod]
        public void ResolverController_EditPost_OperationalProcessesNullOnModel_ResolverGroupUpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            vm.OperationalProcesses = null;
            _target.Edit(vm);
            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ResolverController_EditPost_OperationalProcessesSupplied_OperationalProcessTypesAddedToComponent()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            _target.Edit(vm);
            Assert.IsTrue(vm.OperationalProcesses.OperationalProcessTypes.SequenceEqual(
                _serviceComponentUpdated.Resolver.OperationalProcessTypes.Select(
                    x => x.OperationalProcessTypeRefDataId)));
        }

        [TestMethod]
        public void ResolverController_EditPost_OperationalProcessesSuppliedAndExistingServiceComponentAlreadyHasSome_OnlyAddNewOnes()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            _target.Edit(vm);
            Assert.IsTrue(vm.OperationalProcesses.OperationalProcessTypes.SequenceEqual(
                _serviceComponentUpdated.Resolver.OperationalProcessTypes.Select(
                    x => x.OperationalProcessTypeRefDataId)));
        }

        [TestMethod]
        public void ResolverController_EditPost_OperationalProcessesSupplied_ResolverUpdatedBySet()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            _target.Edit(vm);
            Assert.AreEqual(UserName, _serviceComponentUpdated.Resolver.UpdatedBy);
        }

        [TestMethod]
        public void ResolverController_EditPost_OperationalProcessesSupplied_ResolverUpdatedDateSet()
        {
            var now = DateTime.Now;
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            _target.Edit(vm);
            Assert.AreEqual(now.Year, _serviceComponentUpdated.Resolver.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _serviceComponentUpdated.Resolver.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _serviceComponentUpdated.Resolver.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _serviceComponentUpdated.Resolver.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _serviceComponentUpdated.Resolver.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ResolverController_EditPost_LevelOneOperationalProcessesSupplied_ReturnsToServiceComponentEdit()
        {
            SetGetEditState(ServiceComponentEditState.Level1WithResolver);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelOne.ToString());
            var result = _target.Edit(vm) as RedirectToRouteResult;
            Assert.AreEqual("LevelOne", result.RouteValues["level"]);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
            Assert.AreEqual("ServiceComponent", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ResolverController_EditPost_LevelTwoOperationalProcessesSupplied_ReturnsToServiceComponentEdit()
        {
            SetGetEditState(ServiceComponentEditState.Level2);
            var vm = GetEditResolverViewModel(ServiceComponentIdWithResolverDependent, NavigationLevelName.LevelTwo.ToString());
            var result = _target.Edit(vm) as RedirectToRouteResult;
            Assert.AreEqual("LevelTwo", result.RouteValues["level"]);
            Assert.AreEqual("Edit", result.RouteValues["action"]);
            Assert.AreEqual("ServiceComponent", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ResolverController_Move_ModelStateInvalid_ErrorMessageAddedToResponseHeader()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.Move(new MoveResolverViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, "XXX"), Times.Once);
        }

        [TestMethod]
        public void ResolverController_Move_ModelStateInvalid_SetsStatusCodeTo500()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.Move(new MoveResolverViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_MoveLevelZero_ResolverServiceMoveCalled()
        {
            _target.MoveLevelZero(new MoveResolverLevelZeroViewModel() { Id = 1, DestinationDeskId = 1 });
            _mockResolverService.Verify(x => x.Move(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ResolverController_MoveLevelZero_ModelStateInvalid_ErrorMessageAddedToResponseHeader()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.MoveLevelZero(new MoveResolverLevelZeroViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, "XXX"), Times.Once);
        }

        [TestMethod]
        public void ResolverController_MoveLevelZero_ModelStateInvalid_SetsStatusCodeTo500()
        {
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.MoveLevelZero(new MoveResolverLevelZeroViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_MoveLevelZero_CustomerContextNull_NoMoveOccurs()
        {
            SetCustomerContextNull();
            _target.MoveLevelZero(new MoveResolverLevelZeroViewModel());
            _mockResolverService.Verify(v => v.Move(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_Move_CustomerContextNull_NoMoveOccurs()
        {
            SetCustomerContextNull();
            _target.Move(new MoveResolverViewModel());
            _mockServiceComponentService.Verify(v => v.Update(It.IsAny<ServiceComponent>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_Move_CustomerContextZero_NoMoveOccurs()
        {
            SetCustomerContextZero();
            _target.Move(new MoveResolverViewModel());
            _mockServiceComponentService.Verify(v => v.MoveResolver(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ResolverController_Move_SourceServiceComponentCannotBeFound_ErrorMessageAddedToResponseHeader()
        {
            _target.Move(new MoveResolverViewModel
            {
                DestinationServiceComponentId = ServiceComponentId1,
                ServiceComponentId = ServiceComponentIdNotPresent
            });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, WebResources.ServiceComponentCannotBeFound), Times.Once);
        }

        [TestMethod]
        public void ResolverController_Move_SourceServiceComponentCannotBeFound_SetsStatusCodeTo500()
        {
            _target.Move(new MoveResolverViewModel
            {
                DestinationServiceComponentId = ServiceComponentId1,
                ServiceComponentId = ServiceComponentIdNotPresent
            });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_Move_DestinationServiceComponentCannotBeFound_ErrorMessageAddedToResponseHeader()
        {
            _target.Move(new MoveResolverViewModel
            {
                ServiceComponentId = ServiceComponentId1,
                DestinationServiceComponentId = ServiceComponentIdNotPresent
            });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, WebResources.ServiceComponentCannotBeFound), Times.Once);
        }

        [TestMethod]
        public void ResolverController_Move_DestinationServiceComponentCannotBeFound_SetsStatusCodeTo500()
        {
            _target.Move(new MoveResolverViewModel
            {
                ServiceComponentId = ServiceComponentId1,
                DestinationServiceComponentId = ServiceComponentIdNotPresent
            });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ResolverController_Move_BothServiceComponentsExist_MoveIsCalledWithCorrectParameters()
        {
            _target.Move(new MoveResolverViewModel
            {
                ServiceComponentId = ServiceComponentId1,
                DestinationServiceComponentId = ServiceComponentId2
            });
            _mockServiceComponentService.Verify(v => v.MoveResolver(CustomerId, ServiceComponentId1, ServiceComponentId2), Times.Once);
        }

        [TestMethod]
        public void ResolverController_DeleteResolver_CallsResolverServiceDelete()
        {
            #region Arrange

            #endregion

            #region Act

            _target.DeleteResolver(1);

            #endregion

            #region Assert

            _mockResolverService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockResolverService.Verify(x => x.Delete(It.IsAny<Resolver>()), Times.Once);

            #endregion
        }


        #region Role Checks

        [TestMethod]
        public void ResolverController_Index_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("Index", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("ReadAjaxResolverGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_Add_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Add", (AuthorizeAttribute att) => att.Roles, new[] { typeof(string), typeof(int?) }));
        }

        [TestMethod]
        public void ResolverController_EditGet_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new[] { typeof(string), typeof(int) }));
        }

        [TestMethod]
        public void ResolverController_EditPost_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new[] { typeof(EditResolverViewModel) }));
        }

        [TestMethod]
        public void ResolverController_EditLevelZeroPost_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("EditLevelZero", (AuthorizeAttribute att) => att.Roles, new[] { typeof(EditResolverLevelZeroViewModel) }));
        }

        [TestMethod]
        public void ResolverController_ReadAjaxResolverGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("ReadAjaxAddResolverGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DeleteAjaxResolverGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_DeleteAjaxResolverLevleZeroGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DeleteAjaxResolverLevelZeroGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("CreateAjaxAddResolverGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_CreateAjaxAddResolverLevelZeroGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("CreateAjaxAddResolverLevelZeroGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_UpdateAjaxAddResolverGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxAddResolverGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_DestroyAjaxAddResolverGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("DestroyAjaxAddResolverGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ResolverController_MoveGet_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Move", (AuthorizeAttribute att) => att.Roles, new Type[] { }));
        }

        [TestMethod]
        public void ResolverController_MovePost_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("Move", (AuthorizeAttribute att) => att.Roles, new[] { typeof(MoveResolverViewModel) }));
        }

        [TestMethod]
        public void ResolverController_MoveLevelZeroGet_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("MoveLevelZero", (AuthorizeAttribute att) => att.Roles, new Type[] { }));
        }

        [TestMethod]
        public void ResolverController_MoveLevelZeroPost_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("MoveLevelZero", (AuthorizeAttribute att) => att.Roles, new[] { typeof(MoveResolverLevelZeroViewModel) }));
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

        private void SetGetResolverByCustomerWithHierarchyException()
        {
            _mockServiceComponentService
                .Setup(x => x.GetResolverByCustomerWithHierarchy(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
        }

        private void SetGetListByCustomerException()
        {
            _mockResolverService
                .Setup(x => x.GetListByCustomer(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
        }

        private void SetGetByCustomerByException()
        {
            _mockServiceComponentService
                .Setup(x => x.GetByCustomer(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
        }

        private void SetResolverGroupHelperCanDelete(bool canDelete)
        {
            _mockResolverHelper
                .Setup(x => x.CanDelete(It.IsAny<Resolver>()))
                .Returns(canDelete);
        }

        private void SetGetEditState(ServiceComponentEditState state)
        {
            _mockServiceComponentHelper.Setup(s => s.GetEditState(It.IsAny<ServiceComponent>()))
                .Returns(state);
        }

        private EditResolverViewModel GetEditResolverViewModel(int serviceComponentId, string navigationLevelName)
        {
            return new EditResolverViewModel
            {
                Id = serviceComponentId,
                ComponentName = UnitTestHelper.GenerateRandomData<EditServiceComponentNameViewModel>(),
                EditLevel = navigationLevelName,
                OperationalProcesses = new OperationalProcessTypesViewModel
                {
                    OperationalProcessTypes = new[] { 1, 3 }
                },
                ResolverGroup = UnitTestHelper.GenerateRandomData<EditResolverResolverGroupViewModel>(rg =>
                {
                    rg.ResolverGroupTypeId = 1;
                }),
                ResolverServiceDeliveryOrganisation = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryOrganisationViewModel>(),
                ResolverServiceDeliveryUnit = UnitTestHelper.GenerateRandomData<EditResolverServiceDeliveryUnitViewModel>()
            };
        }

        #endregion
    }
}