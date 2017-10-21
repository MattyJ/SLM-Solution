using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Diagrams.Entities;
using Fujitsu.SLM.Diagrams.Generators;
using Fujitsu.SLM.Diagrams.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Diagram.Tests
{
    [TestClass]
    public class ServiceDeskDotMatrixTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;
        private Mock<IRepository<Resolver>> _mockResolverGroupRepository;
        private Mock<IRepository<OperationalProcessType>> _mockOperationalProcessTypeRepository;
        private Mock<IRepository<OperationalProcessTypeRefData>> _mockOperationalProcessTypeRefDataRepository;
        private Mock<IRepository<ServiceComponent>> _mockServiceComponentRepository;
        private Mock<IUserIdentity> _mockUserIdentity;

        private IServiceDeskService _serviceDeskService;
        private IResolverService _resolverService;
        private Mock<IResolverService> _mockResolverService;

        private IDiagramGenerator _diagramGenerator;

        private List<ServiceDesk> _serviceDesks;
        private List<DeskInputType> _deskInputTypes;
        private List<Resolver> _resolverGroups;
        private List<OperationalProcessType> _operationalProcessTypes;
        private List<OperationalProcessTypeRefData> _operationalProcessTypeRefDataItems;
        private List<List<DotMatrixListItem>> _dotMatrixListItems;

        private const int CustomerId = 1;
        private const int ServiceDeskId = 1;
        private const string ResolverGroup1 = "ResolverGroup1";
        private const string ResolverGroup2 = "ResolverGroup2";
        private const string ResolverGroup3 = "ResolverGroup3";
        private const string ComponentName1 = "ComponentName1";
        private const string ComponentName2 = "ComponentName2";
        private const string ComponentName3 = "ComponentName3ComponentName3ComponentName3ComponentName3ComponentName3ComponentName3ComponentName3ComponentName3";

        [TestInitialize]
        public void TestInitilize()
        {

            _deskInputTypes = new List<DeskInputType>
            {
                new DeskInputType
                {
                    Id = 1,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 1,
                            InputTypeNumber = 1,
                            InputTypeName = "Incident",
                            SortOrder = 5
                        },
                    ServiceDeskId = 1
                }
            };

            _resolverGroups = new List<Resolver>();

            _operationalProcessTypeRefDataItems = new List<OperationalProcessTypeRefData>
            {
                new OperationalProcessTypeRefData
                {
                    Id = 1,
                    OperationalProcessTypeName = "An Operational Process Type",
                    SortOrder = 5,
                    Visible = true
                }
            };
            _operationalProcessTypes = new List<OperationalProcessType>
            {
                new OperationalProcessType
                {
                    Id = 1,
                    OperationalProcessTypeRefData = _operationalProcessTypeRefDataItems.First(x => x.Id == 1)
                }
            };



            #region Service Desks

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id = 1,
                    DeskName = "Service Desk A With Inputs",
                    DeskInputTypes = new List<DeskInputType>
                    {
                        new DeskInputType
                        {
                            Id = 1,
                            InputTypeRefData =
                                new InputTypeRefData
                                {
                                    Id = 1,
                                    InputTypeNumber = 1,
                                    InputTypeName = "Incident",
                                    SortOrder = 5
                                }
                        }
                    },
                    ServiceDomains = new List<ServiceDomain>(),
                    CustomerId = 1
                },
                new ServiceDesk
                {
                    Id = 2,
                    DeskName = "Service Desk B With Inputs",
                    DeskInputTypes = new List<DeskInputType>
                    {
                        new DeskInputType
                        {
                            Id = 1,
                            InputTypeRefData =
                                new InputTypeRefData
                                {
                                    Id = 1,
                                    InputTypeNumber = 1,
                                    InputTypeName = "Incident",
                                    SortOrder = 5
                                }
                        }

                    },
                    ServiceDomains = new List<ServiceDomain>(),
                    CustomerId = 2
                },
            };

            #endregion

            #region Dot Matrix List Item

            _dotMatrixListItems = new List<List<DotMatrixListItem>>
            {
                new List<DotMatrixListItem>
                {
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ResolverName;
                        x.Value = ResolverGroup1;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ComponentName;
                        x.Value = ComponentName1;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 1);
                        x.Value = true;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 2);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 3);
                        x.Value = true;
                    })
                },
                new List<DotMatrixListItem>
                {
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ResolverName;
                        x.Value = ResolverGroup2;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ComponentName;
                        x.Value = ComponentName2;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 1);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 2);
                        x.Value = true;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 3);
                        x.Value = false;
                    })
                },
                new List<DotMatrixListItem>
                {
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ResolverName;
                        x.Value = ResolverGroup3;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = DotMatrixNames.ComponentName;
                        x.Value = ComponentName3;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 1);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 2);
                        x.Value = false;
                    }),
                    UnitTestHelper.GenerateRandomData<DotMatrixListItem>(x =>
                    {
                        x.Name = string.Concat(DotMatrixNames.OpIdPrefix, 3);
                        x.Value = false;
                    })
                }
            };

            #endregion

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);
            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_deskInputTypes, (entity, id) => entity.Id == (int)id);
            _mockResolverGroupRepository = MockRepositoryHelper.Create(_resolverGroups, (entity, id) => entity.Id == (int)id);

            _mockOperationalProcessTypeRepository = MockRepositoryHelper.Create(_operationalProcessTypes, (entity, id) => entity.Id == (int)id);
            _mockOperationalProcessTypeRefDataRepository = MockRepositoryHelper.Create(_operationalProcessTypeRefDataItems, (entity, id) => entity.Id == (int)id);

            _mockServiceComponentRepository = new Mock<IRepository<ServiceComponent>>();

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns("test@uk.fujitsu.com");

            _resolverService = new ResolverService(_mockResolverGroupRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            _mockResolverService = new Mock<IResolverService>();
            _mockResolverService.Setup(s => s.GetDotMatrix(CustomerId, true, ServiceDeskId))
                .Returns(_dotMatrixListItems);

            _serviceDeskService = new ServiceDeskService(_mockServiceDeskRepository.Object,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);
            _diagramGenerator = new ServiceDeskDotMatrix(_serviceDeskService, _resolverService);
            Bootstrapper.SetupAutoMapper();

        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskDotMatrix_Constructor_NoServiceDeskService()
        {
            new ServiceDeskDotMatrix(
                null,
                _resolverService);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskDotMatrix_Constructor_NoResovlerGroupService()
        {
            new ServiceDeskDotMatrix(
                _serviceDeskService,
                null);
        }

        #endregion


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskDotMatrix_Generate_ServiceDeskIdSetToZero_ThrowsArgumentNullException()
        {
            _diagramGenerator.Generate(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ServiceDeskDotMatrix_Generate_NoChartData_ThrowsApplicationException()
        {
            _diagramGenerator = new ServiceDeskDotMatrix(_serviceDeskService, _mockResolverService.Object);
            _diagramGenerator.Generate(2);
        }

        [TestMethod]
        public void ServiceDeskDotMatrix_Generate_CallsServiceDeskRepositoryGetById()
        {
            _diagramGenerator = new ServiceDeskDotMatrix(_serviceDeskService, _mockResolverService.Object);
            _diagramGenerator.Generate(1);
            _mockServiceDeskRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskDotMatrix_Generate_CallsResolverServiceGetDotMatrix()
        {
            _diagramGenerator = new ServiceDeskDotMatrix(_serviceDeskService, _mockResolverService.Object);
            _diagramGenerator.Generate(1);
            _mockResolverService.Verify(x => x.GetDotMatrix(1, true, 1), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskDotMatrix_Generate_ReturnsChartAsListOfChartDataViewModel()
        {
            _diagramGenerator = new ServiceDeskDotMatrix(_serviceDeskService, _mockResolverService.Object);
            var result = _diagramGenerator.Generate(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<ChartDataListItem>));
        }

        [TestMethod]
        public void ServiceDeskDotMatrix_ReadServiceDeskDotMatrixChart_ServiceDeskExists_LastListStartsWithEmptyLayout()
        {
            _diagramGenerator = new ServiceDeskDotMatrix(_serviceDeskService, _mockResolverService.Object);
            var result = _diagramGenerator.Generate(1);
            var last = result.Last();
            Assert.AreEqual(DecompositionTypeNames.EmptyForLayout, last.Type);
        }

        [TestMethod]
        public void ServiceDeskDotMatrix_Generate_ReturnsChartWithComponentThreeTruncatedToOneHundredCharacters()
        {
            _diagramGenerator = new ServiceDeskDotMatrix(_serviceDeskService, _mockResolverService.Object);
            var result = _diagramGenerator.Generate(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.First().TitleTwo.Length);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<ChartDataListItem>));
        }

    }
}
