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
    public class ResolverGroupTypeRefDataServiceTests
    {

        private Mock<IRepository<ResolverGroupTypeRefData>> _mockResolverGroupTypeRefDataRepository;
        private Mock<IRepository<Resolver>> _mockResolverRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IParameterService> _mockParameterService;

        private IResolverGroupTypeRefDataService _resolverGroupTypeRefDataService;
        private List<ResolverGroupTypeRefData> _resolverGroupTypeRefDatas;
        private List<Resolver> _resolvers;

        private const int ResolverGroupTypeRefDataIdInUse = 1;
        private const int ResolverGroupTypeRefDataIdNotInUse = 6;

        private const int CustomerId = 7;

        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _resolverGroupTypeRefDatas = new List<ResolverGroupTypeRefData>
            {
                new ResolverGroupTypeRefData{Id=ResolverGroupTypeRefDataIdInUse,ResolverGroupTypeName= "Resolver Group A", SortOrder = 5,Visible = true},
                new ResolverGroupTypeRefData{Id=2,ResolverGroupTypeName= "Resolver Group B", SortOrder = 5, Visible = true},
                new ResolverGroupTypeRefData{Id=3,ResolverGroupTypeName= "Resolver Group C", SortOrder = 5, Visible = true},
                new ResolverGroupTypeRefData{Id=4,ResolverGroupTypeName= "Resolver Group D", SortOrder = 5, Visible = true},
                new ResolverGroupTypeRefData{Id=5,ResolverGroupTypeName= "Resolver Group E", SortOrder = 5, Visible = false},
                new ResolverGroupTypeRefData{Id=ResolverGroupTypeRefDataIdNotInUse,ResolverGroupTypeName= "Resolver Group F", SortOrder = 5, Visible = false},
                new ResolverGroupTypeRefData{Id=7,ResolverGroupTypeName= "Resolver Group G", SortOrder = 5, Visible = true}
            };

            _resolvers = new List<Resolver>
            {
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 1;
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.First(y => y.Id == 2);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 2;
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.First(y => y.Id == 3);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 3;
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.First(y => y.Id == ResolverGroupTypeRefDataIdInUse);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 4;
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = CustomerId;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.First(y => y.Id == 5);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 5;
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = 1;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.First(y => y.Id == 7);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = 6;
                    x.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    x.ServiceDesk.CustomerId = 2;
                    x.ResolverGroupType = _resolverGroupTypeRefDatas.First(y => y.Id == ResolverGroupTypeRefDataIdInUse);
                }),
            };

            _mockResolverGroupTypeRefDataRepository = MockRepositoryHelper.Create(_resolverGroupTypeRefDatas);

            _mockResolverRepository = MockRepositoryHelper.Create(_resolvers);

            _mockParameterService = new Mock<IParameterService>();

            _resolverGroupTypeRefDataService = new ResolverGroupTypeRefDataService(_mockResolverGroupTypeRefDataRepository.Object,
                _mockResolverRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverGroupTypeRefDataService_Constructor_NoResolverGroupRefDataRepository()
        {
            new ResolverGroupTypeRefDataService(null,
                _mockResolverRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverGroupTypeRefDataService_Constructor_NoResolverGroupRepository()
        {
            new ResolverGroupTypeRefDataService(_mockResolverGroupTypeRefDataRepository.Object,
                null,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverGroupTypeRefDataService_Constructor_NoParameterService()
        {
            new ResolverGroupTypeRefDataService(_mockResolverGroupTypeRefDataRepository.Object,
                _mockResolverRepository.Object,
                null,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverGroupTypeRefDataService_Constructor_NoUnitOfWork()
        {
            new ResolverGroupTypeRefDataService(_mockResolverGroupTypeRefDataRepository.Object,
                _mockResolverRepository.Object,
                _mockParameterService.Object,
                null);
        }

        #endregion


        [TestMethod]
        public void ResolverGroupTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var resolverGroup = new ResolverGroupTypeRefData
            {
                Id = 5,
                ResolverGroupTypeName = "Resolver MJJ",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _resolverGroupTypeRefDataService.Create(resolverGroup);

            #endregion

            #region Assert

            _mockResolverGroupTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<ResolverGroupTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(5, response);

            #endregion
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var component = new ResolverGroupTypeRefData
            {
                Id = 4,
                ResolverGroupTypeName = "Resolver MJJ",
            };

            #endregion

            #region Act

            _resolverGroupTypeRefDataService.Update(component);

            #endregion

            #region Assert

            _mockResolverGroupTypeRefDataRepository.Verify(x => x.Update(It.IsAny<ResolverGroupTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var resolverGroup = new ResolverGroupTypeRefData
            {
                Id = 4,
            };

            #endregion

            #region Act

            _resolverGroupTypeRefDataService.Delete(resolverGroup);

            #endregion

            #region Assert

            _mockResolverGroupTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<ResolverGroupTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _resolverGroupTypeRefDataService.All();

            #endregion

            #region Assert

            _mockResolverGroupTypeRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _resolverGroupTypeRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockResolverGroupTypeRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_IsResolverGroupTypeReferenced_ResolverGroupTypeInUse_ReturnsTrue()
        {
            Assert.IsTrue(_resolverGroupTypeRefDataService.GetNumberOfResolverGroupTypeReferences(ResolverGroupTypeRefDataIdInUse) > 0);
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_IsResolverGroupTypeReferenced_ResolverGroupTypeNotInUse_ReturnsFalse()
        {
            Assert.IsFalse(_resolverGroupTypeRefDataService.GetNumberOfResolverGroupTypeReferences(ResolverGroupTypeRefDataIdNotInUse) > 0);
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_AllVisibleIncluded()
        {
            var resultIds = _resolverGroupTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();
            var expectedIds = _resolverGroupTypeRefDatas
                .Where(x => x.Visible)
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(!expectedIds.Except(resultIds).Any());
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_NonVisibleIncludedForCustomer()
        {
            var resultIds = _resolverGroupTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();
            Assert.AreEqual(6, resultIds.Count);
        }

        [TestMethod]
        public void ResolverGroupRefDataService_GetResolverGroupTypeRefDataWithUsageStats_ReturnsCorrectType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _resolverGroupTypeRefDataService.GetResolverGroupTypeRefDataWithUsageStats();

            #endregion

            #region Assert

            _mockResolverGroupTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsInstanceOfType(result, typeof(List<ResolverGroupTypeRefDataListItem>));

            #endregion
        }

        [TestMethod]
        public void ResolverGroupRefDataService_GetResolverGroupTypeRefDataWithUsageStats_ReturnsCorrectUsageStats()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _resolverGroupTypeRefDataService.GetResolverGroupTypeRefDataWithUsageStats().ToList();

            #endregion

            #region Assert

            _mockResolverGroupTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(2, result.First(x => x.Id == 1).UsageCount);
            Assert.AreEqual(1, result.First(x => x.Id == 2).UsageCount);
            Assert.AreEqual(0, result.First(x => x.Id == ResolverGroupTypeRefDataIdNotInUse).UsageCount);

            #endregion
        }
    }
}