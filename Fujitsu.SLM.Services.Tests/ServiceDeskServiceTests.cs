using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceDeskServiceTests
    {

        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private IServiceDeskService _serviceDeskService;
        private List<ServiceDesk> _serviceDesks;
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";

        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var dateTimeNow = DateTime.Now;

            _serviceDesks = new List<ServiceDesk>
            {
                new ServiceDesk
                {
                    Id = 1,
                    DeskName = "Fujitsu ING Service Desk",
                    CustomerId = 1,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
                new ServiceDesk
                {
                    Id = 2,
                    DeskName = "Fujitsu 3663 Service Desk",
                    CustomerId = 2,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
                new ServiceDesk
                {
                    Id = 99,
                    DeskName = "Fujitsu 3663 Service Desk",
                    CustomerId = 2,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
            };


            _mockServiceDeskRepository = MockRepositoryHelper.Create(_serviceDesks, (entity, id) => entity.Id == (int)id);
            _mockDeskInputTypeRepository = new Mock<IRepository<DeskInputType>>();

            _serviceDeskService = new ServiceDeskService(
                _mockServiceDeskRepository.Object,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskService_Constructor_NoServiceDeskRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDeskService(
                null,
                _mockDeskInputTypeRepository.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskService_Constructor_NoDeskInputTypeRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDeskService(
                _mockServiceDeskRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeskService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDeskService(
                _mockServiceDeskRepository.Object,
                _mockDeskInputTypeRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void ServiceDeskService_Create_CallInsertServiceDeskAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var serviceDesk = new ServiceDesk
            {
                Id = 3,
                DeskName = "A New ServiceDesk MJJ",
                CustomerId = 1,
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            var response = _serviceDeskService.Create(serviceDesk);

            #endregion

            #region Assert

            _mockServiceDeskRepository.Verify(x => x.Insert(It.IsAny<ServiceDesk>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(3, response);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskService_Update_CallUpdateServiceDeskAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var serviceDesk = new ServiceDesk
            {
                Id = 2,
                CustomerId = 1,
                DeskName = "Fujitsu Service Desk",
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            _serviceDeskService.Update(serviceDesk);

            #endregion

            #region Assert

            _mockServiceDeskRepository.Verify(x => x.Update(It.IsAny<ServiceDesk>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ServiceDeskService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDeskService.All();

            #endregion

            #region Assert

            _mockServiceDeskRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDeskService.GetById(1);

            #endregion

            #region Assert

            _mockServiceDeskRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeskService_GetByCustomerAndId_ServiceDeskExistsButWithWrongCustomer_NoResultReturned()
        {
            var existsInTestData = _serviceDesks.Any(x => x.Id == 1 && x.CustomerId == 1);
            var result = _serviceDeskService.GetByCustomerAndId(1, 2);
            Assert.IsTrue(existsInTestData);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ServiceDeskService_GetByCustomerAndId_ServiceDeskExistsWithCorrectCustomer_ResultReturned()
        {
            var existsInTestData = _serviceDesks.Any(x => x.Id == 1 && x.CustomerId == 1);
            var result = _serviceDeskService.GetByCustomerAndId(1, 1);
            Assert.IsTrue(existsInTestData);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceDeskService_GetByCustomer_ServiceDeskExistsWithCorrectCustomer_ResultReturned()
        {
            var result = _serviceDeskService.GetByCustomer(2);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
}