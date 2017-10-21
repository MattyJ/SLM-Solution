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
using System.Text.RegularExpressions;

namespace Fujitsu.SLM.Diagram.Tests
{
    [TestClass]
    public class CustomerServiceOrganisationTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;
        private Mock<IRepository<Resolver>> _mockResolverRepository;

        private Mock<IRepository<ServiceComponent>> _mockServiceComponentRepository;
        private Mock<IUserIdentity> _mockUserIdentity;

        private IServiceDeskService _serviceDeskService;
        private IServiceComponentService _serviceComponentService;
        private IDiagramGenerator _diagramGenerator;

        private List<ServiceDesk> _serviceDesks;
        private List<ServiceComponent> _serviceComponents;
        private List<DomainTypeRefData> _serviceDomainTypes;
        private List<FunctionTypeRefData> _serviceFunctionTypes;
        private List<DeskInputType> _deskInputTypes;
        private List<Resolver> _resolvers;
        private List<ServiceDeliveryOrganisationTypeRefData> _serviceDeliveryOrganisationTypes;
        private List<ServiceDeliveryUnitTypeRefData> _serviceDeliveryUnitTypes;
        private List<ResolverGroupTypeRefData> _resolverGroupTypes;

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

            #endregion

            #region Service Domain Types

            _serviceDomainTypes = new List<DomainTypeRefData>
            {
                new DomainTypeRefData{Id=1,DomainName= "Service Desk", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=2,DomainName= "Infrastructure as a Service", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=3,DomainName= "Service Delivery Management", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=4,DomainName= "Infrastructure Management", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=5,DomainName= "End User Device Services", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=6,DomainName= "Engineering Services", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=7,DomainName= "Programme Management Office", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=8,DomainName= "Consulting Services", SortOrder = 5, Visible = true},
                new DomainTypeRefData{Id=9,DomainName= "Bill of material components", SortOrder = 5, Visible = true}
            };

            #endregion

            #region Service Function Types

