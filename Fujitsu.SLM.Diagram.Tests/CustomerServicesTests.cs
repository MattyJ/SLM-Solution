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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fujitsu.SLM.Diagram.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CustomerServicesTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;

        private IServiceDeskService _serviceDeskService;
        private IDiagramGenerator _diagramGenerator;

        private List<ServiceDesk> _serviceDesks;
        private List<DeskInputType> _deskInputTypes;
        private List<ServiceDeliveryOrganisationTypeRefData> _serviceDeliveryOrganisationTypes;
        private List<ServiceDeliveryUnitTypeRefData> _serviceDeliveryUnitTypes;


        [TestInitialize]
        public void TestInitilize()
        {
            #region Desk Input Types

            _deskInputTypes = new List<DeskInputType>
            {
                new DeskInputType
                {
                    Id = 1,
                    InputTypeRefData =
                        new InputTypeRefData {Id = 1, InputTypeNumber = 1, InputTypeName = "Incident", SortOrder = 5},
                },
                new DeskInputType
                {
                    Id = 2,
                    InputTypeRefData =
                        new InputTypeRefData {Id = 2, InputTypeNumber = 2, InputTypeName = "Event", SortOrder = 5},
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
                        },
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
                        },
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
                        },
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
                        },
                },
            };

            #endregion

            #region Service Delivery Organisation Types

            _serviceDeliveryOrganisationTypes = new List<ServiceDeliveryOrganisationTypeRefData>
            {
                new ServiceDeliveryOrganisationTypeRefData
                {
                    Id = 1,
                    ServiceDeliveryOrganisationTypeName = "Fujitsu",
                    SortOrder = 5
                },
                new ServiceDeliveryOrganisationTypeRefData
                {
                    Id = 2,
                    ServiceDeliveryOrganisationTypeName = "Customer",
                    SortOrder = 5
                },
                new ServiceDeliveryOrganisationTypeRefData
                {
                    Id = 3,
                    ServiceDeliveryOrganisationTypeName = "Customer Third Party",
                    SortOrder = 5
                },
            };

            #endregion

            #region Service Delivery Unit Types

            _serviceDeliveryUnitTypes = new List<ServiceDeliveryUnitTypeRefData>
            {
                new ServiceDeliveryUnitTypeRefData
                {
                    Id = 1,
                    ServiceDeliveryUnitTypeName = "Verizon",
                    SortOrder = 5
                },
                new ServiceDeliveryUnitTypeRefData
                {
                    Id = 2,
                    ServiceDeliveryUnitTypeName = "HP",
                    SortOrder = 5
                },
                new ServiceDeliveryUnitTypeRefData
                {
                    Id = 3,
                    ServiceDeliveryUnitTypeName = "Cisco",
                    SortOrder = 5
                },
                new ServiceDeliveryUnitTypeRefData
                {
                    Id = 4,
                    ServiceDeliveryUnitTypeName = "SAP",
                    SortOrder = 5
                },
                new ServiceDeliveryUnitTypeRefData
                {
                    Id = 5,
                    ServiceDeliveryUnitTypeName = "Business Apps",
                    SortOrder = 5
                },
                new ServiceDeliveryUnitTypeRefData
                {
                    Id = 6,
                    ServiceDeliveryUnitTypeName = "Facilities",
                    SortOrder = 5
                },
                new ServiceDeliveryUnitTypeRefData
                {
                    Id = 7,
                    ServiceDeliveryUnitTypeName = "Oracle",
                    SortOrder = 5
                },
            };

            #endregion

            #region Service Desks

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id =1,
                    DeskName = "Service Desk A With Inputs",
                    DeskInputTypes = new List<DeskInputType>
                    {
                        _deskInputTypes.FirstOrDefault(x => x.Id == 1),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 2),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 3),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 4),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 5),
                        _deskInputTypes.FirstOrDefault(x => x.Id == 6)
                    },
                    ServiceDomains = new List<ServiceDomain>(),
                    Resolvers = new List<Resolver>
                    {
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 1),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 3),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 3),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 4),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 3),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 5),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 3),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 6),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 3),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 7),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 3),
                        },
                    },
                    CustomerId = 1,
                    Customer = new Customer
                    {
                        Id = 1,
                        CustomerName = "3663"
                    }
                },
                new ServiceDesk
                {
                    Id =2,
                    DeskName = "Service Desk B With Domains",
                    ServiceDomains = new List<ServiceDomain>(),
                    Resolvers = new List<Resolver>(),
                    CustomerId = 2,
                    Customer = new Customer
                    {
                        Id = 2,
                        CustomerName = "Customer Two"
                    }
                },
                new ServiceDesk
                {
                    Id = 3,
                    DeskName = "Service Desk C With No Domains",
                    ServiceDomains = new List<ServiceDomain>(),
                    Resolvers = new List<Resolver>
                    {
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceDeliveryUnitNotes = "- Storage"
                        },
                        new Resolver
                        {
                            ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceDeliveryUnitNotes = "- Back-up"
                        }
                    },
                    CustomerId = 2,
                    Customer = new Customer
                    {
                        Id = 2,
                        CustomerName = "Customer Two"
                    }
                }
            };

            #endregion

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_deskInputTypes, (entity, id) => entity.Id == (int)id);
            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);

            _serviceDeskService = new ServiceDeskService(_mockServiceDeskRepository.Object,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);

            _diagramGenerator = new CustomerServices(_serviceDeskService);

            Bootstrapper.SetupAutoMapper();

        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerServices_Constructor_NoServiceDeskService()
        {
            #region Arrange

            #endregion

            #region Act

            new CustomerServices(null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerServices_Generate_ServiceDeskIdSetToZero()
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
        public void CustomerServices_Generate_CallsServiceGetById()
        {
            _diagramGenerator.Generate(3);
            _mockServiceDeskRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsChartAsListOfChartDataViewModel()
        {
            var result = _diagramGenerator.Generate(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<ChartDataListItem>));
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsChartWithADeskWithDeskInputsAndUnits()
        {
            var result = _diagramGenerator.Generate(1).ToList().First();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Inputs.Any());
            Assert.AreEqual(DecompositionType.Desk.ToString(), result.Type);
            Assert.IsTrue(result.Units.Any());
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsChartWithADeskWithSixDeskInputsAndTwoUnits()
        {
            var result = _diagramGenerator.Generate(1).ToList().First();

            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Inputs.Count);
            Assert.AreEqual(2, result.Units.Count);
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsChartWithCustomerOwnerAndCustomerThirdPartyUnits()
        {
            var result = _diagramGenerator.Generate(1).ToList().First();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Units.Count);
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsCorrectServiceDeliveryOrganisationUnits()
        {
            var customerOwned = "3663 Owned Resolver Groups";
            var customerThirdParty = "3663 3rd Party Resolver Groups";

            var result = _diagramGenerator.Generate(1).ToList().First();

            Assert.IsNotNull(result);
            Assert.AreEqual(DecompositionType.ServiceDeliveryOrganisation.ToString(), result.Units[0].Type);
            Assert.AreEqual(customerOwned, result.Units[0].CenteredTitle);
            Assert.AreEqual(customerThirdParty, result.Units[1].CenteredTitle);
        }

        [TestMethod]
        public void CustomerServices_Generate_NoServiceDeliveryOrganisationOrServiceDeliveryUnits_ReturnsNoUnits()
        {
            var result = _diagramGenerator.Generate(2).ToList().First();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Units.Count);
            Assert.AreEqual(DecompositionType.EmptyForLayout.ToString(), result.Units[0].Type);
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsCorrectServiceDeliveryOrganisationUnitsWithCorrectServiceDeliveryUnitTypes()
        {
            var result = _diagramGenerator.Generate(1).ToList().First();

            Assert.IsNotNull(result);
            Assert.AreEqual(DecompositionType.ServiceDeliveryUnit.ToString(), result.Units[0].Units[0].Type);
            Assert.AreEqual(3, result.Units[0].Units.Count);
            Assert.AreEqual(DecompositionType.ServiceDeliveryUnit.ToString(), result.Units[1].Units[0].Type);
            Assert.AreEqual(4, result.Units[1].Units.Count);
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsSDOWithDistinctServiceDeliveryUnitType()
        {
            var result = _diagramGenerator.Generate(3).ToList().First();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Units[0].Units.Count);
        }

        [TestMethod]
        public void CustomerServices_Generate_ReturnsSDOWithDistinctServiceDeliveryUnitTypeAndAggregatedActivities()
        {
            const string expectedServices = "- Storage\r\n- Back-up";
            var result = _diagramGenerator.Generate(3).ToList().First();

            Assert.IsNotNull(result);
            Assert.AreEqual(DecompositionType.CustomerServices.ToString(), result.Units[0].Units[0].Units[0].Type);
            Assert.AreEqual(string.Empty, result.Units[0].Units[0].Units[0].CenteredTitle);
            Assert.AreEqual(expectedServices, result.Units[0].Units[0].Units[0].Title);
        }
    }
}
