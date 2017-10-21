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
    public class ContextHelpRefDataServiceTests
    {

        private Mock<IRepository<ContextHelpRefData>> _mockContextHelpRefDataRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private IContextHelpRefDataService _contextHelpRefDataService;

        private List<ContextHelpRefData> _contextHelpRefData;

        [TestInitialize]
        public void TestInitilize()
        {
            _contextHelpRefData = new List<ContextHelpRefData>
            {
                new ContextHelpRefData
                {
                    Id=1,
                    HelpText = "This is help for the <b>Home Page</b>.",
                    Key="homepage",
                    Title="Home Page Help",
                },
            };

            _mockContextHelpRefDataRepository = MockRepositoryHelper.Create(_contextHelpRefData);

            _unitOfWork = new Mock<IUnitOfWork>();


            _contextHelpRefDataService = new ContextHelpRefDataService(_mockContextHelpRefDataRepository.Object,
                _unitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContextHelpRefDataService_Constructor_NoContextHelpRefDataRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ContextHelpRefDataService(
                null,
                _unitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContextHelpRefDataService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new ContextHelpRefDataService(
                _mockContextHelpRefDataRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void ContextHelpRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var contextHelp = new ContextHelpRefData()
            {
                Id = 2,
                Key = "key",
                Title = "Sample",
                HelpText = "Sample help text.",
            };

            #endregion

            #region Act

            var response = _contextHelpRefDataService.Create(contextHelp);

            #endregion

            #region Assert

            _mockContextHelpRefDataRepository.Verify(x => x.Insert(It.IsAny<ContextHelpRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(2, response);

            #endregion
        }

        [TestMethod]
        public void ContextHelpRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var contextHelp = new ContextHelpRefData()
            {
                Id = 1,
                Key = "homepage",
                Title = "Sample",
                HelpText = "Sample help text.",
            };

            #endregion

            #region Act

            _contextHelpRefDataService.Update(contextHelp);

            #endregion

            #region Assert

            _mockContextHelpRefDataRepository.Verify(x => x.Update(It.IsAny<ContextHelpRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ContextHelpRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var contextHelp = new ContextHelpRefData()
            {
                Id = 1,
            };

            #endregion

            #region Act

            _contextHelpRefDataService.Delete(contextHelp);

            #endregion

            #region Assert

            _mockContextHelpRefDataRepository.Verify(x => x.Delete(It.IsAny<ContextHelpRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ContextHelpRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _contextHelpRefDataService.All();

            #endregion

            #region Assert

            _mockContextHelpRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ContextHelpRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _contextHelpRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockContextHelpRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ContextHelpRefDataService_GetByHelpKey_CallsRepositoryGetByHelpKey()
        {
            #region Arrange

            #endregion

            #region Act

            _contextHelpRefDataService.GetByHelpKey("homepage");

            #endregion

            #region Assert

            _mockContextHelpRefDataRepository.Verify(x => x.FirstOrDefault(It.IsAny<Expression<Func<ContextHelpRefData, bool>>>()), Times.Once);

            #endregion
        }

    }
}