            _serviceFunctionTypes = new List<FunctionTypeRefData>
            {
                new FunctionTypeRefData{Id=1,FunctionName= "System Management Infrastructure", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=2,FunctionName= "Desktop Virtualisation", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=3,FunctionName= "Secure Remote Access", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=4,FunctionName= "Computing Management", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=5,FunctionName= "General Services", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=6,FunctionName= "Network Services Tower A", SortOrder = 5,Visible = true},
                new FunctionTypeRefData{Id=7,FunctionName= "Network Services Tower B", SortOrder = 5,Visible = true}
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

            #region Resolver Groups

            _resolverGroupTypes = new List<ResolverGroupTypeRefData>
            {
                new ResolverGroupTypeRefData{Id=1,ResolverGroupTypeName= "Wintel Team", SortOrder = 5},
                new ResolverGroupTypeRefData{Id=2,ResolverGroupTypeName= "Oracle Team", SortOrder = 10},
                new ResolverGroupTypeRefData{Id=3,ResolverGroupTypeName= "On-Site Support Dispatch", SortOrder = 15},
                new ResolverGroupTypeRefData{Id=4,ResolverGroupTypeName= "On-Site Support Permanent", SortOrder = 15},
                new ResolverGroupTypeRefData{Id=5,ResolverGroupTypeName= "Off-Site Support Dispatch", SortOrder = 15},
                new ResolverGroupTypeRefData{Id=6,ResolverGroupTypeName= "Off-Site Support Permanent", SortOrder = 15},
            };

            #endregion

            #region Service Desks

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id =1,
                    DeskName = "Service Desk A With Inputs",
                    CustomerId = 1,
                    Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                    {
                        c.Id = 1;
                        c.CustomerName = "3663";
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
                    Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                    {
                        c.Id = 2;
                        c.CustomerName = "Robeco";
                    }),
                    ServiceDomains = new List<ServiceDomain>
                    {
                        new ServiceDomain
                        {
                            Id =1,
                            AlternativeName = "Domain A",
                            DomainType = _serviceDomainTypes.FirstOrDefault(x => x.Id == 1),
                            ServiceFunctions = new List<ServiceFunction>
                            {
                                new ServiceFunction
                                {
                                    Id=1,
                                    AlternativeName = "Function A",
                                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 1),
                                },
                                new ServiceFunction
                                {
                                    Id=2,
                                    AlternativeName = "Function B",
                                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 2)
                                }
                            }
                        },
                        new ServiceDomain
                        {
                            Id =2,
                            DomainType = _serviceDomainTypes.FirstOrDefault(x => x.Id == 2),
                            ServiceFunctions = new List<ServiceFunction>
                            {
                                new ServiceFunction
                                {
                                    Id=3,
                                    AlternativeName = "Function C",
                                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 3),
                                },
                                new ServiceFunction
                                {
                                    Id=2,
                                    AlternativeName = "Function D",
                                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 4)
                                }
                            }
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
                            }
                        }
                    }
                }
            };

            #endregion

            #region Service Components

            _serviceComponents = new List<ServiceComponent>
            {
                new ServiceComponent
                {
                    Id = 1,
                    ComponentLevel = 1,
                    ComponentName = "Component A",
                    ServiceActivities = "- Activity One\r\n- Activity Two",
                    ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                    Resolver = new Resolver
                    {
                        Id = 1,
                        ServiceDeliveryOrganisationType =
                            _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                        ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 1),
                        ResolverGroupType = _resolverGroupTypes.FirstOrDefault(x => x.Id == 1)
                    }
                },
                new ServiceComponent
                {
                    Id = 2,
                    ComponentLevel = 1,
                    ComponentName = "Component B",
                    ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                },
            };

            _serviceComponents.Add(
                new ServiceComponent
                {
                    Id = 3,
                    ComponentLevel = 2,
                    ComponentName = "Component C",
                    ServiceActivities = "- Activity Three",
                    ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                    ParentServiceComponent = _serviceComponents[1],
                    Resolver = new Resolver
                    {
                        Id = 2,
                        ServiceDeliveryOrganisationType =
                            _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                        ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 1),
                        ResolverGroupType = _resolverGroupTypes.FirstOrDefault(x => x.Id == 1)
                    }
                });

            _serviceComponents.Add(new ServiceComponent
            {
                Id = 4,
                ComponentLevel = 2,
                ComponentName = "Component D",
                ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                ParentServiceComponent = _serviceComponents[1],
                Resolver = new Resolver
                {
                    Id = 3,
                    ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                    ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                    ResolverGroupType = _resolverGroupTypes.FirstOrDefault(x => x.Id == 3),
                }
            });

            _serviceComponents[1].ChildServiceComponents = new List<ServiceComponent>
            {
                _serviceComponents[2],
                _serviceComponents[3]
            };

            _serviceComponents.Add(new ServiceComponent
            {
                Id = 5,
                ComponentLevel = 1,
                ComponentName = "Component E",
                ServiceActivities = "- Activity X\r\n- Activity Y",
                ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                Resolver = new Resolver
                {
                    Id = 4,
                    ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 3),
                    ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                    ResolverGroupType = _resolverGroupTypes.FirstOrDefault(x => x.Id == 4),
                }
            });

            _serviceComponents.Add(new ServiceComponent
            {
                Id = 6,
                ComponentLevel = 1,
                ComponentName = "Component E",
                ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
            });

            _serviceComponents.Add(new ServiceComponent
            {
                Id = 7,
                ComponentLevel = 2,
                ComponentName = "Component F",
                ServiceActivities = "- Activity X",
                ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                ParentServiceComponent = _serviceComponents[5],
                Resolver = new Resolver
                {
                    Id = 5,
                    ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                    ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                    ResolverGroupType = _resolverGroupTypes.FirstOrDefault(x => x.Id == 3)
                }
            });

            _serviceComponents.Add(new ServiceComponent
            {
                Id = 8,
                ComponentLevel = 2,
                ComponentName = "Component G",
                ServiceActivities = "- Activity Z",
                ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                ParentServiceComponent = _serviceComponents[5],
                Resolver = new Resolver
                {
                    Id = 6,
                    ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 1),
                    ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                    ResolverGroupType = _resolverGroupTypes.FirstOrDefault(x => x.Id == 4)
                }
            });

            _serviceComponents.Add(new ServiceComponent
            {
                Id = 9,
                ComponentLevel = 2,
                ComponentName = "Component H",
                ServiceActivities = "- Activity A",
                ServiceFunction = CreateServiceFunctionWithCustomer(2, "Robeco", 2),
                ParentServiceComponent = _serviceComponents[5],
                Resolver = new Resolver
                {
                    Id = 7,
                    ServiceDeliveryOrganisationType = _serviceDeliveryOrganisationTypes.FirstOrDefault(x => x.Id == 2),
                    ServiceDeliveryUnitType = _serviceDeliveryUnitTypes.FirstOrDefault(x => x.Id == 2),
                    ResolverGroupType = _resolverGroupTypes.FirstOrDefault(x => x.Id == 3),
                }
            });

            _serviceComponents[5].ChildServiceComponents = new List<ServiceComponent>
            {
                _serviceComponents[6],
                _serviceComponents[7],
                _serviceComponents[8]
            };

            #endregion

            _resolvers = new List<Resolver>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);
            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_deskInputTypes, (entity, id) => entity.Id == (int)id);
            _mockResolverRepository = MockRepositoryHelper.Create(_resolvers);
            _mockServiceComponentRepository = MockRepositoryHelper.Create(_serviceComponents, (entity, id) => entity.Id == (int)id);
            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns("test@uk.fujitsu.com");

            _serviceDeskService = new ServiceDeskService(_mockServiceDeskRepository.Object, _mockDeskInputTypeRepository.Object, _mockUnitOfWork.Object);
            _serviceComponentService = new ServiceComponentService(_mockServiceComponentRepository.Object, _mockResolverRepository.Object, _mockUnitOfWork.Object, _mockUserIdentity.Object);

            _diagramGenerator = new CustomerServiceOrganisation(_serviceDeskService, _serviceComponentService);

            Bootstrapper.SetupAutoMapper();

        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerServiceOrganisation_Constructor_NoServiceDeskService()
        {
            #region Arrange

            #endregion

            #region Act

            new CustomerServiceOrganisation(
                null,
                _serviceComponentService);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerServiceOrganisation_Constructor_NoServiceComponentService()
        {
            #region Arrange

            #endregion

            #region Act

            new CustomerServiceOrganisation(
                _serviceDeskService,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerServiceOrganisation_Generate_ServiceDeskIdSetToZero()
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
        public void CustomerServiceOrganisation_Generate_CallsServiceGetById()
        {
            _diagramGenerator.Generate(1);
            _mockServiceDeskRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartAsListOfChartDataViewModel()
        {
            var result = _diagramGenerator.Generate(1, false, false, true, true, true);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<ChartDataListItem>));
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithServiceDeskTypeAsParent()
        {
            var result = _diagramGenerator.Generate(2, false, false, true, true, true);

            Assert.IsNotNull(result);
            var first = result.First();
            Assert.AreEqual(DecompositionType.Desk.ToString(), first.Type);
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithSixInputs()
        {
            var result = _diagramGenerator.Generate(1, false, false, true, true, true);

            Assert.IsNotNull(result);
            var inputs = result.FirstOrDefault().Inputs;
            Assert.AreEqual(6, inputs.Count);
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithTwoResolversBothWithParentComponentsChildComponentsAndActivities()
        {
            var result = _diagramGenerator.Generate(2, false, false, true, true, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units;
            Assert.AreEqual(2, units.Count);
            Assert.IsFalse(units.Any(x => x.Type != DecompositionType.Resolver.ToString()));
            Assert.IsFalse(units[0].Units.Any(x => x.Type != DecompositionType.Component.ToString()));
            Assert.IsFalse(units[0].Units[0].Units.Any(x => x.Type != DecompositionType.LineForDummyChildComponent.ToString() &&
                x.Type != DecompositionType.Component.ToString()));
            Assert.IsFalse(units[0].Units[0].Units[0].Units.Any(x => x.Type != DecompositionType.Activity.ToString()));
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithTwoResolvers()
        {
            var result = _diagramGenerator.Generate(2, false, false, false, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units;
            Assert.AreEqual(2, units.Count);
            Assert.IsFalse(units.Any(x => x.Type != DecompositionType.Resolver.ToString()));
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithTwoResolversBothWithTwoComponents()
        {
            var result = _diagramGenerator.Generate(2, false, false, true, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units;
            Assert.AreEqual(2, units.Count);
            Assert.IsFalse(units.Any(x => x.Type != DecompositionType.Resolver.ToString()));
            Assert.AreEqual(2, units[0].Units.Count);
            Assert.IsFalse(units[0].Units.Any(x => x.Type != DecompositionType.Component.ToString()));
            Assert.AreEqual(2, units[1].Units.Count);
            Assert.IsFalse(units[1].Units.Any(x => x.Type != DecompositionType.Component.ToString()));
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithTwoResolversBothWithOneServiceActivity()
        {
            var result = _diagramGenerator.Generate(2, false, false, false, true, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units;
            Assert.AreEqual(2, units.Count);
            Assert.AreEqual(1, units[0].Units.Count);
            Assert.AreEqual(1, units[1].Units.Count);
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithTwoResolversWithCorrectServiceActivities()
        {
            var result = _diagramGenerator.Generate(2, false, false, false, true, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units;
            var wintel = units.FirstOrDefault(x => x.TitleThree == _resolverGroupTypes[0].ResolverGroupTypeName);
            var activities = Regex.Split(wintel.Units[0].Title, "\r\n");
            Assert.AreEqual("- Activity One", activities[0]);
            Assert.AreEqual("- Activity Two", activities[1]);
            Assert.AreEqual("- Activity Three", activities[2]);
            Assert.AreEqual(3, activities.Length - 1);
            var support = units.FirstOrDefault(x => x.TitleThree == _resolverGroupTypes[2].ResolverGroupTypeName);
            var supportActivity = Regex.Split(support.Units[0].Title, "\r\n");
            Assert.AreEqual("- Activity X", supportActivity[0]);
            Assert.AreEqual("- Activity A", supportActivity[1]);
            Assert.AreEqual(2, supportActivity.Length - 1);
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ComponentsOnlyReturnsChartWithFourComponents()
        {
            var result = _diagramGenerator.Generate(2, false, false, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units;
            Assert.AreEqual(3, units.Count);
            Assert.IsFalse(units.Any(x => x.Type != DecompositionType.Component.ToString()));
            Assert.IsTrue(units.Any(x => x.Title == "Component A"));
            Assert.IsTrue(units.Any(x => x.Title == "Component B"));
            Assert.IsTrue(units.Any(x => x.Title == "Component E"));
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ReturnsChartWithParentComponentsChildComponentsAndActivities()
        {
            var result = _diagramGenerator.Generate(2, false, false, true, false, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units;
            Assert.AreEqual(3, units.Count);
            Assert.IsFalse(units.Any(x => x.Type != DecompositionType.Component.ToString()));
            Assert.IsTrue(units.Any(x => x.Title == "Component A"));
            Assert.IsTrue(units.Any(x => x.Title == "Component B"));
            Assert.IsTrue(units.Any(x => x.Title == "Component E"));
            Assert.IsFalse(units[0].Units.Any(x => x.Type != DecompositionType.LineForDummyChildComponent.ToString() &&
                x.Type != DecompositionType.Component.ToString()));
            Assert.IsFalse(units[0].Units[0].Units.Any(x => x.Type != DecompositionType.Activity.ToString()));
            Assert.IsTrue(units[0].Units[0].Units.Any(x => x.Title == "- Activity One\r\n- Activity Two"));
        }

        [TestMethod]
        public void CustomerServiceOrganisation_Generate_ActivitiesOnlyReturnsChartWithFiveActivities()
        {
            var result = _diagramGenerator.Generate(2, false, false, false, false, true);

            Assert.IsNotNull(result);
            var units = result.FirstOrDefault().Units.ToList();
            Assert.AreEqual(5, units.Count);
            Assert.IsFalse(units.Any(x => x.Type != DecompositionType.Activity.ToString()));
            Assert.IsTrue(units.Any(x => x.Title == "- Activity One\r\n- Activity Two"));
            Assert.IsTrue(units.Any(x => x.Title == "- Activity Three"));
            Assert.IsTrue(units.Any(x => x.Title == "- Activity A"));
            Assert.IsTrue(units.Any(x => x.Title == "None Specified"));
            Assert.IsTrue(units.Any(x => x.Title == "- Activity X"));
        }

        private static ServiceFunction CreateServiceFunctionWithCustomer(int customerId, string customerName, int serviceDeskId)
        {
            return UnitTestHelper.GenerateRandomData<ServiceFunction>(a =>
            {
                a.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(b =>
                {
                    b.ServiceDeskId = serviceDeskId;
                    b.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(c =>
                    {
                        c.Id = serviceDeskId;
                        c.CustomerId = customerId;
                        c.Customer = UnitTestHelper.GenerateRandomData<Customer>(d =>
                        {
                            d.Id = c.CustomerId;
                            d.CustomerName = customerName;
                        });
                    });
                });
            });
        }

    }
}
