using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
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
    public class InputTypeRefDataServiceTests
    {

        private Mock<IRepository<InputTypeRefData>> _mockInputTypeRefDataRepository;
        private Mock<IRepository<DeskInputType>> _mockDeskInputTypeRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private IInputTypeRefDataService _inputTypeRefDataService;
        private List<InputTypeRefData> _inputTypes;
        private List<DeskInputType> _deskInputTypes;

        [TestInitialize]
        public void TestInitilize()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _inputTypes = new List<InputTypeRefData>
            {
                new InputTypeRefData{Id=1,InputTypeNumber = 1, InputTypeName= "Input A", SortOrder = 5},
                new InputTypeRefData{Id=2,InputTypeNumber = 2, InputTypeName= "Input B", SortOrder = 5},
                new InputTypeRefData{Id=3,InputTypeNumber = 3, InputTypeName= "Input C", SortOrder = 5},
                new InputTypeRefData{Id=4,InputTypeNumber = 4, InputTypeName= "Input D", SortOrder = 5}
            };

            _mockInputTypeRefDataRepository = MockRepositoryHelper.Create(_inputTypes);

            _deskInputTypes = new List<DeskInputType>
            {
                UnitTestHelper.GenerateRandomData<DeskInputType>(x =>
                {
                    x.Id = 1;
                    x.InputTypeRefData = _inputTypes.First(y => y.Id == 1);
                }),
                UnitTestHelper.GenerateRandomData<DeskInputType>(x =>
                {
                    x.Id = 2;
                    x.InputTypeRefData = _inputTypes.First(y => y.Id == 1);
                }),
                UnitTestHelper.GenerateRandomData<DeskInputType>(x =>
                {
                    x.Id = 3;
                    x.InputTypeRefData = _inputTypes.First(y => y.Id == 1);
                }),
                UnitTestHelper.GenerateRandomData<DeskInputType>(x =>
                {
                    x.Id = 4;
                    x.InputTypeRefData = _inputTypes.First(y => y.Id == 3);
                }),
            };

            _mockDeskInputTypeRepository = MockRepositoryHelper.Create(_deskInputTypes);

            _inputTypeRefDataService = new InputTypeRefDataService(
                _mockInputTypeRefDataRepository.Object, _mockDeskInputTypeRepository.Object, _unitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InputTypeRefDataService_Constructor_NoInputTypeRefDataRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new InputTypeRefDataService(
                null,
                _mockDeskInputTypeRepository.Object,
                _unitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InputTypeRefDataService_Constructor_NoDeskInputTypeRefRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new InputTypeRefDataService(
                _mockInputTypeRefDataRepository.Object,
                null,
                _unitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InputTypeRefDataService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new InputTypeRefDataService(
                _mockInputTypeRefDataRepository.Object,
                _mockDeskInputTypeRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void InputTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var inputType = new InputTypeRefData
            {
                Id = 5,
                InputTypeName = "Input MJJ",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _inputTypeRefDataService.Create(inputType);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<InputTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(5, response);

            #endregion
        }

        [TestMethod]
        public void InputTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var inputType = new InputTypeRefData
            {
                Id = 4,
                InputTypeName = "Input MJJ",
            };

            #endregion

            #region Act

            _inputTypeRefDataService.Update(inputType);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.Update(It.IsAny<InputTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void InputTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var inputType = new InputTypeRefData
            {
                Id = 4,
            };

            #endregion

            #region Act

            _inputTypeRefDataService.Delete(inputType);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<InputTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void InputTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _inputTypeRefDataService.All();

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void InputTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _inputTypeRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockInputTypeRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void InputTypeRefDataService_IsInputTypeReferenced_CallsRepositoryAny()
        {
            #region Arrange

            #endregion

            #region Act

            _inputTypeRefDataService.IsInputTypeReferenced(1);

            #endregion

            #region Assert

            _mockDeskInputTypeRepository.Verify(x => x.Any(It.IsAny<Expression<Func<DeskInputType, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void InputTypeRefDataService_GetInputTypeRefDataWithUsageStats_ReturnsCorrectType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _inputTypeRefDataService.GetInputTypeRefDataWithUsageStats();

            #endregion

            #region Assert

            _mockDeskInputTypeRepository.Verify(x => x.All(), Times.Exactly(4));
            Assert.IsInstanceOfType(result, typeof(List<InputTypeRefDataListItem>));

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataService_GetInputTypeRefDataWithUsageStats_ReturnsCorrectUsageStats()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _inputTypeRefDataService.GetInputTypeRefDataWithUsageStats().ToList();

            #endregion

            #region Assert

            _mockDeskInputTypeRepository.Verify(x => x.All(), Times.Exactly(4));
            Assert.AreEqual(3, result.First(x => x.Id == 1).UsageCount);
            Assert.AreEqual(1, result.First(x => x.Id == 3).UsageCount);

            #endregion
        }
    }
}
