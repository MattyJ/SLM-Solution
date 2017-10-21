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
using System.Web;
using System.Web.Mvc;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HelpControllerTests
    {
        private Mock<IContextHelpRefDataService> _mockContextHelpRefDataService;
        private Mock<IRepository<ContextHelpRefData>> _mockContextHelpRefDataRepository;
        private Mock<IRepository<Asset>> _mockAssetRepository;
        private Mock<IContextManager> _mockContextManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<ContextHelpRefData> _contextHelpRefData;

        private IContextHelpRefDataService _contextHelpRefDataService;

        private Mock<IResponseManager> _mockResponseManager;

        private HelpController _mockController;
        private HelpController _controller;
        private AppContext _appContext;

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

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _appContext = new AppContext();

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);


            _mockContextManager = new Mock<IContextManager>();
            _mockResponseManager = new Mock<IResponseManager>();
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);

            _contextHelpRefData = new List<ContextHelpRefData>
            {
                new ContextHelpRefData
                {
                    Id = 1,
                    Key = "homepage",
                    Title = "Homepage",
                    HelpText = "This is the <b>Home Page</b> help.",
                    AssetId = 1,
                    Asset =  new Asset {
                        Id = 1,
                        FileExtension = ".mp4",
                        FileName = Guid.NewGuid().ToString(),
                        OriginalFileName = "Register SLM Generator Tutorial",
                        MimeType = "video/mp4",
                        FullPath = "Fujitsu.SLM.Web.Tests\\Register SLM Generator Tutorial.mp4"
                    }
                },
                new ContextHelpRefData
                {
                    Id = 2,
                    Key = "changepasswordpage",
                    Title = "ChangePasswordPage",
                    HelpText = "This is the <b>Change Password</b> help.",
                    AssetId = 2,
                    Asset = new Asset{
                        Id = 2,
                        FileExtension = ".mp4",
                        FileName = Guid.NewGuid().ToString(),
                        OriginalFileName = "ChangePassword",
                        MimeType = "video/mp4",
                        FullPath = "C:\\Media\\Video\\ChangePassword.mp4"
                    }
                }
            };

            _mockContextHelpRefDataRepository = MockRepositoryHelper.Create(_contextHelpRefData, (entity, id) => entity.Id == (int)id);

            _mockContextHelpRefDataService = new Mock<IContextHelpRefDataService>();
            _mockContextHelpRefDataService.Setup(s => s.GetByHelpKey("homepage")).Returns(_contextHelpRefData[0]);
            _mockContextHelpRefDataService.Setup(s => s.GetById(It.IsAny<int>()))
                .Returns<int>(id => _contextHelpRefData.SingleOrDefault(x => x.Id == id));

            _mockAssetRepository = new Mock<IRepository<Asset>>();

            _mockController = new HelpController(
                _mockContextHelpRefDataService.Object,
                _mockAssetRepository.Object,
                _mockContextManager.Object
                );

            _contextHelpRefDataService = new ContextHelpRefDataService(_mockContextHelpRefDataRepository.Object,
                _mockUnitOfWork.Object);

            _controller = new HelpController(
                _contextHelpRefDataService,
                _mockAssetRepository.Object,
                _mockContextManager.Object
                );

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HelpController_Constructor_NoContextHelpRefDataThrowsException()
        {
            new HelpController(
                null,
                _mockAssetRepository.Object,
                _mockContextManager.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HelpController_Constructor_NoAssetRepositoryThrowsException()
        {
            new HelpController(
                _mockContextHelpRefDataService.Object,
                null,
                _mockContextManager.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HomeController_Constructor_NoContextManagerThrowsException()
        {
            new HelpController(
                _mockContextHelpRefDataService.Object,
                _mockAssetRepository.Object,
                null
                );
        }


        #endregion

        [TestMethod]
        public void HelpController_Index_Get_ReturnsViewResult()
        {
            var result = _controller.Index("homepage") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HelpController_Index_ReturnsContextHelpViewModel()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.Index("homepage") as ViewResult;
            var model = result.Model as ContextHelpRefDataViewModel;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(model, typeof(ContextHelpRefDataViewModel));

            #endregion
        }

        [TestMethod]
        public void HelpController_Index_CallsGetByKeyAndDoesntCallCreateForExistingHelp()
        {
            #region Arrange

            const string helpKey = "homepage";

            #endregion

            #region Act

            var result = _mockController.Index(helpKey) as ViewResult;

            #endregion

            #region Assert


            Assert.IsNotNull(result);
            _mockContextHelpRefDataService.Verify(x => x.GetByHelpKey(helpKey), Times.Once);
            _mockContextHelpRefDataService.Verify(x => x.Create(It.IsAny<ContextHelpRefData>()), Times.Never);


            #endregion
        }

        [TestMethod]
        public void HelpController_Index_CallsGetByKeyAndDoesCallCreateForHelpThatDoesntExist()
        {
            #region Arrange

            const string helpKey = "fictitious";

            #endregion

            #region Act

            var result = _mockController.Index(helpKey) as ViewResult;

            #endregion

            #region Assert


            Assert.IsNotNull(result);
            _mockContextHelpRefDataService.Verify(x => x.GetByHelpKey(helpKey), Times.Once);
            _mockContextHelpRefDataService.Verify(x => x.Create(It.IsAny<ContextHelpRefData>()), Times.Once);


            #endregion
        }

        [TestMethod]
        public void HelpController_Edit_Get_ReturnsViewResult()
        {
            var result = _controller.Edit("homepage") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HelpController_Edit_ReturnsContextHelpViewModel()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _controller.Edit("homepage") as ViewResult;
            var model = result.Model as ContextHelpRefDataViewModel;

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(model, typeof(ContextHelpRefDataViewModel));

            #endregion
        }

        [TestMethod]
        public void HelpController_Edit_HttpGet_CallsGetByKey()
        {
            #region Arrange

            const string helpKey = "homepage";

            #endregion

            #region Act

            var result = _mockController.Edit(helpKey) as ViewResult;

            #endregion

            #region Assert


            Assert.IsNotNull(result);
            _mockContextHelpRefDataService.Verify(x => x.GetByHelpKey(helpKey), Times.Once);


            #endregion
        }


        [TestMethod]
        public void HelpController_Edit_Update_HttpPost_WithoutFileCallsContextHelpRefDataUpdateOnlyForValidModelAndAction()
        {
            #region Arrange

            const string action = "update";
            var model = new ContextHelpRefDataViewModel
            {
                Id = 1,
                Key = "homepage",
                Title = "Home Page",
                HelpText = "New amazing help text!"
            };

            #endregion

            #region Act

            _mockController.Edit(model, action);

            #endregion

            #region Assert

            _mockContextHelpRefDataService.Verify(x => x.Update(It.IsAny<ContextHelpRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HelpController_Edit_Update_HttpPost_WithFileCallsAssetInsertAndContextHelpRefDataUpdateForValidModelAndAction()
        {
            #region Arrange
            var file = new Mock<HttpPostedFileBase>();

            const string action = "update";
            var model = new ContextHelpRefDataViewModel
            {
                Id = 1,
                Key = "homepage",
                Title = "Home Page",
                HelpText = "New amazing help text!",
                HelpVideoFile = file.Object
            };

            #endregion

            #region Act

            _mockController.Edit(model, action);

            #endregion

            #region Assert

            _mockContextHelpRefDataService.Verify(x => x.Update(It.IsAny<ContextHelpRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HelpController_Edit__Update_HttpPost_RedirectsToCorrectControllerAndActionForValidModelAndAction()
        {
            const string expectedAction = "Index";
            const string expectedController = "Help";

            const string action = "update";
            var model = new ContextHelpRefDataViewModel
            {
                Id = 1,
                Key = "homepage",
                Title = "Home Page",
                HelpText = "New amazing help text!"
            };

            var result = _mockController.Edit(model, action) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
        }

        [TestMethod]
        public void HelpController_Edit_Cancel_HttpPost_RedirectsToCorrectControllerAndActionForValidModelAndAction()
        {
            const string expectedAction = "Index";
            const string expectedController = "Help";

            const string action = "cancel";
            var model = new ContextHelpRefDataViewModel
            {
                Id = 1,
                Key = "homepage",
                Title = "Home Page",
                HelpText = "New amazing help text! With some new text but user decided to cancel :-("
            };

            var result = _mockController.Edit(model, action) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
        }


        [TestMethod]
        public void HelpController_DeleteHelpVideo_CallsContextHelpRefDataServiceUpdate()
        {
            #region Arrange


            #endregion

            #region Act

            _mockController.DeleteHelpVideo(1);

            #endregion

            #region Assert

            _mockContextHelpRefDataService.Verify(x => x.Update(It.IsAny<ContextHelpRefData>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HelpController_DeleteHelpVideo_CallsDeletesAssetRespository()
        {
            #region Arrange


            #endregion

            #region Act

            _mockController.DeleteHelpVideo(1);

            #endregion

            #region Assert

            _mockAssetRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void HelpController_DeleteAsset_ExceptionAppendsErrorMessageToHeader()
        {
            #region Arrange

            _mockContextHelpRefDataService.Setup(s => s.Update(It.IsAny<ContextHelpRefData>())).Throws(new ApplicationException("Oh no!!"));


            #endregion

            #region Act

            _mockController.DeleteHelpVideo(2);

            #endregion

            #region Assert

            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);

            #endregion
        }

        #region Method Authorization Requirement Tests

        [TestMethod]
        public void HelpController_Edit_HttpGet_CheckRole_RoleIsAdministrator()
        {
            Assert.AreEqual(UserRoles.Administrator, _controller.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(string) }));
        }

        [TestMethod]
        public void HelpController_Edit_HttpPost_CheckRole_RoleIsAdministrator()
        {
            Assert.AreEqual(UserRoles.Administrator, _controller.GetMethodAttributeValue("Edit", (AuthorizeAttribute att) => att.Roles, new Type[] { typeof(ContextHelpRefDataViewModel), typeof(string) }));
        }

        #endregion
    }
}
