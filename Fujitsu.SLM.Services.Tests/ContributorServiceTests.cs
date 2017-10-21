using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ContributorServiceTests
    {

        private Mock<IRepository<Contributor>> _mockContributorRepository;

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private IContributorService _contributorService;
        private List<Contributor> _contributors;
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";
        private const string UserNameTwo = "patrick.williams@uk.fujitsu.com";
        private static readonly string UserIdOne = Guid.NewGuid().ToString();
        private static readonly string UserIdTwo = Guid.NewGuid().ToString();


        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var dateTimeNow = DateTime.Now;

            _contributors = new List<Contributor>
            {
                new Contributor
                {
                    Id =1,
                    CustomerId = 1,
                    UserId = UserIdOne,
                    EmailAddress = UserNameOne,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
                 new Contributor
                {
                    Id =1,
                    CustomerId = 1,
                    UserId = UserIdTwo,
                    EmailAddress = UserNameTwo,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,
                },
            };


            _mockContributorRepository = MockRepositoryHelper.Create(_contributors, (entity, id) => entity.Id == (int)id);

            _contributorService = new ContributorService(
                _mockContributorRepository.Object, _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContributorService_Constructor_NoContributorRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ContributorService(
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContributorService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new ContributorService(
                _mockContributorRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void ContributorService_Create_CallInsertCustomerAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var contributor = new Contributor
            {
                Id = 3,
                UserId = Guid.NewGuid().ToString(),
                CustomerId = 2,
                EmailAddress = "brody.jordan@uk.fujitsu.com",
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            var response = _contributorService.Create(contributor);

            #endregion

            #region Assert

            _mockContributorRepository.Verify(x => x.Insert(It.IsAny<Contributor>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(_contributors.Count, response);

            #endregion
        }

        [TestMethod]
        public void ContributorService_Update_CallUpdateCustomerAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var contributor = new Contributor
            {
                Id = 1,
                EmailAddress = "Some other email address",
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            _contributorService.Update(contributor);

            #endregion

            #region Assert

            _mockContributorRepository.Verify(x => x.Update(It.IsAny<Contributor>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ContributorService_Delete_CallDeleteCustomerAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var contributor = new Contributor
            {
                Id = 3,
            };

            #endregion

            #region Act

            _contributorService.Delete(contributor);

            #endregion

            #region Assert

            _mockContributorRepository.Verify(x => x.Delete(It.IsAny<Contributor>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ContributorService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _contributorService.All();

            #endregion

            #region Assert

            _mockContributorRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ContributorService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _contributorService.GetById(1);

            #endregion

            #region Assert

            _mockContributorRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ContributorService_GetCustomersContributors_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _contributorService.GetCustomersContributors(1);

            #endregion

            #region Assert

            _mockContributorRepository.Verify(x => x.Query(It.IsAny<Expression<Func<Contributor, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ContributorService_GetCustomersContributors_ReturnsIQueryable()
        {
            #region Arrange

            #endregion

            #region Act

            var customersContributors = _contributorService.GetCustomersContributors(1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(customersContributors, typeof(IQueryable<Contributor>));

            #endregion
        }

        [TestMethod]
        public void ContributorService_GetCustomersContributors_ReturnsTwoContributors()
        {
            #region Arrange

            #endregion

            #region Act

            var customerContributors = _contributorService.GetCustomersContributors(1);

            #endregion

            #region Assert

            Assert.IsNotNull(customerContributors);
            Assert.AreEqual(2, customerContributors.Count());

            #endregion
        }

    }
}
