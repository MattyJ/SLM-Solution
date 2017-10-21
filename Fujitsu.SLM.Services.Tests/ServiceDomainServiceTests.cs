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
    public class ServiceDomainServiceTests
    {
        private Mock<IRepository<ServiceDomain>> _mockServiceDomainRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private IServiceDomainService _serviceDomainService;
        private List<ServiceDesk> _serviceDesks;
        private List<ServiceDomain> _serviceDomains;
        private List<DomainTypeRefData> _serviceDomainTypes;
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";


        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var dateTimeNow = DateTime.Now;

            _serviceDomainTypes = new List<DomainTypeRefData>
            {
                new DomainTypeRefData
                {
                    Id =1,
                    DomainName = "Computing & Device Services",
                    SortOrder = 5,
                    Visible = true
                },
                new DomainTypeRefData
                {
                    Id =2,
                    DomainName = "Computing Services",
                    SortOrder = 5,
                    Visible = true
                },
                new DomainTypeRefData
                {
                    Id =3,
                    DomainName = "Device Services",
                    SortOrder = 5,
                    Visible = true
                }
            };


            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id =1,
                    DeskName = "Desk A",
                    DeskInputTypes = new List<DeskInputType>(),
                    ServiceDomains = new List<ServiceDomain>(),
                    CustomerId = 1
                },
                new ServiceDesk
                {
                    Id =2,
                    DeskName = "Desk B",
                    CustomerId = 2
                },
                new ServiceDesk
                {
                    Id =3,
                    DeskName = "Desk C",
                    CustomerId = 3
                },
                new ServiceDesk
                {
                    Id =4,
                    DeskName = "Desk D",
                    CustomerId = 4
                },

            };

            _serviceDomains = new List<ServiceDomain>
            {
                new ServiceDomain
                {
                    Id = 1,
                    DomainType = _serviceDomainTypes.FirstOrDefault(x => x.Id == 1),
                    ServiceDesk = _serviceDesks.FirstOrDefault(x => x.Id == 1),
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                },
                new ServiceDomain
                {
                    Id = 2,
                    DomainType = _serviceDomainTypes.FirstOrDefault(x => x.Id == 2),
                    ServiceDesk = _serviceDesks.FirstOrDefault(x => x.Id == 1),
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                },
                new ServiceDomain
                {
                    Id = 3,
                    DomainType = _serviceDomainTypes.FirstOrDefault(x => x.Id == 3),
                    ServiceDesk = _serviceDesks.FirstOrDefault(x => x.Id == 1),
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                }
            };

            _mockServiceDomainRepository = MockRepositoryHelper.Create(_serviceDomains);

            _serviceDomainService = new ServiceDomainService(
                _mockServiceDomainRepository.Object, _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainService_Constructor_NoServiceDomainRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDomainService(
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDomainService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDomainService(
                _mockServiceDomainRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void ServiceDomainService_Create_CallInsertsDomainAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var serviceDomain = new ServiceDomain()
            {
                Id = 4,
                DomainType = _serviceDomainTypes.FirstOrDefault(x => x.Id == 1),
                ServiceDesk = new ServiceDesk() { Id = 1, DeskName = "Service Desk" },
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            var response = _serviceDomainService.Create(serviceDomain);

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.Insert(It.IsAny<ServiceDomain>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(4, response);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_Update_CallUpdateDomainAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var serviceDomain = new ServiceDomain()
            {
                Id = 3,
                DomainType = _serviceDomainTypes.FirstOrDefault(x => x.Id == 1),
                ServiceDesk = new ServiceDesk() { Id = 1, DeskName = "Service Desk" },
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            _serviceDomainService.Update(serviceDomain);

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.Update(It.IsAny<ServiceDomain>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDomainService.All();

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDomainService.GetById(1);

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_GetByCustomerAndId_EntityExistsButCustomerIdIncorrect_NothingReturned()
        {
            const int customerId = 2;
            const int serviceDomainId = 1;
            var entityExists = _serviceDomains.SingleOrDefault(x => x.Id == serviceDomainId);
            var customerExists = _serviceDomains.Any(x => x.ServiceDesk.CustomerId == customerId);
            var result = _serviceDomainService.GetByCustomerAndId(customerId, serviceDomainId);
            Assert.IsNotNull(entityExists);
            Assert.IsFalse(customerExists);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ServiceDomainService_GetByCustomerAndId_EntityIdIncorrectButCustomerIdCorrect_NothingReturned()
        {
            const int customerId = 1;
            const int serviceDomainId = 101;
            var entityExists = _serviceDomains.SingleOrDefault(x => x.Id == serviceDomainId);
            var customerExists = _serviceDomains.Any(x => x.ServiceDesk.CustomerId == customerId);
            var result = _serviceDomainService.GetByCustomerAndId(customerId, serviceDomainId);
            Assert.IsNull(entityExists);
            Assert.IsTrue(customerExists);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ServiceDomainService_GetByCustomerAndId_EntityIdCorrectAndCustomerIdCorrect_EntityReturned()
        {
            const int customerId = 1;
            const int serviceDomainId = 2;
            var entityExists = _serviceDomains.SingleOrDefault(x => x.Id == serviceDomainId);
            var customerExists = _serviceDomains.Any(x => x.ServiceDesk.CustomerId == customerId);
            var result = _serviceDomainService.GetByCustomerAndId(customerId, serviceDomainId);
            Assert.IsNotNull(entityExists);
            Assert.IsTrue(customerExists);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceDomainService_CustomerServiceDomains_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDomainService.CustomerServiceDomains(1);

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.Query(It.IsAny<Expression<Func<ServiceDomain, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_CustomerServiceDomains_ReturnsIQueryableServiceDomainListItem()
        {
            #region Arrange

            #endregion

            #region Act

            var domains = _serviceDomainService.CustomerServiceDomains(1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(domains, typeof(IQueryable<ServiceDomainListItem>));

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_ServiceDeskDomains_ReturnsIQueryableServiceDomainListItem()
        {
            #region Arrange

            #endregion

            #region Act

            var domains = _serviceDomainService.ServiceDeskDomains(1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(domains, typeof(IQueryable<ServiceDomainListItem>));

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_ServiceDeskDomains_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDomainService.ServiceDeskDomains(1);

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.Query(It.IsAny<Expression<Func<ServiceDomain, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_GetByCustomer_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDomainService.GetByCustomer(1);

            #endregion

            #region Assert

            _mockServiceDomainRepository.Verify(x => x.Query(It.IsAny<Expression<Func<ServiceDomain, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDomainService_GetByCustomer_ReturnsIQueryableServiceDomain()
        {
            #region Arrange

            #endregion

            #region Act

            var domains = _serviceDomainService.GetByCustomer(1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(domains, typeof(IQueryable<ServiceDomain>));

            #endregion
        }

    }
}
