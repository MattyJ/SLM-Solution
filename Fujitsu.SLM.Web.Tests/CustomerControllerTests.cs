using CloneExtensions;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using KellermanSoftware.CompareNetObjects;
using Kendo.Mvc.UI;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CustomerControllerTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<IContributorService> _mockContributorService;

        private Mock<IUserManager> _mockUserManager;
        private Mock<IResponseManager> _mockResponseManager;

        private CustomerController _controller;
        private CustomerController _controllerWithMockedCustomerService;
        private AppContext _appContext;

        private ICustomerService _customerService;
        private IContributorService _contributorService;
        private Mock<IRepository<Customer>> _mockCustomerRepository;
        private Mock<IRepository<Contributor>> _mockContributorRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private List<Customer> _customers;
        private List<Contributor> _contributors;
        private const string UserName = "bigbertha@uk.fujitsu.com";
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";
        private const string UserNameTwo = "patrick.williams@uk.fujitsu.com";
        private Customer _customerUpdated;


        private const int CustomerId = 2;

        [TestInitialize]
        public void Initialize()
        {
            var container = new ObjectBuilder(ObjectBuilderHelper.SetupObjectBuilder).GetContainer();
            Logger.SetLogWriter(new LogWriterFactory().Create(), false);

            var config = ConfigurationSourceFactory.Create();
            var factory = new ExceptionPolicyFactory(config);

            var exceptionManager = factory.CreateManager();
            container.RegisterInstance(exceptionManager);

            ExceptionPolicy.SetExceptionManager(exceptionManager, false);

            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            };

            _mockAppUserContext = new Mock<IAppUserContext>();

            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserName);
            _mockResponseManager = new Mock<IResponseManager>();

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

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
                    Baseline = false,
                    AssignedArchitect = UserNameOne,
                    ServiceDesks = new List<ServiceDesk>
                    {
                        new ServiceDesk
                        {
                            DeskName = "Desk 1",
                            DeskInputTypes = new List<DeskInputType>()
                        }
                    },
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                },
                new Customer
                {
                    Id =2,
                    CustomerName = "ING",
                    CustomerNotes = "Some ING Customer Notes.",
                    Active = true,
                    Baseline = false,
                    AssignedArchitect = UserNameTwo,
                    InsertedBy = UserNameTwo,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameTwo,
                    UpdatedDate = dateTimeNow
                },
                new Customer
                {
                    Id =3,
                    CustomerName = "ING InActive",
                    CustomerNotes = "Some ING Customer Notes.",
                    Active = false,
                    Baseline = false,
                    AssignedArchitect = UserNameOne,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                },
                new Customer
                {
                    Id =4,
                    CustomerName = "3663 MJJ",
                    CustomerNotes = "Some 3663 MJJ Customer Notes.",
                    Active = true,
                    Baseline = false,
                    AssignedArchitect = UserNameOne,
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                },
            };

            _contributors = new List<Contributor>
            {
                new Contributor
                {
                    Id = 1,
                    CustomerId = 2,
                    UserId = Guid.NewGuid().ToString(),
                    EmailAddress = UserNameOne,
                    Customer = _customers[1]
                }
            };



            _mockCustomerRepository = MockRepositoryHelper.Create(_customers, (entity, id) => entity.Id == (int)id);
            _mockContributorRepository = MockRepositoryHelper.Create(_contributors, (entity, id) => entity.Id == (int)id);

            _customerService = new CustomerService(
                _mockCustomerRepository.Object, _mockContributorRepository.Object, _mockUnitOfWork.Object);

            _contributorService = new ContributorService(_mockContributorRepository.Object, _mockUnitOfWork.Object);

            _mockContributorService = new Mock<IContributorService>();
            _mockContributorService.Setup(s => s.GetById(1)).Returns(_contributors[0]);
            _mockContributorService.Setup(s => s.GetCustomersContributors(2)).Returns(_contributors.Where(c => c.CustomerId == 2).AsQueryable());

            _controller = new CustomerController(_customerService,
                _contributorService,
                _mockContextManager.Object,
                _mockAppUserContext.Object);

            _mockCustomerService = new Mock<ICustomerService>();
            var inits = new Dictionary<Type, Func<object, object>>
            {
                {typeof (ICollection<ServiceDesk>), (s) => null}
            };
            _mockCustomerService.Setup(s => s.GetById(1)).Returns(_customers[0].GetClone(inits));
            _mockCustomerService.Setup(s => s.GetById(2)).Returns(_customers[1].GetClone(inits));
            _mockCustomerService.Setup(s => s.Update(It.IsAny<Customer>()))
                .Callback<Customer>(c => _customerUpdated = c);


            _controllerWithMockedCustomerService = new CustomerController(_mockCustomerService.Object,
                _mockContributorService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerController_Constructor_NoCustomerServiceThrowsException()
        {
            new CustomerController(
                null,
                _mockContributorService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerController_Constructor_NoContributorServiceThrowsException()
        {
            new CustomerController(
                _mockCustomerService.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerController_Constructor_NoContextManagerThrowsException()
        {
            new CustomerController(
                _mockCustomerService.Object,
                _mockContributorService.Object,
                null,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CustomerController_Constructor_NoAppUserContextManagerThrowsException()
        {
            new CustomerController(
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockContextManager.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void CustomerController_MyCustomers_Get_ReturnsViewResult()
        {
            var result = _controller.MyCustomers() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CustomerController_MyArchives_Get_ReturnsViewResult()
        {
            var result = _controller.MyArchives() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CustomerController_Customers_Get_ReturnsViewResult()
        {
            var result = _controller.Customers() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CustomerController_Archives_Get_ReturnsViewResult()
        {
            var result = _controller.Archives() as ViewResult;

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void CustomerController_ReadAjaxMyCustomersGrid_ReturnsActiveCustomersForArchitect()
        {
            #region Arrange

            var request = new DataSourceRequest();
            _mockUserManager.Setup(s => s.Name).Returns(UserNameOne);

            #endregion

            #region Act

            var result = _controller.ReadAjaxMyCustomersGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(true, model.First().Active);
            Assert.AreEqual(3, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyCustomersGrid_ReturnsAnActiveCustomerWhereArchitectIsContributor()
        {
            #region Arrange

            var request = new DataSourceRequest();
            _mockUserManager.Setup(s => s.Name).Returns(UserNameOne);

            #endregion

            #region Act

            var result = _controller.ReadAjaxMyCustomersGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(true, model[2].Active);
            Assert.AreEqual(UserNameTwo, model[2].AssignedArchitect);
            Assert.AreEqual(3, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyCustomersGrid_ReturnsActiveCustomersWhereOwnerOrContributor()
        {
            #region Arrange

            var request = new DataSourceRequest();
            _mockUserManager.Setup(s => s.Name).Returns(UserNameOne);

            #endregion

            #region Act

            var result = _controller.ReadAjaxMyCustomersGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(true, model[0].Active);
            Assert.AreEqual(UserNameOne, model[0].AssignedArchitect); // Owner
            Assert.AreEqual(true, model[1].Active);
            Assert.AreEqual(UserNameOne, model[1].AssignedArchitect); // Owner
            Assert.AreEqual(true, model[2].Active);
            Assert.AreEqual(UserNameTwo, model[2].AssignedArchitect); // Contributor
            Assert.AreEqual(3, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyCustomersGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            _mockCustomerService.Setup(s => s.MyCustomers(It.IsAny<string>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.ReadAjaxMyCustomersGrid(request);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxCustomersGrid_ReturnsActiveCustomersForArchitect()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _controller.ReadAjaxCustomersGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(true, model.First().Active);
            Assert.AreEqual(3, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxCustomersGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            _mockCustomerService.Setup(s => s.Customers()).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.ReadAjaxCustomersGrid(request);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyCustomersGrid_ReturnsNoCustomersForArchitect()
        {
            #region Arrange

            var request = new DataSourceRequest();

            // An Architect with no Customer Decompositions
            _mockUserManager.Setup(s => s.Name).Returns("a.fictitious@uk.fujitsu.com");

            #endregion

            #region Act

            var result = _controller.ReadAjaxMyCustomersGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyCustomersGrid_ReturnsArchivedCustomersForArchitect()
        {
            #region Arrange

            var request = new DataSourceRequest();
            _mockUserManager.Setup(s => s.Name).Returns(UserNameOne);

            #endregion

            #region Act

            var result = _controller.ReadAjaxMyArchivesGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(false, model.Single().Active);
            Assert.AreEqual(1, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyArchivesGrid_ReturnsNoArchivesForArchitect()
        {
            #region Arrange

            var request = new DataSourceRequest();

            // An Architect with no Archived Decompositions
            _mockUserManager.Setup(s => s.Name).Returns("a.fictitious@uk.fujitsu.com");

            #endregion

            #region Act

            var result = _controller.ReadAjaxMyArchivesGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyArchivesGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            _mockCustomerService.Setup(s => s.MyArchives(It.IsAny<string>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.ReadAjaxMyArchivesGrid(request);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxArchivesGrid_ReturnsArchivesForArchitect()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _controller.ReadAjaxArchivesGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<CustomerViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(false, model.First().Active);
            Assert.AreEqual(1, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ReadAjaxArchivesGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            _mockCustomerService.Setup(s => s.Archives()).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.ReadAjaxArchivesGrid(request);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_CreateAjaxCustomerGrid_CallsCustomerServiceCreate()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var insert = new CustomerViewModel
            {
                Id = 5,
                CustomerName = "A New Customer",
                CustomerNotes = "Some New Customer Notes....",
            };

            #endregion

            #region Act

            _controllerWithMockedCustomerService.CreateAjaxCustomerGrid(request, insert);

            #endregion

            #region Assert

            _mockCustomerService.Verify(x => x.Create(It.IsAny<Customer>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_CreateAjaxCustomerGrid_CallsRepositoryInsertAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new CustomerViewModel
            {
                Id = 5,
                CustomerName = "A New Customer",
                CustomerNotes = "Some New Customer Notes....",
            };

            #endregion

            #region Act

            _controller.CreateAjaxCustomerGrid(request, insert);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Insert(It.IsAny<Customer>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_CreateAjaxCustomerGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new CustomerViewModel
            {
                Id = 5,
                CustomerName = "A New Customer",
                CustomerNotes = "Some New Customer Notes....",
            };

            _mockCustomerService.Setup(s => s.Create(It.IsAny<Customer>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.CreateAjaxCustomerGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_CreateAjaxCustomerGrid_DuplicateCustomerNameAppendsHandledErrorToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var insert = new CustomerViewModel
            {
                Id = 5,
                CustomerName = "3663",
                CustomerNotes = "Some New Customer Notes....",
            };

            _mockCustomerService.Setup(s => s.All()).Returns(_customers);

            #endregion

            #region Act

            _controllerWithMockedCustomerService.CreateAjaxCustomerGrid(request, insert);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_UpdateAjaxCustomerGrid_Update_CallsCustomerServiceUpdate()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new CustomerViewModel
            {
                Id = 2,
                CustomerName = "An Updated Customer",
                CustomerNotes = "Some Updated Customer Notes....",
            };

            #endregion

            #region Act

            _controllerWithMockedCustomerService.UpdateAjaxCustomerGrid(request, update);

            #endregion

            #region Assert

            _mockCustomerService.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_UpdateAjaxCustomerGrid_CallsRepositoryUpdateAndUnitOfWork()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var update = new CustomerViewModel
            {
                Id = 2,
                CustomerName = "An Updated Customer",
                CustomerNotes = "Some Updated Customer Notes....",
            };

            #endregion

            #region Act

            _controller.UpdateAjaxCustomerGrid(request, update);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_UpdateAjaxCustomerGrid_DeletedCustomerAppendsHandledErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new CustomerViewModel
            {
                Id = 99,
            };

            #endregion

            #region Act

            _controller.UpdateAjaxCustomerGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", WebResources.CustomerCannotFindCustomerToBeUpdated), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_UpdateAjaxCustomerGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var update = new CustomerViewModel
            {
                Id = 99,
            };

            _mockCustomerService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.UpdateAjaxCustomerGrid(request, update);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxCustomerGrid_CallsCustomerServiceDelete()
        {
            #region Arrange
            var request = new DataSourceRequest();

            var delete = new CustomerViewModel
            {
                Id = 2,
            };

            #endregion

            #region Act

            _controllerWithMockedCustomerService.DeleteAjaxCustomerGrid(request, delete);

            #endregion

            #region Assert

            _mockCustomerService.Verify(x => x.Delete(It.IsAny<Customer>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxCustomerGrid_CallsRepositoryDeleteAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new CustomerViewModel
            {
                Id = 2,
            };

            #endregion

            #region Act

            _controller.DeleteAjaxCustomerGrid(request, delete);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Delete(It.IsAny<Customer>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxCustomerGrid_CustomerWithServiceDeskDoesNotCallsRepositoryDeleteAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new CustomerViewModel
            {
                Id = 1,
            };

            #endregion

            #region Act

            _controller.DeleteAjaxCustomerGrid(request, delete);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.Delete(It.IsAny<Customer>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Never);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxCustomerGrid_CustomerWithServiceDeskAppendsHandledErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new CustomerViewModel
            {
                Id = 1,
            };

            #endregion

            #region Act

            _controller.DeleteAjaxCustomerGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", WebResources.CustomerCannotBeDeletedDueToServiceDesksExisting), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxCustomerGrid_DeletedCustomerAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new CustomerViewModel
            {
                Id = 99,
            };

            #endregion

            #region Act

            _controller.DeleteAjaxCustomerGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", WebResources.CustomerCannotFindCustomerToBeDeleted), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxCustomerGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new CustomerViewModel
            {
                Id = 99,
            };

            _mockCustomerService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.DeleteAjaxCustomerGrid(request, delete);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_GetCustomers_ReturnsListOfSelectListItems()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetCustomers();
            var selectListItems = result.Data as List<SelectListItem>;

            #endregion

            #region Assert

            Assert.IsNotNull(selectListItems);
            Assert.IsInstanceOfType(selectListItems, typeof(List<SelectListItem>));

            #endregion
        }

        [TestMethod]
        public void CustomerController_GetCustomers_ReturnsCustomersWithDefaultDropDown()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.GetCustomers();
            var selectListItems = result.Data as List<SelectListItem>;

            #endregion

            #region Assert

            Assert.IsNotNull(selectListItems);
            Assert.IsTrue(selectListItems.Any(x => x.Text == WebResources.DefaultDropDownListText));
            Assert.AreEqual(_customers.Count + 1, selectListItems.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteArchivedCustomer_CallsCustomerServiceDeleteArchivedCustomer()
        {
            #region Arrange


            #endregion

            #region Act

            _controllerWithMockedCustomerService.DeleteArchivedCustomer(2);

            #endregion

            #region Assert

            _mockCustomerService.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteArchivedCustomer_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockCustomerService.Setup(s => s.Delete(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.DeleteArchivedCustomer(2);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ActivateCustomer_Update_CallsCustomerServiceUpdate()
        {
            #region Arrange


            #endregion

            #region Act

            _controllerWithMockedCustomerService.ActivateCustomer(2);

            #endregion

            #region Assert

            _mockCustomerService.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ActivateCustomer_CallsRepositoryGetByIdUpdateAndUnitOfWork()
        {
            #region Arrange


            #endregion

            #region Act

            _controller.ActivateCustomer(2);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockCustomerRepository.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ActivateCustomer_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockCustomerService.Setup(s => s.Update(It.IsAny<Customer>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.ActivateCustomer(2);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ArchiveCustomer_Update_CallsCustomerServiceUpdate()
        {
            #region Arrange


            #endregion

            #region Act

            _controllerWithMockedCustomerService.ArchiveCustomer(2);

            #endregion

            #region Assert

            _mockCustomerService.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ArchiveCustomer_CallsRepositoryGetByIdUpdateAndUnitOfWork()
        {
            #region Arrange


            #endregion

            #region Act

            _controller.ArchiveCustomer(2);

            #endregion

            #region Assert

            _mockCustomerRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockCustomerRepository.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ArchiveCustomer_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockCustomerService.Setup(s => s.Update(It.IsAny<Customer>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _controllerWithMockedCustomerService.ArchiveCustomer(2);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_ChangeOwner_Exception_AppendsErrorMessageToHeader()
        {
            _mockCustomerService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedCustomerService.ChangeOwner(new ChangeCustomerOwnerViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CustomerController_ChangeOwner_Exception_SetsStatusCodeTo500()
        {
            _mockCustomerService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedCustomerService.ChangeOwner(new ChangeCustomerOwnerViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void CustomerController_ChangeOwner_CustomerExists_RetrievedFromService()
        {
            const string email = "XXX@XXX.COM";
            _controllerWithMockedCustomerService.ChangeOwner(new ChangeCustomerOwnerViewModel() { CustomerId = 2, Email = email });
            _mockCustomerService.Verify(v => v.GetById(2), Times.Once);
        }

        [TestMethod]
        public void CustomerController_ChangeOwner_CustomerExists_OwnerUpdatedFromModel()
        {
            const string email = "XXX@XXX.COM";
            _controllerWithMockedCustomerService.ChangeOwner(new ChangeCustomerOwnerViewModel() { CustomerId = 2, Email = email });
            Assert.AreEqual(email, _customerUpdated.AssignedArchitect);
        }

        [TestMethod]
        public void CustomerController_ChangeOwner_CustomerExists_UpdatedDateSetOnEntity()
        {
            var now = DateTime.Now;
            const string email = "XXX@XXX.COM";
            _controllerWithMockedCustomerService.ChangeOwner(new ChangeCustomerOwnerViewModel { CustomerId = 2, Email = email });
            Assert.AreEqual(now.Year, _customerUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _customerUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _customerUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _customerUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _customerUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void CustomerController_ChangeOwner_CustomerExists_UpdatedBySetOnEntity()
        {
            const string email = "XXX@XXX.COM";
            _controllerWithMockedCustomerService.ChangeOwner(new ChangeCustomerOwnerViewModel { CustomerId = 2, Email = email });
            Assert.AreEqual(UserName, _customerUpdated.UpdatedBy);
        }

        [TestMethod]
        public void CustomerController_ChangeOwner_CustomerExists_OnlyExpectedFieldsUpdated()
        {
            var expected = _customers.SingleOrDefault(s => s.Id == 2);
            const string email = "XXX@XXX.COM";
            _controllerWithMockedCustomerService.ChangeOwner(new ChangeCustomerOwnerViewModel { CustomerId = 2, Email = email });

            var compare = new CompareLogic(new ComparisonConfig
            {
                MaxDifferences = 100,
                MembersToIgnore = new List<string>
                {
                    "AssignedArchitect",
                    "UpdatedBy",
                    "UpdatedDate"
                }
            });
            var same = compare.Compare(expected, _customerUpdated);
            Assert.IsTrue(same.AreEqual);

        }

        [TestMethod]
        public void CustomerController_ReadAjaxContributorGrid_ReturnsContributorViewModel()
        {
            #region Arrange

            var request = new DataSourceRequest();

            #endregion

            #region Act

            var result = _controller.ReadAjaxContributorGrid(request) as JsonResult;

            #endregion

            #region Assert

            var dataSourceResult = result.Data as DataSourceResult;
            var model = dataSourceResult.Data as List<ContributorViewModel>;

            Assert.IsNotNull(model);
            Assert.AreEqual(_contributors.Count, model.Count);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxServiceDeskGrid_CallsContributorServiceDelete()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new ContributorViewModel
            {
                Id = 1
            };

            #endregion

            #region Act

            _controllerWithMockedCustomerService.DeleteAjaxContributorGrid(request, delete);

            #endregion

            #region Assert

            _mockContributorService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockContributorService.Verify(x => x.Delete(It.IsAny<Contributor>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxContributorGrid_CallsRepositoryDeleteAndUnitOfWork()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var delete = new ContributorViewModel
            {
                Id = 1,
            };

            #endregion

            #region Act

            _controller.DeleteAjaxContributorGrid(request, delete);

            #endregion

            #region Assert

            _mockContributorRepository.Verify(x => x.Delete(It.IsAny<Contributor>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);

            #endregion
        }


        #region Method Authorization Requirement Tests

        [TestMethod]
        public void CustomerController_ChangeOwner_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Administrator, _controller.GetMethodAttributeValue("ChangeOwner", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_MyCustomers_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("MyCustomers", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_Customers_CheckRole_RoleIsAdministrator()
        {
            Assert.AreEqual(UserRoles.Administrator, _controller.GetMethodAttributeValue("Customers", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_MyArchives_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("MyArchives", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_Archives_CheckRole_RoleIsAdministrator()
        {
            Assert.AreEqual(UserRoles.Administrator, _controller.GetMethodAttributeValue("Archives", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyCustomersGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("ReadAjaxMyCustomersGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_ReadAjaxMyArchivesGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("ReadAjaxMyArchivesGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_CreateAjaxCustomerGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("CreateAjaxCustomerGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_UpdateAjaxCustomerGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("UpdateAjaxCustomerGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxCustomerGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxCustomerGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_DeleteArchivedCustomer_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteArchivedCustomer", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_ArchiveCustomer_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("ArchiveCustomer", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_ActivateCustomer_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("ActivateCustomer", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_CreateAjaxServiceDeskGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("ReadAjaxContributorGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void CustomerController_DeleteAjaxServiceDeskGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("DeleteAjaxContributorGrid", (AuthorizeAttribute att) => att.Roles));
        }

        #endregion
    }
}
