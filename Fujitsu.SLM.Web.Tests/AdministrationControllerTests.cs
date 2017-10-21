using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
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
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    [DeploymentItem("Fujitsu.SLM.Web.Tests\\Register SLM Generator Tutorial.mp4")]
    public class AdministrationControllerTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IParameterService> _mockParameterService;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<IContributorService> _mockContributorService;
        private Mock<IResponseManager> _mockResponseManager;
        private Mock<IAssetService> _mockAssetService;
        private Mock<IRegionTypeRefDataService> _mockRegionTypeRefDataService;
        private List<Parameter> _parameters;

        private const string ExistingParameterName = "YadaYadaYada";

        private Mock<ApplicationUserManager> _mockApplicationUserManager;
        private Mock<IUserStore<ApplicationUser>> _mockUserStore;

        private Mock<ApplicationRoleManager> _mockApplicationRoleManager;
        private Mock<IRoleStore<IdentityRole>> _mockRoleStore;

        private AdministrationController _target;
        private AppContext _appContext;
        private ApplicationUser _applicationUser;

        private List<ApplicationUser> _applicationUsers;
        private List<IdentityRole> _identityRoles;
        private List<Asset> _assets;
        private List<RegionTypeRefData> _regionTypeRefDatas;

        private string _setLockoutEndDateAsyncUserId;
        private DateTimeOffset _setLockoutEndDateAsyncDateTime;
        private DateTimeOffset _setLastLogonDateTime;

        private const string HasArchitect = "HasArchitect";
        private const string DefaultPassword = "Pa$$w0rd";
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

        private RegionTypeRefData _regionTypeRefDataCreated;
        private RegionTypeRefData _regionTypeRefDataUpdated;

        private const int RegionTypeRefDataIdExists = 64;

        #region Ctor

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

            _regionTypeRefDatas = new List<RegionTypeRefData>
            {
                new RegionTypeRefData
                {
                    Id = 1,
                    RegionName = "Japan",
                    SortOrder = 5
                },
                new RegionTypeRefData
                {
                    Id = 2,
                    RegionName = "EMEIA",
                    SortOrder = 5
                },
                new RegionTypeRefData
                {
                    Id = RegionTypeRefDataIdExists,
                    RegionName = "Global",
                    SortOrder = 5
                }
            };

            _applicationUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = _userOneId,
                    Email = "matthew.jordan@uk.fujitsu.com",
                    UserName = "matthew.jordan@uk.fujitsu.com",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-7),
                    RegionType = _regionTypeRefDatas.FirstOrDefault(x => x.Id == 1),
                    RegionTypeId = 1
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
                    Email = HasArchitect,
                    UserName = "userthree@tiscali.co.uk",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-2)
                },
                new ApplicationUser
                {
                    Id = _userFourId,
                    Email = HasArchitect,
                    UserName = "userfour@tiscali.co.uk",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-2)
                },
                new ApplicationUser
                {
                    Id = _userFiveId,
                    Email = "userfive@tiscali.co.uk",
                    UserName = "mattjordan@tiscali.co.uk",
                    LockoutEnabled = true,
                    LastLoginUtc = DateTime.Now.AddDays(-2)
                }

            };

            _assets = new List<Asset>
            {
                new Asset
                {
                    Id = 1,
                    FileExtension = ".mp4",
                    FileName = Guid.NewGuid().ToString(),
                    OriginalFileName = "Register SLM Generator Tutorial",
                    MimeType = "video/mp4",
                    FullPath = "Fujitsu.SLM.Web.Tests\\Register SLM Generator Tutorial.mp4"
                },
                new Asset
                {
                    Id = 2,
                    FileExtension = ".mp4",
                    FileName = Guid.NewGuid().ToString(),
                    OriginalFileName = "ChangePassword",
                    MimeType = "video/mp4",
                    FullPath = "C:\\Media\\Video\\ChangePassword.mp4"
                }
            };

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

            _appContext = new AppContext();

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockUserManager = new Mock<IUserManager>();
            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);

            _mockContextManager = new Mock<IContextManager>();
            _mockResponseManager = new Mock<IResponseManager>();
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

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
            _mockApplicationUserManager
                .Setup(s => s.SetLockoutEndDateAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
                .Callback<string, DateTimeOffset>((s, d) =>
                {
                    _setLockoutEndDateAsyncUserId = s;
                    _setLockoutEndDateAsyncDateTime = d;
                });
            _mockApplicationUserManager
                .Setup(s => s.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Callback<ApplicationUser>(user =>
                {
                    _setLockoutEndDateAsyncUserId = user.Id;
                    _setLockoutEndDateAsyncDateTime = user.LockoutEndDateUtc.Value;
                    _setLastLogonDateTime = user.LastLoginUtc;
                });

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

            _mockParameterService = new Mock<IParameterService>();
            _parameters = new List<Parameter>
            {
                UnitTestHelper.GenerateRandomData<Parameter>(),
                UnitTestHelper.GenerateRandomData<Parameter>(),
                UnitTestHelper.GenerateRandomData<Parameter>(x => x.ParameterName = ExistingParameterName),
                UnitTestHelper.GenerateRandomData<Parameter>(),
                UnitTestHelper.GenerateRandomData<Parameter>()
            };
            _mockParameterService.Setup(s => s.All()).Returns(_parameters);
            _mockParameterService.Setup(s => s.Find(It.Is<string>(m => m == ExistingParameterName)))
                .Returns(_parameters.Single(x => x.ParameterName == ExistingParameterName));
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<string>(It.Is<string>(m => m == ParameterNames.UserResetPassword)))
                .Returns(DefaultPassword);

            Bootstrapper.SetupAutoMapper();

            _mockCustomerService = new Mock<ICustomerService>();
            _mockContributorService = new Mock<IContributorService>();
            _mockAssetService = new Mock<IAssetService>();


            _mockAssetService.Setup(s => s.All()).Returns(_assets);
            _mockAssetService.Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _assets.SingleOrDefault(x => x.Id == id));

            _mockRegionTypeRefDataService = new Mock<IRegionTypeRefDataService>();
            _mockRegionTypeRefDataService.Setup(s => s.All()).Returns(_regionTypeRefDatas);
            _mockRegionTypeRefDataService
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _regionTypeRefDatas.SingleOrDefault(x => x.Id == id));
            _mockRegionTypeRefDataService
                .Setup(s => s.Create(It.IsAny<RegionTypeRefData>()))
                .Callback<RegionTypeRefData>(rg => _regionTypeRefDataCreated = rg);
            _mockRegionTypeRefDataService
               .Setup(s => s.Update(It.IsAny<RegionTypeRefData>()))
               .Callback<RegionTypeRefData>(rg => _regionTypeRefDataUpdated = rg);

            _target = new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoParameterService_ThrowsException()
        {
            new AdministrationController(null,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoCustomerService_ThrowsException()
        {
            new AdministrationController(_mockParameterService.Object,
                null,
                _mockContributorService.Object,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoContributorService_ThrowsException()
        {
            new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                null,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoRegionTypeService_ThrowsException()
        {
            new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockAssetService.Object,
                null,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoAssetService_ThrowsException()
        {
            new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                null,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoContextManager_ThrowsException()
        {
            new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                null,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoApplicationUserManager_ThrowsException()
        {
            new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                null,
                _mockApplicationRoleManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdministrationController_Constructor_NoApplicationRoleManager_ThrowsException()
        {
            new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                null);
        }

        #endregion

        #region Index

        [TestMethod]
        public void AdministrationController_Index_Get_RedirectsToCustomersLandingMenuItem()
        {
            const string expectedAction = "Users";
            const string expectedController = "Administration";

            var result = _target.Index() as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
        }

        #endregion

        #region User Management

        [TestMethod]
        public void AdministrationController_Users_ReturnsViewResult()
        {
            var result = _target.Users() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_Roles_Administrator_ReturnsViewResult()
        {
            var result = _target.Roles(UserRoles.Administrator) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_Roles_Architect_ReturnsViewResult()
        {
            var result = _target.Roles(UserRoles.Architect) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_Roles_Viewer_ReturnsViewResult()
        {
            var result = _target.Roles(UserRoles.Viewer) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxUsersGrid_ReturnsJsonResponse()
        {

            #region Act

            var result = _target.ReadAjaxUsersGrid(new DataSourceRequest()) as JsonResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxUsersGrid_ExceptionThrown_StatusCodeSetTo500()
        {
            #region Arrange

            _mockApplicationUserManager.Setup(x => x.Users).Throws(new Exception("Exception!!"));

            #endregion

            #region Act

            _target.ReadAjaxUsersGrid(new DataSourceRequest());

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxUsersGrid_ReturnsCorrectNumberOfUsers()
        {
            var result = _target.ReadAjaxUsersGrid(new DataSourceRequest()) as JsonResult;

            var dataSourceResult = result.Data as DataSourceResult;
            Assert.IsNotNull(dataSourceResult);

            var outputData = dataSourceResult.Data as List<UserViewModel>;

            Assert.IsNotNull(outputData);
            Assert.AreEqual(_applicationUsers.Count, outputData.Count);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxUserRolesGrid_ReturnsJsonResponse()
        {
            var result = _target.ReadAjaxUserRolesGrid(new DataSourceRequest(), UserRoles.Administrator) as JsonResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxUserRolesGrid_ExceptionThrown_StatusCodeSetTo500()
        {
            # region Arrange

            _mockApplicationUserManager.Setup(x => x.Users).Throws(new Exception("Exception!!"));

            #endregion

            #region Act

            var result = _target.ReadAjaxUserRolesGrid(new DataSourceRequest(), UserRoles.Administrator) as JsonResult;

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxUserRolesGrid_ReturnsCorrectNumberOfUsers()
        {
            var result = _target.ReadAjaxUserRolesGrid(new DataSourceRequest(), UserRoles.Administrator) as JsonResult;

            var dataSourceResult = result.Data as DataSourceResult;
            Assert.IsNotNull(dataSourceResult);

            var outputData = dataSourceResult.Data as List<UserViewModel>;

            Assert.IsNotNull(outputData);
            Assert.AreEqual(1, outputData.Count);
        }

        [TestMethod]
        public void AdministrationController_AddUserToRole_ReturnsView_IsPartialView()
        {
            var result = _target.AddUserToRole(UserRoles.Administrator) as PartialViewResult;
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual("_AddUserToRole", result.ViewName);
        }

        [TestMethod]
        public void AdministrationController_AddUserToRole_CompletesSuccessfully()
        {
            #region Arrange

            _mockApplicationUserManager.Setup(s => s.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var user = new RoleViewModel
            {
                RoleName = UserRoles.Administrator,
                UserId = _userOneId,
            };

            #endregion

            var result = _target.AddUserToRole(user);

            dynamic data = result.Data;

            Assert.IsNotNull(data);
            Assert.AreEqual("Success", data.Message);
            Assert.IsTrue(Boolean.Parse(data.Success));
        }

        [TestMethod]
        public void AdministrationController_AddUserToRole_Admininistrator_CallsAddToRoleThreeTimes()
        {
            #region Arrange

            _mockApplicationUserManager.Setup(s => s.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var user = new RoleViewModel
            {
                RoleName = UserRoles.Administrator,
                UserId = _userOneId,
            };


            #endregion

            var result = _target.AddUserToRole(user);

            _mockApplicationUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));
        }

        [TestMethod]
        public void AdministrationController_AddUserToRole_Adrchitect_CallsAddToRoleTwice()
        {
            #region Arrange

            _mockApplicationUserManager.Setup(s => s.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var user = new RoleViewModel
            {
                RoleName = UserRoles.Architect,
                UserId = _userOneId,
            };


            #endregion

            var result = _target.AddUserToRole(user);

            _mockApplicationUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void AdministrationController_AddUserToRole_Viewer_CallsAddToRoleOnce()
        {
            #region Arrange

            _mockApplicationUserManager.Setup(s => s.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var user = new RoleViewModel
            {
                RoleName = UserRoles.Viewer,
                UserId = _userOneId,
            };


            #endregion

            var result = _target.AddUserToRole(user);

            _mockApplicationUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void AdministrationController_AddUserToRole_ModelHasErrors_ReturnsErrorInResult()
        {
            _target.ModelState.AddModelError("XXX", "XXX");

            var result = _target.AddUserToRole(new RoleViewModel()) as JsonResult;
            dynamic data = result.Data;

            Assert.IsNotNull(data);
            Assert.AreEqual("XXX", data.Message);
            Assert.IsFalse(Boolean.Parse(data.Success));
        }

        [TestMethod]
        public void AdministrationController_AddUserToRole_ExceptionThrown_StatusCodeSetTo500()
        {
            # region Arrange

            _mockApplicationUserManager.Setup(s => s.AddToRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Exception!!"));

            var model = new RoleViewModel { RoleName = UserRoles.Administrator, UserId = _userOneId };

            #endregion

            #region Act

            var result = _target.AddUserToRole(model) as JsonResult;

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_DeleteUserRolesGrid_Admininistrator_CallsRemoveFromRoleOnce()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var model = new UserViewModel
            {
                Id = _userOneId,
            };

            _mockApplicationUserManager.Setup(s => s.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _mockApplicationUserManager.Setup(s => s.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            #endregion

            #region Act

            _target.DeleteUserRolesGrid(request, model, UserRoles.Administrator);

            #endregion

            #region Assert

            _mockApplicationUserManager.Verify(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_DeleteUserRolesGrid_Architect_CallsRemoveFromRoleTwice()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var model = new UserViewModel
            {
                Id = _userOneId,
            };

            _mockApplicationUserManager.Setup(s => s.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _mockApplicationUserManager.Setup(s => s.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            #endregion

            #region Act

            _target.DeleteUserRolesGrid(request, model, UserRoles.Architect);

            #endregion

            #region Assert

            _mockApplicationUserManager.Verify(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

            #endregion
        }

        [TestMethod]
        public void AdministrationController_DeleteUserRolesGrid_Viewer_CallsRemoveFromRoleThreeTimes()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var model = new UserViewModel
            {
                Id = _userOneId,
            };

            _mockApplicationUserManager.Setup(s => s.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success));
            _mockApplicationUserManager.Setup(s => s.IsInRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            #endregion

            #region Act

            _target.DeleteUserRolesGrid(request, model, UserRoles.Viewer);

            #endregion

            #region Assert

            _mockApplicationUserManager.Verify(x => x.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));

            #endregion
        }

        [TestMethod]
        public void AdministrationController_DeleteUserRolesGrid_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            var request = new DataSourceRequest();

            var model = new UserViewModel
            {
                Id = _userOneId,
            };

            _mockApplicationUserManager.Setup(s => s.RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>())).Throws(new ApplicationException("Oh no!!"));

            #endregion

            #region Act

            _target.DeleteUserRolesGrid(request, model, UserRoles.Administrator);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_GetUsers_ReturnsListOfSelectListItems()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _target.GetUsers(UserRoles.Administrator);
            var selectListItems = result.Data as List<SelectListItem>;

            #endregion

            #region Assert

            Assert.IsNotNull(selectListItems);
            Assert.IsInstanceOfType(selectListItems, typeof(List<SelectListItem>));
            Assert.AreEqual(5, selectListItems.Count);


            #endregion
        }

        [TestMethod]
        public void AdministrationController_GetUsers_ReturnsAllServiceDesksWithDefaultDropDown()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _target.GetUsers(UserRoles.Administrator);
            var selectListItems = result.Data as List<SelectListItem>;

            #endregion

            #region Assert

            Assert.IsNotNull(selectListItems);
            Assert.IsTrue(selectListItems.Any(x => x.Text == WebResources.DefaultDropDownListText));

            #endregion
        }

        [TestMethod]
        public void AdministrationController_GetUsersInRole_RoleIsArchitect_AnyUserWithArchitectIsReturned()
        {
            const string userThree = "userthree@tiscali.co.uk";
            const string userFour = "userfour@tiscali.co.uk";

            var result = _target.GetUsersInRole(UserRoles.Architect);
            var selectListItems = result.Data as List<SelectListItem>;

            Assert.IsNotNull(selectListItems);
            Assert.AreEqual(2, selectListItems.Count);
            Assert.IsTrue(selectListItems.Any(x => x.Text == userThree));
            Assert.IsTrue(selectListItems.Any(x => x.Text == userFour));
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateNotValid_ResponseSuccessIsFalse()
        {
            const string errorMessage = "XXX";
            _target.ModelState.AddModelError(errorMessage, errorMessage);
            var result = await _target.LockUser(new LockUserViewModel());
            dynamic data = result.Data;
            Assert.AreEqual(false, data.Success);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateNotValid_ModelStateErrorReturnedInResponse()
        {
            const string errorMessage = "XXX";
            _target.ModelState.AddModelError(errorMessage, errorMessage);
            var result = await _target.LockUser(new LockUserViewModel());
            dynamic data = result.Data;
            Assert.AreEqual(errorMessage, data.Message);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeLocked_FindByIdAsyncIsCalledWithCorrectUserId()
        {
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = true, UserId = userId });
            _mockApplicationUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);

        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeLocked_UpdateAsyncIsCalled()
        {
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = true, UserId = userId });
            _mockApplicationUserManager.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUpdateUserToBeLocked_UpdateAsynchThrowsExceptionReturnsStatusCode()
        {

            _mockApplicationUserManager.Setup(s => s.UpdateAsync(It.IsAny<ApplicationUser>())).Throws(new Exception("Exception !!!"));
            var userId = Guid.NewGuid().ToString();


            await _target.LockUser(new LockUserViewModel { Lock = true, UserId = userId });

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeUnLocked_FindByIdAsyncIsCalledWithCorrectUserId()
        {
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            _mockApplicationUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);

        }


        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeUnLocked_UpdateAsyncAsyncIsCalled()
        {
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            _mockApplicationUserManager.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeUnLocked_UpdateAsynchThrowsExceptionReturnsStatusCode()
        {

            _mockApplicationUserManager.Setup(s => s.UpdateAsync(It.IsAny<ApplicationUser>())).Throws(new Exception("Exception !!!"));
            var userId = Guid.NewGuid().ToString();


            await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeLocked_SetLockOutCalledWithCorrectDateTime()
        {
            var now = DateTime.MaxValue;
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = true, UserId = userId });
            Assert.AreEqual(now.Year, _setLockoutEndDateAsyncDateTime.Year);
            Assert.AreEqual(now.Month, _setLockoutEndDateAsyncDateTime.Month);
            Assert.AreEqual(now.Day, _setLockoutEndDateAsyncDateTime.Day);
            Assert.AreEqual(now.Hour, _setLockoutEndDateAsyncDateTime.Hour);
            Assert.AreEqual(now.Minute, _setLockoutEndDateAsyncDateTime.Minute);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeUnlocked_SetLockOutCalledWithCorrectDateTime()
        {
            var now = DateTime.UtcNow;
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            Assert.AreEqual(now.Year, _setLockoutEndDateAsyncDateTime.Year);
            Assert.AreEqual(now.Month, _setLockoutEndDateAsyncDateTime.Month);
            Assert.AreEqual(now.Day, _setLockoutEndDateAsyncDateTime.Day);
            Assert.AreEqual(now.Hour, _setLockoutEndDateAsyncDateTime.Hour);
            Assert.AreEqual(now.Minute, _setLockoutEndDateAsyncDateTime.Minute);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeLocked_SetLastLogonCalledWithCorrectDateTime()
        {
            var now = DateTime.UtcNow;
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = true, UserId = userId });
            Assert.AreEqual(now.Year, _setLastLogonDateTime.Year);
            Assert.AreEqual(now.Month, _setLastLogonDateTime.Month);
            Assert.AreEqual(now.Day, _setLastLogonDateTime.Day);
            Assert.AreEqual(now.Hour, _setLastLogonDateTime.Hour);
            Assert.AreEqual(now.Minute, _setLastLogonDateTime.Minute);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_ModelStateValidUserToBeUnlocked_SetLastLogonCalledWithCorrectDateTime()
        {
            var now = DateTime.UtcNow;
            var userId = Guid.NewGuid().ToString();
            await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            Assert.AreEqual(now.Year, _setLastLogonDateTime.Year);
            Assert.AreEqual(now.Month, _setLastLogonDateTime.Month);
            Assert.AreEqual(now.Day, _setLastLogonDateTime.Day);
            Assert.AreEqual(now.Hour, _setLastLogonDateTime.Hour);
            Assert.AreEqual(now.Minute, _setLastLogonDateTime.Minute);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_UserSuccessfullyUpdated_ResponseSuccessIsTrue()
        {
            _mockApplicationUserManager.Setup(s => s.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(IdentityResult.Success));
            var userId = Guid.NewGuid().ToString();
            var result = await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            dynamic data = result.Data;
            Assert.AreEqual(true, data.Success);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_UserUnSuccessfullyUpdated_ResponseSuccessIsFalse()
        {
            _mockApplicationUserManager.Setup(s => s.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(new IdentityResult()));

            var userId = Guid.NewGuid().ToString();
            var result = await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            dynamic data = result.Data;
            Assert.AreEqual(false, data.Success);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_UserFailsToUpdate_ResponseSuccessIsFalse()
        {
            _mockApplicationUserManager.Setup(s => s.SetLockoutEndDateAsync(It.IsAny<string>(), It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(new IdentityResult()));
            var userId = Guid.NewGuid().ToString();
            var result = await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            dynamic data = result.Data;
            Assert.AreEqual(false, data.Success);
        }

        [TestMethod]
        public async Task AdministrationController_LockUser_UserFailsToUpdate_ResponseMessageComesFromIdentityResult()
        {
            _mockApplicationUserManager.Setup(s => s.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(new IdentityResult("XXX")));
            var userId = Guid.NewGuid().ToString();
            var result = await _target.LockUser(new LockUserViewModel { Lock = false, UserId = userId });
            dynamic data = result.Data;
            Assert.AreEqual("XXX", data.Message);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_ModelStateNotValid_ResponseSuccessIsFalse()
        {
            const string errorMessage = "XXX";
            _target.ModelState.AddModelError(errorMessage, errorMessage);
            var result = await _target.ResetUserPassword(new ResetUserPasswordViewModel());
            dynamic data = result.Data;
            Assert.AreEqual(false, data.Success);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_ModelStateNotValid_ModelStateErrorReturnedInResponse()
        {
            const string errorMessage = "XXX";
            _target.ModelState.AddModelError(errorMessage, errorMessage);
            var result = await _target.ResetUserPassword(new ResetUserPasswordViewModel());
            dynamic data = result.Data;
            Assert.AreEqual(errorMessage, data.Message);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_ModelStateValid_UserTokenGeneratedWithCorrectUserId()
        {
            var userId = Guid.NewGuid().ToString();
            await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            _mockApplicationUserManager.Verify(v => v.GeneratePasswordResetTokenAsync(userId), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_ModelStateValid_DefaultPasswordObtainedFromParameters()
        {
            var userId = Guid.NewGuid().ToString();
            await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            _mockParameterService.Verify(v => v.GetParameterByNameAndCache<string>(ParameterNames.UserResetPassword), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_ResetPassword_CorrectUserIdAndPasswordUsed()
        {
            var userId = Guid.NewGuid().ToString();
            await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            _mockApplicationUserManager.Verify(v => v.ResetPasswordAsync(userId, It.IsAny<string>(), DefaultPassword), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_UserPasswordSuccessfullyReset_ResponseSuccessIsTrue()
        {
            _mockApplicationUserManager.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));
            var userId = Guid.NewGuid().ToString();
            var result = await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            dynamic data = result.Data;
            Assert.AreEqual(true, data.Success);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_UserPasswordFailsToUpdate_ResponseSuccessIsFalse()
        {
            _mockApplicationUserManager.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult()));
            var userId = Guid.NewGuid().ToString();
            var result = await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            dynamic data = result.Data;
            Assert.AreEqual(false, data.Success);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_UserPasswordFailsToUpdate_ResponseMessageComesFromIdentityResult()
        {
            _mockApplicationUserManager.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("XXX")));
            var userId = Guid.NewGuid().ToString();
            var result = await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            dynamic data = result.Data;
            Assert.AreEqual("XXX", data.Message);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_UserPasswordFailsToUpdate_StatusCode500()
        {
            _mockApplicationUserManager.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("XXX")));
            var userId = Guid.NewGuid().ToString();
            await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_UserPasswordFailsToUpdate_ErrorHeaderAppended()
        {
            _mockApplicationUserManager.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("XXX")));
            var userId = Guid.NewGuid().ToString();
            await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_ExceptionOccurs_StatusCode500()
        {
            _mockApplicationUserManager.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Oh no!!"));
            var userId = Guid.NewGuid().ToString();
            await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_ResetUserPassword_ExceptionOccurs_ErrorHeaderAppended()
        {
            _mockApplicationUserManager.Setup(s => s.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Throws(new Exception("Oh no!!"));
            var userId = Guid.NewGuid().ToString();
            await _target.ResetUserPassword(new ResetUserPasswordViewModel { UserId = userId });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_UserCannotBeFound_JsonReturnedWithMessage()
        {
            var result = await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "666" });
            var jsonData = result as JsonResult;
            Assert.IsNotNull(jsonData);
            dynamic data = jsonData.Data;
            Assert.IsNotNull(data.Errors);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_UserHasLogins_RemoveLoginAsyncCalledToRemove()
        {
            await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "XXX" });
            _mockApplicationUserManager.Verify(x => x.RemoveLoginAsync(_applicationUser.Id, It.IsAny<UserLoginInfo>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_RemoveLoginsError_JsonResultReturnedWithErrors()
        {
            _mockApplicationUserManager.Setup(x => x
                .RemoveLoginAsync(It.IsAny<string>(), It.IsAny<UserLoginInfo>()))
                .Returns(Task.FromResult(new IdentityResult("XXX", "YYY")));
            var result = await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "XXX" });
            var jsonData = result as JsonResult;
            Assert.IsNotNull(jsonData);
            dynamic data = jsonData.Data;
            var errors = data.Errors as string[];
            Assert.IsNotNull(errors);
            Assert.AreEqual("XXX", errors[0]);
            Assert.AreEqual("YYY", errors[1]);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_UserHasRoles_RemoveFromRoleAsyncCalledToRemove()
        {
            await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "XXX" });
            _mockApplicationUserManager.Verify(x => x.RemoveFromRoleAsync(_applicationUser.Id, It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_UserHasRolesError_JsonResultReturnedWithErrors()
        {
            _mockApplicationUserManager.Setup(x => x
                .RemoveFromRoleAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new IdentityResult("XXX", "YYY")));
            var result = await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "XXX" });
            var jsonData = result as JsonResult;
            Assert.IsNotNull(jsonData);
            dynamic data = jsonData.Data;
            var errors = data.Errors as string[];
            Assert.IsNotNull(errors);
            Assert.AreEqual("XXX", errors[0]);
            Assert.AreEqual("YYY", errors[1]);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_UserDeleted_DeleteAsyncCalledToRemove()
        {
            await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "XXX" });
            _mockApplicationUserManager.Verify(x => x.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_UserDeletedError_JsonResultReturnedWithErrors()
        {
            _mockApplicationUserManager.Setup(x => x
                .DeleteAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(new IdentityResult("XXX", "YYY")));
            var result = await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "XXX" });
            var jsonData = result as JsonResult;
            Assert.IsNotNull(jsonData);
            dynamic data = jsonData.Data;
            var errors = data.Errors as string[];
            Assert.IsNotNull(errors);
            Assert.AreEqual("XXX", errors[0]);
            Assert.AreEqual("YYY", errors[1]);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_ExceptionOccurs_StatusCode500()
        {
            _mockApplicationUserManager.Setup(s => s.FindByIdAsync(It.IsAny<string>()))
                .Throws(new Exception("Oh no!!"));
            var userId = Guid.NewGuid().ToString();
            await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = userId });
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_ExceptionOccurs_ErrorHeaderAppended()
        {
            _mockApplicationUserManager.Setup(s => s.FindByIdAsync(It.IsAny<string>()))
                .Throws(new Exception("Oh no!!"));
            var userId = Guid.NewGuid().ToString();
            await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = userId });
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task AdministrationController_DeleteAjaxUsersGrid_ArchitectIsStillCustomerOwner_CorrectModelStateErrorAdded()
        {
            _mockCustomerService.Setup(s => s.IsArchitectACustomerOwner(It.IsAny<string>())).Returns(true);
            var result = await _target.DeleteAjaxUsersGrid(new DataSourceRequest(), new UserViewModel { Id = "XXX" });

            var jsonData = result as JsonResult;
            Assert.IsNotNull(jsonData);
            dynamic data = jsonData.Data;
            var errors = data.Errors as Dictionary<string, Dictionary<string, object>>;


            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.Any(x => x.Key == ModelStateErrorNames.ArchitectIsStillCustomerOwner));
        }

        #endregion

        #region Parameters

        [TestMethod]
        public void AdministrationController_Parameters_ReturnsViewResult()
        {
            var result = _target.Parameters();

            Assert.IsNotNull(result);
            var viewModel = result.Model as ParameterViewModel;
            Assert.IsNotNull(viewModel);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxParametersGrid_ReturnsJsonResponse()
        {
            var result = _target.ReadAjaxParametersGrid(new DataSourceRequest()) as JsonResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxParametersGrid_ReturnsCorrectNumberOfParameters()
        {
            var result = _target.ReadAjaxParametersGrid(new DataSourceRequest()) as JsonResult;

            var dataSourceResult = result.Data as DataSourceResult;
            Assert.IsNotNull(dataSourceResult);

            var outputData = dataSourceResult.Data as List<ParameterViewModel>;

            Assert.IsNotNull(outputData);
            Assert.AreEqual(5, outputData.Count);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxParametersGrid_ExceptionThrown_StatusCodeSetTo500()
        {
            _mockParameterService
                .Setup(s => s.All())
                .Throws(new Exception("Foobar!!"));
            _target.ReadAjaxParametersGrid(new DataSourceRequest());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxParametersGrid_ExceptionThrown_ErrorHeaderAppended()
        {
            _mockParameterService
                .Setup(s => s.All())
                .Throws(new Exception("Foobar!!"));
            _target.ReadAjaxParametersGrid(new DataSourceRequest());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void AdministrationController_UpdateAjaxParametersGrid_UpdatesParameterValue()
        {
            const string updatedParameterValue = "XXX666";
            var request = new DataSourceRequest();
            var viewModel = new ParameterViewModel()
            {
                ParameterName = ExistingParameterName,
                ParameterValue = updatedParameterValue
            };

            _target.UpdateAjaxParametersGrid(request, viewModel);
            _mockParameterService.Verify(v => v.SaveParameter(ExistingParameterName, updatedParameterValue, ParameterType.Admin));
        }

        [TestMethod]
        public void AdministrationController_UpdateAjaxParametersGrid_ModelHasErrors_NothingSaved()
        {
            _mockParameterService = new Mock<IParameterService>(MockBehavior.Strict);
            _target = new AdministrationController(_mockParameterService.Object,
                _mockCustomerService.Object,
                _mockContributorService.Object,
                _mockAssetService.Object,
                _mockRegionTypeRefDataService.Object,
                _mockContextManager.Object,
                _mockApplicationUserManager.Object,
                _mockApplicationRoleManager.Object);
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.UpdateAjaxParametersGrid(new DataSourceRequest(), new ParameterViewModel { ParameterValue = "XXX666" });
        }

        [TestMethod]
        public void AdministrationController_UpdateAjaxParametersGrid_ParameterIsSystem_NothingSaved()
        {
            const string updatedParameterValue = "XXX666";
            var p = _parameters.Single(x => x.ParameterName == ExistingParameterName);
            p.Type = ParameterType.System;
            _mockParameterService.Setup(s => s.Find(ExistingParameterName)).Returns(p);
            var result = _target.UpdateAjaxParametersGrid(new DataSourceRequest(),
                new ParameterViewModel
                {
                    ParameterName = ExistingParameterName,
                    ParameterValue = updatedParameterValue
                }) as JsonResult;
            var kendoData = result.Data as DataSourceResult;
            var errors = kendoData.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors.ContainsKey(ModelStateErrorNames.ParameterIsSystem));
        }

        [TestMethod]
        public void AdministrationController_UpdateAjaxParametersGrid_ExceptionThrown_StatusCodeSetTo500()
        {
            _mockParameterService
                .Setup(s => s.Find(It.IsAny<string>()))
                .Throws(new Exception("Foobar!!"));
            _target.UpdateAjaxParametersGrid(new DataSourceRequest(), new ParameterViewModel());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void AdministrationController_UpdateAjaxParametersGrid_ExceptionThrown_ErrorHeaderAppended()
        {
            _mockParameterService
                .Setup(s => s.Find(It.IsAny<string>()))
                .Throws(new Exception("Foobar!!"));
            _target.UpdateAjaxParametersGrid(new DataSourceRequest(), new ParameterViewModel());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        #endregion

        [TestMethod]
        public void AdministrationController_Videos_ReturnsViewResult()
        {
            var result = _target.Videos() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxVideosGrid_ReturnsJsonResponse()
        {

            #region Act

            var result = _target.ReadAjaxVideosGrid(new DataSourceRequest()) as JsonResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxVideosGrid_ExceptionThrown_StatusCodeSetTo500()
        {
            # region Arrange

            _mockAssetService.Setup(x => x.All()).Throws(new Exception("Exception!!"));

            #endregion

            #region Act

            _target.ReadAjaxVideosGrid(new DataSourceRequest());

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxVideosGrid_ReturnsCorrectNumberOfVideos()
        {
            var result = _target.ReadAjaxVideosGrid(new DataSourceRequest()) as JsonResult;

            var dataSourceResult = result.Data as DataSourceResult;
            Assert.IsNotNull(dataSourceResult);

            var outputData = dataSourceResult.Data as List<AssetViewModel>;

            Assert.IsNotNull(outputData);
            Assert.AreEqual(_assets.Count, outputData.Count);
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAsset_CallsAssetServiceDelete()
        {
            #region Arrange


            #endregion

            #region Act

            _target.DeleteAsset(1);

            #endregion

            #region Assert

            _mockAssetService.Verify(x => x.Delete(It.IsAny<Asset>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAsset_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockAssetService.Setup(s => s.Delete(It.IsAny<Asset>())).Throws(new ApplicationException("Oh no!!"));


            #endregion

            #region Act

            _target.DeleteAsset(2);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAsset_CallsRegionTypeRefDataServiceDelete_StillUtilised_ReturnsAppropiateErrorMessage()
        {
            #region Arrange

            _mockAssetService.Setup(s => s.GetNumberOfAssetReferences(3)).Returns(2);
            var expectedErrorMessage = string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Video");

            #endregion

            #region Act

            var result = _target.DeleteAsset(3) as JsonResult;

            #endregion

            #region Assert

            var kendoDataSource = result.Data as DataSourceResult;
            Assert.AreEqual(expectedErrorMessage, kendoDataSource.Errors.ToString());

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteAsset_CallsRegionTypeRefDataServiceDelete_StillUtilised_SetsStatusCodeTo500()
        {
            #region Arrange

            _mockAssetService.Setup(s => s.GetNumberOfAssetReferences(3)).Returns(1);

            #endregion

            #region Act

            var result = _target.DeleteAsset(3) as JsonResult;

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DownloadAsset_CallsGetById()
        {
            #region Arrange


            #endregion

            #region Act

            _target.DownloadAsset(1);

            #endregion

            #region Assert

            _mockAssetService.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DownloadAsset_ReturnsTypeOfFilePathResult()
        {
            #region Arrange


            #endregion

            #region Act

            var result = _target.DownloadAsset(1) as FilePathResult;

            #endregion

            #region Assert

            Assert.IsInstanceOfType(result, typeof(FilePathResult));

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DownloadAsset_ReturnsCorrectFileNameAndContentType()
        {
            #region Arrange


            #endregion

            #region Act

            var result = _target.DownloadAsset(1) as FilePathResult;

            #endregion

            #region Assert

            Assert.IsTrue(result.FileName.IndexOf($"{_assets[0].OriginalFileName}{_assets[0].FileExtension}", StringComparison.CurrentCultureIgnoreCase) > 0);
            Assert.AreEqual(_assets[0].MimeType, result.ContentType);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_Regions_ReturnsViewResult()
        {
            var result = _target.Regions() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxRegionsRefDataGrid_ReturnsJsonResponse()
        {

            #region Act

            var result = _target.ReadAjaxRegionsRefDataGrid(new DataSourceRequest()) as JsonResult;

            #endregion

            #region Assert

            Assert.IsNotNull(result);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxRegionsRefDataGrid_ExceptionThrown_StatusCodeSetTo500()
        {
            # region Arrange

            _mockRegionTypeRefDataService.Setup(x => x.All()).Throws(new Exception("Exception!!"));

            #endregion

            #region Act

            var result = _target.ReadAjaxRegionsRefDataGrid(new DataSourceRequest()) as JsonResult;

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }

        [TestMethod]
        public void AdministrationController_ReadAjaxRegionsRefDataGrid_ReturnsCorrectNumberOfVideos()
        {
            var result = _target.ReadAjaxRegionsRefDataGrid(new DataSourceRequest()) as JsonResult;

            var dataSourceResult = result.Data as DataSourceResult;
            Assert.IsNotNull(dataSourceResult);

            var outputData = dataSourceResult.Data as List<RegionTypeRefDataViewModel>;

            Assert.IsNotNull(outputData);
            Assert.AreEqual(_regionTypeRefDatas.Count, outputData.Count);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>();
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockRegionTypeRefDataService.Verify(v => v.Create(It.IsAny<RegionTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_ModelStateValid_CreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>();
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockRegionTypeRefDataService.Verify(v => v.Create(It.IsAny<RegionTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_ModelStateValid_IdIsZero()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = 0;
            });
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(0, _regionTypeRefDataCreated.Id);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_ModelStateValid_ResolverGroupTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>();
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.RegionTypeName, _regionTypeRefDataCreated.RegionName);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>();
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _regionTypeRefDataCreated.SortOrder);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>();
            _mockRegionTypeRefDataService
                .Setup(s => s.Create(It.IsAny<RegionTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>();
            _mockRegionTypeRefDataService
                .Setup(s => s.Create(It.IsAny<RegionTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void AdministrationController_CreateAjaxRegionsRefDataGrid_DuplicateName_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = 99;
                x.RegionTypeName = "Japan";
            });
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_CreateAjaxRegionsRefDataGrid_DuplicateName_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = 99;
                x.RegionTypeName = "Japan";
            });
            _target.CreateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGrid_ModelStateNotValid_NoCreateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
            });
            _target.ModelState.AddModelError("XXX", "XXX");
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockRegionTypeRefDataService.Verify(v => v.Update(It.IsAny<RegionTypeRefData>()), Times.Never);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGridModelStateValid_UpdateTakesPlace()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
            });
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockRegionTypeRefDataService.Verify(v => v.Update(It.IsAny<RegionTypeRefData>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGridModelStateValid_IdSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
            });
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.Id, _regionTypeRefDataUpdated.Id);
        }

        [TestMethod]
        public void AdministrationController_UpdateAjaxRegionsRefDataGrid_DuplicateName_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
                x.RegionTypeName = "Japan";
            });
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGrid_DuplicateName_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
                x.RegionTypeName = "Japan";
            });
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader("HandledError", It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGrid_ModelStateValid_ResolverGroupTypeNameSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
            });
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.RegionTypeName, _regionTypeRefDataUpdated.RegionName);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGrid_ModelStateValid_SortOrderSet()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
            });
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            Assert.AreEqual(entity.SortOrder, _regionTypeRefDataUpdated.SortOrder);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGrid_ExceptionOccurs_SetsStatusCodeTo500()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
            });
            _mockRegionTypeRefDataService
                .Setup(s => s.Update(It.IsAny<RegionTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void ReferenceDataController_UpdateAjaxRegionsRefDataGrid_ExceptionOccurs_AppendsErrorToHeader()
        {
            var entity = UnitTestHelper.GenerateRandomData<RegionTypeRefDataViewModel>(x =>
            {
                x.Id = RegionTypeRefDataIdExists;
            });
            _mockRegionTypeRefDataService
                .Setup(s => s.Update(It.IsAny<RegionTypeRefData>()))
                .Throws(new ApplicationException("Oh no!!"));
            _target.UpdateAjaxRegionsRefDataGrid(new DataSourceRequest(), entity);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }


        [TestMethod]
        public void ReferenceDataController_DeleteRegion_CallsRegionTypeRefDataServiceDelete()
        {
            #region Arrange


            #endregion

            #region Act

            _target.DeleteRegion(2);

            #endregion

            #region Assert

            _mockRegionTypeRefDataService.Verify(x => x.Delete(It.IsAny<RegionTypeRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteRegion_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockRegionTypeRefDataService.Setup(s => s.Delete(It.IsAny<RegionTypeRefData>())).Throws(new ApplicationException("Oh no!!"));


            #endregion

            #region Act

            _target.DeleteRegion(2);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteRegion_CallsRegionTypeRefDataServiceDelete_StillUtilised_ReturnsAppropiateErrorMessage()
        {
            #region Arrange

            var expectedErrorMessage = string.Format(WebResources.ReferenceDataItemIsStillUtilised, "Region");

            #endregion

            #region Act

            var result = _target.DeleteRegion(1) as JsonResult;

            #endregion

            #region Assert

            var kendoDataSource = result.Data as DataSourceResult;
            Assert.AreEqual(expectedErrorMessage, kendoDataSource.Errors.ToString());

            #endregion
        }

        [TestMethod]
        public void ReferenceDataController_DeleteRegion_CallsRegionTypeRefDataServiceDelete_StillUtilised_SetsStatusCodeTo500()
        {
            #region Arrange


            #endregion

            #region Act

            var result = _target.DeleteRegion(1) as JsonResult;

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);

            #endregion
        }



        #region Method Authorization Requirement Tests



        #endregion
    }
}
