using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    public class ServiceComponentServiceTests
    {
        private Mock<IRepository<ServiceComponent>> _mockServiceComponentRepository;
        private Mock<IRepository<Resolver>> _mockResolverRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;

        private List<ServiceComponent> _serviceComponents;
        private List<Resolver> _resolvers;
        private IServiceComponentService _target;
        private ServiceComponent _serviceComponentMoveResolverSource;
        private ServiceComponent _serviceComponentMoveResolverDestination;
        private ServiceComponent _serviceComponentMoveResolverDestinationWithDependencies;

        private const string NoneSpecified = "None Specified";
        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private const string Sdu = "Facilities";
        private const string ResolverGroup = "Wintel";
        private const string ServiceActivity = "A Service Activity";

        private const int CustomerId = 15;
        private const int CustomerIdMoveResolver = 34;
        private const int ServiceComponentId = 10;
        private const int ServiceComponentDoesNotExist = 120;


        [TestInitialize]
        public void Initialize()
        {
            //TODO: Exception Logging Handler exception during tests needs further investigation
            //Bootstrapper.SetupAutoMapper();
            //var ob = new ObjectBuilder();
            //UnityConfig.RegisterTypes(ob.GetContainer(), () => new PerResolveLifetimeManager());
            //Logger.Reset();
            //Logger.SetLogWriter(new LogWriterFactory().Create());
            //var config = ConfigurationSourceFactory.Create();
            //var factory = new ExceptionPolicyFactory(config);
            //var exceptionManager = factory.CreateManager();
            //ExceptionPolicy.Reset();
            //ExceptionPolicy.SetExceptionManager(exceptionManager);

            #region Lists

            _serviceComponentMoveResolverSource = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.ServiceFunction = CreateServiceFunctionWithCustomer(CustomerIdMoveResolver);
            });
            _serviceComponentMoveResolverDestination = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.ServiceFunction = CreateServiceFunctionWithCustomer(CustomerIdMoveResolver);
            });
            _serviceComponentMoveResolverDestinationWithDependencies = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.ServiceFunction = CreateServiceFunctionWithCustomer(CustomerIdMoveResolver);
                CreateResolver(x);
            });

            _resolvers = new List<Resolver>
            {
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 55;
                    x.OperationalProcessTypes = new List<OperationalProcessType>
                    {
                        UnitTestHelper.GenerateRandomData<OperationalProcessType>(y =>
                        {
                            y.Resolver = x;
                            y.OperationalProcessTypeRefData = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>();
                        }),
                        UnitTestHelper.GenerateRandomData<OperationalProcessType>(y =>
                        {
                            y.Resolver = x;
                            y.OperationalProcessTypeRefData = UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>();
                        })
                    };
                })
            };

            _serviceComponentMoveResolverSource = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.ServiceFunction = CreateServiceFunctionWithCustomer(CustomerIdMoveResolver);
                x.Resolver = _resolvers[0];
            });

            _serviceComponents = new List<ServiceComponent>
            {
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.ServiceFunction = CreateServiceFunctionWithCustomer(CustomerId);
                    CreateResolver(x);
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.ServiceFunction = CreateServiceFunctionWithCustomer(56);
                    CreateResolver(x);
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.Id = ServiceComponentId;
                    x.ServiceFunction = CreateServiceFunctionWithCustomer(CustomerId);
                    CreateResolver(x);
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.ServiceFunction = CreateServiceFunctionWithCustomer(56);
                    CreateResolver(x);
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.ServiceFunction = CreateServiceFunctionWithCustomer(56);
                    CreateResolver(x);
                }),

                UnitTestHelper.GenerateRandomData<ServiceComponent>(a =>
                {
                    a.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(b =>
                    {
                        b.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(c =>
                        {
                            c.ServiceDeskId = 1;
                            c.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(d =>
                            {
                                d.Id = 1;
                                d.DeskName = "Service Desk One";
                                d.CustomerId = CustomerId;
                                d.Customer = UnitTestHelper.GenerateRandomData<Customer>(e =>
                                {
                                    e.Id = CustomerId;
                                    e.CustomerName = "3663";
                                });
                            });
                         });
                    });
                    a.ServiceActivities = ServiceActivity;
                    CreateServiceOrganisationResolver(a,ServiceDeliveryOrganisationNames.Fujitsu, 1);
                }),

                UnitTestHelper.GenerateRandomData<ServiceComponent>(a =>
                {
                    a.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(b =>
                    {
                        b.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(c =>
                        {
                            c.ServiceDeskId = 1;
                            c.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(d =>
                            {
                                d.Id = 1;
                                d.DeskName = "Service Desk One";
                                d.CustomerId = CustomerId;
                                d.Customer = UnitTestHelper.GenerateRandomData<Customer>(e =>
                                {
                                    e.Id = CustomerId;
                                    e.CustomerName = "3663";
                                });
                            });
                         });
                    });
                    a.ServiceActivities = null;
                    CreateServiceOrganisationResolverNoResolverDependencies(a,ServiceDeliveryOrganisationNames.Fujitsu, 2);
                }),

                UnitTestHelper.GenerateRandomData<ServiceComponent>(a =>
                {
                    a.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(b =>
                    {
                        b.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(c =>
                        {
                            c.ServiceDeskId = 1;
                            c.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(d =>
                            {
                                d.Id = 1;
                                d.DeskName = "Service Desk One";
                                d.CustomerId = CustomerId;
                                d.Customer = UnitTestHelper.GenerateRandomData<Customer>(e =>
                                {
                                    e.Id = CustomerId;
                                    e.CustomerName = "3663";
                                });
                            });
                         });
                    });
                    a.ServiceActivities = ServiceActivity;
                    CreateServiceOrganisationResolver(a,ServiceDeliveryOrganisationNames.Customer, 3);
                }),

                UnitTestHelper.GenerateRandomData<ServiceComponent>(a =>
                {
                    a.ServiceActivities = string.Empty;
                    a.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(b =>
                    {
                        b.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(c =>
                        {
                            c.ServiceDeskId = 1;
                            c.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(d =>
                            {
                                d.Id = 1;
                                d.DeskName = "Service Desk One";
                                d.CustomerId = CustomerId;
                                d.Customer = UnitTestHelper.GenerateRandomData<Customer>(e =>
                                {
                                    e.Id = CustomerId;
                                    e.CustomerName = "3663";
                                });
                            });
                         });
                    });
                    a.ServiceActivities = null;
                    CreateServiceOrganisationResolverNoResolverDependencies(a,ServiceDeliveryOrganisationNames.Customer, 4);
                }),

                UnitTestHelper.GenerateRandomData<ServiceComponent>(a =>
                {
                    a.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(b =>
                    {
                        b.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(c =>
                        {
                            c.ServiceDeskId = 1;
                            c.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(d =>
                            {
                                d.Id = 1;
                                d.DeskName = "Service Desk One";
                                d.CustomerId = CustomerId;
                                d.Customer = UnitTestHelper.GenerateRandomData<Customer>(e =>
                                {
                                    e.Id = CustomerId;
                                    e.CustomerName = "3663";
                                });
                            });
                         });
                    });
                    a.ServiceActivities = null;
                    CreateServiceOrganisationResolver(a,ServiceDeliveryOrganisationNames.CustomerThirdParty, 5);
                }),

                UnitTestHelper.GenerateRandomData<ServiceComponent>(a =>
                {
                    a.ServiceFunction = UnitTestHelper.GenerateRandomData<ServiceFunction>(b =>
                    {
                        b.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(c =>
                        {
                            c.ServiceDeskId = 1;
                            c.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(d =>
                            {
                                d.Id = 1;
                                d.DeskName = "Service Desk One";
                                d.CustomerId = CustomerId;
                                d.Customer = UnitTestHelper.GenerateRandomData<Customer>(e =>
                                {
                                    e.Id = CustomerId;
                                    e.CustomerName = "3663";
                                });
                            });
                         });
                    });
                    a.ServiceActivities = null;
                    CreateServiceOrganisationResolverNoResolverDependencies(a,ServiceDeliveryOrganisationNames.CustomerThirdParty, 6);
                }),

                _serviceComponentMoveResolverSource,
                _serviceComponentMoveResolverDestination,
                _serviceComponentMoveResolverDestinationWithDependencies
            };

            #endregion

            _mockServiceComponentRepository = MockRepositoryHelper.Create(_serviceComponents,
                (entity, id) => entity.Id == (int)id,
                (p1, p2) => p1.Id == p2.Id);

            _mockResolverRepository = MockRepositoryHelper.Create(_resolvers,
                (entity, id) => entity.Id == (int)id);

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns(UserName);

            _target = new ServiceComponentService(_mockServiceComponentRepository.Object,
                _mockResolverRepository.Object,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object);
        }

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentService_Ctor_ServiceComponentServiceNull_ThrowsArgumentNullException()
        {
            new ServiceComponentService(null,
                _mockResolverRepository.Object,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentService_Ctor_ResolverGroupRepositoryNull_ThrowsArgumentNullException()
        {
            new ServiceComponentService(_mockServiceComponentRepository.Object,
                null,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentService_Ctor_UnitOfWorkNull_ThrowsArgumentNullException()
        {
            new ServiceComponentService(_mockServiceComponentRepository.Object,
                _mockResolverRepository.Object,
                null,
                _mockUserIdentity.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceComponentService_Ctor_UserIdentityNull_ThrowsArgumentNullException()
        {
            new ServiceComponentService(_mockServiceComponentRepository.Object,
                _mockResolverRepository.Object,
                _mockUnitOfWork.Object,
                null);
        }

        #endregion


        [TestMethod]
        public void ServiceComponentService_GetById_IdExists_EntityReturned()
        {
            var result = _target.GetById(ServiceComponentId);
            Assert.IsNotNull(result);
            Assert.AreEqual(ServiceComponentId, result.Id);
        }

        [TestMethod]
        public void ServiceComponentService_GetById_IdDoesNotExist_NoEntityReturned()
        {
            var result = _target.GetById(666);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ServiceComponentService_All_ReturnsAllServiceComponents_EightEntitiesReturned()
        {
            var result = _target.All();
            Assert.IsNotNull(result);
            Assert.AreEqual(_serviceComponents.Count, result.Count());
        }

        [TestMethod]
        public void ServiceComponentService_Create_NewEntity_AddedToRepository()
        {
            var newEntity = UnitTestHelper.GenerateRandomData<ServiceComponent>();

            _target.Create(newEntity);

            Assert.AreEqual(15, _serviceComponents.Count);
            Assert.IsTrue(_serviceComponents.Any(x => x.Id == newEntity.Id));
        }

        [TestMethod]
        public void ServiceComponentService_Create_NewEntity_UnitOfWorkSaveCalled()
        {
            var newEntity = UnitTestHelper.GenerateRandomData<ServiceComponent>();
            _target.Create(newEntity);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentService_Update_EntityExists_EntityIsUpdated()
        {
            var newEntity = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.Id = ServiceComponentId;
            });

            _target.Update(newEntity);

            Assert.AreEqual(14, _serviceComponents.Count);
            Assert.IsTrue(_serviceComponents.Any(x => x.ComponentName == newEntity.ComponentName));
        }

        [TestMethod]
        public void ServiceComponentService_Update_EntityExists_UnitOfWorkSaveCalled()
        {
            var newEntity = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.Id = ServiceComponentId;
            });
            _target.Update(newEntity);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentService_GetByCustomer_CustomerIdSupplied_ReturnsOnlyThoseComponentsForCustomer()
        {
            var results = _target
                .GetByCustomer(CustomerId)
                .ToList();
            Assert.IsFalse(results.Any(x => x.ServiceFunction.ServiceDomain.ServiceDesk.CustomerId != CustomerId));
        }

        [TestMethod]
        public void ServiceComponentService_GetByCustomerWithHierarchy_CustomerIdSupplied_ReturnsOnlyThoseComponentsForCustomer()
        {
            var results = _target
                .GetByCustomerWithHierarchy(CustomerId)
                .ToList();
            Assert.IsFalse(results.Any(x => x.ServiceComponent.ServiceFunction.ServiceDomain.ServiceDesk.CustomerId != CustomerId));
        }

        [TestMethod]
        public void ServiceComponentService_GetResolverByCustomerWithHierarchy_CustomerIdSupplied_ReturnsOnlyThoseComponentsForCustomer()
        {
            var results = _target
                .GetResolverByCustomerWithHierarchy(CustomerId)
                .ToList();

            Assert.AreEqual(8, results.Count);
        }

        //[TestMethod]
        //[ExpectedException(typeof(TooManyRetriesException))]
        //public void ServiceComponentService_MoveResolver_SourceServiceComponentDoesNotExist_ThrowsAnException()
        //{
        //    _target.MoveResolver(CustomerId, ServiceComponentDoesNotExist, _serviceComponentMoveResolverDestination.Id);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(TooManyRetriesException))]
        //public void ServiceComponentService_MoveResolver_DestinationServiceComponentDoesNotExist_ThrowsAnException()
        //{
        //    _target.MoveResolver(CustomerId, _serviceComponentMoveResolverSource.Id, ServiceComponentDoesNotExist);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(TooManyRetriesException))]
        //public void ServiceComponentService_MoveResolver_DestinationServiceComponentHasDependencies_ThrowsAnException()
        //{
        //    _target.MoveResolver(CustomerId, _serviceComponentMoveResolverSource.Id, _serviceComponentMoveResolverDestinationWithDependencies.Id);
        //}

        [TestMethod]
        public void ServiceComponentService_MoveResolver_ResolverMoved_ExistingResolverTypeDeleted()
        {
            _target.MoveResolver(CustomerIdMoveResolver, _serviceComponentMoveResolverSource.Id, _serviceComponentMoveResolverDestination.Id);
            Assert.IsFalse(_resolvers.Any(x => x.ServiceComponent.Id == _serviceComponentMoveResolverSource.Id));
        }

        [TestMethod]
        public void ServiceComponentService_MoveResolver_ResolverMoved_NewResolverTypeInserted()
        {
            _target.MoveResolver(CustomerIdMoveResolver, _serviceComponentMoveResolverSource.Id, _serviceComponentMoveResolverDestination.Id);
            Assert.IsTrue(_resolvers.Any(x => x.ServiceComponent.Id == _serviceComponentMoveResolverDestination.Id));
        }

        [TestMethod]
        public void ServiceComponentService_MoveResolver_ResolverMoved_UnitOfWorkInvoked()
        {
            _target.MoveResolver(CustomerIdMoveResolver, _serviceComponentMoveResolverSource.Id, _serviceComponentMoveResolverDestination.Id);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_ReturnsListServiceOrganisationListItem()
        {
            #region Arrange

            #endregion

            #region Act

            var serviceOrganisationListItems = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Fujitsu);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(serviceOrganisationListItems, typeof(List<ServiceOrganisationListItem>));

            #endregion
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_CallsServiceComponentRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Fujitsu);

            #endregion

            #region Assert

            _mockServiceComponentRepository.Verify(x => x.Query(It.IsAny<Expression<Func<ServiceComponent, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_Fujitsu_ReturnsTwoServiceOrganisationListItems()
        {
            #region Arrange

            #endregion

            #region Act

            var serviceOrganisationListItems = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Fujitsu);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(serviceOrganisationListItems, typeof(List<ServiceOrganisationListItem>));
            Assert.AreEqual(2, serviceOrganisationListItems.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_Customer_ReturnsTwoServiceOrganisationListItems()
        {
            #region Arrange

            #endregion

            #region Act

            var serviceOrganisationListItems = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Customer);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(serviceOrganisationListItems, typeof(List<ServiceOrganisationListItem>));
            Assert.AreEqual(2, serviceOrganisationListItems.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_CustomerThirdParty_ReturnsTwoServiceOrganisationListItems()
        {
            #region Arrange

            #endregion

            #region Act

            var serviceOrganisationListItems = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.CustomerThirdParty);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(serviceOrganisationListItems, typeof(List<ServiceOrganisationListItem>));
            Assert.AreEqual(2, serviceOrganisationListItems.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_Fujitsu_DeliveryUnitResolverActivitiesSpecified_ReturnsCorrectResult()
        {
            var results = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Fujitsu).ToList();

            var resolver = results.FirstOrDefault(x => x.Resolver.Id == 1);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(Sdu, resolver.ServiceDeliveryUnitTypeName);
            Assert.AreEqual(ResolverGroup, resolver.ResolverGroupTypeName);
            Assert.AreEqual(ServiceActivity, resolver.ServiceActivities);
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_Fujitsu_NoDeliveryUnitNoResolverNoActivities_ReturnsNoneSpecified()
        {
            var results = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Fujitsu).ToList();

            var resolver = results.FirstOrDefault(x => x.Resolver.Id == 2);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(NoneSpecified, resolver.ServiceDeliveryUnitTypeName);
            Assert.AreEqual(NoneSpecified, resolver.ResolverGroupTypeName);
            Assert.AreEqual(NoneSpecified, resolver.ServiceActivities);
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_Customer_DeliveryUnitResolverActivitiesSpecified_ReturnsCorrectResult()
        {
            var results = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Customer).ToList();

            var resolver = results.FirstOrDefault(x => x.Resolver.Id == 3);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(Sdu, resolver.ServiceDeliveryUnitTypeName);
            Assert.AreEqual(ResolverGroup, resolver.ResolverGroupTypeName);
            Assert.AreEqual(ServiceActivity, resolver.ServiceActivities);
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_Customer_NoDeliveryUnitNoResolverNoActivities_ReturnsNoneSpecified()
        {
            var results = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.Customer).ToList();

            var resolver = results.FirstOrDefault(x => x.Resolver.Id == 4);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(NoneSpecified, resolver.ServiceDeliveryUnitTypeName);
            Assert.AreEqual(NoneSpecified, resolver.ResolverGroupTypeName);
            Assert.AreEqual(NoneSpecified, resolver.ServiceActivities);
        }

        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_CustomerThirdParty_DeliveryUnitResolverActivitiesSpecified_ReturnsCorrectResult()
        {
            var results = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.CustomerThirdParty).ToList();

            var resolver = results.FirstOrDefault(x => x.Resolver.Id == 5);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(Sdu, resolver.ServiceDeliveryUnitTypeName);
            Assert.AreEqual(ResolverGroup, resolver.ResolverGroupTypeName);
            Assert.AreEqual(ServiceActivity, resolver.ServiceActivities);
        }

        [TestMethod]
        public void ServiceComponentService_GetServiceOrganisationResolversByDesk_CustomerThirdParty_NoDeliveryUnitNoResolverNoActivities_ReturnsNoneSpecified()
        {
            var results = _target.GetServiceOrganisationResolversByDesk(1, ServiceDeliveryOrganisationNames.CustomerThirdParty).ToList();

            var resolver = results.FirstOrDefault(x => x.Resolver.Id == 6);

            Assert.IsNotNull(resolver);
            Assert.AreEqual(NoneSpecified, resolver.ServiceDeliveryUnitTypeName);
            Assert.AreEqual(NoneSpecified, resolver.ResolverGroupTypeName);
            Assert.AreEqual(NoneSpecified, resolver.ServiceActivities);
        }


        #region Helpers

        private static ServiceFunction CreateServiceFunctionWithCustomer(int customerId)
        {
            return UnitTestHelper.GenerateRandomData<ServiceFunction>(x =>
            {
                x.ServiceDomain = UnitTestHelper.GenerateRandomData<ServiceDomain>(y =>
                {
                    y.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(z =>
                    {
                        z.CustomerId = customerId;
                    });
                });
            });
        }

        private static void CreateServiceOrganisationResolver(ServiceComponent component, string organisationType, int id)
        {
            component.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(x =>
            {
                x.Id = id;
                x.ServiceDeliveryOrganisationType = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(
                    r =>
                    {
                        r.ServiceDeliveryOrganisationTypeName = organisationType;
                    });
                x.ServiceDeliveryUnitType = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>(y =>
                {
                    y.ServiceDeliveryUnitTypeName = Sdu;
                });
                x.ResolverGroupType = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(y =>
                {
                    y.ResolverGroupTypeName = ResolverGroup;
                });
                x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
            });
        }

        private static void CreateServiceOrganisationResolverNoResolverDependencies(ServiceComponent component, string organisationType, int id)
        {
            component.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(x =>
            {
                x.Id = id;
                x.ServiceDeliveryOrganisationType = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(
                    r =>
                    {
                        r.ServiceDeliveryOrganisationTypeName = organisationType;
                    });
                x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
            });
        }


        private static void CreateResolver(ServiceComponent component)
        {
            component.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(x =>
            {
                x.ServiceDeliveryOrganisationType = UnitTestHelper.GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>();
                x.ServiceDeliveryUnitType = UnitTestHelper.GenerateRandomData<ServiceDeliveryUnitTypeRefData>();
                x.ResolverGroupType = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>();
                x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
            });
        }

        #endregion
    }
}
