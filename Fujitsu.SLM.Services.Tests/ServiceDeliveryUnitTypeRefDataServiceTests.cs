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

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceDeliveryUnitTypeRefDataServiceTests
    {

        private Mock<IRepository<ServiceDeliveryUnitTypeRefData>> _mockServiceDeliveryUnitRefDataRepository;
        private Mock<IRepository<Resolver>> _mockServiceDeliveryUnitRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IParameterService> _mockParameterService;

        private IServiceDeliveryUnitTypeRefDataService _serviceDeliveryUnitRefDataService;
        private List<ServiceDeliveryUnitTypeRefData> _serviceDeliveryUnitTypeRefDatas;
        private List<Resolver> _resolvers;

        private const int ServiceDeliveryUnitTypeRefDataIdInUse = 1;
        private const int ServiceDeliveryUnitTypeRefDataIdNotInUse = 127;

        private const int CustomerId = 7;

        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _serviceDeliveryUnitTypeRefDatas = new List<ServiceDeliveryUnitTypeRefData>
            {
                new ServiceDeliveryUnitTypeRefData{Id=ServiceDeliveryUnitTypeRefDataIdInUse,ServiceDeliveryUnitTypeName= "ServiceDeliveryUnit A", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=2,ServiceDeliveryUnitTypeName= "ServiceDeliveryUnit B", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=3,ServiceDeliveryUnitTypeName= "ServiceDeliveryUnit C", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=4,ServiceDeliveryUnitTypeName= "ServiceDeliveryUnit D", SortOrder = 5, Visible = true},
                new ServiceDeliveryUnitTypeRefData{Id=5,ServiceDeliveryUnitTypeName= "ServiceDeliveryUnit E", SortOrder = 5, Visible = false},
                new ServiceDeliveryUnitTypeRefData{Id=ServiceDeliveryUnitTypeRefDataIdNotInUse,ServiceDeliveryUnitTypeName= "ServiceDeliveryUnit F", SortOrder = 5, Visible = false},
                new ServiceDeliveryUnitTypeRefData{Id=7,ServiceDeliveryUnitTypeName= "ServiceDeliveryUnit G", SortOrder = 5, Visible = true}
            };

            _resolvers = new List<Resolver>
            {
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas.First(y => y.Id == 2);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas.First(y => y.Id == 3);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas.First(y => y.Id == ServiceDeliveryUnitTypeRefDataIdInUse);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas.First(y => y.Id == 5);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = 1;
                    x.ServiceDeliveryUnitType = _serviceDeliveryUnitTypeRefDatas.First(y => y.Id == ServiceDeliveryUnitTypeRefDataIdInUse);
                }),
            };

            _mockServiceDeliveryUnitRefDataRepository = MockRepositoryHelper.Create(_serviceDeliveryUnitTypeRefDatas);

            _mockServiceDeliveryUnitRepository = MockRepositoryHelper.Create(_resolvers);

            _mockParameterService = new Mock<IParameterService>();

            _serviceDeliveryUnitRefDataService = new ServiceDeliveryUnitTypeRefDataService(_mockServiceDeliveryUnitRefDataRepository.Object,
                _mockServiceDeliveryUnitRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeliveryUnitTypeRefDataService_Constructor_NoServiceDeliveryUnitRefDataRepository()
        {
            new ServiceDeliveryUnitTypeRefDataService(null,
                _mockServiceDeliveryUnitRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeliveryUnitTypeRefDataService_Constructor_NoServiceDeliveryUnitRepository()
        {
            new ServiceDeliveryUnitTypeRefDataService(_mockServiceDeliveryUnitRefDataRepository.Object,
                null,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeliveryUnitTypeRefDataService_Constructor_NoParameterService()
        {
            new ServiceDeliveryUnitTypeRefDataService(_mockServiceDeliveryUnitRefDataRepository.Object,
                _mockServiceDeliveryUnitRepository.Object,
                null,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeliveryUnitTypeRefDataService_Constructor_NoUnitOfWork()
        {
            new ServiceDeliveryUnitTypeRefDataService(_mockServiceDeliveryUnitRefDataRepository.Object,
                _mockServiceDeliveryUnitRepository.Object,
                _mockParameterService.Object,
                null);
        }

        #endregion


        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var serviceDeliveryUnit = new ServiceDeliveryUnitTypeRefData
            {
                Id = 5,
                ServiceDeliveryUnitTypeName = "ServiceDeliveryUnit MJJ",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _serviceDeliveryUnitRefDataService.Create(serviceDeliveryUnit);

            #endregion

            #region Assert

            _mockServiceDeliveryUnitRefDataRepository.Verify(x => x.Insert(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(5, response);

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var component = new ServiceDeliveryUnitTypeRefData
            {
                Id = 4,
                ServiceDeliveryUnitTypeName = "ServiceDeliveryUnit MJJ",
            };

            #endregion

            #region Act

            _serviceDeliveryUnitRefDataService.Update(component);

            #endregion

            #region Assert

            _mockServiceDeliveryUnitRefDataRepository.Verify(x => x.Update(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var serviceDeliveryUnit = new ServiceDeliveryUnitTypeRefData
            {
                Id = 4,
            };

            #endregion

            #region Act

            _serviceDeliveryUnitRefDataService.Delete(serviceDeliveryUnit);

            #endregion

            #region Assert

            _mockServiceDeliveryUnitRefDataRepository.Verify(x => x.Delete(It.IsAny<ServiceDeliveryUnitTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDeliveryUnitRefDataService.All();

            #endregion

            #region Assert

            _mockServiceDeliveryUnitRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDeliveryUnitRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockServiceDeliveryUnitRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_IsServiceDeliveryUnitTypeReferenced_ServiceDeliveryUnitTypeInUse_ReturnsTrue()
        {
            Assert.IsTrue(_serviceDeliveryUnitRefDataService.IsServiceDeliveryUnitTypeReferenced(ServiceDeliveryUnitTypeRefDataIdInUse));
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_IsServiceDeliveryUnitTypeReferenced_ServiceDeliveryUnitTypeNotInUse_ReturnsFalse()
        {
            Assert.IsFalse(_serviceDeliveryUnitRefDataService.IsServiceDeliveryUnitTypeReferenced(ServiceDeliveryUnitTypeRefDataIdNotInUse));
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_AllVisibleIncluded()
        {
            var resultIds = _serviceDeliveryUnitRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();
            var expectedIds = _serviceDeliveryUnitTypeRefDatas
                .Where(x => x.Visible)
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(!expectedIds.Except(resultIds).Any());
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_NonVisibleIncludedForCustomer()
        {
            var resultIds = _serviceDeliveryUnitRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();
            Assert.AreEqual(6, resultIds.Count);
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_GetServiceDeliveryUnitTypeRefDataWithUsageStats_ReturnsCorrectType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _serviceDeliveryUnitRefDataService.GetServiceDeliveryUnitTypeRefDataWithUsageStats();

            #endregion

            #region Assert

            _mockServiceDeliveryUnitRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsInstanceOfType(result, typeof(List<ServiceDeliveryUnitTypeRefDataListItem>));

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_GetServiceDeliveryUnitTypeRefDataWithUsageStats_ReturnsCorrectUsageStats()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _serviceDeliveryUnitRefDataService.GetServiceDeliveryUnitTypeRefDataWithUsageStats().ToList();

            #endregion

            #region Assert

            _mockServiceDeliveryUnitRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(2, result.First(x => x.Id == ServiceDeliveryUnitTypeRefDataIdInUse).UsageCount);
            Assert.AreEqual(1, result.First(x => x.Id == 2).UsageCount);

            #endregion
        }
    }
}