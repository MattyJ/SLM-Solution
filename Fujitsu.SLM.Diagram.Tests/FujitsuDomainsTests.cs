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
    public class FujitsuDomainsTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;

        private IServiceDeskService _serviceDeskService;
        private IDiagramGenerator _diagramGenerator;

        private List<ServiceDesk> _serviceDesks;
        private List<DeskInputType> _deskInputTypes;

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
                        _deskInputTypes.FirstOrDefault(x => x.Id == 6),
                    },
                    ServiceDomains = new List<ServiceDomain>(),
                    CustomerId = 1,
                    Customer = UnitTestHelper.GenerateRandomData<Customer>(x =>
                    {
                        x.Id = 1;
                    })
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
                    ServiceDomains = new List<ServiceDomain>
                    {
                        new ServiceDomain
                        {
                            Id =1,
                            AlternativeName = "Domain A",
                            DomainType = new DomainTypeRefData
                            {
                                Id =1,
                                DomainName = "A Domain",
                                SortOrder = 5,
                                Visible = true
                            },
                            DiagramOrder = 20,
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
                                    ServiceComponents = new List<ServiceComponent>
                                    {
                                        new ServiceComponent
                                        {
                                            Id =1,
                                            ComponentLevel = 1,
                                            ComponentName = "Component A",
                                            ServiceActivities = "Email\r\nPhone",
                                            Resolver = new Resolver
                                            {
                                                Id = 1,
                                                ResolverGroupType =  UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>()
                                            }
                                        }
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
                                        Visible = true,
                                    }
                                }
                            },

                        },
                        new ServiceDomain
                        {
                            Id =2,
                            DomainType = new DomainTypeRefData
                            {
                                Id =2,
                                DomainName = "Domain B",
                                SortOrder = 5,
                                Visible = true,
                            },
                            DiagramOrder = 15
                        },
                        new ServiceDomain
                        {
                            Id =3,
                            DomainType = new DomainTypeRefData
                            {
                                Id =3,
                                DomainName = "Domain C",
                                SortOrder = 5,
                                Visible = true,
                            },
                            DiagramOrder = 10
                        }
                    }
                },
                new ServiceDesk
                {
                    Id = 3,
                    DeskName = "Service Desk C With No Domains",
                    CustomerId = 2,
                    Customer = UnitTestHelper.GenerateRandomData<Customer>(x =>
                    {
                        x.Id = 2;
                    }),
                    ServiceDomains = new List<ServiceDomain>()
                },
            };

            #endregion

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);
            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_deskInputTypes, (entity, id) => entity.Id == (int)id);

            _serviceDeskService = new ServiceDeskService(_mockServiceDeskRepository.Object, _mockDeskInputTypeRepository.Object, _mockUnitOfWork.Object);
            _diagramGenerator = new FujitsuDomains(_serviceDeskService);
            Bootstrapper.SetupAutoMapper();

        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FujitsuDomains_Constructor_NoServiceDeskService()
        {
            #region Arrange

            #endregion

            #region Act

            new FujitsuDomains(null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FujitsuDomains_Generate_ServiceDeskIdSetToZero()
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
        public void FujitsuDomains_Generate_CallsServiceGetById()
        {
            _diagramGenerator.Generate(1);
            _mockServiceDeskRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void FujitsuDomains_Generate_ReturnsChartAsListOfChartDataViewModel()
        {
            var result = _diagramGenerator.Generate(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<ChartDataListItem>));
        }

        [TestMethod]
        public void FujitsuDomains_Generate_ReturnsChartWithADeskWithDeskInputsAndUnits()
        {
            var result = _diagramGenerator.Generate(1)[0];

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Inputs.Any());
            Assert.AreEqual(DecompositionType.Desk.ToString(), result.Type);
            Assert.IsTrue(result.Units.Any());
        }

        [TestMethod]
        public void FujitsuDomains_Generate_ReturnsServiceDeliveryOrganisationWithThreeDomains()
        {
            var result = _diagramGenerator.Generate(2);

            Assert.IsNotNull(result);
            var sdo = result.First().Units.First().Units.Count;
            Assert.AreEqual(3, sdo);
        }

        [TestMethod]
        public void FujitsuDomains_Generate_ReturnsServiceDeliveryOrganisationWithThreeDomainsInTheCorrectOrder()
        {
            var serviceDomains = _serviceDesks.First(x => x.Id == 2).ServiceDomains.OrderBy(x => x.DiagramOrder).ThenBy(x => x.DomainType.DomainName).ToList();
            var result = _diagramGenerator.Generate(2);

            Assert.IsNotNull(result);
            var sdo = result.First().Units.First().Units.Count;
            Assert.AreEqual(3, sdo);
            Assert.AreEqual(result.First().Units[0].Units[0].Id, serviceDomains[0].Id);
            Assert.AreEqual(result.First().Units[0].Units[1].Id, serviceDomains[1].Id);
            Assert.AreEqual(result.First().Units[0].Units[2].Id, serviceDomains[2].Id);
        }

        [TestMethod]
        public void FujitsuDomains_Generate_NoDomains_ReturnsServiceDeliveryOrganisationWithNoUnits()
        {
            var result = _diagramGenerator.Generate(3);

            Assert.IsNotNull(result);
            var sdo = result.First().Units.First();
            Assert.AreEqual(string.Empty, sdo.Title);
            Assert.AreEqual("Fujitsu", sdo.CenteredTitle);
            Assert.AreEqual(DecompositionType.ServiceDeliveryOrganisation.ToString(), sdo.Type);
            Assert.AreEqual(0, sdo.Units.Count);
        }
    }
}
