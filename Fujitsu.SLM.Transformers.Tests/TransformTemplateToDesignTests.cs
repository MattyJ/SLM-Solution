using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Transformers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.SLM.Transformers.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TransformTemplateToDesignTests
    {
        private Mock<IServiceDeskService> _mockServiceDeskService;
        private Mock<IServiceDomainService> _mockServiceDomainService;
        private Mock<IServiceFunctionService> _mockServiceFunctionService;
        private Mock<IServiceComponentService> _mockServiceComponentService;
        private Mock<IDomainTypeRefDataService> _mockDomainTypeRefDataService;
        private Mock<IFunctionTypeRefDataService> _mockFunctionTypeRefDataService;
        private Mock<IServiceDeliveryOrganisationTypeRefDataService> _mockServiceDeliveryOrganisationTypeRefDataService;
        private Mock<IServiceDeliveryUnitTypeRefDataService> _mockServiceDeliveryUnitTypeRefDataService;
        private Mock<IResolverGroupTypeRefDataService> _mockResolverGroupTypeRefDataService;
        private Mock<IUserIdentity> _mockUserIdentity;

        private ITransformTemplateToDesign _transformTemplateToDesign;

        [TestInitialize]
        public void TestInitilize()
        {
            _mockServiceDeskService = new Mock<IServiceDeskService>();
            _mockServiceDomainService = new Mock<IServiceDomainService>();
            _mockServiceFunctionService = new Mock<IServiceFunctionService>();
            _mockServiceComponentService = new Mock<IServiceComponentService>();
            _mockDomainTypeRefDataService = new Mock<IDomainTypeRefDataService>();
            _mockFunctionTypeRefDataService = new Mock<IFunctionTypeRefDataService>();
            _mockServiceDeliveryOrganisationTypeRefDataService = new Mock<IServiceDeliveryOrganisationTypeRefDataService>();
            _mockServiceDeliveryUnitTypeRefDataService = new Mock<IServiceDeliveryUnitTypeRefDataService>();
            _mockResolverGroupTypeRefDataService = new Mock<IResolverGroupTypeRefDataService>();
            _mockUserIdentity = new Mock<IUserIdentity>();

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoServiceDeskService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(null,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoServiceDomainService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                null,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoServiceFunctionService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                null,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoServiceComponentService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                null,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoDomainTypeRefDataService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                null,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoFunctionTypeRefDataService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                null,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoServiceDeliveryOrganisationTypeRefDataService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                null,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoServiceDeliveryUnitTypeRefDataService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                null,
                _mockResolverGroupTypeRefDataService.Object,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoResolverGroupTypeRefDataService_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                null,
                _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformTemplateToDesign_Contructor_NoUserIdentity_ThrowsException()
        {
            #region Arrange

            _transformTemplateToDesign = new TransformTemplateToDesign(_mockServiceDeskService.Object,
                _mockServiceDomainService.Object,
                _mockServiceFunctionService.Object,
                _mockServiceComponentService.Object,
                _mockDomainTypeRefDataService.Object,
                _mockFunctionTypeRefDataService.Object,
                _mockServiceDeliveryOrganisationTypeRefDataService.Object,
                _mockServiceDeliveryUnitTypeRefDataService.Object,
                _mockResolverGroupTypeRefDataService.Object,
                null);

            #endregion
        }

        #endregion

    }
}
