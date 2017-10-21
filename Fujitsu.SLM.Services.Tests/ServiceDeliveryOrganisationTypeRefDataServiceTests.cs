using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class ServiceDeliveryOrganisationTypeRefDataServiceTests
    {

        private Mock<IRepository<ServiceDeliveryOrganisationTypeRefData>> _serviceDeliveryOrganisationTypeRefDataRepository;
        private Mock<IRepository<Resolver>> _resolverRepository;
        private Mock<IUnitOfWork> _unitOfWork;

        private IServiceDeliveryOrganisationTypeRefDataService _serviceDeliveryOrganisationRefDataService;
        private List<ServiceDeliveryOrganisationTypeRefData> _serviceDeliveryOrganisationTypeRefDatas;
        private List<Resolver> _resolvers;

        private const int ServiceDeliveryOrganisationTypeIdInUse = 3;
        private const int ServiceDeliveryOrganisationTypeIdNotInUse = 45;

        [TestInitialize]
        public void TestInitilize()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _serviceDeliveryOrganisationTypeRefDatas = new List<ServiceDeliveryOrganisationTypeRefData>
            {
                new ServiceDeliveryOrganisationTypeRefData{Id=1,ServiceDeliveryOrganisationTypeName= "ServiceDeliveryOrganisation A", SortOrder = 5},
                new ServiceDeliveryOrganisationTypeRefData{Id=2,ServiceDeliveryOrganisationTypeName= "ServiceDeliveryOrganisation B", SortOrder = 5},
                new ServiceDeliveryOrganisationTypeRefData{Id=3,ServiceDeliveryOrganisationTypeName= "ServiceDeliveryOrganisation C", SortOrder = 5},
                new ServiceDeliveryOrganisationTypeRefData{Id=4,ServiceDeliveryOrganisationTypeName= "ServiceDeliveryOrganisation D", SortOrder = 5},
            };

            _resolvers = new List<Resolver>
            {
                UnitTestHelper.GenerateRandomData<Resolver>(),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.ServiceDeliveryOrganisationType = UnitTestHelper
                        .GenerateRandomData<ServiceDeliveryOrganisationTypeRefData>(
                            y =>
                            {
                                y.Id = ServiceDeliveryOrganisationTypeIdInUse;
                                y.ServiceDeliveryOrganisationTypeName = "XYZ";
                            });
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(),
                UnitTestHelper.GenerateRandomData<Resolver>(),
                UnitTestHelper.GenerateRandomData<Resolver>()
            };

            _serviceDeliveryOrganisationTypeRefDataRepository = MockRepositoryHelper.Create(_serviceDeliveryOrganisationTypeRefDatas, (entity, id) => entity.Id == (int)id);

            _resolverRepository = MockRepositoryHelper.Create(_resolvers, (entity, id) => entity.Id == (int)id);

            _serviceDeliveryOrganisationRefDataService = new ServiceDeliveryOrganisationTypeRefDataService(_serviceDeliveryOrganisationTypeRefDataRepository.Object,
                _resolverRepository.Object,
                _unitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeliveryOrganisationTypeRefDataService_Constructor_NoResolverGroupRefDataRepository_ThrowsException()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDeliveryOrganisationTypeRefDataService(
                null,
                _resolverRepository.Object,
                _unitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeliveryOrganisationTypeRefDataService_Constructor_NoResolverTypeRepository_ThrowsException()
        {
            new ServiceDeliveryOrganisationTypeRefDataService(_serviceDeliveryOrganisationTypeRefDataRepository.Object,
                null,
                _unitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDeliveryOrganisationTypeRefDataService_Constructor_NoUnitOfWork_ThrowsException()
        {
            #region Arrange

            #endregion

            #region Act

            new ServiceDeliveryOrganisationTypeRefDataService(
                _serviceDeliveryOrganisationTypeRefDataRepository.Object,
                _resolverRepository.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var resolverGroup = new ServiceDeliveryOrganisationTypeRefData()
            {
                Id = 5,
                ServiceDeliveryOrganisationTypeName = "Resolver MJJ",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _serviceDeliveryOrganisationRefDataService.Create(resolverGroup);

            #endregion

            #region Assert

            _serviceDeliveryOrganisationTypeRefDataRepository.Verify(x => x.Insert(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(5, response);

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var component = new ServiceDeliveryOrganisationTypeRefData()
            {
                Id = 4,
                ServiceDeliveryOrganisationTypeName = "ServiceDeliveryOrganisation MJJ",
            };

            #endregion

            #region Act

            _serviceDeliveryOrganisationRefDataService.Update(component);

            #endregion

            #region Assert

            _serviceDeliveryOrganisationTypeRefDataRepository.Verify(x => x.Update(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var resolverGroup = new ServiceDeliveryOrganisationTypeRefData()
            {
                Id = 4,
            };

            #endregion

            #region Act

            _serviceDeliveryOrganisationRefDataService.Delete(resolverGroup);

            #endregion

            #region Assert

            _serviceDeliveryOrganisationTypeRefDataRepository.Verify(x => x.Delete(It.IsAny<ServiceDeliveryOrganisationTypeRefData>()), Times.Once());
            _unitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDeliveryOrganisationRefDataService.All();

            #endregion

            #region Assert

            _serviceDeliveryOrganisationTypeRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _serviceDeliveryOrganisationRefDataService.GetById(1);

            #endregion

            #region Assert

            _serviceDeliveryOrganisationTypeRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataService_IsResolverGroupTypeReferenced_ResolverGroupTypeInUse_ReturnsTrue()
        {
            Assert.IsTrue(_serviceDeliveryOrganisationRefDataService.IsServiceDeliveryOrganisationTypeReferenced(ServiceDeliveryOrganisationTypeIdInUse));
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataService_IsResolverGroupTypeReferenced_ResolverGroupTypeNotInUse_ReturnsFalse()
        {
            Assert.IsFalse(_serviceDeliveryOrganisationRefDataService.IsServiceDeliveryOrganisationTypeReferenced(ServiceDeliveryOrganisationTypeIdNotInUse));
        }
    }
}
