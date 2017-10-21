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
    public class ServiceFunctionServiceTests
    {

        private Mock<IRepository<ServiceFunction>> _mockServiceFunctionRepository;
        private Mock<IRepository<FunctionTypeRefData>> _mockServiceFunctionTypeRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private IServiceFunctionService _serviceFunctionService;
        private List<ServiceFunction> _serviceFunctions;
        private List<FunctionTypeRefData> _serviceFunctionTypes;
        private const string Username = "matthew.jordan@uk.fujitsu.com";


        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            var dateTimeNow = DateTime.Now;

            _serviceFunctionTypes = new List<FunctionTypeRefData>
            {
                new FunctionTypeRefData
                {
                    Id =1,
                    FunctionName = "System Management Infrastructure",
                    SortOrder = 5,
                    Visible = true,
                },
                new FunctionTypeRefData
                {
                    Id =2,
                    FunctionName = "Desktop Virtualisation",
                    SortOrder = 5,
                    Visible = true,
                },
                new FunctionTypeRefData
                {
                    Id =3,
                    FunctionName = "Secure Remote Access",
                    SortOrder = 5,
                    Visible = true,
                },
                new FunctionTypeRefData
                {
                    Id =3,
                    FunctionName = "Computing Management",
                    SortOrder = 5,
                    Visible = true,
                }
            };


            _serviceFunctions = new List<ServiceFunction>
            {
                new ServiceFunction
                {
                    Id = 1,
                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 1),
                    ServiceDomain = new ServiceDomain{Id =1, ServiceDeskId = 1},
                    InsertedBy = Username,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = Username,
                    UpdatedDate = dateTimeNow,

                },
                new ServiceFunction()
                {
                    Id = 2,
                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 2),
                    ServiceDomain = new ServiceDomain{Id =1, ServiceDeskId = 1},
                    InsertedBy = Username,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = Username,
                    UpdatedDate = dateTimeNow,
                },
                new ServiceFunction()
                {
                    Id = 3,
                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 3),
                    ServiceDomain = new ServiceDomain{Id =1, ServiceDeskId = 1},
                    InsertedBy = Username,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = Username,
                    UpdatedDate = dateTimeNow,
                },
                new ServiceFunction()
                {
                    Id = 4,
                    FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 4),
                    ServiceDomain = new ServiceDomain{Id =1, ServiceDeskId = 1},
                    InsertedBy = Username,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = Username,
                    UpdatedDate = dateTimeNow,
                },
            };

            _mockServiceFunctionTypeRepository = MockRepositoryHelper.Create(_serviceFunctionTypes, (entity, id) => entity.Id == (int)id);
            _mockServiceFunctionRepository = MockRepositoryHelper.Create(_serviceFunctions);

            _serviceFunctionService = new ServiceFunctionService(
                _mockServiceFunctionRepository.Object, _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctionService_Constructor_NoServiceFunctionRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceFunctionService(
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceFunctionService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceFunctionService(
                _mockServiceFunctionRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void ServiceFunctionService_Create_CallInsertsFunctionAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var serviceFunction = new ServiceFunction()
            {
                Id = 5,
                FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 1),
                ServiceDomain = new ServiceDomain { Id = 1, ServiceDeskId = 1 },
                InsertedBy = Username,
                InsertedDate = dateTimeNow,
                UpdatedBy = Username,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            var response = _serviceFunctionService.Create(serviceFunction);

            #endregion

            #region Assert

            _mockServiceFunctionRepository.Verify(x => x.Insert(It.IsAny<ServiceFunction>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(_serviceFunctions.Count, response);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionService_Update_CallUpdatesFunctionAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var serviceFunction = new ServiceFunction
            {
                Id = 3,
                FunctionType = _serviceFunctionTypes.FirstOrDefault(x => x.Id == 1),
                ServiceDomain = new ServiceDomain { Id = 1, ServiceDeskId = 1 },
                InsertedBy = Username,
                InsertedDate = dateTimeNow,
                UpdatedBy = Username,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            _serviceFunctionService.Update(serviceFunction);

            #endregion

            #region Assert

            _mockServiceFunctionRepository.Verify(x => x.Update(It.IsAny<ServiceFunction>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceFunctionService.All();

            #endregion

            #region Assert

            _mockServiceFunctionRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceFunctionService.GetById(1);

            #endregion

            #region Assert

            _mockServiceFunctionRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionService_CustomerServiceFunctions_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceFunctionService.CustomerServiceFunctions(1);

            #endregion

            #region Assert

            _mockServiceFunctionRepository.Verify(x => x.Query(It.IsAny<Expression<Func<ServiceFunction, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionService_CustomerServiceFunctions_ReturnsIQueryableServiceFunctionListItem()
        {
            #region Arrange

            #endregion

            #region Act

            var functions = _serviceFunctionService.CustomerServiceFunctions(1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(functions, typeof(IQueryable<ServiceFunctionListItem>));

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionService_ServiceDomainFunctions_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceFunctionService.ServiceDomainFunctions(1);

            #endregion

            #region Assert

            _mockServiceFunctionRepository.Verify(x => x.Query(It.IsAny<Expression<Func<ServiceFunction, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceFunctionService_ServiceDomainFunctions_ReturnsIQueryableServiceFunctionListItem()
        {
            #region Arrange

            #endregion

            #region Act

            var functions = _serviceFunctionService.ServiceDomainFunctions(1);

            #endregion

            #region Assert

            Assert.IsInstanceOfType(functions, typeof(IQueryable<ServiceFunctionListItem>));

            #endregion
        }
    }
}
