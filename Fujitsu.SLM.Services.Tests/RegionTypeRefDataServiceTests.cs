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

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RegionTypeRefDataServiceTests
    {

        private Mock<IRepository<RegionTypeRefData>> _mockRegionTypeRefDataRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private IRegionTypeRefDataService _regionTypeRefDataService;
        private List<RegionTypeRefData> _regions;


        [TestInitialize]
        public void TestInitilize()
        {
            _regions = new List<RegionTypeRefData>
            {
                new RegionTypeRefData{Id=1, RegionName= "Americas", SortOrder = 5},
                new RegionTypeRefData{Id=2, RegionName= "Asia", SortOrder = 5},
                new RegionTypeRefData{Id=3, RegionName= "EMEIA", SortOrder = 5},
                new RegionTypeRefData{Id=4, RegionName= "Global Delivery", SortOrder = 5},
                new RegionTypeRefData{Id=5, RegionName= "Japan", SortOrder = 5},
                new RegionTypeRefData{Id=6, RegionName= "Ocenia", SortOrder = 5}
            };

            _mockRegionTypeRefDataRepository = MockRepositoryHelper.Create(_regions);
            _unitOfWork = new Mock<IUnitOfWork>();

            _regionTypeRefDataService = new RegionTypeRefDataService(_mockRegionTypeRefDataRepository.Object,
                _unitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegionTypeRefDataService_Constructor_NoRegionRefDataRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new RegionTypeRefDataService(
                null,
                _unitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegionTypeRefDataService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new RegionTypeRefDataService(
                _mockRegionTypeRefDataRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void RegionTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var region = new RegionTypeRefData
            {
                Id = 9,
                RegionName = "UK",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _regionTypeRefDataService.Create(region);

            #endregion

            #region Assert

            _mockRegionTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<RegionTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(9, response);

            #endregion
        }

        [TestMethod]
        public void RegionTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var region = new RegionTypeRefData
            {
                Id = 4,
                RegionName = "GD",
            };

            #endregion

            #region Act

            _regionTypeRefDataService.Update(region);

            #endregion

            #region Assert

            _mockRegionTypeRefDataRepository.Verify(x => x.Update(It.IsAny<RegionTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void RegionTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var region = new RegionTypeRefData()
            {
                Id = 4,
            };

            #endregion

            #region Act

            _regionTypeRefDataService.Delete(region);

            #endregion

            #region Assert

            _mockRegionTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<RegionTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void RegionTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _regionTypeRefDataService.All();

            #endregion

            #region Assert

            _mockRegionTypeRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void RegionTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _regionTypeRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockRegionTypeRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

    }
}
