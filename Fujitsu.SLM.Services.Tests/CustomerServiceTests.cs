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
using System.Linq.Expressions;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CustomerServiceTests
    {

        private Mock<IRepository<Customer>> _mockCustomerRepository;
        private Mock<IRepository<Contributor>> _mockContributorRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private ICustomerService _customerService;
        private List<Customer> _customers;
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";
        private const string UserNameTwo = "patrick.williams@uk.fujitsu.com";


        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var dateTimeNow = DateTime.Now;

            _customers = new List<Customer>
            {
                new Customer
                {
                    Id =1,
                    CustomerName = "3663",
                    CustomerNotes = "Some 3663 Customer Notes.",
                    Active = true,
                    AssignedArchitect = UserNameOne,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
                new Customer
                {
                    Id =2,
                    CustomerName = "ING",
                    CustomerNotes = "Some ING Customer Notes.",
                    Active = true,
                    AssignedArchitect = UserNameTwo,
                    InsertedBy = UserNameTwo,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameTwo,
                    UpdatedDate = dateTimeNow,
                },
                new Customer
                {
                    Id =3,
                    CustomerName = "ING InActive",
                    CustomerNotes = "Some ING Customer Notes.",
                    Active = false,
                    AssignedArchitect = UserNameOne,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
                new Customer
                {
                    Id =4,
                    CustomerName = "3663 MJJ",
                    CustomerNotes = "Some 3663 MJJ Customer Notes.",
                    Active = true,
                    AssignedArchitect = UserNameOne,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
            };


            _mockCustomerRepository = MockRepositoryHelper.Create(_customers, (entity, id) => entity.Id == (int)id);
            _mockContributorRepository = new Mock<IRepository<Contributor>>();

            _customerService = new CustomerService(
                _mockCustomerRepository.Object,
                _mockContributorRepository.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerService_Constructor_NoCustomerRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new CustomerService(
                null,
                _mockContributorRepository.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerService_Constructor_NoContributorRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new CustomerService(
                _mockCustomerRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new CustomerService(
                _mockCustomerRepository.Object,
                _mockContributorRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void CustomerService_Create_CallInsertCustomerAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var customer = new Customer
            {
                Id = 5,
                CustomerName = "A New Customer MJJ",
                CustomerNotes = "Some New Customer MJJ Notes.",
                Active = true,
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            var response = _customerService.Create(customer);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Insert(It.IsAny<Customer>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(_customers.Count, response);

            #endregion
        }

        [TestMethod]
        public void CustomerService_Update_CallUpdateCustomerAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var customer = new Customer
            {
                Id = 3,
                CustomerName = "ING",
                CustomerNotes = "Some Amended Notes.",
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            _customerService.Update(customer);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void CustomerService_Delete_CallDeleteCustomerAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var customer = new Customer
            {
                Id = 3,
            };

            #endregion

            #region Act

            _customerService.Delete(customer);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Delete(It.IsAny<Customer>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }


        [TestMethod]
        public void CustomerService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _customerService.All();

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _customerService.GetById(1);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerService_MyCustomers_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _customerService.MyCustomers(UserNameOne);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerService_MyCustomers_ReturnsIEnumerable()
        {
            #region Arrange

            #endregion

            #region Act

            var myCustomers = _customerService.MyCustomers(UserNameOne);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(myCustomers, typeof(IEnumerable<Customer>));

            #endregion
        }

        [TestMethod]
        public void CustomerService_MyCustomers_ReturnsTwoActiveCustomers()
        {
            #region Arrange

            #endregion

            #region Act

            var myCustomers = _customerService.MyCustomers(UserNameOne);

            #endregion

            #region Assert

            Assert.IsNotNull(myCustomers);
            Assert.AreEqual(2, myCustomers.Count());

            #endregion
        }

        [TestMethod]
        public void CustomerService_Customers_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _customerService.Customers();

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerService_Customers_ReturnsIQueryable()
        {
            #region Arrange

            #endregion

            #region Act

            var customers = _customerService.Customers();

            #endregion

            #region Assert

            Assert.IsInstanceOfType(customers, typeof(IQueryable<Customer>));

            #endregion
        }

        [TestMethod]
        public void CustomerService_Customers_ReturnsThreeActiveCustomers()
        {
            #region Arrange

            #endregion

            #region Act

            var customers = _customerService.Customers();

            #endregion

            #region Assert

            Assert.IsNotNull(customers);
            Assert.AreEqual(3, customers.Count());

            #endregion
        }

        [TestMethod]
        public void CustomerService_MyArchives_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _customerService.MyArchives(UserNameOne);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerService_MyArchives_ReturnsIQueryable()
        {
            #region Arrange

            #endregion

            #region Act

            var myArchives = _customerService.MyArchives(UserNameOne);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(myArchives, typeof(IQueryable<Customer>));

            #endregion
        }

        [TestMethod]
        public void CustomerService_MyArchives_ReturnsOneArchivedCustomer()
        {
            #region Arrange

            #endregion

            #region Act

            var myArchives = _customerService.MyArchives(UserNameOne);

            #endregion

            #region Assert

            Assert.IsNotNull(myArchives);
            Assert.AreEqual(1, myArchives.Count());

            #endregion
        }

        [TestMethod]
        public void CustomerService_Archives_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _customerService.Archives();

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Customer, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerService_Archives_ReturnsIQueryable()
        {
            #region Arrange

            #endregion

            #region Act

            var archives = _customerService.Archives();

            #endregion

            #region Assert

            Assert.IsInstanceOfType(archives, typeof(IQueryable<Customer>));

            #endregion
        }

        [TestMethod]
        public void CustomerService_Archives_ReturnsOneArchivedCustomer()
        {
            #region Arrange

            #endregion

            #region Act

            var archives = _customerService.Archives();

            #endregion

            #region Assert

            Assert.IsNotNull(archives);
            Assert.AreEqual(1, archives.Count());

            #endregion
        }

        [TestMethod]
        public void CustomerService_IsArchitectACustomerOwner_AssignedArchitectReturnsTrue()
        {
            var result = _customerService.IsArchitectACustomerOwner(UserNameOne);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CustomerService_IsArchitectACustomerOwner_UnAssignedArchitectReturnsFalse()
        {
            var result = _customerService.IsArchitectACustomerOwner("Brody.Jordan@uk.fujitsu.com");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CustomerService_IsArchitectACustomerOwner_ArchitectAssignedToInActiveCustomerReturnsFalse()
        {
            var userName = "Claudia.Jordan@uk.fujitsu.com";
            _customers.Add(
                new Customer
                {
                    Id = 1,
                    CustomerName = "3663 CDJ",
                    CustomerNotes = "Some 3663 Customer Notes.",
                    Active = false,
                    AssignedArchitect = userName,
                    InsertedBy = userName,
                    InsertedDate = DateTime.Now,
                    UpdatedBy = userName,
                    UpdatedDate = DateTime.Now,
                });

            var result = _customerService.IsArchitectACustomerOwner(userName);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CustomerService_IsArchitectACustomerOwner_ArchitectAssignedToActiveCustomerReturnsTrue()
        {
            var userName = "Claudia.Jordan@uk.fujitsu.com";
            _customers.Add(
                new Customer
                {
                    Id = 1,
                    CustomerName = "3663 CDJ",
                    CustomerNotes = "Some 3663 Customer Notes.",
                    Active = true,
                    AssignedArchitect = userName,
                    InsertedBy = userName,
                    InsertedDate = DateTime.Now,
                    UpdatedBy = userName,
                    UpdatedDate = DateTime.Now,
                });

            var result = _customerService.IsArchitectACustomerOwner(userName);

            Assert.IsTrue(result);
        }
    }
}
