using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Diagrams.Entities;
using Fujitsu.SLM.Diagrams.Generators;
using Fujitsu.SLM.Diagrams.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services;
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
    public class ServiceDeskStructureTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;
        private Mock<IRepository<Resolver>> _mockResolverRepository;
        private Mock<IRepository<OperationalProcessType>> _mockOperationalProcessTypeRepository;
        private Mock<IRepository<OperationalProcessTypeRefData>> _mockOperationalProcessTypeRefDataRepository;
        private Mock<IRepository<ServiceComponent>> _mockServiceComponentRepository;
        private Mock<IUserIdentity> _mockUserIdentity;

        private IServiceDeskService _serviceDeskService;
        private IResolverService _resolverService;
        private IDiagramGenerator _diagramGenerator;

        private List<ServiceDesk> _serviceDesks;
        private List<DeskInputType> _deskInputTypes;
        private List<Resolver> _resolvers;
        private List<OperationalProcessType> _operationalProcessTypes;
        private List<OperationalProcessTypeRefData> _operationalProcessTypeRefDataItems;

        [TestInitialize]
        public void TestInitilize()
        {
            _deskInputTypes = new List<DeskInputType>
            {
                new DeskInputType
                {
                    Id = 1,
                    InputTypeRefData =
                        new InputTypeRefData {Id = 1, InputTypeNumber = 1, InputTypeName = "Incident", SortOrder = 5}
                },
                new DeskInputType
                {
                    Id = 2,
                    InputTypeRefData =
                        new InputTypeRefData {Id = 2, InputTypeNumber = 2, InputTypeName = "Event", SortOrder = 5}
                },
                new DeskInputType
                {
                    Id = 3,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 3,
                            InputTypeNumber = 3,
                            InputTypeName = "Authorized Request",
                            SortOrder = 5
                        }
                },
                new DeskInputType
                {
                    Id = 4,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 4,
                            InputTypeNumber = 4,
                            InputTypeName = "How do I.. Questions",
                            SortOrder = 5
                        }
                },
                new DeskInputType
                {
                    Id = 5,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 5,
                            InputTypeNumber = 5,
                            InputTypeName = "Authorized Standard Change",
                            SortOrder = 5
                        }
                },
                new DeskInputType
                {
                    Id = 6,
                    InputTypeRefData =
                        new InputTypeRefData
                        {
                            Id = 6,
                            InputTypeNumber = 6,
                            InputTypeName = "Authorized Non-Standard Change",
                            SortOrder = 5
                        }
                }
            };

            #region Service Desks

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id =1,
                    DeskName = "Service Desk A With Inputs",
                    CustomerId = 1,
                    Customer = UnitTestHelper.GenerateRandomData<Customer>(x =>
                    {
                        x.Id = 1;
                    }),
                    DeskInputTypes = new List<DeskInputType>
                    {
                        _deskInputTypes.FirstOrDefault(x => x.Id == 1),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 2),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 3),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 4),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 5),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 6)
                    },
                    ServiceDomains = new List<ServiceDomain>()
                },
                new ServiceDesk
                {
                    Id =2,
                    DeskName = "Service Desk B With Domains",
                    CustomerId = 2,
                    Customer = UnitTestHelper.GenerateRandomData<Customer>(x =>
                    {
                        x.Id = 2;
                    }),
                    DeskInputTypes = new List<DeskInputType>
                    {
                        _deskInputTypes.FirstOrDefault(x => x.Id == 1),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 2),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 3),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 4)
                    },
                    ServiceDomains = new List<ServiceDomain>
                    {
                        new ServiceDomain
                        {
                            Id =1,
                            AlternativeName = "Domain A",
                            DiagramOrder = 30,
                            DomainType = new DomainTypeRefData
                            {
                                Id =1,
                                DomainName = "A Domain",
                                SortOrder = 5,
                                Visible = true,
                            },
                            ServiceFunctions = new List<ServiceFunction>
                            {
                                new ServiceFunction
                                {
                                    Id=1,
                                    AlternativeName = "Function A",
                                    FunctionType = new FunctionTypeRefData
                                    {
                                        Id=1,
                                        FunctionName = "A Function",
                                        SortOrder = 5,
                                        Visible = true,
                                    },
                                    DiagramOrder = 20,
                                    ServiceComponents = new List<ServiceComponent>
                                    {
                                        new ServiceComponent
                                        {
                                            Id =1,
                                            ComponentLevel = 1,
                                            ComponentName = "Component A",
                                            ServiceActivities = "Email\r\nPhone",
                                            DiagramOrder = 20,
                                            Resolver = new Resolver
                                            {
                                                Id = 1,
                                                ServiceDeliveryOrganisationType = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                                                ServiceDeliveryUnitType = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                                                ResolverGroupType =  UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>()
                                            }
                                        },
                                        new ServiceComponent
                                        {
                                            Id =2,
                                            ComponentLevel = 1,
                                            ComponentName = "Component B",
                                            ServiceActivities = "Fax",
                                            DiagramOrder = 10,
                                            Resolver = new Resolver
                                            {
                                                Id = 2,
                                                ServiceDeliveryOrganisationType = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                                                ServiceDeliveryUnitType = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                                                ResolverGroupType =  UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>()
                                            }
                                        },
                                        new ServiceComponent
                                        {
                                            Id =3,
                                            ComponentLevel = 1,
                                            ComponentName = "Component C",
                                            ServiceActivities = "Text",
                                            DiagramOrder = 1,
                                            Resolver = new Resolver
                                            {
                                                Id = 3,
                                                ServiceDeliveryOrganisationType = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(),
                                                ServiceDeliveryUnitType = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(),
                                                ResolverGroupType =  UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>()
                                            }
                                        },
                                    }
                                },
                                new ServiceFunction
                                {
                                    Id=2,
                                    AlternativeName = "Function B",
                                    FunctionType = new FunctionTypeRefData
                                    {
                                        Id=2,
                                        FunctionName = "B Function",
                                        SortOrder = 5,
                                        Visible = true
                                    },
                                    DiagramOrder = 10
                                }
                            },

                        },
                        new ServiceDomain
                        {
                            Id =2,
                            DiagramOrder = 20,
                            DomainType = new DomainTypeRefData
                            {
                                Id =2,
                                DomainName = "Domain B",
                                SortOrder = 5,
                                Visible = true
                            }
                        },
                        new ServiceDomain
                        {
                            Id =3,
                            DiagramOrder = 10,
                            DomainType = new DomainTypeRefData
                            {
                                Id =3,
                                DomainName = "Domain C",
                                SortOrder = 5,
                                Visible = true
                            }
                        }
                    }
                }
            };

            #endregion

            _resolvers = new List<Resolver>();
            _operationalProcessTypes = new List<OperationalProcessType>();
            _operationalProcessTypeRefDataItems = new List<OperationalProcessTypeRefData>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);
            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_deskInputTypes, (entity, id) => entity.Id == (int)id);
            _mockResolverRepository = MockRepositoryHelper.Create(_resolvers);
            _mockOperationalProcessTypeRepository = MockRepositoryHelper.Create(_operationalProcessTypes, (entity, id) => entity.Id == (int)id);
            _mockOperationalProcessTypeRefDataRepository = MockRepositoryHelper.Create(_operationalProcessTypeRefDataItems, (entity, id) => entity.Id == (int)id);
            _mockServiceComponentRepository = new Mock<IRepository<ServiceComponent>>();
            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns("test@uk.fujitsu.com");

            _serviceDeskService = new ServiceDeskService(_mockServiceDeskRepository.Object,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);

            _resolverService = new ResolverService(_mockResolverRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);

            _diagramGenerator = new ServiceDeskStructure(_serviceDeskService, _resolverService);
            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskStructure_Constructor_NoServiceDeskService()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDeskStructure(
                null,
                _resolverService);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskStructure_Constructor_NoResovlerGroupService()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDeskStructure(
                _serviceDeskService,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskStructure_Generate_ServiceDeskIdSetToZero()
        {
            #region Arrange

            #endregion

            #region Act

            _diagramGenerator.Generate(0);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_CallsServiceGetById()
        {
            _diagramGenerator.Generate(1);
            _mockServiceDeskRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ReturnsChartAsListOfChartDataViewModel()
        {
            var result = _diagramGenerator.Generate(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<ChartDataListItem>));
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ReturnsChartWithServiceDeskTypeAsParent()
        {
            var result = _diagramGenerator.Generate(2);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ServiceDeskOneReturnsChartWithSixDeskInputs()
        {
            var result = _diagramGenerator.Generate(1, true);

            Assert.IsNotNull(result);
            var inputs = result.First().Inputs;
            Assert.AreEqual(6, inputs.Count);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ReturnsChartWithADeskWithDeskInputsAndUnits()
        {
            var result = _diagramGenerator.Generate(2, true).ToList().First();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Inputs.Any());
            Assert.AreEqual(DecompositionType.Desk.ToString(), result.Type);
            Assert.IsTrue(result.Units.Any());
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ServiceDeskTwoReturnsChartWithFourDeskInputsAndThreeDomainUnits()
        {
            var result = _diagramGenerator.Generate(2, true);

            Assert.IsNotNull(result);
            var inputs = result.First().Inputs;
            var units = result.First().Units;
            Assert.AreEqual(4, inputs.Count);
            Assert.AreEqual(3, units.Count);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ReturnsChartWithThreeDomainUnits()
        {
            var result = _diagramGenerator.Generate(2, true);

            Assert.IsNotNull(result);
            var units = result.First().Units;
            Assert.AreEqual(3, units.Count);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_OneSelectedDomainReturnsChartWithOneDomain()
        {
            var domainsSelected = new[] { "1" };
            var result = _diagramGenerator.Generate(2, true, false, false, false, false, false, domainsSelected);

            Assert.IsNotNull(result);
            var units = result.First().Units;
            Assert.AreEqual(1, units.Count);
            Assert.AreEqual(1, units[0].Id);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_TwoSelectedDomainsReturnsChartWithTwoDomains()
        {
            var domainsSelected = new[] { "1", "2" };
            var result = _diagramGenerator.Generate(2, true, false, false, false, false, false, domainsSelected);

            Assert.IsNotNull(result);
            var units = result.First().Units;
            Assert.AreEqual(2, units.Count);
            Assert.AreEqual(1, units[1].Id);
            Assert.AreEqual(2, units[0].Id);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ThreeSelectedDomainsWithServiceDomainsFalseReturnsChartWithThreeDomains()
        {
            var domainsSelected = new[] { "1", "2", "3" };
            var result = _diagramGenerator.Generate(2, false, false, false, false, false, false, domainsSelected);

            Assert.IsNotNull(result);
            var units = result.First().Units;
            Assert.AreEqual(3, units.Count);
            Assert.AreEqual(1, units[2].Id);
            Assert.AreEqual(2, units[1].Id);
            Assert.AreEqual(3, units[0].Id);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_DomainsWithNoDomainsSelectedReturnsChartWithAllDomains()
        {
            var result = _diagramGenerator.Generate(2, true, false, false, false, false, false, null);

            Assert.IsNotNull(result);
            var units = result.First().Units;
            Assert.AreEqual(3, units.Count);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_OneSelectedDomainReturnsUnitTitleUsingAlternativeName()
        {
            var domainsSelected = new[] { "1" };
            var result = _diagramGenerator.Generate(2, true, false, false, false, false, false, domainsSelected);

            Assert.IsNotNull(result);
            var units = result.First().Units;
            Assert.AreEqual(1, units.Count);
            Assert.AreEqual(1, units[0].Id);
            Assert.AreEqual("Domain A", units[0].Title);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_NoDomains_ReturnsChartWithNoUnits()
        {
            var result = _diagramGenerator.Generate(1);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(1, first.Units.Count);
            Assert.AreEqual(DecompositionType.EmptyForLayout.ToString(), first.Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_NoDomains_AllEntitiesSetToFalse_ReturnsChartWithNoUnits()
        {
            var result = _diagramGenerator.Generate(2);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(1, first.Units.Count);
            Assert.AreEqual(DecompositionType.EmptyForLayout.ToString(), first.Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_DomainsOnly_ReturnsChildUnitsAsServiceDomain()
        {
            var result = _diagramGenerator.Generate(2, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Domain.ToString(), first.Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_FunctionsOnly_ReturnsChildUnitsAsServiceFunction()
        {
            var result = _diagramGenerator.Generate(2, false, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Function.ToString(), first.Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ComponentsOnly_ReturnsChildUnitsAsServiceComponent()
        {
            var result = _diagramGenerator.Generate(2, false, false, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ResolverGroupsOnly_ReturnsChildUnitsAsResolverGroup()
        {
            var result = _diagramGenerator.Generate(2, false, false, false, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Resolver.ToString(), first.Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ReturnsChildUnitsAsActivities()
        {
            var result = _diagramGenerator.Generate(2, false, false, false, false, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Activity.ToString(), first.Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_AllEntitiesSet_ReturnsChartContainingEachEntity()
        {
            var result = _diagramGenerator.Generate(2, true, true, true, true, true, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Domain.ToString(), first.Units[2].Type);
            Assert.AreEqual(DecompositionType.Function.ToString(), first.Units[2].Units[1].Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[2].Units[1].Units[0].Type);
            Assert.AreEqual(DecompositionType.Resolver.ToString(), first.Units[2].Units[1].Units[0].Units[0].Units[0].Type);
            Assert.AreEqual(DecompositionType.Activity.ToString(), first.Units[2].Units[1].Units[0].Units[0].Units[0].Units[0].Type);

        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_DomainSelectedAllOtherEntitiesRequested_ReturnsChartContainingEachEntity()
        {
            var domainsSelected = new[] { "1" };
            var result = _diagramGenerator.Generate(2, false, true, true, true, true, true, domainsSelected);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Domain.ToString(), first.Units[0].Type);
            Assert.AreEqual(DecompositionType.Function.ToString(), first.Units[0].Units[1].Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[0].Units[1].Units[0].Type);
            Assert.AreEqual(DecompositionType.Resolver.ToString(), first.Units[0].Units[1].Units[0].Units[0].Units[0].Type);
            Assert.AreEqual(DecompositionType.Activity.ToString(), first.Units[0].Units[1].Units[0].Units[0].Units[0].Units[0].Type);

        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_DomainSelectedAllOtherEntitiesRequested_ReturnsOneDomainContainingEachEntity()
        {
            var domainsSelected = new[] { "1" };
            var result = _diagramGenerator.Generate(2, false, true, true, true, true, true, domainsSelected);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Domain.ToString(), first.Units[0].Type);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(DecompositionType.Function.ToString(), first.Units[0].Units[1].Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[0].Units[1].Units[0].Type);
            Assert.AreEqual(DecompositionType.Resolver.ToString(), first.Units[0].Units[1].Units[0].Units[0].Units[0].Type);
            Assert.AreEqual(DecompositionType.Activity.ToString(), first.Units[0].Units[1].Units[0].Units[0].Units[0].Units[0].Type);

        }


        [TestMethod]
        public void ServiceDeskStructure_Generate_DomainFunctionComponentEntitiesSet_ReturnsOnlyThoseEntities()
        {
            var result = _diagramGenerator.Generate(2, true, true, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Domain.ToString(), first.Units[2].Type);
            Assert.AreEqual(DecompositionType.Function.ToString(), first.Units[2].Units[1].Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[2].Units[1].Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_DomainComponentResolverEntitiesSet_ReturnsOnlyThoseEntities()
        {
            var result = _diagramGenerator.Generate(2, true, false, true, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Domain.ToString(), first.Units[2].Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[2].Units[0].Type);
            Assert.AreEqual(DecompositionType.LineForDummyChildComponent.ToString(), first.Units[2].Units[0].Units[0].Type);
            Assert.AreEqual(DecompositionType.Resolver.ToString(), first.Units[2].Units[0].Units[0].Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_FunctionComponentResolverActivitiesEntitiesSet_ReturnsOnlyThoseEntities()
        {
            var result = _diagramGenerator.Generate(2, false, true, true, true, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Function.ToString(), first.Units[1].Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[1].Units[0].Type);
            Assert.AreEqual(DecompositionType.LineForDummyChildComponent.ToString(), first.Units[1].Units[0].Units[0].Type);
            Assert.AreEqual(DecompositionType.Resolver.ToString(), first.Units[1].Units[0].Units[0].Units[0].Type);
            Assert.AreEqual(DecompositionType.Activity.ToString(), first.Units[1].Units[0].Units[0].Units[0].Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_FunctionComponentActivitiesEntitiesSet_ReturnsOnlyThoseEntities()
        {
            var result = _diagramGenerator.Generate(2, false, true, true, false, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
            Assert.AreEqual(DecompositionType.Function.ToString(), first.Units[1].Type);
            Assert.AreEqual(DecompositionType.Component.ToString(), first.Units[1].Units[0].Type);
            Assert.AreEqual(DecompositionType.LineForDummyChildComponent.ToString(), first.Units[1].Units[0].Units[0].Type);
            Assert.AreEqual(DecompositionType.Activity.ToString(), first.Units[1].Units[0].Units[0].Units[0].Type);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ServiceDeskTwoReturnsChartWithServiceDomainsInTheCorrectOrder()
        {
            var result = _diagramGenerator.Generate(2, true);

            Assert.IsNotNull(result);
            var units = result.First().Units;
            var serviceDomains = _serviceDesks.First(x => x.CustomerId == 2).ServiceDomains.OrderBy(x => x.DiagramOrder).ThenBy(x => x.DomainType.DomainName).ToList();
            Assert.AreEqual(serviceDomains.Count, units.Count);
            Assert.AreEqual(units[0].Id, serviceDomains[0].Id);
            Assert.AreEqual(units[1].Id, serviceDomains[1].Id);
            Assert.AreEqual(units[2].Id, serviceDomains[2].Id);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ServiceDeskTwoReturnsChartWithServiceFunctionsInTheCorrectOrder()
        {
            var result = _diagramGenerator.Generate(2, false, true);
            var units = result.First().Units;
            var serviceFunctions = _serviceDesks.First(x => x.CustomerId == 2).ServiceDomains.First().ServiceFunctions.OrderBy(x => x.DiagramOrder).ThenBy(x => x.FunctionType.FunctionName).ToList();

            Assert.AreEqual(serviceFunctions.Count, units.Count);
            Assert.AreEqual(units[0].Id, serviceFunctions[0].Id);
            Assert.AreEqual(units[1].Id, serviceFunctions[1].Id);
        }

        [TestMethod]
        public void ServiceDeskStructure_Generate_ServiceDeskTwoReturnsChartWithServiceComponentsInTheCorrectOrder()
        {
            var result = _diagramGenerator.Generate(2, false, false, true);
            var units = result.First().Units;
            var serviceComponents = _serviceDesks.First(x => x.CustomerId == 2).ServiceDomains.First(x => x.Id == 1).ServiceFunctions.First(x => x.Id == 1).ServiceComponents.OrderBy(x => x.DiagramOrder).ThenBy(x => x.ComponentName).ToList();

            Assert.AreEqual(serviceComponents.Count, units.Count);
            Assert.AreEqual(units[0].Id, serviceComponents[0].Id);
            Assert.AreEqual(units[1].Id, serviceComponents[1].Id);
            Assert.AreEqual(units[2].Id, serviceComponents[2].Id);
        }
    }
}
