using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    public class AuditServiceTests
    {
        private Mock<IRepository<Audit>> _mockAuditRepository;
        private Mock<IRepository<Customer>> _mockCustomerRepository;
        private Mock<IUserIdentity> _mockUserIdentity;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Audit> _audits;

        private const int AuditId = 1;

        private IAuditService _target;

        private const string UserName = "matthew.jordan@uk.fujitsu.com";

        [TestInitialize]
        public void Initialize()
        {
            _audits = new List<Audit>
            {
                UnitTestHelper.GenerateRandomData<Audit>(x =>
                {
                    x.Id = AuditId;
                }),
                UnitTestHelper.GenerateRandomData<Audit>(),
                UnitTestHelper.GenerateRandomData<Audit>(),
                UnitTestHelper.GenerateRandomData<Audit>()
            };

            _mockAuditRepository = MockRepositoryHelper.Create(_audits,
                (entity, id) => entity.Id == (int)id,
                (p1, p2) => p1.Id == p2.Id);

            _mockCustomerRepository = new Mock<IRepository<Customer>>();

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns(UserName);

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _target = new AuditService(_mockAuditRepository.Object,
                _mockCustomerRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AuditService_Ctor_AuditRepositoryIsNull_ThrowsArgumentNullException()
        {
            new AuditService(null,
                _mockCustomerRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AuditService_Ctor_CustomerRepositoryIsNull_ThrowsArgumentNullException()
        {
            new AuditService(_mockAuditRepository.Object,
                null,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AuditService_Ctor_UserIdentityIsNull_ThrowsArgumentNullException()
        {
            new AuditService(_mockAuditRepository.Object,
                _mockCustomerRepository.Object,
                null,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AuditService_Ctor_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            new AuditService(_mockAuditRepository.Object,
                _mockCustomerRepository.Object,
                _mockUserIdentity.Object,
                null);
        }

        #endregion


        [TestMethod]
        public void AuditService_GetById_AuditExists_AuditEntityReturned()
        {
            var result = _target.GetById(AuditId);
            Assert.IsNotNull(result);
            Assert.AreEqual(AuditId, result.Id);
        }

        [TestMethod]
        public void AuditService_GetById_AuditDoesNotExist_NoAuditEntityReturned()
        {
            var result = _target.GetById(666);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuditService_All_NoFilters_AllAuditsReturned()
        {
            var result = _target.All();
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void AuditService_Create_EntityPassedIn_InsertCalledOnRepository()
        {
            _target.Create(new Audit());
            _mockAuditRepository.Verify(v => v.Insert(It.IsAny<Audit>()));
        }

        [TestMethod]
        public void AuditService_Create_EntityPassedIn_UnitOfWorkSaveCalled()
        {
            _target.Create(new Audit());
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void AuditService_Update_EntityPassedIn_InsertCalledOnRepository()
        {
            _target.Update(new Audit());
            _mockAuditRepository.Verify(v => v.Update(It.IsAny<Audit>()));
        }

        [TestMethod]
        public void AuditService_Update_EntityPassedIn_UnitOfWorkSaveCalled()
        {
            _target.Create(new Audit());
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void AuditService_Delete_EntityPassedIn_InsertCalledOnRepository()
        {
            _target.Delete(new Audit());
            _mockAuditRepository.Verify(v => v.Delete(It.IsAny<Audit>()));
        }

        [TestMethod]
        public void AuditService_Delete_EntityPassedIn_UnitOfWorkSaveCalled()
        {
            _target.Delete(new Audit());
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }
    }
}
