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
    public class OperationalProcessTypeRefDataServiceTests
    {

        private Mock<IRepository<OperationalProcessTypeRefData>> _mockOperationalProcessTypeRefDataRepository;
        private Mock<IRepository<OperationalProcessType>> _mockOperationalProcessTypeRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private IOperationalProcessTypeRefDataService _operationalProcessTypeRefDataService;
        private List<OperationalProcessTypeRefData> _operationalProcessTypeRefDatas;
        private List<OperationalProcessType> _operationalProcessTypes;

        private const int OperationalProcessTypeIdInUse = 1;
        private const int OperationalProcessTypeIdNotInUse = 4;

        private const int CustomerId = 298;

        [TestInitialize]
        public void TestInitilize()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _operationalProcessTypeRefDatas = new List<OperationalProcessTypeRefData>
            {
                new OperationalProcessTypeRefData{Id=OperationalProcessTypeIdInUse,OperationalProcessTypeName= "OperationalProcess A", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=2,OperationalProcessTypeName= "OperationalProcess B", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=3,OperationalProcessTypeName= "OperationalProcess C", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=OperationalProcessTypeIdNotInUse,OperationalProcessTypeName= "OperationalProcess D", SortOrder = 5, Visible=true},
                new OperationalProcessTypeRefData{Id=5,OperationalProcessTypeName= "3663 OperationalProcess D", SortOrder = 5, Visible=false},
                new OperationalProcessTypeRefData{Id=6,OperationalProcessTypeName= "Robeco OperationalProcess A", SortOrder = 5, Visible=false},
                new OperationalProcessTypeRefData{Id=7,OperationalProcessTypeName= "Robeco OperationalProcess B", SortOrder = 5, Visible=false}
            };

            _operationalProcessTypes = new List<OperationalProcessType>
            {
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Id = 1;
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(o => o.Id == OperationalProcessTypeIdInUse);
                    x.OperationalProcessTypeRefDataId = OperationalProcessTypeIdInUse;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(y =>
                    {
                        y.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                        y.ServiceDesk.CustomerId = CustomerId;
                    });
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Id = 2;
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(o => o.Id == 2);
                    x.OperationalProcessTypeRefDataId = 2;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(y =>
                    {
                        y.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                        y.ServiceDesk.CustomerId = CustomerId;
                    });
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Id = 3;
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(o => o.Id == OperationalProcessTypeIdInUse);
                    x.OperationalProcessTypeRefDataId = OperationalProcessTypeIdInUse;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(y =>
                    {
                        y.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                        y.ServiceDesk.CustomerId = CustomerId;
                    });
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Id = 4;
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(o => o.Id == 5);
                    x.OperationalProcessTypeRefDataId = 5;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(y =>
                    {
                        y.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                        y.ServiceDesk.CustomerId = CustomerId;
                    });
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Id = 5;
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas.First(o => o.Id == 6);
                    x.OperationalProcessTypeRefDataId = 6;
                    x.Resolver = UnitTestHelper.GenerateRandomData<Resolver>(y =>
                    {
                        y.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>();
                    });
                })
            };

            _mockOperationalProcessTypeRefDataRepository = MockRepositoryHelper.Create(_operationalProcessTypeRefDatas);

            _mockOperationalProcessTypeRepository = MockRepositoryHelper.Create(_operationalProcessTypes);

            _operationalProcessTypeRefDataService = new OperationalProcessTypeRefDataService(_mockOperationalProcessTypeRefDataRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _unitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationalProcessTypeRefDataService_Constructor_NoOperationalProcessRefDataRepository_ThrowsException()
        {
            new OperationalProcessTypeRefDataService(null,
                _mockOperationalProcessTypeRepository.Object,
                _unitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationalProcessTypeRefDataService_Constructor_NoOperationalProcessTypeRepository_ThrowsException()
        {
            new OperationalProcessTypeRefDataService(_mockOperationalProcessTypeRefDataRepository.Object,
                null,
                _unitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationalProcessTypeRefDataService_Constructor_NoUnitOfWork_ThrowsException()
        {
            new OperationalProcessTypeRefDataService(_mockOperationalProcessTypeRefDataRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                null);
        }

        #endregion


        [TestMethod]
        public void OperationalProcessTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var operationalProcessType = new OperationalProcessTypeRefData
            {
                Id = 5,
                OperationalProcessTypeName = "OperationalProcess MJJ",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _operationalProcessTypeRefDataService.Create(operationalProcessType);

            #endregion

            #region Assert

            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<OperationalProcessTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(5, response);

            #endregion
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var operationalProcess = new OperationalProcessTypeRefData()
            {
                Id = 4,
                OperationalProcessTypeName = "OperationalProcess MJJ",
            };

            #endregion

            #region Act

            _operationalProcessTypeRefDataService.Update(operationalProcess);

            #endregion

            #region Assert

            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.Update(It.IsAny<OperationalProcessTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var operationalProcess = new OperationalProcessTypeRefData()
            {
                Id = 4,
            };

            #endregion

            #region Act

            _operationalProcessTypeRefDataService.Delete(operationalProcess);

            #endregion

            #region Assert

            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<OperationalProcessTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _operationalProcessTypeRefDataService.All();

            #endregion

            #region Assert

            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _operationalProcessTypeRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_PurgeOrphans_Calls_Delete_Save()
        {
            _mockOperationalProcessTypeRefDataRepository.Setup(x => x.Delete(It.IsAny<OperationalProcessTypeRefData>())).Verifiable();
            _operationalProcessTypeRefDataService.PurgeOrphans();
            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<OperationalProcessTypeRefData>()), Times.Exactly(1));
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(2));
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_IsOperationalProcessTypeReferenced_OperationalProcessTypeInUse_ReturnsTrue()
        {
            Assert.IsTrue(_operationalProcessTypeRefDataService.GetNumberOfOperationalProcessTypeReferences(OperationalProcessTypeIdInUse) > 0);
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_IsOperationalProcessTypeReferenced_OperationalProcessTypeNotInUse_ReturnsFalse()
        {
            Assert.IsFalse(_operationalProcessTypeRefDataService.GetNumberOfOperationalProcessTypeReferences(OperationalProcessTypeIdNotInUse) > 0);
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_AllVisibleIncluded()
        {
            var resultIds = _operationalProcessTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();

            var expectedIds = _operationalProcessTypeRefDatas
                .Where(x => x.Visible)
                .Select(s => s.Id)
                .ToList();

            Assert.IsTrue(!expectedIds.Except(resultIds).Any());
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_NonVisibleIncludedForCustomer()
        {
            var resultIds = _operationalProcessTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();

            Assert.AreEqual(5, resultIds.Count);
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_GetOperationalProcessRefDataWithUsageStats_ReturnsCorrectType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _operationalProcessTypeRefDataService.GetOperationalProcessTypeRefDataWithUsageStats();

            #endregion

            #region Assert

            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsInstanceOfType(result, typeof(List<OperationalProcessTypeRefDataListItem>));

            #endregion
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataService_GetOperationalProcessTypeRefDataWithUsageStats_ReturnsCorrectUsageStats()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _operationalProcessTypeRefDataService.GetOperationalProcessTypeRefDataWithUsageStats().ToList();

            #endregion

            #region Assert

            _mockOperationalProcessTypeRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(2, result.First(x => x.Id == 1).UsageCount);
            Assert.AreEqual(1, result.First(x => x.Id == 2).UsageCount);
            Assert.AreEqual(0, result.First(x => x.Id == OperationalProcessTypeIdNotInUse).UsageCount);

            #endregion
        }
    }
}
