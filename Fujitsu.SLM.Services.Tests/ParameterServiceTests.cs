using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    public class ParameterServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IUserIdentity> _mockUserIdentity;
        private Mock<ICacheManager> _mockCacheManager;
        private Mock<IRepository<Parameter>> _mockParameterRepository;
        private const string UserName = "mark.hart@uk.fujitsu.com";
        private List<Parameter> _parameters;
        private IParameterService _target;
        private const int ExistingId = 10;
        private const string ExistingParameterName = "HanSolo";
        private const string ExistingParameterValue = "10.867";

        [TestInitialize]
        public void Initialize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockCacheManager = new Mock<ICacheManager>();

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns(UserName);

            _parameters = new List<Parameter>
            {
                UnitTestHelper.GenerateRandomData<Parameter>(),
                UnitTestHelper.GenerateRandomData<Parameter>(),
                UnitTestHelper.GenerateRandomData<Parameter>(x => x.Id = ExistingId),
                UnitTestHelper.GenerateRandomData<Parameter>(x =>
                {
                    x.ParameterName = ExistingParameterName;
                    x.ParameterValue = ExistingParameterValue;
                }),
                UnitTestHelper.GenerateRandomData<Parameter>()
            };

            _mockParameterRepository = MockRepositoryHelper.Create(_parameters,
                (entity, id) => entity.Id == (int)id,
                (p1, p2) => p1.Id == p2.Id);

            _target = new ParameterService(_mockParameterRepository.Object,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object,
                _mockCacheManager.Object);
        }

        #region Ctor
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Ctor_ParameterRepositoryIsNull_ThrowsException()
        {
            new ParameterService(null,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object,
                _mockCacheManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Ctor_UnitOfWorkIsNull_ThrowsException()
        {
            new ParameterService(_mockParameterRepository.Object,
                null,
                _mockUserIdentity.Object,
                _mockCacheManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Ctor_UserIdentityIsNull_ThrowsException()
        {
            new ParameterService(_mockParameterRepository.Object,
                _mockUnitOfWork.Object,
                null,
                _mockCacheManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParameterService_Ctor_CacheManagerIsNull_ThrowsException()
        {
            new ParameterService(_mockParameterRepository.Object,
                _mockUnitOfWork.Object,
                _mockUserIdentity.Object,
                null);
        }
        #endregion

        [TestMethod]
        public void ParameterService_All_ReturnsAllParameters()
        {
            var result = _target.All();
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public void ParameterService_Create_NewParameterEntityPassed_IsAddedToCollection()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>(x => x.ParameterName = "XXX");
            _target.Create(p);
            Assert.IsTrue(_parameters.Any(x => x.ParameterName == "XXX"));
        }

        [TestMethod]
        public void ParameterService_Create_NewParameterEntityPassed_UnitOfWorkCalled()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>(x => x.ParameterName = "XXX");
            _target.Create(p);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ParameterService_Update_ExistingParameterEntityPassed_CacheIsInvalidated()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>();
            _target.Update(p);
            _mockCacheManager.Verify(v => v.Remove(p.ParameterName));
        }

        [TestMethod]
        public void ParameterService_Update_ExistingParameterEntityPassed_ParameterIsUpdated()
        {
            var existing = _parameters.First(x => x.Id == ExistingId);
            var p = UnitTestHelper.GenerateRandomData<Parameter>(x =>
            {
                x.Id = existing.Id;
                x.ParameterName = existing.ParameterName;
                x.ParameterValue = "XXX666";
            });
            _target.Update(p);
            existing = existing = _parameters.First(x => x.Id == ExistingId);
            Assert.AreEqual("XXX666", existing.ParameterValue);
        }

        [TestMethod]
        public void ParameterService_Update_ExistingParameterEntityPassed_UnitOfWorkCalled()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>();
            _target.Update(p);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ParameterService_Delete_ExistingParameterEntityPassed_IsRemovedFromCollection()
        {
            var p = _parameters.First(x => x.Id == ExistingId);
            _target.Delete(p);
            Assert.AreEqual(4, _parameters.Count());
        }

        [TestMethod]
        public void ParameterService_Delete_ParameterEntityDoesNotExist_NothingIsRemovedFromCollection()
        {
            var p = UnitTestHelper.GenerateRandomData<Parameter>();
            _target.Delete(p);
            Assert.AreEqual(5, _parameters.Count());
        }

        [TestMethod]
        public void ParameterService_Delete_ExistingParameterEntityPassed_UnitOfWorkCalled()
        {
            var p = _parameters.First(x => x.Id == ExistingId);
            _target.Delete(p);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ParameterService_Find_ParameterNameExists_ParameterEntityIsReturned()
        {
            var expected = _parameters.Single(x => x.ParameterName == ExistingParameterName);
            var result = _target.Find(ExistingParameterName);
            Assert.AreEqual(expected.Id, result.Id);
        }

        [TestMethod]
        public void ParameterService_Find_ParameterNameDoesNotExists_NullIsReturned()
        {
            var result = _target.Find("XXX666");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ParameterService_GetParameterByName_ParameterNameExists_ReturnedTypeIsDecimal()
        {
            var result = _target.GetParameterByName<decimal>(ExistingParameterName);
            Assert.IsInstanceOfType(result, typeof(decimal));
        }

        [TestMethod]
        public void ParameterService_GetParameterByName_ParameterNameExists_ReturnedTypeIsExpectedValue()
        {
            var expected = Decimal.Parse(ExistingParameterValue);
            var result = _target.GetParameterByName<decimal>(ExistingParameterName);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameExists_ReturnedTypeIsExpectedValue()
        {
            var expected = Decimal.Parse(ExistingParameterValue);
            var result = _target.GetParameterByNameOrCreate<decimal>(ExistingParameterName, 6, ParameterType.Admin);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryIsCreated()
        {
            const string parameterName = "XXX666";
            _target.GetParameterByNameOrCreate<decimal>("XXX666", 6, ParameterType.Admin);
            Assert.IsTrue(_parameters.Any(x => x.ParameterName == parameterName));
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasCorrectParameterValue()
        {
            const string parameterName = "XXX666";
            _target.GetParameterByNameOrCreate<decimal>("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual("6", result.ParameterValue);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasCorrectType()
        {
            const string parameterName = "XXX666";
            _target.GetParameterByNameOrCreate<decimal>("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(ParameterType.Admin, result.Type);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasInsertedBy()
        {
            const string parameterName = "XXX666";
            _target.GetParameterByNameOrCreate<decimal>("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(UserName, result.InsertedBy);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasInsertedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _target.GetParameterByNameOrCreate<decimal>("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(now.Year, result.InsertedDate.Year);
            Assert.AreEqual(now.Month, result.InsertedDate.Month);
            Assert.AreEqual(now.Day, result.InsertedDate.Day);
            Assert.AreEqual(now.Hour, result.InsertedDate.Hour);
            Assert.AreEqual(now.Minute, result.InsertedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedBy()
        {
            const string parameterName = "XXX666";
            _target.GetParameterByNameOrCreate<decimal>("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(UserName, result.UpdatedBy);
        }

        [TestMethod]
        public void ParameterService_GetParameterByNameOrCreate_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _target.GetParameterByNameOrCreate<decimal>("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(now.Year, result.UpdatedDate.Year);
            Assert.AreEqual(now.Month, result.UpdatedDate.Month);
            Assert.AreEqual(now.Day, result.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, result.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, result.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameExist_CacheIsInvalidated()
        {
            const string parameterName = "XXX666";
            _target.SaveParameter(parameterName, 6, ParameterType.Admin);
            _mockCacheManager.Verify(v => v.Remove(parameterName));
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterIsCreated()
        {
            const string parameterName = "XXX666";
            _target.SaveParameter(parameterName, 6, ParameterType.Admin);
            Assert.IsTrue(_parameters.Any(x => x.ParameterName == parameterName));
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterHasCorrectType()
        {
            const string parameterName = "XXX666";
            _target.SaveParameter(parameterName, 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(ParameterType.Admin, result.Type);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterHasCorrectValue()
        {
            const string parameterName = "XXX666";
            _target.SaveParameter(parameterName, 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual("6", result.ParameterValue);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasInsertedBy()
        {
            const string parameterName = "XXX666";
            _target.SaveParameter("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(UserName, result.InsertedBy);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasInsertedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _target.SaveParameter("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(now.Year, result.InsertedDate.Year);
            Assert.AreEqual(now.Month, result.InsertedDate.Month);
            Assert.AreEqual(now.Day, result.InsertedDate.Day);
            Assert.AreEqual(now.Hour, result.InsertedDate.Hour);
            Assert.AreEqual(now.Minute, result.InsertedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedBy()
        {
            const string parameterName = "XXX666";
            _target.SaveParameter("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(UserName, result.UpdatedBy);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_NewParameterEntryHasUpdatedDate()
        {
            var now = DateTime.Now;
            const string parameterName = "XXX666";
            _target.SaveParameter("XXX666", 6, ParameterType.Admin);
            var result = _parameters.Single(x => x.ParameterName == parameterName);
            Assert.AreEqual(now.Year, result.UpdatedDate.Year);
            Assert.AreEqual(now.Month, result.UpdatedDate.Month);
            Assert.AreEqual(now.Day, result.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, result.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, result.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ParameterService_SaveParameter_ParameterNameDoesNotExist_UnitOfWorkIsCalled()
        {
            _target.SaveParameter("XXX666", 6, ParameterType.Admin);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }
    }
}