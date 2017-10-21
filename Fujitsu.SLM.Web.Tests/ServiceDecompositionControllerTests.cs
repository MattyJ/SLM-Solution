using CloneExtensions;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Data;
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
using KellermanSoftware.CompareNetObjects;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;
using Diagram = Fujitsu.SLM.Model.Diagram;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceDecompositionControllerTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<IResponseManager> _mockResponseManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<ApplicationUserManager> _mockApplicationUserManager;
        private Mock<ApplicationRoleManager> _mockApplicationRoleManager;
        private Mock<IUserStore<ApplicationUser>> _mockUserStore;
        private Mock<IRoleStore<IdentityRole>> _mockRoleStore;

        private Mock<ICustomerService> _mockCustomerService;
        private Mock<IDiagramService> _mockDiagramService;
        private Mock<IContributorService> _mockContributorService;

        private Mock<ICustomerPackService> _mockCustomerPackService;

        private ICustomerService _customerService;

        private ServiceDecompositionController _controller;
        private ServiceDecompositionController _controllerWithMockedServices;
        private AppContext _appContext;

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<IRepository<Customer>> _mockCustomerRepository;
        private Mock<IRepository<Contributor>> _mockContributorRepository;

        private List<Customer> _customers;
        private List<Contributor> _contributors;

        private const string ZipFilename = "CustomerPack.zip";
        private const string UserName = "mark.hart@uk.fujitsu.com";
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";
        private const string UserNameTwo = "patrick.williams@uk.fujitsu.com";

        private List<Diagram> _diagrams;
        private List<CustomerPack> _customerPacks;
        private CustomerPack _customerPack;
        private CustomerPack _customerPackUpdated;
        private const string DiagramFilename = "DiagramFilename.pdf";

        private const int CustomerId = 1;

        private const string HasArchitect = "HasArchitect";
        private ApplicationUser _applicationUser;

        private List<ApplicationUser> _applicationUsers;
        private List<IdentityUserRole> _identityUserRoles;
        private List<IdentityRole> _identityRoles;

        private const string Role1 = "Role1";
        private const string Role2 = "Role2";

        readonly string _adminstratorRoleId = Guid.NewGuid().ToString();
        readonly string _architectRoleId = Guid.NewGuid().ToString();
        readonly string _viewerRoleId = Guid.NewGuid().ToString();
        readonly string _userOneId = Guid.NewGuid().ToString();
        readonly string _userTwoId = Guid.NewGuid().ToString();
        readonly string _userThreeId = Guid.NewGuid().ToString();
        readonly string _userFourId = Guid.NewGuid().ToString();
        readonly string _userFiveId = Guid.NewGuid().ToString();

        private ControllerContextMocks _controllerContextMocks;

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

            _applicationUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = _userOneId,
                    Email = "matthew.jordan@uk.fujitsu.com",
                    UserName = "matthew.jordan@uk.fujitsu.com",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-7)
                },
                new ApplicationUser
                {
                    Id = _userTwoId,
                    Email = "mattjordan@tiscali.co.uk",
                    UserName = "mattjordan@tiscali.co.uk",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-2)
                },
                new ApplicationUser
                {
                    Id = _userThreeId,
                    Email = "userthree@tiscali.co.uk",
                    UserName = "userthree@tiscali.co.uk",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-2)
                },
                new ApplicationUser
                {
                    Id = _userFourId,
                    Email = "userfour@tiscali.co.uk",
                    UserName = "userfour@tiscali.co.uk",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-2)
                },
                new ApplicationUser
                {
                    Id = _userFiveId,
                    Email = "userfive@tiscali.co.uk",
                    UserName = "userfive@tiscali.co.uk",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-2)
                }

            };

            var userOne = _applicationUsers.Single(x => x.Id == _userOneId);
            userOne.Roles.Add(new IdentityUserRole { RoleId = _adminstratorRoleId, UserId = _userOneId });
            userOne.Roles.Add(new IdentityUserRole { RoleId = _architectRoleId, UserId = _userOneId });
            userOne.Roles.Add(new IdentityUserRole { RoleId = _viewerRoleId, UserId = _userOneId });

            var userThree = _applicationUsers.Single(x => x.Id == _userThreeId);
            userThree.Roles.Add(new IdentityUserRole { RoleId = _adminstratorRoleId, UserId = _userThreeId });
            userThree.Roles.Add(new IdentityUserRole { RoleId = _architectRoleId, UserId = _userThreeId });
            userThree.Roles.Add(new IdentityUserRole { RoleId = _viewerRoleId, UserId = _userThreeId });

            var userFour = _applicationUsers.Single(x => x.Id == _userFourId);
            userFour.Roles.Add(new IdentityUserRole { RoleId = _architectRoleId, UserId = _userFourId });
            userFour.Roles.Add(new IdentityUserRole { RoleId = _viewerRoleId, UserId = _userFourId });

            var userFive = _applicationUsers.Single(x => x.Id == _userFiveId);
            userFive.Roles.Add(new IdentityUserRole { RoleId = _viewerRoleId, UserId = _userFiveId });

            _identityRoles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = _adminstratorRoleId,
                    Name = UserRoles.Administrator
                },
                new IdentityRole
                {
                    Id = _architectRoleId,
                    Name = UserRoles.Architect
                },
                new IdentityRole
                {
                    Id = _viewerRoleId,
                    Name = UserRoles.Viewer
                }
            };

            _identityUserRoles = new List<IdentityUserRole>
            {
                new IdentityUserRole
                {
                    UserId = _userOneId,
                    RoleId = _adminstratorRoleId
                },
                new IdentityUserRole
                {
                    UserId = _userThreeId,
                    RoleId = _adminstratorRoleId
                },
                new IdentityUserRole
                {
                    UserId = _userThreeId,
                    RoleId = _architectRoleId
                },
                new IdentityUserRole
                {
                    UserId = _userThreeId,
                    RoleId = _viewerRoleId
                },
                new IdentityUserRole
                {
                    UserId = _userFourId,
                    RoleId = _architectRoleId
                },
                new IdentityUserRole
                {
                    UserId = _userFourId,
                    RoleId = _viewerRoleId
                }
            };

            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId,
                    CustomerName = "3663"
                }
            };

            _diagrams = new List<Diagram>
            {
                 UnitTestHelper.GenerateRandomData<Diagram>(x =>
                 {
                     x.Level = 0;
                     x.Filename = "DiagramFilename";
                 }),
                 UnitTestHelper.GenerateRandomData<Diagram>(x =>
                 {
                     x.Level = 0;
                     x.Filename = "DiagramFilename";
                 }),
                 UnitTestHelper.GenerateRandomData<Diagram>(x =>
                 {
                     x.Level = 1;
                     x.Filename = "DiagramFilename";
                 }),
                 UnitTestHelper.GenerateRandomData<Diagram>(x =>
                 {
                     x.Level = 1;
                     x.Filename = "DiagramFilename";
                 }),
                 UnitTestHelper.GenerateRandomData<Diagram>(x =>
                 {
                     x.Level = 0;
                     x.Filename = "DiagramFilename";
                 })
            };

            _customerPacks = new List<CustomerPack>
            {
                UnitTestHelper.GenerateRandomData<CustomerPack>(x =>
                {
                    x.Level = 0;
                    x.CustomerId = CustomerId;
                }),
                UnitTestHelper.GenerateRandomData<CustomerPack>(x =>
                {
                    x.Level = 2;
                    x.CustomerId = CustomerId;
                }),
                UnitTestHelper.GenerateRandomData<CustomerPack>(x =>
                {
                    x.Level = 1;
                    x.CustomerId = CustomerId;
                }),
                UnitTestHelper.GenerateRandomData<CustomerPack>(x =>
                {
                    x.Level = 0;
                    x.CustomerId = CustomerId;
                }),
                UnitTestHelper.GenerateRandomData<CustomerPack>(x =>
                {
                    x.Level = 0;
                    x.CustomerId = 6;
                })
            };

            _customerPack = UnitTestHelper.GenerateRandomData<CustomerPack>();

            _mockCustomerPackService = new Mock<ICustomerPackService>();
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.Is<int>(m => m != 666))).Returns(_customerPack.GetClone());
            _mockCustomerPackService.Setup(s => s.CustomerPacks()).Returns(_customerPacks.AsQueryable());
            _mockCustomerPackService.Setup(s => s.Update(It.IsAny<CustomerPack>())).Callback<CustomerPack>(c => _customerPackUpdated = c);
            _mockCustomerPackService.Setup(s => s.Create(It.IsAny<CustomerPack>())).Callback<CustomerPack>(c => _customerPackUpdated = c);

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockResponseManager = new Mock<IResponseManager>();
            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserNameOne);
            _mockUserManager.Setup(s => s.IsSLMAdministrator()).Returns(true);
            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

            _mockDiagramService = new Mock<IDiagramService>();
            _mockDiagramService.Setup(s => s.Diagrams(It.IsAny<int>())).Returns(_diagrams.AsQueryable());

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

            _contributors = new List<Contributor>
            {
                new Contributor
                {
                    Id = 1,
                    UserId = _userFiveId,
                    EmailAddress = "userfive@tiscali.co.uk",
                    CustomerId = 4,
                    Customer = _customers[3],
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow,

                }
            };

            _mockCustomerRepository = MockRepositoryHelper.Create(_customers, (entity, id) => entity.Id == (int)id);
            _mockContributorRepository = MockRepositoryHelper.Create(_contributors, (entity, id) => entity.Id == (int)id);

            _customerService = new CustomerService(
                _mockCustomerRepository.Object, _mockContributorRepository.Object, _mockUnitOfWork.Object);


            _applicationUser = UnitTestHelper.GenerateRandomData<ApplicationUser>();
            _applicationUser.Logins.Add(new IdentityUserLogin { LoginProvider = "XXX", ProviderKey = "XXX", UserId = _applicationUser.Id });
            _applicationUser.Logins.Add(new IdentityUserLogin { LoginProvider = "YYY", ProviderKey = "YYY", UserId = _applicationUser.Id });

            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockApplicationUserManager = new Mock<ApplicationUserManager>(_mockUserStore.Object);
            _mockApplicationUserManager
                .Setup(x => x.FindByIdAsync(It.Is<string>(m => m != "666")))
                .Returns(Task.FromResult(_applicationUser));
            _mockApplicationUserManager
                .Setup(x => x.RemoveLoginAsync(It.IsAny<string>(), It.IsAny<UserLoginInfo>()))
                .Returns(Task.FromResult(IdentityResult.Success));
            _mockApplicationUserManager
                .Setup(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));
            _mockApplicationUserManager
                .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(IdentityResult.Success));
            _mockApplicationUserManager
                .Setup(x => x.Users)
                .Returns(_applicationUsers.AsQueryable());
            var roles = new List<string> { Role1, Role2 };
            _mockApplicationUserManager
                .Setup(x => x.GetRolesAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<IList<string>>(roles));

            _mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockApplicationRoleManager = new Mock<ApplicationRoleManager>(_mockRoleStore.Object);
            _mockApplicationRoleManager.Setup(x => x.Roles).Returns(_identityRoles.AsQueryable());

            // Note : The Asynch methods are being called via the UserManager Extension methods (i.e. the synchronous methods)
            _mockApplicationRoleManager.Setup(x => x.FindByNameAsync(It.Is<string>(m => m == UserRoles.Administrator)))
                .Returns(Task.FromResult(_identityRoles.Single(r => r.Id == _adminstratorRoleId)));
            _mockApplicationRoleManager.Setup(x => x.FindByNameAsync(It.Is<string>(m => m == UserRoles.Architect)))
                .Returns(Task.FromResult(_identityRoles.Single(r => r.Id == _architectRoleId)));
            _mockApplicationRoleManager.Setup(x => x.FindByNameAsync(It.Is<string>(m => m == UserRoles.Viewer)))
                .Returns(Task.FromResult(_identityRoles.Single(r => r.Id == _viewerRoleId)));
            _mockContributorService = new Mock<IContributorService>();

            _controller = new ServiceDecompositionController(_customerService,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);

            _mockCustomerService = new Mock<ICustomerService>();
            _mockCustomerService.Setup(s => s.GetById(2)).Returns(_customers[1]);

            _controllerWithMockedServices = new ServiceDecompositionController(_mockCustomerService.Object,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);

            _controllerContextMocks = new ControllerContextMocks();
            _controllerWithMockedServices.ControllerContext = new ControllerContext(_controllerContextMocks.MockHttpContextBase.Object,
                new RouteData(),
                _controllerWithMockedServices);
            if (RouteTable.Routes.Count == 0)
            {
                RouteConfig.RegisterRoutes(RouteTable.Routes);
            }
            _controllerWithMockedServices.Url = new UrlHelper(new RequestContext(_controllerContextMocks.MockHttpContextBase.Object, new RouteData()));

            _controller.ControllerContext = new ControllerContext(_controllerContextMocks.MockHttpContextBase.Object,
                new RouteData(),
                _controller);

            _controller.Url = new UrlHelper(new RequestContext(_controllerContextMocks.MockHttpContextBase.Object, new RouteData()));

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoServiceDecompositionThrowsException()
        {
            new ServiceDecompositionController(
                null,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoCustomerPackServiceThrowsException()
        {
            new ServiceDecompositionController(
                _mockCustomerService.Object,
                null,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoDiagramServiceThrowsException()
        {
            new ServiceDecompositionController(
                _mockCustomerService.Object,
                _mockCustomerPackService.Object,
                null,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoContributorServiceThrowsException()
        {
            new ServiceDecompositionController(
                _mockCustomerService.Object,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                null,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoApplicationUserManagerThrowsException()
        {
            new ServiceDecompositionController(
                _mockCustomerService.Object,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                null,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoApplicationRoleManagerThrowsException()
        {
            new ServiceDecompositionController(
                _mockCustomerService.Object,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoContextManagerThrowsException()
        {
            new ServiceDecompositionController(
                _mockCustomerService.Object,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                null,
                _mockAppUserContext.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionController_Constructor_NoAppUserContextThrowsException()
        {
            new ServiceDecompositionController(
                _mockCustomerService.Object,
                _mockCustomerPackService.Object,
                _mockDiagramService.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Get_ReturnsCustomerViewModel()
        {
            var result = _controller.EditCustomer(1) as ViewResult;
            var viewModel = result.Model as CustomerViewModel;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(viewModel, typeof(CustomerViewModel));

        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void ServiceDecompositionController_EditCustomer_Get_NoLevelOfAccessThrowsException()
        {
            _mockUserManager.Setup(s => s.Name).Returns(UserNameTwo);
            _mockUserManager.Setup(s => s.IsSLMAdministrator()).Returns(false);

            var result = _controller.EditCustomer(1) as ViewResult;
            var viewModel = result.Model as CustomerViewModel;
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Get_ReturnsViewResult()
        {
            var result = _controller.EditCustomer(1) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Get_KnownCustomerCallsGetById()
        {
            var result = _controllerWithMockedServices.EditCustomer(2) as ViewResult;

            Assert.IsNotNull(result);
            _mockCustomerService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Get_UnKnownCustomerRedirectsToCorrectAction()
        {
            #region Arrange

            const string expectedAction = "MyCustomers";
            const string expectedController = "Customer";

            #endregion

            #region Act

            var result = _controllerWithMockedServices.EditCustomer(99) as RedirectToRouteResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues.Count);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);

            #endregion
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_ReturnEditCustomerView()
        {
            var result = _controllerWithMockedServices.EditCustomer(2) as ViewResult;
            Assert.AreEqual("EditCustomer", result.ViewName);
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Post_SaveRedirectsUsingCorrectRouteValues()
        {
            #region Arrange

            const string expectedAction = "MyCustomers";
            const string expectedController = "Customer";

            var viewModel = new CustomerViewModel
            {
                Id = 1,
                CustomerName = "Customer Name Changed",
            };

            #endregion

            #region Act

            var result = _controller.EditCustomer(viewModel, "Save", NavigationLevelNames.None) as RedirectToRouteResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues.Count);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);

            #endregion
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Post_ChecksDuplicateName()
        {
            #region Arrange

            var viewModel = new CustomerViewModel
            {
                Id = 2,
                CustomerName = "Customer Name Changed",
            };

            #endregion

            #region Act

            var result = _controllerWithMockedServices.EditCustomer(viewModel, "Save", NavigationLevelNames.None) as RedirectToRouteResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            _mockCustomerService.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Post_CallsUpdateService()
        {
            #region Arrange

            var viewModel = new CustomerViewModel
            {
                Id = 2,
                CustomerName = "Customer Name Changed",
            };

            #endregion

            #region Act

            var result = _controllerWithMockedServices.EditCustomer(viewModel, "Save", NavigationLevelNames.None) as RedirectToRouteResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            _mockCustomerService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            _mockCustomerService.Verify(x => x.Update(It.IsAny<Customer>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Post_SaveWithDuplicateNamesSetModelStateError()
        {
            #region Arrange

            var viewModel = new CustomerViewModel
            {
                Id = 2,
                CustomerName = "3663",
            };

            #endregion

            #region Act

            var result = _controller.EditCustomer(viewModel, "Save", NavigationLevelNames.None) as ViewResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            var modelState = result.ViewData.ModelState;
            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelState["CustomerName"].Errors.Count);

            #endregion
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_PostWithNoLevel_ReturnRedirectsToIndexUsingCorrectRouteValues()
        {
            const string expectedAction = "MyCustomers";
            const string expectedController = "Customer";
            var viewModel = new CustomerViewModel();
            var result = _controller.EditCustomer(viewModel, "Cancel", NavigationLevelNames.None) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues.Count);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
        }

        [TestMethod]
        public void ServiceDecompositionController_DownloadCustomerPack_DiagramNotFound_NullActionResultReturned()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(null as CustomerPack);
            var result = _controllerWithMockedServices.DownloadCustomerPack(1);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ServiceDecompositionController_DownloadCustomerPack_DiagramFound_ContentDispositionAddedToHeader()
        {
            _controllerWithMockedServices.DownloadCustomerPack(1);
            _mockResponseManager.Verify(v => v.AppendHeader("Content-Disposition", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DownloadCustomerPack_DiagramFound_FileStreamResultReturned()
        {
            var result = _controllerWithMockedServices.DownloadCustomerPack(1) as FileStreamResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ServiceDecompositionController_DownloadCustomerPack_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _controllerWithMockedServices.GetMethodAttributeValue("DownloadCustomerPack", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDecompositionController_DownloadCustomerPack_Exception_SetsStatusCodeTo500()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DownloadCustomerPack(1);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DownloadCustomerPack_Exception_AppendsErrorMessageToHeader()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DownloadCustomerPack(1);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_ReadAjaxCustomerPackGrid_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _controllerWithMockedServices.GetMethodAttributeValue("ReadAjaxCustomerPackGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDecompositionController_ReadAjaxCustomerPackGrid_CurrentCustomerNotSet_ResultDataEmpty()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext() { CurrentCustomer = null });
            var result = _controllerWithMockedServices.ReadAjaxCustomerPackGrid(new DataSourceRequest()) as JsonResult;
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void ServiceDecompositionController_ReadAjaxCustomerPackGrid_CurrentCustomerSet_ResultHasData()
        {
            var result = _controllerWithMockedServices.ReadAjaxCustomerPackGrid(new DataSourceRequest()) as JsonResult;
            var dataWrapper = result.Data as DataSourceResult;
            var data = dataWrapper.Data as List<CustomerPackViewModel>;
            Assert.AreEqual(4, data.Count);
        }

        [TestMethod]
        public void ServiceDecompositionController_ReadAjaxCustomerPackGrid_Exception_SetsStatusCodeTo500()
        {
            _mockCustomerPackService.Setup(s => s.CustomerPacks()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxCustomerPackGrid(new DataSourceRequest());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_ReadAjaxCustomerPackGrid_Exception_AppendsErrorMessageToHeader()
        {
            _mockCustomerPackService.Setup(s => s.CustomerPacks()).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.ReadAjaxCustomerPackGrid(new DataSourceRequest());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_ModelStateIsNotValid_UpdateNotInvoked()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockCustomerPackService.Verify(v => v.Update(It.IsAny<CustomerPack>()), Times.Never);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_ModelStateIsNotValid_ErrorIsIncludedInResponse()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            var response = _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel()) as JsonResult;
            var dsr = response.Data as DataSourceResult;
            Assert.IsNotNull(dsr);
            var err = dsr.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.IsNotNull(err);
            Assert.IsTrue(err.ContainsKey("XXX"));
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_CurrentCustomerNotSet_NothingHappens()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext() { CurrentCustomer = null });
            var response = _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel()) as JsonResult;
            _mockCustomerPackService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_DiagramCannotBeFound_ErrorAddedToModelState()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(new AppContext());
            var response = _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel() { Id = 666 }) as JsonResult;
            var dsr = response.Data as DataSourceResult;
            Assert.IsNotNull(dsr);
            var err = dsr.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.IsNotNull(err);
            Assert.IsTrue(err.ContainsKey(ModelStateErrorNames.CustomerPackCannotBeFound));
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_DiagramUpdated_DiagramIdUsedFromModel()
        {
            var response = _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(),
                    new CustomerPackViewModel() { Id = 666 }) as JsonResult;
            _mockCustomerPackService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), 666), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_DiagramUpdated_OnlyAllowedFieldsUpdated()
        {
            var vm = UnitTestHelper.GenerateRandomData<CustomerPackViewModel>();
            var response = _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(), vm) as JsonResult;
            var compare = new CompareLogic(
                new ComparisonConfig
                {
                    IgnoreObjectTypes = true,
                    MaxDifferences = 100,
                    MembersToIgnore = new List<string>
                    {
                        "Filename",
                        "PackNotes",
                        "UpdatedBy",
                        "UpdatedDate"
                    }
                });
            var same = compare.Compare(_customerPack, _customerPackUpdated);
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_DiagramUpdated_FilenameUpdated()
        {
            const string s = "XXX";
            _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(),
                    new CustomerPackViewModel() { Filename = s });
            Assert.AreEqual(s, _customerPackUpdated.Filename);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_DiagramUpdated_DiagramNotesUpdated()
        {
            const string s = "XXX";
            _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(),
                    new CustomerPackViewModel() { PackNotes = s });
            Assert.AreEqual(s, _customerPackUpdated.PackNotes);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_DiagramUpdated_UpdatedByUpdated()
        {
            _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(),
                    new CustomerPackViewModel());
            Assert.AreEqual(UserNameOne, _customerPackUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_DiagramUpdated_UpdatedDateUpdated()
        {
            var now = DateTime.Now;
            _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(),
                    new CustomerPackViewModel());
            Assert.AreEqual(now.Year, _customerPackUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _customerPackUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _customerPackUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _customerPackUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _customerPackUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controllerWithMockedServices.GetMethodAttributeValue("UpdateAjaxCustomerPackGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_Exception_SetsStatusCodeTo500()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_UpdateAjaxCustomerPackGrid_Exception_AppendsErrorMessageToHeader()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.UpdateAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_ModelStateValid_CorrectServiceCalledToGetDiagram()
        {
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockCustomerPackService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_ServiceCalledWithCorrectParameters_IdIsUsedFromModel()
        {
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(),
                new CustomerPackViewModel() { Id = 666 });
            _mockCustomerPackService.Verify(v => v.GetByCustomerAndId(It.IsAny<int>(), 666), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_ServiceCalledWithCorrectParameters_CustomerIdIsUsedFromContext()
        {
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(),
                new CustomerPackViewModel() { Id = 1 });
            _mockCustomerPackService.Verify(v => v.GetByCustomerAndId(1, It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_DiagramNotFound_SetsStatusCodeTo500()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Returns(null as CustomerPack);
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_DiagramNotFound_AppendsErrorMessageToHeader()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Returns(null as CustomerPack);
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_ModelStateIsNotValid_DeleteNotInvoked()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockCustomerPackService.Verify(v => v.Delete(It.IsAny<CustomerPack>()), Times.Never);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_DiagramFound_DeleteInvoked()
        {
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockCustomerPackService.Verify(v => v.Delete(It.IsAny<CustomerPack>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controllerWithMockedServices.GetMethodAttributeValue("DeleteAjaxCustomerPackGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_Exception_SetsStatusCodeTo500()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_DeleteAjaxCustomerPackGrid_Exception_AppendsErrorMessageToHeader()
        {
            _mockCustomerPackService.Setup(s => s.GetByCustomerAndId(It.IsAny<int>(), It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.DeleteAjaxCustomerPackGrid(new DataSourceRequest(), new CustomerPackViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controllerWithMockedServices.GetMethodAttributeValue("AddCustomerPack", (AuthorizeAttribute att) => att.Roles, new[] { typeof(AddCustomerPackViewModel) }));
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_ModelStateNotValid_NoServicesInvoked()
        {
            var strictDiagramServiceMock = new Mock<IDiagramService>(MockBehavior.Strict);
            var target = new ServiceDecompositionController(_mockCustomerService.Object,
                _mockCustomerPackService.Object,
                strictDiagramServiceMock.Object,
                _mockContributorService.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
            target.ModelState.AddModelError("XXX", "XXX");
            target.AddCustomerPack(new AddCustomerPackViewModel());
            strictDiagramServiceMock.Verify();
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_Exception_SetsStatusCodeTo500()
        {
            _mockDiagramService.Setup(s => s.Diagrams(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.AddCustomerPack(new AddCustomerPackViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_Exception_AppendsErrorMessageToHeader()
        {
            _mockDiagramService.Setup(s => s.Diagrams(It.IsAny<int>())).Throws(new ApplicationException("Oh no!!"));
            _controllerWithMockedServices.AddCustomerPack(new AddCustomerPackViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_SpecifiedLevelIsObserved_OnlyLevel0DiagramsInZip()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            var memoryStream = new MemoryStream(_customerPackUpdated.PackData);
            var zipArchive = new ZipArchive(memoryStream);
            Assert.AreEqual(3, zipArchive.Entries.Count);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_PackNames_FilenamesInPackAreUnique()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            var memoryStream = new MemoryStream(_customerPackUpdated.PackData);
            var zipArchive = new ZipArchive(memoryStream);
            var distinctCount = zipArchive.Entries.Select(s => s.Name).Distinct().Count();
            Assert.AreEqual(3, distinctCount);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_CustomerIdSetFromContext()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(1, _customerPackUpdated.CustomerId);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_FilenameComesFromModel()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(ZipFilename, _customerPackUpdated.Filename);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_LevelComesFromModel()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(0, _customerPackUpdated.Level);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_MimeTypeCorrect()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(MimeTypeNames.Zip, _customerPackUpdated.MimeType);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_PackDataIsNotNull()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.IsNotNull(_customerPackUpdated.PackData);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_PackNotesComesFromModel()
        {
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename,
                PackNotes = "XXX"
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual("XXX", _customerPackUpdated.PackNotes);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_InsertedDateSet()
        {
            var now = DateTime.Now;
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(now.Year, _customerPackUpdated.InsertedDate.Year);
            Assert.AreEqual(now.Month, _customerPackUpdated.InsertedDate.Month);
            Assert.AreEqual(now.Day, _customerPackUpdated.InsertedDate.Day);
            Assert.AreEqual(now.Hour, _customerPackUpdated.InsertedDate.Hour);
            Assert.AreEqual(now.Minute, _customerPackUpdated.InsertedDate.Minute);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_InsertedBySet()
        {
            var now = DateTime.Now;
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(UserNameOne, _customerPackUpdated.InsertedBy);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_UpdatedDateSet()
        {
            var now = DateTime.Now;
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(now.Year, _customerPackUpdated.UpdatedDate.Year);
            Assert.AreEqual(now.Month, _customerPackUpdated.UpdatedDate.Month);
            Assert.AreEqual(now.Day, _customerPackUpdated.UpdatedDate.Day);
            Assert.AreEqual(now.Hour, _customerPackUpdated.UpdatedDate.Hour);
            Assert.AreEqual(now.Minute, _customerPackUpdated.UpdatedDate.Minute);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddCustomerPack_EntityIsCreatedWithCorrectData_UpdatedBySet()
        {
            var now = DateTime.Now;
            var vm = new AddCustomerPackViewModel
            {
                EditLevel = NavigationLevelNames.LevelZero,
                Filename = ZipFilename
            };
            _controllerWithMockedServices.AddCustomerPack(vm);
            Assert.AreEqual(UserNameOne, _customerPackUpdated.UpdatedBy);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddContributor_Get_ReturnsAddCustomerContributorViewModel()
        {
            var result = _controllerWithMockedServices.AddContributor() as ViewResult;
            var vm = result.Model as AddCustomerContributorViewModel;
            Assert.IsNotNull(vm);
            Assert.IsInstanceOfType(vm, typeof(AddCustomerContributorViewModel));
        }

        [TestMethod]
        public void ServiceDecompositionController_AddContributor_Get_CustomerIdAndCustomerNameIsSet()
        {
            var customerId = 1;
            var customerName = "3663";
            var result = _controllerWithMockedServices.AddContributor() as ViewResult;
            var vm = result.Model as AddCustomerContributorViewModel;
            Assert.IsNotNull(vm);
            Assert.AreEqual(customerId, vm.CustomerId);
            Assert.AreEqual(customerName, vm.CustomerName);
        }

        [TestMethod]
        public void ServiceDecompositionController_AddContributor_Get_UsersIsSelectListItemList()
        {
            var result = _controllerWithMockedServices.AddContributor() as ViewResult;
            var vm = result.Model as AddCustomerContributorViewModel;
            Assert.IsNotNull(vm);
            Assert.IsInstanceOfType(vm.Users, typeof(List<SelectListItem>));
        }

        [TestMethod]
        public void ServiceDecompositionController_AddContributor_Get_ReturnsTwoPossibleContributors()
        {
            var result = _controllerWithMockedServices.AddContributor() as ViewResult;
            var vm = result.Model as AddCustomerContributorViewModel;
            Assert.IsNotNull(vm);
            Assert.IsInstanceOfType(vm.Users, typeof(List<SelectListItem>));
        }

        [TestMethod]
        public void ServiceDecompositionController_AddContributor_Get_ReturnsCorrectReturnUrl()
        {
            var expectedUrl = "/ServiceDecomposition/EditCustomer/1";
            var result = _controllerWithMockedServices.AddContributor() as ViewResult;
            var vm = result.Model as AddCustomerContributorViewModel;
            Assert.IsNotNull(vm);
            Assert.AreEqual(expectedUrl, vm.ReturnUrl);
        }


        [TestMethod]
        public void ServiceDecompositionController_CreateAjaxAddCustomerContributorGrid_ModelStateNotValid_CheckMessageIsInResponse()
        {
            _controllerWithMockedServices.ModelState.AddModelError("XXX", "XXX");
            var result = _controllerWithMockedServices.CreateAjaxAddCustomerContributorGrid(new DataSourceRequest(), new List<BulkCustomerContributorViewModel>()) as JsonResult;
            dynamic data = result.Data;
            var errors = (Dictionary<string, Dictionary<string, object>>)data.Errors;
            Assert.IsTrue(errors.ContainsKey("XXX"));
        }

        [TestMethod]
        public void ServiceDecompositionController_CreateAjaxAddCustomerContributorGrid_NoContributorsAdded_DoesNotCallGetCustomerContributors()
        {
            var result = _controllerWithMockedServices.CreateAjaxAddCustomerContributorGrid(new DataSourceRequest(), new List<BulkCustomerContributorViewModel>());

            Assert.IsNotNull(result);
            _mockContributorService.Verify(v => v.GetCustomersContributors(CustomerId), Times.Never);
        }

        [TestMethod]
        public void ServiceDecompositionController_CreateAjaxAddCustomerContributorGrid_NoContributorsAdded_EmptyResultReturned()
        {
            var result = _controllerWithMockedServices.CreateAjaxAddCustomerContributorGrid(new DataSourceRequest(), new List<BulkCustomerContributorViewModel>()) as JsonResult;

            dynamic data = result.Data;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, data.Data.Count);
        }

        [TestMethod]
        public void ServiceDecompositionController_CreateAjaxAddCustomerContributorGrid_OneContributorsAdded_CorrectResultSetReturned()
        {
            var bulkContributors = new List<BulkCustomerContributorViewModel>
            {
                new BulkCustomerContributorViewModel
                {
                    CustomerId = 1,
                    CustomerName = "3663",
                    UserId = _userThreeId,
                }
            };

            var result = _controllerWithMockedServices.CreateAjaxAddCustomerContributorGrid(new DataSourceRequest(), bulkContributors) as JsonResult;

            dynamic data = result.Data;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, data.Data.Count);
            _mockContributorService.Verify(v => v.Create(It.IsAny<Contributor>()), Times.Once);
        }

        [TestMethod]
        public void ServiceDecompositionController_CreateAjaxAddCustomerContributorGrid_ThreeContributorsAdded_CreateCalledThreeTimes()
        {
            var bulkContributors = new List<BulkCustomerContributorViewModel>
            {
                new BulkCustomerContributorViewModel
                {
                    CustomerId = 1,
                    CustomerName = "3663",
                    UserId = _userThreeId,
                },
                new BulkCustomerContributorViewModel
                {
                    CustomerId = 1,
                    CustomerName = "3663",
                    UserId = _userFourId,
                },
                new BulkCustomerContributorViewModel
                {
                    CustomerId = 1,
                    CustomerName = "3663",
                    UserId = _userFiveId,
                }
            };

            var result = _controllerWithMockedServices.CreateAjaxAddCustomerContributorGrid(new DataSourceRequest(), bulkContributors) as JsonResult;

            dynamic data = result.Data;
            Assert.IsNotNull(result);
            Assert.AreEqual(3, data.Data.Count);
            _mockContributorService.Verify(v => v.Create(It.IsAny<Contributor>()), Times.Exactly(3));
        }

        [TestMethod]
        public void ServiceDecompositionController_CreateAjaxAddCustomerContributorGrid_ExistingContributorsAdded_NoCreationPerformed()
        {
            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = 4,
                    CustomerName = "3663 MJJ"
                }
            };

            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockContributorService.Setup(s => s.GetCustomersContributors(4)).Returns(_contributors.Where(c => c.CustomerId == 4).AsQueryable());

            var bulkContributors = new List<BulkCustomerContributorViewModel>
            {
                new BulkCustomerContributorViewModel
                {
                    CustomerId = 4,
                    CustomerName = "3663 MJJ",
                    UserId = _userFiveId
                }
            };

            var result = _controllerWithMockedServices.CreateAjaxAddCustomerContributorGrid(new DataSourceRequest(), bulkContributors) as JsonResult;

            dynamic data = result.Data;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, data.Data.Count);
            _mockContributorService.Verify(v => v.Create(It.IsAny<Contributor>()), Times.Never);
        }

        #region Method Authorization Requirement Tests


        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Get_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("EditCustomer", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(int) }));
        }

        [TestMethod]
        public void ServiceDecompositionController_EditCustomer_Post_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("EditCustomer", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(CustomerViewModel), typeof(string), typeof(string) }));
        }

        [TestMethod]
        public void ServiceDecompositionController_AddContributor_Get_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _controller.GetMethodAttributeValue("AddContributor", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceDecompositionController_UploadCustomerFile_Get_ReturnsPartialViewResult()
        {
            var result = _controller.UploadCustomerFile("Level One") as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("_UploadCustomerFile", result.ViewName);
        }

        [TestMethod]
        public void ServiceDecompositionController_Edit_ReturnsUploadCustomerFileViewModel()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.UploadCustomerFile("Level Zero") as PartialViewResult;
            var model = result.Model as UploadCustomerFileViewModel;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(model, typeof(UploadCustomerFileViewModel));

            #endregion
        }

        [TestMethod]
        public void ServiceDecompositionController_Edit_InvalidModelState()
        {
            #region Arrange

            _controller.ModelState.AddModelError("XXX", "XXX");
            var model = new UploadCustomerFileViewModel();

            #endregion

            #region Act

            var result = _controller.UploadCustomerFile(model) as JsonResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }

        #endregion

    }
}
