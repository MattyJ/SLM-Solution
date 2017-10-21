using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Web.Tests.Identity
{
    [TestClass]
    public class ApplicationSignInManagerTests
    {
        private Mock<IUserStore<ApplicationUser>> _mockUserStore;
        private Mock<ApplicationUserManager> _mockApplicationUserManager;
        private Mock<IAuthenticationManager> _mockAuthenticationManager;
        private Mock<IParameterService> _mockParameterService;

        private const int UserInactivityLockoutDays = 30;
        private const string UserName = "bob.balls@hotmail.com";
        private const string UserPassword = "XXX";
        private ApplicationUser _applicationUser;
        private ApplicationUser _applicationUserUpdated;

        private ApplicationSignInManager _target;

        [TestInitialize]
        public void Initialize()
        {
            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();

            _applicationUser = UnitTestHelper.GenerateRandomData<ApplicationUser>(x =>
            {
                x.UserName = UserName;
                x.LastLoginUtc = DateTime.UtcNow;
            });

            _mockApplicationUserManager = new Mock<ApplicationUserManager>(_mockUserStore.Object);
            _mockApplicationUserManager.Setup(s => s.FindByNameAsync(UserName))
                .Returns(Task.FromResult(_applicationUser));
            _mockApplicationUserManager.Setup(s => s.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(IdentityResult.Success))
                .Callback<ApplicationUser>(c => _applicationUserUpdated = c);

            _mockAuthenticationManager = new Mock<IAuthenticationManager>();

            _mockParameterService = new Mock<IParameterService>(MockBehavior.Strict);
            _mockParameterService.Setup(s => s.GetParameterByNameAndCache<int>(ParameterNames.UserInactivityLockoutDays))
                .Returns(UserInactivityLockoutDays);

            _target = new ApplicationSignInManager(_mockApplicationUserManager.Object,
                _mockAuthenticationManager.Object,
                _mockParameterService.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplicationSignInManager_Ctor_UserManagerIsNull_ThrowsArgumentNullException()
        {
            new ApplicationSignInManager(null,
                _mockAuthenticationManager.Object,
                _mockParameterService.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplicationSignInManager_Ctor_AuthenticationManagerIsNull_ThrowsArgumentNullException()
        {
            new ApplicationSignInManager(_mockApplicationUserManager.Object,
                null,
                _mockParameterService.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplicationSignInManager_Ctor_ParameterServiceIsNull_ThrowsArgumentNullException()
        {
            new ApplicationSignInManager(_mockApplicationUserManager.Object,
                _mockAuthenticationManager.Object,
                null);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_LockoutPeriodRetrieved_ParameterServiceInvoked()
        {
            var taskResult = await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_UserCannotBeFound_FailureStatusReturned()
        {
            var taskResult = await _target.PasswordSignInAsync("XXX", UserPassword, true, true);
            Assert.AreEqual(SignInStatus.Failure, taskResult);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_UserIsFoundDateOK_UserNotUpdated()
        {
            await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
            _mockApplicationUserManager.Verify(v => v.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_UserIsFoundDateTooOld_UserIsUpdated()
        {
            _applicationUser.LastLoginUtc = DateTime.UtcNow.AddDays(UserInactivityLockoutDays * -2);
            await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
            _mockApplicationUserManager.Verify(v => v.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_UserIsFoundDateTooOld_UserLockoutDateIsSetToMaximum()
        {
            _applicationUser.LastLoginUtc = DateTime.UtcNow.AddDays(UserInactivityLockoutDays * -2);
            await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
            Assert.IsTrue(_applicationUserUpdated.LockoutEndDateUtc.HasValue);
            Assert.AreEqual(9999, _applicationUserUpdated.LockoutEndDateUtc.Value.Year);
            Assert.AreEqual(12, _applicationUserUpdated.LockoutEndDateUtc.Value.Month);
            Assert.AreEqual(31, _applicationUserUpdated.LockoutEndDateUtc.Value.Day);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_UserIsFoundDateTooOld_ReturnedStatusIsLocked()
        {
            _applicationUser.LastLoginUtc = DateTime.UtcNow.AddDays(UserInactivityLockoutDays * -2);
            var taskResult = await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
            Assert.AreEqual(SignInStatus.LockedOut, taskResult);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_ViewerUserIsFoundDateTooOld_ReturnedStatusIsLocked()
        {
            _mockApplicationUserManager.Setup(s => s.IsInRoleAsync(_applicationUser.Id, "Viewer")).Returns(Task.FromResult(true));
            _applicationUser.LastLoginUtc = DateTime.UtcNow.AddDays(UserInactivityLockoutDays * -2);
            var taskResult = await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
            Assert.AreEqual(SignInStatus.LockedOut, taskResult);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_ArchitectUserIsFoundDateTooOld_ReturnedStatusIsLocked()
        {
            _mockApplicationUserManager.Setup(s => s.IsInRoleAsync(_applicationUser.Id, "Architect")).Returns(Task.FromResult(true));
            _applicationUser.LastLoginUtc = DateTime.UtcNow.AddDays(UserInactivityLockoutDays * -2);
            var taskResult = await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
            Assert.AreEqual(SignInStatus.LockedOut, taskResult);
        }

        [TestMethod]
        public async Task ApplicationSignInManager_PasswordSignInAsync_AdministratorUserIsFoundDateTooOld_ReturnedStatusIsNotLocked()
        {
            _mockApplicationUserManager.Setup(s => s.IsInRoleAsync(_applicationUser.Id, "Administrator")).Returns(Task.FromResult(true));
            _applicationUser.LastLoginUtc = DateTime.UtcNow.AddDays(UserInactivityLockoutDays * -2);
            var taskResult = await _target.PasswordSignInAsync(UserName, UserPassword, true, true);
            Assert.AreEqual(SignInStatus.Failure, taskResult);
        }
    }
}