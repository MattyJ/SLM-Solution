using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
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
using Kendo.Mvc.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ReferenceDataControllerTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<IAppUserContext> _mockAppUserContext;

        private Mock<IDomainTypeRefDataService> _mockDomainTypeRefDataService;
        private Mock<IFunctionTypeRefDataService> _mockFunctionTypeRefDataService;
        private Mock<IInputTypeRefDataService> _mockInputTypeRefDataService;
        private Mock<IServiceDeliveryUnitTypeRefDataService> _mockServiceDeliveryUnitTypeRefDataService;
        private Mock<IServiceDeliveryOrganisationTypeRefDataService> _mockServiceDeliveryOrganisationTypeRefDataService;
        private Mock<IResolverGroupTypeRefDataService> _mockResolverGroupTypeRefDataService;
        private Mock<IOperationalProcessTypeRefDataService> _mockOperationalProcessTypeRefDataService;
        private Mock<IRegionTypeRefDataService> _mockRegionTypeRefDataService;
        private Mock<IParameterService> _mockParameterService;

        private Mock<IUserManager> _mockUserManager;
        private Mock<IResponseManager> _mockResponseManager;
        private Mock<ILoggingManager> _mockLoggingManager;

        private ReferenceDataController _controller;
        private ReferenceDataController _controllerWithMockedServices;
        private AppContext _appContext;
        private AppContext _appContextWithCustomer;

        private IDomainTypeRefDataService _domainTypeRefDataService;
        private IFunctionTypeRefDataService _functionTypeRefDataService;
        private IInputTypeRefDataService _inputTypeRefDataService;
        private IResolverGroupTypeRefDataService _resolverGroupTypeRefDataService;
        private IOperationalProcessTypeRefDataService _operationalProcessTypeRefDataService;
        private IServiceDeliveryUnitTypeRefDataService _serviceDeliveryUnitTypeRefDataService;

        private Mock<IRepository<DomainTypeRefData>> _mockDomainTypeRefDataRepository;
        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<FunctionTypeRefData>> _mockFunctionTypeRefDataRepository;
        private Mock<IRepository<InputTypeRefData>> _mockInputTypeRefDataRepository;
        private Mock<IRepository<ServiceDomain>> _mockServiceDomainRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;
        private Mock<IRepository<ServiceFunction>> _mockServiceFunctionRepository;
        private Mock<IRepository<ResolverGroupTypeRefData>> _mockResolverGroupTypeRefDataRepository;
        private Mock<IRepository<OperationalProcessTypeRefData>> _mockOperationalProcessTypeRefDataRepository;
        private Mock<IRepository<OperationalProcessType>> _mockOperationalProcessTypeRepository;
        private Mock<IRepository<ServiceDeliveryUnitTypeRefData>> _mockServiceDeliveryUnitTypeRefDataRepository;

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<DomainTypeRefData> _domainTypeRefData;
        private List<FunctionTypeRefData> _functionTypeRefData;
        private List<InputTypeRefData> _inputTypeRefData;
        private List<ServiceDeliveryOrganisationTypeRefData> _serviceDeliveryOrganisationTypeRefDatas;
        private List<ServiceDeliveryUnitTypeRefData> _serviceDeliveryUnitTypeRefDatas;
        private List<ResolverGroupTypeRefData> _resolverGroupTypeRefDatas;
        private List<OperationalProcessTypeRefData> _operationalProcessTypeRefDatas;
        private List<Resolver> _resolvers;
        private ServiceDeliveryOrganisationTypeRefData _serviceDeliveryOrganisationTypeRefDataCreated;
        private ServiceDeliveryOrganisationTypeRefData _serviceDeliveryOrganisationTypeRefDataUpdated;
        private ServiceDeliveryOrganisationTypeRefData _serviceDeliveryOrganisationTypeRefDataDeleted;
        private OperationalProcessTypeRefData _operationalProcessTypeRefDataCreated;
        private OperationalProcessTypeRefData _operationalProcessTypeRefDataUpdated;
        private OperationalProcessTypeRefData _operationalProcessTypeRefDataDeleted;
        private ResolverGroupTypeRefData _resolverGroupTypeRefDataCreated;
        private ResolverGroupTypeRefData _resolverGroupTypeRefDataUpdated;
        private ResolverGroupTypeRefData _resolverGroupTypeRefDataDeleted;
        private ServiceDeliveryUnitTypeRefData _serviceDeliveryUnitTypeRefDataCreated;
        private ServiceDeliveryUnitTypeRefData _serviceDeliveryUnitTypeRefDataUpdated;
        private ServiceDeliveryUnitTypeRefData _serviceDeliveryUnitTypeRefDataDeleted;
        private List<ServiceDomain> _domains;
        private List<ServiceFunction> _functions;
        private Mock<IRepository<Resolver>> _mockResolverRespository;
        private List<RegionTypeRefData> _regionTypeRefDatas;
        private const int CustomerId = 666;
        private const int ServiceDeliveryOrganisationTypeRefDataIdExists = 192;
        private const int ServiceDeliveryOrganisationTypeRefDataIdNotExists = 122;
        private const int OperationalProcessTypeRefDataIdExists = 13292;
        private const int OperationalProcessTypeRefDataIdNotExists = 1222;
        private const int ServiceDeliveryUnitTypeRefDataIdExists = 32;
        private const int ServiceDeliveryUnitTypeRefDataIdNotExists = 65;
        private const int ResolverGroupTypeRefDataIdExists = 64;
        private const int ResolverGroupTypeRefDataIdNotExists = 130;

        [TestInitialize]
        public void Initialize()
        {
            #region Lists

            _operationalProcessTypeRefDatas = new List<OperationalProcessTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Id = 1;
                    x.OperationalProcessTypeName = "Operational Process Type One";
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Id = 2;
                    x.Visible = true;

                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Id = OperationalProcessTypeRefDataIdExists;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                     x.Id = 3;
                    x.Visible = true;

                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                     x.Id = 4;
                    x.Visible = true;

                })
            };

            _serviceDeliveryOrganisationTypeRefDatas = new List<ServiceDeliveryOrganisationTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(x =>
                {
                    x.ServiceDeliveryOrganisationTypeName = "Service Delivery Organisation One";
                }),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(x =>
                {
                    x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>()
            };

            _serviceDeliveryUnitTypeRefDatas = new List<ServiceDeliveryUnitTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(x =>
                {
                    x.Id = 1;
                    x.ServiceDeliveryUnitTypeName = "Service Delivery Unit One";
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(x =>
                {
                    x.Id = 2;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(x =>
                {
                    x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(x =>
                {
                    x.Id = 3;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(x =>
                {
                    x.Id = 4;
                    x.Visible = true;
                }),
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
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = ResolverGroupTypeRefDataIdExists;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = 3;
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(x =>
                {
                    x.Id = 4;
                    x.Visible = false;
                })
            };

            _domainTypeRefData = new List<DomainTypeRefData>
            {
                new DomainTypeRefData
                {
                    Id =1,
                    DomainName = "Domain A",
                    Visible = true,
                    SortOrder = 5
                },
                new DomainTypeRefData
                {
                    Id =2,
                    DomainName = "Domain B",
                    Visible = true,
                    SortOrder = 5
                },
                new DomainTypeRefData
                {
                    Id =3,
                    DomainName = "Domain C",
                    Visible = true,
                    SortOrder = 5
                },
                new DomainTypeRefData
                {
                    Id =4,
                    DomainName = "Domain D",
                    Visible = true,
                    SortOrder = 5
                },
            };

            _functionTypeRefData = new List<FunctionTypeRefData>
            {
                new FunctionTypeRefData
                {
                    Id =1,
                    FunctionName = "Function A",
                    Visible = true,
                    SortOrder = 5
                },
                new FunctionTypeRefData
                {
                    Id =2,
                    FunctionName = "Function B",
                    Visible = true,
                    SortOrder = 5
                },
                new FunctionTypeRefData
                {
                    Id =3,
                    FunctionName = "Function C",
                    Visible = true,
                    SortOrder = 5
                },
                new FunctionTypeRefData
                {
                    Id =4,
                    FunctionName = "Function D",
                    Visible = true,
                    SortOrder = 5
                },
            };

            _inputTypeRefData = new List<InputTypeRefData>
            {
                new InputTypeRefData
                {
                    Id =1,
                    InputTypeName = "Input A",
                    SortOrder = 5
                },
                new InputTypeRefData
                {
                    Id =2,
                    InputTypeName = "Input B",
                    SortOrder = 5
                },
                new InputTypeRefData
                {
                    Id =3,
                    InputTypeName = "Input C",
                    SortOrder = 5
                },
                new InputTypeRefData
                {
                    Id =4,
                    InputTypeName = "Input D",
                    SortOrder = 5
                },
            };


            _domains = new List<ServiceDomain>
            {
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.Id = 1;
                    x.DomainTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.Id = 2;
                    x.DomainTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.Id = 3;
                    x.DomainTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.Id = 4;
                    x.DomainTypeId = 2;
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.Id = 5;
                    x.DomainTypeId = 3;
                })
            };

            _functions = new List<ServiceFunction>
            {
                UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
                {
                    x.Id = 1;
                    x.FunctionTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
                {
                    x.Id = 2;
                    x.FunctionTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
                {
                    x.Id = 3;
                    x.FunctionTypeId = 1;
                }),
                UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
                {
                    x.Id = 4;
                    x.FunctionTypeId = 2;
                }),
                UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
                {
                    x.Id = 3;
                    x.FunctionTypeId =3;
                }),
            };

            _resolvers = new List<Resolver>
            {
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 1;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.FirstOrDefault(y => y.Id == 1);
                    x.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas.FirstOrDefault(y => y.Id == 1);
                    x.OperationalProcessTypes = new List<OperationalProcessType>
                    {
                        new OperationalProcessType
                        {
                            Id = 1,
                            OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(z => z.Id == 1),
                            OperationalProcessTypeRefDataId = 1
                        },
                        new OperationalProcessType
                        {
                            Id = 2,
                            OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(z => z.Id == 2),
                            OperationalProcessTypeRefDataId = 2
                        }
                    };
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 2;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.FirstOrDefault(y => y.Id == 2);
                     x.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas.FirstOrDefault(y => y.Id == ServiceDeliveryUnitTypeRefDataIdExists);
                     x.OperationalProcessTypes = new List<OperationalProcessType>
                    {
                        new OperationalProcessType
                        {
                            Id = 3,
                            OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(z => z.Id == 3),
                            OperationalProcessTypeRefDataId = 3
                        },
                        new OperationalProcessType
                        {
                            Id = 4,
                            OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(z => z.Id == 1),
                            OperationalProcessTypeRefDataId = 1
                        }
                    };
                })
            };

            _regionTypeRefDatas = new List<RegionTypeRefData>
            {
                new RegionTypeRefData
                {
                    Id = 1,
                    RegionName = "EMEMIA",
                    SortOrder = 5
                },
                new RegionTypeRefData
                {
                    Id = 2,
                    RegionName = "Japan",
                    SortOrder = 5
                },
                new RegionTypeRefData
                {
                    Id = 3,
                    RegionName = "Ocenia",
                    SortOrder = 5
                }
            };

            #endregion

            _appContext = new AppContext();

            _appContextWithCustomer = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            };

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockUserManager = new Mock<IUserManager>();
            _mockResponseManager = new Mock<IResponseManager>();

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel(),
            };

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockLoggingManager = new Mock<ILoggingManager>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockDomainTypeRefDataRepository = MockRepositoryHelper.Create(_domainTypeRefData, (entity, id) => entity.Id == (int)id);
            _mockFunctionTypeRefDataRepository = MockRepositoryHelper.Create(_functionTypeRefData, (entity, id) => entity.Id == (int)id);
            _mockInputTypeRefDataRepository = MockRepositoryHelper.Create(_inputTypeRefData, (entity, id) => entity.Id == (int)id);
            _mockServiceDomainRepository = MockRepositoryHelper.Create(_domains, (entity, id) => entity.Id == (int)id);
            _mockServiceFunctionRepository = MockRepositoryHelper.Create(_functions, (entity, id) => entity.Id == (int)id);
            _mockResolverGroupTypeRefDataRepository = MockRepositoryHelper.Create(_resolverGroupTypeRefDatas, (entity, id) => entity.Id == (int)id);
            _mockOperationalProcessTypeRefDataRepository = MockRepositoryHelper.Create(_operationalProcessTypeRefDatas, (entity, id) => entity.Id == (int)id);
            _mockServiceDeskRepository = new Mock<IRepository<ServiceDesk>>();
            _mockDeskInputTypeRepository = new Mock<IRepository<DeskInputType>>();
            _mockResolverRespository = MockRepositoryHelper.Create(_resolvers, (entity, id) => entity.Id == (int)id);
            _mockOperationalProcessTypeRepository = new Mock<IRepository<OperationalProcessType>>();
            _mockServiceDeliveryUnitTypeRefDataRepository = MockRepositoryHelper.Create(_serviceDeliveryUnitTypeRefDatas, (entity, id) => entity.Id == (int)id);
            _mockParameterService = new Mock<IParameterService>();

            _domainTypeRefDataService = new DomainTypeRefDataService(_mockDomainTypeRefDataRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            _functionTypeRefDataService = new FunctionTypeRefDataService(
                _mockFunctionTypeRefDataRepository.Object,
                _mockServiceFunctionRepository.Object,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            _inputTypeRefDataService = new InputTypeRefDataService(
                _mockInputTypeRefDataRepository.Object,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);

            _resolverGroupTypeRefDataService = new ResolverGroupTypeRefDataService(
                _mockResolverGroupTypeRefDataRepository.Object,
                _mockResolverRespository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            _operationalProcessTypeRefDataService = new OperationalProcessTypeRefDataService(
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockUnitOfWork.Object);

            _serviceDeliveryUnitTypeRefDataService = new ServiceDeliveryUnitTypeRefDataService(
                _mockServiceDeliveryUnitTypeRefDataRepository.Object,
                _mockResolverRespository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            _mockRegionTypeRefDataService = new Mock<IRegionTypeRefDataService>();
            _mockRegionTypeRefDataService.Setup(s => s.All()).Returns(_regionTypeRefDatas.AsQueryable());

            _mockServiceDeliveryOrganisationTypeRefDataService = new Mock<IServiceDeliveryOrganisationTypeRefDataService>();
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.All())
                .Returns(_serviceDeliveryOrganisationTypeRefDatas.AsQueryable());
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()))
                .Callback<ServiceDeliveryOrganisationTypeRefData>(rg => _serviceDeliveryOrganisationTypeRefDataUpdated = rg);
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.Delete(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()))
                .Callback<ServiceDeliveryOrganisationTypeRefData>(rg => _serviceDeliveryOrganisationTypeRefDataDeleted = rg);
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()))
                .Callback<ServiceDeliveryOrganisationTypeRefData>(rg => _serviceDeliveryOrganisationTypeRefDataCreated = rg);
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _serviceDeliveryOrganisationTypeRefDatas.SingleOrDefault(x => x.Id == id));

            _mockServiceDeliveryUnitTypeRefDataService = new Mock<IServiceDeliveryUnitTypeRefDataService>();
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.All())
                .Returns(_serviceDeliveryUnitTypeRefDatas.AsQueryable());
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Callback<ServiceDeliveryUnitTypeRefData>(rg => _serviceDeliveryUnitTypeRefDataUpdated = rg);
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Delete(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Callback<ServiceDeliveryUnitTypeRefData>(rg => _serviceDeliveryUnitTypeRefDataDeleted = rg);
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Callback<ServiceDeliveryUnitTypeRefData>(rg => _serviceDeliveryUnitTypeRefDataCreated = rg);
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _serviceDeliveryUnitTypeRefDatas.SingleOrDefault(x => x.Id == id));

            _mockResolverGroupTypeRefDataService = new Mock<IResolverGroupTypeRefDataService>();
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.All())
                .Returns(_resolverGroupTypeRefDatas.AsQueryable());
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ResolverGroupTypeRefData>()))
                .Callback<ResolverGroupTypeRefData>(rg => _resolverGroupTypeRefDataUpdated = rg);
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Delete(It.IsAny<ResolverGroupTypeRefData>()))
                .Callback<ResolverGroupTypeRefData>(rg => _resolverGroupTypeRefDataDeleted = rg);
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ResolverGroupTypeRefData>()))
                .Callback<ResolverGroupTypeRefData>(rg => _resolverGroupTypeRefDataCreated = rg);
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _resolverGroupTypeRefDatas.SingleOrDefault(x => x.Id == id));

            _mockOperationalProcessTypeRefDataService = new Mock<IOperationalProcessTypeRefDataService>();
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.All())
                .Returns(_operationalProcessTypeRefDatas.AsQueryable());
            _mockOperationalProcessTypeRefDataService
               .Setup(s => s.GetAllAndNotVisibleForCustomer(It.IsAny<int>()))
               .Returns(_operationalProcessTypeRefDatas.AsQueryable().OrderBy(x => x.SortOrder));
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Update(It.IsAny<OperationalProcessTypeRefData>()))
                .Callback<OperationalProcessTypeRefData>(rg => _operationalProcessTypeRefDataUpdated = rg);
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Delete(It.IsAny<OperationalProcessTypeRefData>()))
                .Callback<OperationalProcessTypeRefData>(rg => _operationalProcessTypeRefDataDeleted = rg);
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Create(It.IsAny<OperationalProcessTypeRefData>()))
                .Callback<OperationalProcessTypeRefData>(rg => _operationalProcessTypeRefDataCreated = rg);
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _operationalProcessTypeRefDatas.SingleOrDefault(x => x.Id == id));

            _controller = new ReferenceDataController(_domainTypeRefDataService,
                _functionTypeRefDataService,
                _inputTypeRefDataService,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _serviceDeliveryUnitTypeRefDataService,
                _resolverGroupTypeRefDataService,
                _operationalProcessTypeRefDataService,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _mockDomainTypeRefDataService = new Mock<IDomainTypeRefDataService>();
            _mockDomainTypeRefDataService
               .Setup(s => s.All())
               .Returns(_domainTypeRefData.AsQueryable());
            _mockDomainTypeRefDataService.Setup(s => s.GetById(2)).Returns(_domainTypeRefData[1]);

            _mockFunctionTypeRefDataService = new Mock<IFunctionTypeRefDataService>();
            _mockFunctionTypeRefDataService
               .Setup(s => s.All())
               .Returns(_functionTypeRefData.AsQueryable());
            _mockFunctionTypeRefDataService.Setup(s => s.GetById(2)).Returns(_functionTypeRefData[1]);

            _mockInputTypeRefDataService = new Mock<IInputTypeRefDataService>();
            _mockInputTypeRefDataService
               .Setup(s => s.All())
               .Returns(_inputTypeRefData.AsQueryable());
            _mockInputTypeRefDataService.Setup(s => s.GetById(2)).Returns(_inputTypeRefData[1]);

            _controllerWithMockedServices = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoDomainTypeService_ThrowsException()
        {
            new ReferenceDataController(
                null,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoFunctionTypeService_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                null,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoInputTypeService_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                null,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoSerivceDeliveryOraganisationTypeRefDataService_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                null,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoServiceDeliveryUnitTypeRefDataService_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                null,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoResolverGroupTypeRefDataService_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                null,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoOperationalProcessTypeRefDataService_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                null,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void
            ReferenceDataController_Constructor_NoResolverTypeRefDataService_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoContextManager_ThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                null,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoAppUserContextManagerThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                null,
                _mockLoggingManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReferenceDataController_Constructor_NoLoggingManagerThrowsException()
        {
            new ReferenceDataController(
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ReferenceDataController_Index_Get_RedirectsToLandingMenuItem()
        {
            #region Arrange

            const string expectedAction = "InputTypes";

            #endregion

            #region Act

            var result = _controller.Index() as RedirectToRouteResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);

            #endregion
        }

        #region Domain Types

        [TestMethod]
        public void ReferenceDataController_Domains_Get_ReturnsViewResult()
        {
            var result = _controller.Domains() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxDomainRefDataGrid_ReturnsReferenceData()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _controller.ReadAjaxDomainRefDataGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<DomainTypeRefDataViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(_domainTypeRefData.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxDomainRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            _mockDomainTypeRefDataService.Setup(s => s.GetDomainTypeRefData(It.IsAny<bool>(), null)).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.ReadAjaxDomainRefDataGrid(request);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxDomainRefGrid_CallsDomainTypeRefDataServiceCreate()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new DomainTypeRefDataViewModel()
            {
                Id = 5,
                DomainName = "Domain E",
                SortOrder = 5,
                Visible = true,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxDomainRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockDomainTypeRefDataService.Verify(x => x.Create(It.IsAny<DomainTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxDomainRefGrid_CallsRepositoryInsertAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new DomainTypeRefDataViewModel()
            {
                Id = 5,
                DomainName = "Domain E",
                SortOrder = 5,
                Visible = true,
            };

            #endregion

            #region Act

            _controller.CreateAjaxDomainRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<DomainTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxDomainRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new DomainTypeRefDataViewModel()
            {
                Id = 5,
                DomainName = "Domain E",
                SortOrder = 5,
                Visible = true,
            };

            _mockDomainTypeRefDataService.Setup(s => s.Create(It.IsAny<DomainTypeRefData>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxDomainRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_DuplicateNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            var expectedErrorMsg = string.Format(WebResources.EntityNameIsNotUnique, "Domain Type");

            var insert = new DomainTypeRefDataViewModel
            {
                DomainName = "Domain A",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxDomainRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedErrorMsg), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxDomainRefDataGrid_Update_CallsDomainTypeRefDataServiceUpdate()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new DomainTypeRefDataViewModel()
            {
                Id = 2,
                DomainName = "Domain BB",
            };

            #endregion

            #region Act

            _controllerWithMockedServices.UpdateAjaxDomainRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockDomainTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxDomainRefDataGrid_CallsRepositoryUpdateAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new DomainTypeRefDataViewModel
            {
                Id = 2,
                DomainName = "Domain BB"
            };

            #endregion

            #region Act

            _controller.UpdateAjaxDomainRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxDomainRefDataGrid_DeletedInputTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new DomainTypeRefDataViewModel
            {
                Id = 99
            };

            #endregion

            #region Act

            _controller.UpdateAjaxDomainRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxDomainRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new DomainTypeRefDataViewModel
            {
                Id = 99
            };

            _mockDomainTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.UpdateAjaxDomainRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_CallsDomainTypeRefDataServiceDelete()
        {
            #region Arrange

            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(2)).Returns(0);

            var request = new DataSourceRequest();

            var delete = new DomainTypeRefDataViewModel
            {
                Id = 2
            };

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxDomainRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockDomainTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetNumberOfDomainTypeReferences(It.IsAny<int>()), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.Delete(It.IsAny<DomainTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_ReferencedDomainTypeRefDataDoesNotCallServiceDelete()
        {
            #region Arrange

            _mockDomainTypeRefDataService.Setup(s => s.GetNumberOfDomainTypeReferences(2)).Returns(1);

            var request = new DataSourceRequest();

            var delete = new DomainTypeRefDataViewModel
            {
                Id = 2
            };

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxDomainRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockDomainTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.GetNumberOfDomainTypeReferences(It.IsAny<int>()), Times.Once);
            _mockDomainTypeRefDataService.Verify(x => x.Delete(It.IsAny<DomainTypeRefData>()), Times.Never);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_CallsRepositoryDeleteAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new DomainTypeRefDataViewModel
            {
                Id = 4
            };

            #endregion

            #region Act

            _controller.DeleteAjaxDomainRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<DomainTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_DeletedDomainTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new DomainTypeRefDataViewModel
            {
                Id = 99,
            };

            var expectedMessage = string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Domain Type");

            #endregion

            #region Act

            _controller.DeleteAjaxDomainRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedMessage), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_ReferencedDomainTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new DomainTypeRefDataViewModel
            {
                Id = 1
            };

            var expectedMessage = string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Domain Type");


            #endregion

            #region Act

            _controller.DeleteAjaxDomainRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedMessage), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new DomainTypeRefDataViewModel
            {
                Id = 99,
            };

            _mockDomainTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxDomainRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_GetDomainTypes_ReturnsListOfDomainTypeRefData()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetDomainTypes() as JsonResult;
            var domainTypes = result.Data as List<DomainTypeRefData>;

            #endregion

            #region Assert

            Assert.IsNotNull(domainTypes);
            Assert.IsInstanceOfType(domainTypes, typeof(List<DomainTypeRefData>));

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_GetDomainTypes_ReturnsCustomersWithDefaultDropDown()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetDomainTypes() as JsonResult;
            var domainTypes = result.Data as List<DomainTypeRefData>;

            #endregion

            #region Assert

            Assert.IsNotNull(domainTypes);
            Assert.IsTrue(domainTypes.Any(x => x.DomainName == WebResources.DefaultDropDownListText));
            Assert.AreEqual(domainTypes.Count, _domainTypeRefData.Count + 1);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleDomainTypesForCustomer_CustomerNotInAppContext_ReturnsEmptyResult()
        {
            var result = _controller.GetAllAndNotVisibleDomainTypesForCustomer();
            var domainTypes = result.Data as List<DomainTypeRefData>;
            Assert.IsNotNull(domainTypes);
            Assert.AreEqual(0, domainTypes.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleDomainTypesForCustomer_CustomerInAppContext_CorrectServiceMethodIsCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleDomainTypesForCustomer();

            _mockDomainTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(_appContextWithCustomer.CurrentCustomer.Id), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleeDomainTypesForCustomer_CustomerNotInAppContext_DoesNotCallServiceMethod()
        {
            _appContext = new AppContext();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleDomainTypesForCustomer();

            _mockDomainTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleDomainTypesForCustomer_CustomerInAppContext_ReturnsResults()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _mockDomainTypeRefDataService
                .Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId))
                .Returns(new List<DomainTypeRefData>
                {
                    UnitTestHelper.GenerateRandomData<DomainTypeRefData>(),
                    UnitTestHelper.GenerateRandomData<DomainTypeRefData>()
                });
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
            var result = _controller.GetAllAndNotVisibleDomainTypesForCustomer();
            var domainTypes = result.Data as List<DomainTypeRefData>;
            Assert.IsNotNull(domainTypes);
            Assert.AreEqual(2, domainTypes.Count);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_ViewIsCorrect_ReturnsPartialView()
        {
            var result = _controller.CreateDomainType() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_ViewIsCorrect_ReturnsPartialDomainType()
        {
            var result = _controller.CreateDomainType() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_DomainType", result.ViewName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_DuplicateNameDoesNotStatusCodeSetTo500()
        {
            var entity = new DomainTypeRefDataViewModel
            {
                DomainName = "Domain A",
                SortOrder = 5
            };

            _controllerWithMockedServices.CreateDomainType(entity);

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_DuplicateNameReturnsCorrectDomainType()
        {
            var entity = new DomainTypeRefDataViewModel
            {
                DomainName = "Domain A",
                SortOrder = 5
            };

            var result = _controllerWithMockedServices.CreateDomainType(entity) as JsonResult;

            var kendoData = result.Data as DomainTypeRefDataViewModel;
            Assert.AreEqual(1, kendoData.Id);
            Assert.AreEqual(entity.DomainName, kendoData.DomainName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_Get_ReturnsDomainType()
        {
            var result = _controller.CreateDomainType() as PartialViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(DomainTypeRefDataViewModel));
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<DomainTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateDomainType(entity);
            _mockDomainTypeRefDataService.Verify(v => v.Create(It.IsAny<DomainTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<DomainTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateDomainType(entity);
            _mockDomainTypeRefDataService.Verify(v => v.Create(It.IsAny<DomainTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<DomainTypeRefDataViewModel>();
            _mockDomainTypeRefDataService
                .Setup(s => s.Create(It.IsAny<DomainTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateDomainType(entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateDomainType_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<DomainTypeRefDataViewModel>();
            _mockDomainTypeRefDataService
                .Setup(s => s.Create(It.IsAny<DomainTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateDomainType(entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region Function Types

        [TestMethod]
        public void ReferenceDataController_Functions_Get_ReturnsViewResult()
        {
            var result = _controller.Functions() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxFunctionRefDataGrid_ReturnsReferenceData()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _controller.ReadAjaxFunctionRefDataGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<FunctionTypeRefDataViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(_functionTypeRefData.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxFunctionRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            _mockFunctionTypeRefDataService.Setup(s => s.GetFunctionTypeRefData(It.IsAny<bool>(), null)).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.ReadAjaxFunctionRefDataGrid(request);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxFunctionRefDataGrid_CallsFunctionTypeRefDataServiceCreate()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new FunctionTypeRefDataViewModel()
            {
                Id = 5,
                FunctionName = "Function E",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxFunctionRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockFunctionTypeRefDataService.Verify(x => x.Create(It.IsAny<FunctionTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxFunctionRefDataGrid_CallsRepositoryInsertAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new FunctionTypeRefDataViewModel()
            {
                Id = 5,
                FunctionName = "Function E",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controller.CreateAjaxFunctionRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockFunctionTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<FunctionTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxFunctionRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new FunctionTypeRefDataViewModel()
            {
                Id = 5,
                FunctionName = "Function E",
                SortOrder = 5,
            };

            _mockFunctionTypeRefDataService.Setup(s => s.Create(It.IsAny<FunctionTypeRefData>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxFunctionRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxFunctionRefDataGrid_DuplicateNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            var expectedErrorMsg = string.Format(WebResources.EntityNameIsNotUnique, "Function Type");

            var insert = new FunctionTypeRefDataViewModel
            {
                FunctionName = "Function A",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxFunctionRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedErrorMsg), Times.Once);

            #endregion
        }


        [TestMethod]
        public void ReferenceDataController_UpdateAjaxFunctionRefDataGrid_Update_CallsFunctionTypeRefDataServiceUpdate()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new FunctionTypeRefDataViewModel()
            {
                Id = 2,
                FunctionName = "Function BB",
            };

            #endregion

            #region Act

            _controllerWithMockedServices.UpdateAjaxFunctionRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockFunctionTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxFunctionRefDataGrid_CallsRepositoryUpdateAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new FunctionTypeRefDataViewModel()
            {
                Id = 2,
                FunctionName = "Function BB",
            };

            #endregion

            #region Act

            _controller.UpdateAjaxFunctionRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockFunctionTypeRefDataRepository.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxFunctionRefDataGrid_DeletedInputTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new FunctionTypeRefDataViewModel
            {
                Id = 99,
            };

            #endregion

            #region Act

            _controller.UpdateAjaxFunctionRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxFunctionRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new FunctionTypeRefDataViewModel
            {
                Id = 99,
            };

            _mockFunctionTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.UpdateAjaxFunctionRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxFunctionRefDataGrid_CallsFunctionTypeRefDataServiceDelete()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new FunctionTypeRefDataViewModel
            {
                Id = 2
            };

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxFunctionRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockFunctionTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetNumberOfFunctionTypeReferences(It.IsAny<int>()), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.Delete(It.IsAny<FunctionTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxFunctionRefDataGrid_FunctionTypeRefDataServiceDoesNotCallDelete()
        {
            #region Arrange

            _mockFunctionTypeRefDataService.Setup(s => s.GetNumberOfFunctionTypeReferences(2)).Returns(1);

            var request = new DataSourceRequest();

            var delete = new FunctionTypeRefDataViewModel
            {
                Id = 2
            };

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxFunctionRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockFunctionTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.GetNumberOfFunctionTypeReferences(It.IsAny<int>()), Times.Once);
            _mockFunctionTypeRefDataService.Verify(x => x.Delete(It.IsAny<FunctionTypeRefData>()), Times.Never);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxFunctionRefDataGrid_CallsRepositoryDeleteAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new FunctionTypeRefDataViewModel
            {
                Id = 4
            };

            #endregion

            #region Act

            _controller.DeleteAjaxFunctionRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockFunctionTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<FunctionTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxFunctionRefDataGrid_DeletedDomainTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new FunctionTypeRefDataViewModel
            {
                Id = 99,
            };

            var expectedMessage = string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Function Type");

            #endregion

            #region Act

            _controller.DeleteAjaxFunctionRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedMessage), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxFunctionRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new FunctionTypeRefDataViewModel
            {
                Id = 99,
            };

            _mockFunctionTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxFunctionRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxFunctionRefDataGrid_ReferencedFunctionTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new FunctionTypeRefDataViewModel
            {
                Id = 1
            };

            var expectedMessage = string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Function Type");

            _mockServiceFunctionRepository.Setup(s => s.Any(It.IsAny<Expression<Func<ServiceFunction, bool>>>())).Returns(true);

            #endregion

            #region Act

            _controller.DeleteAjaxFunctionRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedMessage), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_GetFunctionTypes_ReturnsListOfFunctionTypeRefData()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetFunctionTypes();
            var functionTypes = result.Data as List<FunctionTypeRefData>;

            #endregion

            #region Assert

            Assert.IsNotNull(functionTypes);
            Assert.IsInstanceOfType(functionTypes, typeof(List<FunctionTypeRefData>));

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_GetFunctionTypes_ReturnsCustomersWithDefaultDropDown()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetFunctionTypes();
            var functionTypes = result.Data as List<FunctionTypeRefData>;

            #endregion

            #region Assert

            Assert.IsNotNull(functionTypes);
            Assert.IsTrue(functionTypes.Any(x => x.FunctionName == WebResources.DefaultDropDownListText));
            Assert.AreEqual(functionTypes.Count, _functionTypeRefData.Count + 1);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_ViewIsCorrect_ReturnsPartialView()
        {
            var result = _controller.CreateFunctionType() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_ViewIsCorrect_ReturnsPartialFunctionType()
        {
            var result = _controller.CreateFunctionType() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_FunctionType", result.ViewName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_DuplicateNameDoesNotStatusCodeSetTo500()
        {
            var entity = new FunctionTypeRefDataViewModel
            {
                FunctionName = "Function A",
                SortOrder = 5
            };

            _controllerWithMockedServices.CreateFunctionType(entity);

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_DuplicateNameReturnsCorrectFunctionType()
        {
            var entity = new FunctionTypeRefDataViewModel
            {
                FunctionName = "Function A",
                SortOrder = 5
            };

            var result = _controllerWithMockedServices.CreateFunctionType(entity) as JsonResult;

            var kendoData = result.Data as FunctionTypeRefDataViewModel;
            Assert.AreEqual(1, kendoData.Id);
            Assert.AreEqual(entity.FunctionName, kendoData.FunctionName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_Get_ReturnsFunctionType()
        {
            var result = _controller.CreateFunctionType() as PartialViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(FunctionTypeRefDataViewModel));
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<FunctionTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateFunctionType(entity);
            _mockFunctionTypeRefDataService.Verify(v => v.Create(It.IsAny<FunctionTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<FunctionTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateFunctionType(entity);
            _mockFunctionTypeRefDataService.Verify(v => v.Create(It.IsAny<FunctionTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<FunctionTypeRefDataViewModel>();
            _mockFunctionTypeRefDataService
                .Setup(s => s.Create(It.IsAny<FunctionTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateFunctionType(entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateFunctionType_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<FunctionTypeRefDataViewModel>();
            _mockFunctionTypeRefDataService
                .Setup(s => s.Create(It.IsAny<FunctionTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateFunctionType(entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleFunctionTypesForCustomer_CustomerNotInAppContext_ReturnsEmptyResult()
        {
            var result = _controller.GetAllAndNotVisibleFunctionTypesForCustomer();
            var functionTypeRefDatas = result.Data as List<FunctionTypeRefData>;
            Assert.IsNotNull(functionTypeRefDatas);
            Assert.AreEqual(0, functionTypeRefDatas.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleFunctionTypesForCustomer_CustomerInAppContext_CorrectServiceMethodIsCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);

            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _inputTypeRefDataService,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleFunctionTypesForCustomer();

            _mockFunctionTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(_appContextWithCustomer.CurrentCustomer.Id), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleeFunctionTypesForCustomer_CustomerNotInAppContext_DoesNotCallServiceMethod()
        {
            _appContext = new AppContext();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleFunctionTypesForCustomer();

            _mockFunctionTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleFunctionTypesForCustomer_CustomerInAppContext_ReturnsResults()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _mockFunctionTypeRefDataService
                .Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId))
                .Returns(new List<FunctionTypeRefData>
                {
                    UnitTestHelper.GenerateRandomData<FunctionTypeRefData>(),
                    UnitTestHelper.GenerateRandomData<FunctionTypeRefData>()
                });
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);
            var result = _controller.GetAllAndNotVisibleFunctionTypesForCustomer();
            var functionTypes = result.Data as List<FunctionTypeRefData>;
            Assert.IsNotNull(functionTypes);
            Assert.AreEqual(2, functionTypes.Count);
        }

        #endregion

        #region Input Types

        [TestMethod]
        public void ReferenceDataController_InputTypes_Get_ReturnsViewResult()
        {
            var result = _controller.InputTypes() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxInputTypeRefDataGrid_ReturnsInputTypesReferenceData()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _controller.ReadAjaxInputTypeRefDataGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<InputTypeRefDataViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(_inputTypeRefData.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxInputTypeRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            _mockInputTypeRefDataService.Setup(s => s.GetInputTypeRefDataWithUsageStats()).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.ReadAjaxInputTypeRefDataGrid(request);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxInputTypeRefDataGrid_CallsInputTypeRefDataServiceCreate()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new InputTypeRefDataViewModel()
            {
                Id = 5,
                InputTypeName = "Input E",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxInputTypeRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockInputTypeRefDataService.Verify(x => x.Create(It.IsAny<InputTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_CallsInputTypeRefDataServiceCreate()
        {
            #region Arrange


            var insert = new InputTypeRefDataViewModel()
            {
                Id = 5,
                InputTypeName = "Input E",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateInputType(insert);

            #endregion

            #region Assert

            _mockInputTypeRefDataService.Verify(x => x.Create(It.IsAny<InputTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxInputTypeRefDataGrid_CallsRepositoryInsertAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new InputTypeRefDataViewModel()
            {
                Id = 5,
                InputTypeName = "Input E",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controller.CreateAjaxInputTypeRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<InputTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputTypeRef_CallsRepositoryInsertAndUnitOfWork()
        {
            #region Arrange

            var insert = new InputTypeRefDataViewModel()
            {
                Id = 5,
                InputTypeName = "Input E",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controller.CreateInputType(insert);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<InputTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxInputTypeRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new InputTypeRefDataViewModel()
            {
                Id = 5,
                InputTypeNumber = 5,
                InputTypeName = "Input Type E",
                SortOrder = 5,
            };

            _mockInputTypeRefDataService.Setup(s => s.Create(It.IsAny<InputTypeRefData>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxInputTypeRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxInputTypeRefDataGrid_DuplicateNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            var expectedErrorMsg = string.Format(WebResources.EntityNameIsNotUnique, "Input Type");

            var insert = new InputTypeRefDataViewModel
            {
                InputTypeNumber = 1,
                InputTypeName = "Input A",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxInputTypeRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedErrorMsg), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxInputTypeRefDataGrid_Update_CallsInputTypeRefDataServiceUpdate()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new InputTypeRefDataViewModel()
            {
                Id = 2,
                InputTypeName = "Input BB",
            };

            #endregion

            #region Act

            _controllerWithMockedServices.UpdateAjaxInputTypeRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockInputTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockInputTypeRefDataService.Verify(x => x.Update(It.IsAny<InputTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxInputTypeRefDataGrid_CallsRepositoryUpdateAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new InputTypeRefDataViewModel
            {
                Id = 2,
                InputTypeName = "Input BB",
            };

            #endregion

            #region Act

            _controller.UpdateAjaxInputTypeRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.Update(It.IsAny<InputTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxInputTypeRefDataGrid_DeletedInputTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new InputTypeRefDataViewModel
            {
                Id = 99,
            };

            #endregion

            #region Act

            _controller.UpdateAjaxInputTypeRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxCustomerGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new InputTypeRefDataViewModel
            {
                Id = 99,
            };

            _mockInputTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.UpdateAjaxInputTypeRefDataGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxInputTypeRefDataGrid_CallsInputTypeRefDataServiceDelete()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new InputTypeRefDataViewModel()
            {
                Id = 2,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxInputTypeRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockInputTypeRefDataService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockInputTypeRefDataService.Verify(x => x.IsInputTypeReferenced(It.IsAny<int>()), Times.Once);
            _mockInputTypeRefDataService.Verify(x => x.Delete(It.IsAny<InputTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxInputTypeRefDataGrid_CallsRepositoryDeleteAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new InputTypeRefDataViewModel
            {
                Id = 2,
            };

            #endregion

            #region Act

            _controller.DeleteAjaxInputTypeRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<InputTypeRefData>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxInputTypeRefDataGrid_DeletedDomainTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new InputTypeRefDataViewModel
            {
                Id = 99,
            };

            var expectedMessage = string.Format(WebResources.CannotFindReferenceDataItemToBeDeleted, "Input Type");

            #endregion

            #region Act

            _controller.DeleteAjaxInputTypeRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedMessage), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxInputTypeRefDataGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new InputTypeRefDataViewModel
            {
                Id = 99,
            };

            _mockInputTypeRefDataService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedServices.DeleteAjaxInputTypeRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxInputTypeRefDataGrid_ReferencedInputTypeAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new InputTypeRefDataViewModel
            {
                Id = 1,
            };

            var expectedMessage = string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Input Type");

            _mockDeskInputTypeRepository.Setup(s => s.Any(It.IsAny<Expression<Func<DeskInputType, bool>>>())).Returns(true);

            #endregion

            #region Act

            _controller.DeleteAjaxInputTypeRefDataGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedMessage), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_GetInputTypes_ReturnsListOfInputTypeRefData()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetInputTypes();
            var inputTypeRefDataItems = result.Data as List<InputTypeRefData>;

            #endregion

            #region Assert

            Assert.IsNotNull(inputTypeRefDataItems);
            Assert.IsInstanceOfType(inputTypeRefDataItems, typeof(List<InputTypeRefData>));
            Assert.AreEqual(_inputTypeRefData.Count, inputTypeRefDataItems.Count);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_Get_Returns_InputTypePartialViewResult()
        {
            var result = _controller.CreateInputType() as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("_InputType", result.ViewName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_Get_ReturnsTypeOfInputTypeRefDataViewModel()
        {
            var result = _controller.CreateInputType() as PartialViewResult;

            Assert.IsInstanceOfType(result.Model, typeof(InputTypeRefDataViewModel));
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_Get_ReturnsInputTypeType()
        {
            var result = _controller.CreateInputType() as PartialViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(InputTypeRefDataViewModel));
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<InputTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateInputType(entity);
            _mockInputTypeRefDataService.Verify(v => v.Create(It.IsAny<InputTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<InputTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateInputType(entity);
            _mockInputTypeRefDataService.Verify(v => v.Create(It.IsAny<InputTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<InputTypeRefDataViewModel>();
            _mockInputTypeRefDataService
                .Setup(s => s.Create(It.IsAny<InputTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateInputType(entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<InputTypeRefDataViewModel>();
            _mockInputTypeRefDataService
                .Setup(s => s.Create(It.IsAny<InputTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateInputType(entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region Service Delivery Organisation Types

        [TestMethod]
        public void ReferenceDataController_ServiceDeliveryOrganisationTypes_Get_ReturnsViewResult()
        {
            var result = _controller.ServiceDeliveryOrganisationTypes() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_GetServiceDeliveryOrganisationTypes_Get_ReturnsAllTypesAddsAdditionalOneForDefaultDropDownList()
        {
            var result = _controller.GetServiceDeliveryOrganisationTypes();
            var data = result.Data as List<ServiceDeliveryOrganisationTypeRefData>;
            Assert.AreEqual(_serviceDeliveryOrganisationTypeRefDatas.Count, data.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetServiceDeliveryOrganisationTypes_Get_DataIsInSortOrder()
        {
            var expected = _serviceDeliveryOrganisationTypeRefDatas
                .OrderBy(x => x.SortOrder)
                .Select(s => s.Id)
                .ToList();
            var result = _controller.GetServiceDeliveryOrganisationTypes();
            var data = result.Data as List<ServiceDeliveryOrganisationTypeRefData>;
            var actual = data
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxServiceDeliveryOrganisationTypesRefDataGrid_ReturnsReferenceData_CorrectNumberOfRecord()
        {
            var result = _controller.ReadAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest()) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceDeliveryOrganisationTypeRefDataViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(_serviceDeliveryOrganisationTypeRefDatas.Count, model.Count);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxServiceDeliveryOrganisationTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            _mockServiceDeliveryOrganisationTypeRefDataService.Setup(s => s.All()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxServiceDeliveryOrganisationTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            _mockServiceDeliveryOrganisationTypeRefDataService.Setup(s => s.All()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryOrganisationTypeRefDataService.Verify(v => v.Create(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryOrganisationTypeRefDataService.Verify(v => v.Create(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_IdIsZero()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(0, _serviceDeliveryOrganisationTypeRefDataCreated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_ServiceDeliveryOrganisationTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.ServiceDeliveryOrganisationTypeName, _serviceDeliveryOrganisationTypeRefDataCreated.ServiceDeliveryOrganisationTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _serviceDeliveryOrganisationTypeRefDataCreated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>();
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>();
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid_DuplicateNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            var expectedErrorMsg = string.Format(WebResources.EntityNameIsNotUnique, "Service Delivery Organisation Type");

            var insert = new ServiceDeliveryOrganisationTypeRefDataViewModel
            {
                ServiceDeliveryOrganisationTypeName = "Service Delivery Organisation One",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedErrorMsg), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryOrganisationTypeRefDataService.Verify(v => v.Update(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryOrganisationTypeRefDataService.Verify(v => v.Update(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_IdSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.Id, _serviceDeliveryOrganisationTypeRefDataUpdated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_ServiceDeliveryOrganisationTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.ServiceDeliveryOrganisationTypeName, _serviceDeliveryOrganisationTypeRefDataUpdated.ServiceDeliveryOrganisationTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _serviceDeliveryOrganisationTypeRefDataUpdated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid_ModelStateNotValid_NoDeleteTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryOrganisationTypeRefDataService.Verify(v => v.Delete(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid_GroupCannotBeFound_ModelStateErrorAdded()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdNotExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid_GroupIsBeingUsed_ModelStateErrorAdded()
        {
            _mockServiceDeliveryOrganisationTypeRefDataService
                .Setup(s => s.IsServiceDeliveryOrganisationTypeReferenced(It.IsAny<int>()))
                .Returns(true);
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ReferenceDataItemIsStillUtilised));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid_DeleteGroup_DeleteCalled()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryOrganisationTypeRefDataService.Verify(v => v.Delete(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid_DeleteGroup_DeleteCalledWithCorrectGroup()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryOrganisationTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(ServiceDeliveryOrganisationTypeRefDataIdExists, _serviceDeliveryOrganisationTypeRefDataDeleted.Id);
        }

        #endregion

        #region Resolver Group Types

        [TestMethod]
        public void ReferenceDataController_ResolverGroupTypes_Get_ReturnsViewResult()
        {
            var result = _controller.ResolverGroupTypes() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_GetResolverGroupTypes_Get_ReturnsAllTypesAddsAdditionalOneForDefaultDropDownList()
        {
            var result = _controller.GetResolverGroupTypes();
            var data = result.Data as List<ResolverGroupTypeRefData>;
            Assert.AreEqual(_resolverGroupTypeRefDatas.Count, data.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetResolverGroupTypes_Get_DataIsInSortOrder()
        {
            var expected = _resolverGroupTypeRefDatas
                .OrderBy(x => x.SortOrder)
                .Select(s => s.Id)
                .ToList();
            var result = _controller.GetResolverGroupTypes();
            var data = result.Data as List<ResolverGroupTypeRefData>;
            var actual = data
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxResolverGroupTypesRefDataGrid_ReturnsReferenceData_CorrectNumberOfRecord()
        {
            var result = _controller.ReadAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest()) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ResolverGroupTypeRefDataViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(_resolverGroupTypeRefDatas.Count, model.Count);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxResolverGroupTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetResolverGroupTypeRefDataWithUsageStats()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxResolverGroupTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            _mockResolverGroupTypeRefDataService.Setup(s => s.GetResolverGroupTypeRefDataWithUsageStats()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Create(It.IsAny<ResolverGroupTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Create(It.IsAny<ResolverGroupTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_IdIsZero()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(0, _resolverGroupTypeRefDataCreated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_ResolverGroupTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.ResolverGroupTypeName, _resolverGroupTypeRefDataCreated.ResolverGroupTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.Order, _resolverGroupTypeRefDataCreated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ResolverGroupTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ResolverGroupTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.UpdateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Update(It.IsAny<ResolverGroupTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Update(It.IsAny<ResolverGroupTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_IdSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.Id, _resolverGroupTypeRefDataUpdated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_ResolverGroupTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.ResolverGroupTypeName, _resolverGroupTypeRefDataUpdated.ResolverGroupTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.Order, _resolverGroupTypeRefDataUpdated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ResolverGroupTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ResolverGroupTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxResolverGroupTypesRefDataGrid_ModelStateNotValid_NoDeleteTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.DeleteAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Delete(It.IsAny<ResolverGroupTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxResolverGroupTypesRefDataGrid_GroupCannotBeFound_ModelStateErrorAdded()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdNotExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxResolverGroupTypesRefDataGrid_GroupIsBeingUsed_ModelStateErrorAdded()
        {
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.GetNumberOfResolverGroupTypeReferences(It.IsAny<int>()))
                .Returns(1);
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ReferenceDataItemIsStillUtilised));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxResolverGroupTypesRefDataGrid_DeleteGroup_DeleteCalled()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Delete(It.IsAny<ResolverGroupTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxResolverGroupTypesRefDataGrid_DeleteGroup_DeleteCalledWithCorrectGroup()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>(x =>
            {
                x.Id = ResolverGroupTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxResolverGroupTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(ResolverGroupTypeRefDataIdExists, _resolverGroupTypeRefDataDeleted.Id);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxResolverGroupTypesRefDataGrid_DuplicateNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            var expectedErrorMsg = string.Format(WebResources.EntityNameIsNotUnique, "Resolver Group Type");

            var insert = new ResolverGroupTypeRefDataViewModel
            {
                ResolverGroupTypeName = "Resolver Group One",
                Order = 5
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxResolverGroupTypesRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedErrorMsg), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleResolverGroupTypesForCustomer_CustomerNotInAppContext_ReturnsEmptyResult()
        {
            var result = _controller.GetAllAndNotVisibleResolverGroupTypesForCustomer();
            var resolverGroupTypes = result.Data as List<ResolverGroupTypeRefData>;
            Assert.IsNotNull(resolverGroupTypes);
            Assert.AreEqual(0, resolverGroupTypes.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleResolverGroupTypesForCustomer_CustomerInAppContext_CorrectServiceMethodIsCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleResolverGroupTypesForCustomer();

            _mockResolverGroupTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(_appContextWithCustomer.CurrentCustomer.Id), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleResolverGroupTypesForCustomer_CustomerInAppContext_ReturnsResults()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId))
                .Returns(new List<ResolverGroupTypeRefData>
                {
                    UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(),
                    UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>()
                });
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            var result = _controller.GetAllAndNotVisibleResolverGroupTypesForCustomer();

            var resolverGroupTypes = result.Data as List<ResolverGroupTypeRefData>;
            Assert.IsNotNull(resolverGroupTypes);
            Assert.AreEqual(2, resolverGroupTypes.Count);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_ViewIsCorrect_ReturnsPartialView()
        {
            var result = _controller.CreateResolverGroupType() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_ViewIsCorrect_ReturnsPartialFunctionType()
        {
            var result = _controller.CreateResolverGroupType() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_ResolverGroupType", result.ViewName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_DuplicateNameDoesNotStatusCodeSetTo500()
        {
            var entity = new ResolverGroupTypeRefDataViewModel
            {
                ResolverGroupTypeName = "Resolver Group One",
                Order = 5
            };

            _controllerWithMockedServices.CreateResolverGroupType(entity);

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_DuplicateNameReturnsCorrectOperationalProcessType()
        {
            var entity = new ResolverGroupTypeRefDataViewModel
            {
                ResolverGroupTypeName = "Resolver Group One",
                Order = 5
            };

            var result = _controllerWithMockedServices.CreateResolverGroupType(entity) as JsonResult;

            var kendoData = result.Data as ResolverGroupTypeRefDataViewModel;
            Assert.AreEqual(1, kendoData.Id);
            Assert.AreEqual(entity.ResolverGroupTypeName, kendoData.ResolverGroupTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_Get_ReturnsResolverGroupType()
        {
            var result = _controller.CreateResolverGroupType() as PartialViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(ResolverGroupTypeRefDataViewModel));
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateResolverGroupType(entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Create(It.IsAny<ResolverGroupTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateResolverGroupType(entity);
            _mockResolverGroupTypeRefDataService.Verify(v => v.Create(It.IsAny<ResolverGroupTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ResolverGroupTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateResolverGroupType(entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateResolverGroupType_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefDataViewModel>();
            _mockResolverGroupTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ResolverGroupTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateResolverGroupType(entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region Service Delivery Unit Types

        [TestMethod]
        public void ReferenceDataController_ServiceDeliveryUnitTypes_Get_ReturnsViewResult()
        {
            var result = _controller.ServiceDeliveryUnitTypes() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_GetServiceDeliveryUnitTypes_Get_ReturnsAllTypesAddsAdditionalOneForDefaultDropDownList()
        {
            var result = _controller.GetServiceDeliveryUnitTypes();
            var data = result.Data as List<ServiceDeliveryUnitTypeRefData>;
            Assert.AreEqual(_serviceDeliveryUnitTypeRefDatas.Count, data.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetServiceDeliveryUnitTypes_Get_DataIsInSortOrder()
        {
            var expected = _serviceDeliveryUnitTypeRefDatas
                .OrderBy(x => x.SortOrder)
                .Select(s => s.Id)
                .ToList();
            var result = _controller.GetServiceDeliveryUnitTypes();
            var data = result.Data as List<ServiceDeliveryUnitTypeRefData>;
            var actual = data
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxServiceDeliveryUnitTypesRefDataGrid_ReturnsReferenceData_CorrectNumberOfRecord()
        {
            var result = _controller.ReadAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest()) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ServiceDeliveryUnitTypeRefDataViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(_serviceDeliveryUnitTypeRefDatas.Count, model.Count);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxServiceDeliveryUnitTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            _mockServiceDeliveryUnitTypeRefDataService.Setup(s => s.GetServiceDeliveryUnitTypeRefDataWithUsageStats()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxServiceDeliveryUnitTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            _mockServiceDeliveryUnitTypeRefDataService.Setup(s => s.GetServiceDeliveryUnitTypeRefDataWithUsageStats()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.Create(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.Create(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_IdIsZero()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(0, _serviceDeliveryUnitTypeRefDataCreated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_ServiceDeliveryUnitTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.ServiceDeliveryUnitTypeName, _serviceDeliveryUnitTypeRefDataCreated.ServiceDeliveryUnitTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _serviceDeliveryUnitTypeRefDataCreated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_DuplicateNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            var expectedErrorMsg = string.Format(WebResources.EntityNameIsNotUnique, "Service Delivery Unit Type");

            var insert = new ServiceDeliveryUnitTypeRefDataViewModel
            {
                ServiceDeliveryUnitTypeName = "Service Delivery Unit One",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedErrorMsg), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxServiceDeliveryUnitTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.Update(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.Update(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_IdSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.Id, _serviceDeliveryUnitTypeRefDataUpdated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_ServiceDeliveryUnitTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.ServiceDeliveryUnitTypeName, _serviceDeliveryUnitTypeRefDataUpdated.ServiceDeliveryUnitTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _serviceDeliveryUnitTypeRefDataUpdated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryUnitTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxServiceDeliveryUnitTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Update(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryUnitTypesRefDataGrid_ModelStateNotValid_NoDeleteTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.DeleteAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.Delete(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryUnitTypesRefDataGrid_GroupCannotBeFound_ModelStateErrorAdded()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdNotExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryUnitTypesRefDataGrid_GroupIsBeingUsed_ModelStateErrorAdded()
        {
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.IsServiceDeliveryUnitTypeReferenced(It.IsAny<int>()))
                .Returns(true);
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ReferenceDataItemIsStillUtilised));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryUnitTypesRefDataGrid_DeleteGroup_DeleteCalled()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.Delete(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxServiceDeliveryUnitTypesRefDataGrid_DeleteGroup_DeleteCalledWithCorrectGroup()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>(x =>
            {
                x.Id = ServiceDeliveryUnitTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxServiceDeliveryUnitTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(ServiceDeliveryUnitTypeRefDataIdExists, _serviceDeliveryUnitTypeRefDataDeleted.Id);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer_CustomerNotInAppContext_ReturnsEmptyResult()
        {
            var result = _controller.GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer();
            var resolverGroupTypes = result.Data as List<ServiceDeliveryUnitTypeRefData>;
            Assert.IsNotNull(resolverGroupTypes);
            Assert.AreEqual(0, resolverGroupTypes.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer_CustomerInAppContext_CorrectServiceMethodIsCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer();

            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(_appContextWithCustomer.CurrentCustomer.Id), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer_CustomerNotInAppContext_DoesNotCallServiceMethod()
        {
            _appContext = new AppContext();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer();

            _mockServiceDeliveryUnitTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer_CustomerInAppContext_ReturnsResults()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId))
                .Returns(new List<ServiceDeliveryUnitTypeRefData>
                {
                    UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                    UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>()
                });
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            var result = _controller.GetAllAndNotVisibleServiceDeliveryUnitTypesForCustomer();

            var serviceDeliveryUnitTypes = result.Data as List<ServiceDeliveryUnitTypeRefData>;
            Assert.IsNotNull(serviceDeliveryUnitTypes);
            Assert.AreEqual(2, serviceDeliveryUnitTypes.Count);
        }

        [TestMethod]
        public void ReferenceDataController_CreateServiceDeliveryUnitType_ViewIsCorrect_ReturnsPartialView()
        {
            var result = _controller.CreateServiceDeliveryUnitType() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_CreateServiceDeliveryUnitType_ViewIsCorrect_ReturnsPartialFunctionType()
        {
            var result = _controller.CreateServiceDeliveryUnitType() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_ServiceDeliveryUnitType", result.ViewName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateServiceDeliveryUnitType_DuplicateNameDoesNotStatusCodeSetTo500()
        {
            var entity = new ServiceDeliveryUnitTypeRefDataViewModel
            {
                ServiceDeliveryUnitTypeName = "Service Delivery Unit One",
                SortOrder = 5
            };

            _controllerWithMockedServices.CreateServiceDeliveryUnitType(entity);

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateServiceDeliveryUnitType_DuplicateNameReturnsCorrectOperationalProcessType()
        {
            var entity = new ServiceDeliveryUnitTypeRefDataViewModel
            {
                ServiceDeliveryUnitTypeName = "Service Delivery Unit One",
                SortOrder = 5
            };

            var result = _controllerWithMockedServices.CreateServiceDeliveryUnitType(entity) as JsonResult;

            var kendoData = result.Data as ServiceDeliveryUnitTypeRefDataViewModel;
            Assert.AreEqual(1, kendoData.Id);
            Assert.AreEqual(entity.ServiceDeliveryUnitTypeName, kendoData.ServiceDeliveryUnitTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateServiceDeliveryUnitType_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateServiceDeliveryUnitType(entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateServiceDeliveryUnitType_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefDataViewModel>();
            _mockServiceDeliveryUnitTypeRefDataService
                .Setup(s => s.Create(It.IsAny<ServiceDeliveryUnitTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateServiceDeliveryUnitType(entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region Operational Process Types

        [TestMethod]
        public void ReferenceDataController_OperationalProcessTypes_Get_ReturnsViewResult()
        {
            var result = _controller.OperationalProcessTypes() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_Get_ReturnsPartialViewResult()
        {
            var result = _controller.CreateOperationalProcessType() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_Get_ReturnsOperationalProcessType()
        {
            var result = _controller.CreateOperationalProcessType() as PartialViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(OperationalProcessTypeRefDataViewModel));
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateOperationalProcessType(entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Create(It.IsAny<OperationalProcessTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateOperationalProcessType(entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Create(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Create(It.IsAny<OperationalProcessTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateOperationalProcessType(entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Create(It.IsAny<OperationalProcessTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateOperationalProcessType(entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_ViewIsCorrect_ReturnsPartialView()
        {
            var result = _controller.CreateOperationalProcessType() as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_ViewIsCorrect_ReturnsPartialFunctionType()
        {
            var result = _controller.CreateOperationalProcessType() as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("_OperationalProcessType", result.ViewName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_DuplicateNameDoesNotStatusCodeSetTo500()
        {
            var entity = new OperationalProcessTypeRefDataViewModel
            {
                OperationalProcessTypeName = "Operational Process Type One",
                SortOrder = 5
            };

            _controllerWithMockedServices.CreateOperationalProcessType(entity);

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateOperationalProcessType_DuplicateNameReturnsCorrectOperationalProcessType()
        {
            var entity = new OperationalProcessTypeRefDataViewModel
            {
                OperationalProcessTypeName = "Operational Process Type One",
                SortOrder = 5
            };

            var result = _controllerWithMockedServices.CreateOperationalProcessType(entity) as JsonResult;

            var kendoData = result.Data as OperationalProcessTypeRefDataViewModel;
            Assert.AreEqual(1, kendoData.Id);
            Assert.AreEqual(entity.OperationalProcessTypeName, kendoData.OperationalProcessTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_OperationalProcessTypes_Get_ReturnsAllTypes()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            var result = _controllerWithMockedServices.GetAllAndNotVisibleOperationalProcessTypesForCustomer();
            var data = result.Data as List<OperationalProcessTypeRefData>;
            Assert.AreEqual(_serviceDeliveryUnitTypeRefDatas.Count, data.Count);
        }

        [TestMethod]
        public void ReferenceDataController_OperationalProcessTypes_Get_DataIsInSortOrder()
        {
            var expected = _operationalProcessTypeRefDatas
                .OrderBy(x => x.SortOrder)
                .Select(s => s.Id)
                .ToList();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            var result = _controllerWithMockedServices.GetAllAndNotVisibleOperationalProcessTypesForCustomer();
            var data = result.Data as List<OperationalProcessTypeRefData>;
            var actual = data
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxOperationalProcessTypesRefDataGrid_ReturnsReferenceData_CorrectNumberOfRecord()
        {
            var result = _controller.ReadAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest()) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<OperationalProcessTypeRefDataViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(_operationalProcessTypeRefDatas.Count, model.Count);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxOperationalProcessTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetOperationalProcessTypeRefDataWithUsageStats()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_ReadAjaxOperationalProcessTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            _mockOperationalProcessTypeRefDataService.Setup(s => s.GetOperationalProcessTypeRefDataWithUsageStats()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Create(It.IsAny<OperationalProcessTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Create(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_IdIsZero()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(0, _operationalProcessTypeRefDataCreated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_OperationalProcessTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.OperationalProcessTypeName, _operationalProcessTypeRefDataCreated.OperationalProcessTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _operationalProcessTypeRefDataCreated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Create(It.IsAny<OperationalProcessTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>();
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Create(It.IsAny<OperationalProcessTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_DuplicateNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();
            var expectedErrorMsg = string.Format(WebResources.EntityNameIsNotUnique, "Operational Process Type");

            var insert = new OperationalProcessTypeRefDataViewModel
            {
                OperationalProcessTypeName = "Operational Process Type One",
                SortOrder = 5,
            };

            #endregion

            #region Act

            _controllerWithMockedServices.CreateAjaxOperationalProcessTypesRefDataGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", expectedErrorMsg), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.UpdateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_IdSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.Id, _operationalProcessTypeRefDataUpdated.Id);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_OperationalProcessTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.OperationalProcessTypeName, _operationalProcessTypeRefDataUpdated.OperationalProcessTypeName);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.UpdateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _operationalProcessTypeRefDataUpdated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Update(It.IsAny<OperationalProcessTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.Update(It.IsAny<OperationalProcessTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxOperationalProcessTypesRefDataGrid_ModelStateNotValid_NoDeleteTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.DeleteAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Delete(It.IsAny<OperationalProcessTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxOperationalProcessTypesRefDataGrid_GroupCannotBeFound_ModelStateErrorAdded()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdNotExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.CannotFindReferenceDataItemToBeDeleted));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxOperationalProcessTypesRefDataGrid_GroupIsBeingUsed_ModelStateErrorAdded()
        {
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.GetNumberOfOperationalProcessTypeReferences(It.IsAny<int>()))
                .Returns(1);
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            var result = _controllerWithMockedServices.DeleteAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity) as JsonResult;
            var kendoDataSource = result.Data as DataSourceResult;
            var errors = kendoDataSource.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ReferenceDataItemIsStillUtilised));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxOperationalProcessTypesRefDataGrid_DeleteGroup_DeleteCalled()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            _mockOperationalProcessTypeRefDataService.Verify(v => v.Delete(It.IsAny<OperationalProcessTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxOperationalProcessTypesRefDataGrid_DeleteGroup_DeleteCalledWithCorrectGroup()
        {
            var entity = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefDataViewModel>(x =>
            {
                x.Id = OperationalProcessTypeRefDataIdExists;
            });
            _controllerWithMockedServices.DeleteAjaxOperationalProcessTypesRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(OperationalProcessTypeRefDataIdExists, _operationalProcessTypeRefDataDeleted.Id);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleOperationalProcessTypesForCustomer_CustomerNotInAppContext_ReturnsEmptyResult()
        {
            var result = _controller.GetAllAndNotVisibleOperationalProcessTypesForCustomer();
            var resolverGroupTypes = result.Data as List<OperationalProcessTypeRefData>;
            Assert.IsNotNull(resolverGroupTypes);
            Assert.AreEqual(0, resolverGroupTypes.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleOperationalProcessTypesForCustomer_CustomerInAppContext_CorrectServiceMethodIsCalled()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleOperationalProcessTypesForCustomer();

            _mockOperationalProcessTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(_appContextWithCustomer.CurrentCustomer.Id), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleOperationalProcessTypesForCustomer_CustomerNotInAppContext_DoesNotCallServiceMethod()
        {
            _appContext = new AppContext();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            _controller.GetAllAndNotVisibleOperationalProcessTypesForCustomer();

            _mockOperationalProcessTypeRefDataService.Verify(v => v.GetAllAndNotVisibleForCustomer(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_GetAllAndNotVisibleOperationalProcessTypesForCustomer_CustomerInAppContext_ReturnsResults()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContextWithCustomer);
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.GetAllAndNotVisibleForCustomer(CustomerId))
                .Returns(new List<OperationalProcessTypeRefData>
                {
                    UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(),
                    UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>()
                });
            _controller = new ReferenceDataController(_mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockInputTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockOperationalProcessTypeRefDataService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockLoggingManager.Object);

            var result = _controller.GetAllAndNotVisibleOperationalProcessTypesForCustomer();

            var operationalProcessTypes = result.Data as List<OperationalProcessTypeRefData>;
            Assert.IsNotNull(operationalProcessTypes);
            Assert.AreEqual(2, operationalProcessTypes.Count);
        }

        #endregion

        #region Region Types

        [TestMethod]
        public void ReferenceDataController_GetRegionTypes_Get_ReturnsAllTypesForDefaultDropDownList()
        {
            var result = _controller.GetRegionTypes();
            var data = result.Data as List<RegionTypeRefData>;
            Assert.AreEqual(_regionTypeRefDatas.Count, data.Count);
        }

        [TestMethod]
        public void ReferenceDataController_GetRegionTypes_Get_DataIsInSortOrder()
        {
            var expected = _regionTypeRefDatas
                .OrderBy(x => x.SortOrder)
                .Select(s => s.Id)
                .ToList();
            var result = _controller.GetRegionTypes();
            var data = result.Data as List<RegionTypeRefData>;
            var actual = data
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        #endregion

        #region Method Authorization Requirement Tests

        [TestMethod]
        public void ReferenceDataController_CreateAjaxDomainRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateAjaxDomainRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxDomainRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("UpdateAjaxDomainRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxDomainRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxDomainRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxFunctionRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateAjaxFunctionRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxFunctionRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("UpdateAjaxFunctionRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxFunctionRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxFunctionRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxInputTypeRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("UpdateAjaxInputTypeRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxInputTypeRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxInputTypeRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxInputTypeRefDataGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateAjaxInputTypeRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_Get_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateInputType", (AuthorizeAttribute att) => att.Roles, new Type[] { }));
        }

        [TestMethod]
        public void ReferenceDataController_CreateInputType_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateInputType", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(InputTypeRefDataViewModel) }));
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxResolverGroupTypesRefDataGrid_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateAjaxServiceDeliveryOrganisationTypesRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxResolverGroupTypesRefDataGrid_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("UpdateAjaxServiceDeliveryOrganisationTypesRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxResolverGroupTypesRefDataGrid_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxServiceDeliveryOrganisationTypesRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxOperationalProcessTypesRefDataGrid_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateAjaxOperationalProcessTypesRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxOperationalProcessTypesRefDataGrid_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("UpdateAjaxOperationalProcessTypesRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAjaxOperationalProcessTypesRefDataGrid_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxOperationalProcessTypesRefDataGrid", (AuthorizeAttribute att) => att.Roles));
        }

        #endregion
    }
}
