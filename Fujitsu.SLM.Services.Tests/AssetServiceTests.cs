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
using System.Linq.Expressions;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AssetServiceTests
    {
        private Mock<IRepository<Asset>> _mockAssetRepository;
        private Mock<IRepository<ContextHelpRefData>> _mockContextHelpRefDataRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private IAssetService _assetService;
        private List<Asset> _assets;


        [TestInitialize]
        public void TestInitilize()
        {
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

            _mockAssetRepository = MockRepositoryHelper.Create(_assets);
            _mockContextHelpRefDataRepository = new Mock<IRepository<ContextHelpRefData>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _assetService = new AssetService(
                _mockAssetRepository.Object,
                _mockContextHelpRefDataRepository.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssetService_Constructor_NoAssetRepository()
        {
            #region Act

            new AssetService(
                null,
                _mockContextHelpRefDataRepository.Object,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssetService_Constructor_NoContextHelpRefDataRepository()
        {
            #region Act

            new AssetService(
                _mockAssetRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssetService_Constructor_NoUnitOfWork()
        {

            #region Act

            new AssetService(
                _mockAssetRepository.Object,
                _mockContextHelpRefDataRepository.Object,
                null);

            #endregion
        }

        #endregion


        [TestMethod]
        public void AssetService_Create_CallInsertsAssetAndCallsSaveChanges()
        {
            #region Arrange

            var asset = new Asset
            {
                Id = 3,
                FileExtension = ".mp4",
                FileName = Guid.NewGuid().ToString(),
                OriginalFileName = "AddServiceDomain",
                MimeType = "video/mp4",
                FullPath = "C:\\Media\\Video\\AddServiceDomain.mp4"
            };

            #endregion

            #region Act

            var response = _assetService.Create(asset);

            #endregion

            #region Assert

            _mockAssetRepository.Verify(x => x.Insert(It.IsAny<Asset>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(3, response);

            #endregion
        }

        [TestMethod]
        public void AssetService_Update_CallUpdateAssetAndCallsSaveChanges()
        {
            #region Arrange

            var asset = new Asset
            {
                Id = 2
            };

            #endregion

            #region Act

            _assetService.Update(asset);

            #endregion

            #region Assert

            _mockAssetRepository.Verify(x => x.Update(It.IsAny<Asset>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void AssetService_Delete_CallDeleteAssetAndCallsSaveChanges()
        {
            #region Arrange

            var asset = new Asset
            {
                Id = 2
            };

            #endregion

            #region Act

            _assetService.Delete(asset);

            #endregion

            #region Assert

            _mockAssetRepository.Verify(x => x.Delete(It.IsAny<Asset>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void AssetService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _assetService.All();

            #endregion

            #region Assert

            _mockAssetRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void AssetService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _assetService.GetById(1);

            #endregion

            #region Assert

            _mockAssetRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void AssetService_GetNumberOfAssetReferences_CallsContextHelpRefDataRepositoryFind()
        {
            #region Arrange

            #endregion

            #region Act

            _assetService.GetNumberOfAssetReferences(1);

            #endregion

            #region Assert

            _mockContextHelpRefDataRepository.Verify(x => x.Find(It.IsAny<Expression<Func<ContextHelpRefData, bool>>>()), Times.Once);

            #endregion
        }

    }
}
