using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DomainTypeRefDataServiceTests
    {

        private Mock<IRepository<DomainTypeRefData>> _mockDomainTypeRefDataRepository;
        private Mock<IRepository<ServiceDomain>> _mockServiceDomainRepository;
        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IParameterService> _mockParameterService;

        private IDomainTypeRefDataService _domainTypeRefDataService;
        private List<DomainTypeRefData> _domains;
        private List<ServiceDesk> _serviceDesks;
        private List<ServiceDomain> _serviceDomains;

        private const int CustomerId = 298;

        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";
        private const string UserNameTwo = "patrick.williams@uk.fujitsu.com";
        private const string UserNameThree = "joanne.jordan@uk.fujitsu.com";

        [TestInitialize]
        public void TestInitilize()
        {
            _domains = new List<DomainTypeRefData>
            {
                new DomainTypeRefData{Id=1,DomainName= "Domain A", SortOrder = 5,Visible = true},
                new DomainTypeRefData{Id=2,DomainName= "Domain B", SortOrder = 5,Visible = true},
                new DomainTypeRefData{Id=3,DomainName= "Domain C", SortOrder = 5,Visible = true},
                new DomainTypeRefData{Id=4,DomainName= "Domain D", SortOrder = 5,Visible = true},
                new DomainTypeRefData{Id=5,DomainName= "3663 Domain E", SortOrder = 5,Visible = false},
                new DomainTypeRefData{Id=6,DomainName= "Robeco Domain F", SortOrder = 5,Visible = false}
            };

            _serviceDomains = new List<ServiceDomain>
            {

                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.DomainType = _domains.First(y => y.Id == 1);
                    x.DomainTypeId = 1;
                    x.ServiceDesk = new ServiceDesk
                    {
                        Id = 1,
                        CustomerId = CustomerId,
                        Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                        {
                            c.Id = CustomerId;
                            c.AssignedArchitect = UserNameTwo;
                            c.Contributors = new List<Contributor>();
                        })
                    };
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.DomainType = _domains.First(y => y.Id == 2);
                    x.DomainTypeId = 2;
                    x.ServiceDesk = new ServiceDesk
                    {
                        Id =2,
                        CustomerId = 2,
                        Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                        {
                            c.Id = 2;
                            c.AssignedArchitect = UserNameTwo;
                            c.Contributors = new List<Contributor>();
                        })
                    };
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.DomainType = _domains.First(y => y.Id == 5);
                    x.DomainTypeId = 5;
                    x.ServiceDesk = new ServiceDesk
                    {
                        Id =3,
                        CustomerId = 3,
                        Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                        {
                            c.Id = 3;
                            c.AssignedArchitect = UserNameOne;
                            c.Contributors = new List<Contributor>
                            {
                                new Contributor
                                {
                                    Id = 1,
                                    CustomerId = 3,
                                    EmailAddress = UserNameThree
                                }
                            };
                        })
                    };
                }),
                UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                {
                    x.DomainType = _domains.First(y => y.Id == 1);
                    x.DomainTypeId = 1;
                    x.ServiceDesk = new ServiceDesk
                    {
                        Id =4,
                        CustomerId = 4,
                        Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                        {
                            c.Id = 4;
                            c.AssignedArchitect = UserNameTwo;
                            c.Contributors = new List<Contributor>();
                        })
                    };
                })
            };

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    CustomerId = CustomerId,
                    ServiceDomains = new List<ServiceDomain>
                    {
                        UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                        {
                            x.DomainType = _domains.First(y => y.Id == 1);
                            x.DomainTypeId = 1;
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                        {
                            x.DomainType = _domains.First(y => y.Id == 2);
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                        {
                            x.DomainType = _domains.First(y => y.Id == 5);
                        })
                    }
                },
                new ServiceDesk
                {
                    Customer = UnitTestHelper.GenerateRandomData<Customer>(),
                    ServiceDomains = new List<ServiceDomain>
                    {
                        UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                        {
                            x.DomainType = _domains.First(y => y.Id == 1);
                            x.DomainTypeId = 1;
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                        {
                            x.DomainType = _domains.First(y => y.Id == 2);
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceDomain>(x =>
                        {
                            x.DomainType = _domains.First(y => y.Id == 6);
                        })
                    }
                }
            };

            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks);

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockDomainTypeRefDataRepository = MockRepositoryHelper.Create(_domains);

            _mockServiceDomainRepository = MockRepositoryHelper.Create(_serviceDomains);

            _mockParameterService = new Mock<IParameterService>();

            _domainTypeRefDataService = new DomainTypeRefDataService(_mockDomainTypeRefDataRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DomainTypeRefDataService_Constructor_NoDomainRefDataRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new DomainTypeRefDataService(
                null,
                _mockServiceDeskRepository.Object,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DomainTypeRefDataService_Constructor_NoServiceDeskRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new DomainTypeRefDataService(
                _mockDomainTypeRefDataRepository.Object,
                null,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DomainTypeRefDataService_Constructor_NoServiceDomainRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new DomainTypeRefDataService(
                _mockDomainTypeRefDataRepository.Object,
                _mockServiceDeskRepository.Object,
                null,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DomainTypeRefDataService_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new DomainTypeRefDataService(
                _mockDomainTypeRefDataRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockServiceDomainRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DomainTypeRefDataService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new DomainTypeRefDataService(
                _mockDomainTypeRefDataRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void DomainTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var domain = new DomainTypeRefData()
            {
                Id = 5,
                DomainName = "MJJ Management",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _domainTypeRefDataService.Create(domain);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<DomainTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(5, response);

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var domain = new DomainTypeRefData()
            {
                Id = 4,
                DomainName = "MJJ Management",
            };

            #endregion

            #region Act

            _domainTypeRefDataService.Update(domain);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.Update(It.IsAny<DomainTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var domain = new DomainTypeRefData()
            {
                Id = 4,
            };

            #endregion

            #region Act

            _domainTypeRefDataService.Delete(domain);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<DomainTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _domainTypeRefDataService.All();

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _domainTypeRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_AllVisibleIncluded()
        {
            var resultIds = _domainTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();
            var expectedIds = _domains
                .Where(x => x.Visible)
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(!expectedIds.Except(resultIds).Any());
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_NonVisibleIncludedForCustomer()
        {
            var resultIds = _domainTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();
            Assert.AreEqual(5, resultIds.Count);
        }

        [TestMethod]
        public void DomainTypeRefDataService_DomainTypeReferences_CallsServiceDomainRepositoryFind()
        {
            #region Arrange

            #endregion

            #region Act

            var numberOfReferences = _domainTypeRefDataService.GetNumberOfDomainTypeReferences(1);

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.Find(It.IsAny<Expression<Func<ServiceDomain, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_DomainTypeReferences_DomainTypeReferencesReturnsTwoReferences()
        {
            #region Arrange

            #endregion

            #region Act

            var numberOfReferences = _domainTypeRefDataService.GetNumberOfDomainTypeReferences(1);

            #endregion

            #region Assert

            Assert.AreEqual(2, numberOfReferences);

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetDomainTypeRefDataWithUsageStats_ReturnsCorrectType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _domainTypeRefDataService.GetDomainTypeRefData(true, null);

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsInstanceOfType(result, typeof(List<DomainTypeRefDataListItem>));

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetDomainTypeRefDataAsAdmin_AdministratorCanEditAndDeleteAllDomainTypes()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _domainTypeRefDataService.GetDomainTypeRefData(true, null).ToList();

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsFalse(result.Any(x => x.CanEdit == false || x.CanDelete == false));

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetDomainTypeRefDataAsAdmin_NotAdminAssignedArchitectCanEditDomainType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _domainTypeRefDataService.GetDomainTypeRefData(false, UserNameOne).ToList();

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsTrue(result.Any(x => x.Id == 5 && x.CanEdit && x.CanDelete == false));

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetDomainTypeRefDataAsAdmin_NotAdminUnknownAssignedArchitectCannotEditOrDeleteAny()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _domainTypeRefDataService.GetDomainTypeRefData(false, "unknownArchitect@uk.fujitsu.com").ToList();

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsFalse(result.Any(x => x.CanEdit || x.CanDelete));

            #endregion
        }

        [TestMethod]
        public void DomainTypeRefDataService_GetDomainTypeRefDataAsAdmin_NotAdminContributorCanEditDomainType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _domainTypeRefDataService.GetDomainTypeRefData(false, UserNameThree).ToList();

            #endregion

            #region Assert

            _mockDomainTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsTrue(result.Any(x => x.Id == 5 && x.CanEdit && x.CanDelete == false));

            #endregion
        }
    }
}
