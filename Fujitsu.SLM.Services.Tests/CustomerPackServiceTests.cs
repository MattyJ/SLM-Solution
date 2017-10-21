using System;
using System.Collections.Generic;
using System.Linq;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    public class CustomerPackServiceTests
    {
        private Mock<IRepository<CustomerPack>> _mockCustomerPackRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<CustomerPack> _customerPacks;

        private const int CustomerId = 666;
        private const int CustomerPackId = 1;

        private ICustomerPackService _target;
            
        [TestInitialize]
        public void Initialize()
        {
            _customerPacks = new List<CustomerPack>
            {
                UnitTestHelper.GenerateRandomData<CustomerPack>(x =>
                {
                    x.Id = CustomerPackId;
                    x.CustomerId = CustomerId;
                }),
                UnitTestHelper.GenerateRandomData<CustomerPack>(),
                UnitTestHelper.GenerateRandomData<CustomerPack>(x =>
                {
                    x.CustomerId = CustomerId;
                }),
                UnitTestHelper.GenerateRandomData<CustomerPack>()
            };

            _mockCustomerPackRepository = MockRepositoryHelper.Create(_customerPacks,
                (entity, id) => entity.Id == (int)id,
                (p1, p2) => p1.Id == p2.Id);

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _target = new CustomerPackService(_mockCustomerPackRepository.Object,
                _mockUnitOfWork.Object);
        }

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CustomerPackService_Ctor_CustomerPackRepositoryIsNull_ThrowsArgumentNullException()
        {
            new CustomerPackService(null,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerPackService_Ctor_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            new CustomerPackService(_mockCustomerPackRepository.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void CustomerPackService_CustomerPacks_ReturnsQueryable_ResultIsIQueryable()
        {
            var result = _target.CustomerPacks() as IQueryable<CustomerPack>;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CustomerPackService_GetById_CustomerPackExists_CustomerPackEntityReturned()
        {
            var result = _target.GetById(CustomerPackId);
            Assert.IsNotNull(result);
            Assert.AreEqual(CustomerPackId, result.Id);
        }

        [TestMethod]
        public void CustomerPackService_GetById_CustomerPackDoesNotExist_NoCustomerPackEntityReturned()
        {
            var result = _target.GetById(666);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CustomerPackService_GetByCustomerAndId_CustomerPackExists_CustomerPackEntityReturned()
        {
            var result = _target.GetByCustomerAndId(CustomerId, CustomerPackId);
            Assert.IsNotNull(result);
            Assert.AreEqual(CustomerId, result.CustomerId);
            Assert.AreEqual(CustomerPackId, result.Id);
        }

        [TestMethod]
        public void CustomerPackService_GetByCustomerAndId_CustomerPackDoesNotExist_NoCustomerPackEntityReturned()
        {
            var result = _target.GetByCustomerAndId(666, 666);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CustomerPackService_All_NoFilters_AllCustomerPacksReturned()
        {
            var result = _target.All();
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void CustomerPackService_Create_EntityPassedIn_InsertCalledOnRepository()
        {
            _target.Create(new CustomerPack());
            _mockCustomerPackRepository.Verify(v => v.Insert(It.IsAny<CustomerPack>()));
        }

        [TestMethod]
        public void CustomerPackService_Create_EntityPassedIn_UnitOfWorkSaveCalled()
        {
            _target.Create(new CustomerPack());
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void CustomerPackService_Update_EntityPassedIn_InsertCalledOnRepository()
        {
            _target.Update(new CustomerPack());
            _mockCustomerPackRepository.Verify(v => v.Update(It.IsAny<CustomerPack>()));
        }

        [TestMethod]
        public void CustomerPackService_Update_EntityPassedIn_UnitOfWorkSaveCalled()
        {
            _target.Create(new CustomerPack());
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void CustomerPackService_Delete_EntityPassedIn_InsertCalledOnRepository()
        {
            _target.Delete(new CustomerPack());
            _mockCustomerPackRepository.Verify(v => v.Delete(It.IsAny<CustomerPack>()));
        }

        [TestMethod]
        public void CustomerPackService_Delete_EntityPassedIn_UnitOfWorkSaveCalled()
        {
            _target.Delete(new CustomerPack());
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }
    }
}
