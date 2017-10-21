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
    public class TemplateServiceTests
    {
        private Mock<IRepository<Template>> _mockTemplateRepository;
        private Mock<IRepository<TemplateDomain>> _mockTemplateDomainRepository;
        private Mock<IRepository<TemplateRow>> _mockTemplateRowRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private ITemplateService _templateService;
        private List<Template> _templates;
        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";


        [TestInitialize]
        public void TestInitilize()
        {
            var dateTimeNow = DateTime.Now;

            _templates = new List<Template>
            {
                new Template
                {
                    Id = 1,
                    Filename = "templateOne.xls",
                    TemplateData = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 },
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                },
                new Template
                {
                    Id = 2,
                    Filename = "templateTwo.xls",
                    TemplateData = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 },
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                },
                new Template
                {
                    Id = 3,
                    Filename = "templateThree.xls",
                    TemplateData = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 },
                    InsertedBy = UserNameOne,
                    InsertedDate = dateTimeNow,
                    UpdatedBy = UserNameOne,
                    UpdatedDate = dateTimeNow
                }
            };

            _mockTemplateRepository = MockRepositoryHelper.Create(_templates);
            _mockTemplateDomainRepository = new Mock<IRepository<TemplateDomain>>();
            _mockTemplateRowRepository = new Mock<IRepository<TemplateRow>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();


            _templateService = new TemplateService(
                _mockTemplateRepository.Object,
                _mockTemplateDomainRepository.Object,
                _mockTemplateRowRepository.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateService_Constructor_NoTemplateRepository()
        {
            #region Act

            new TemplateService(
                null,
                _mockTemplateDomainRepository.Object,
                _mockTemplateRowRepository.Object,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateService_Constructor_NoTemplateDomainRepository()
        {
            #region Act

            new TemplateService(
                _mockTemplateRepository.Object,
                null,
                _mockTemplateRowRepository.Object,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateService_Constructor_NoTemplateRowRepository()
        {

            #region Act

            new TemplateService(
                _mockTemplateRepository.Object,
                _mockTemplateDomainRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateService_Constructor_NoUnitOfWork()
        {

            #region Act

            new TemplateService(
                _mockTemplateRepository.Object,
                _mockTemplateDomainRepository.Object,
                _mockTemplateRowRepository.Object,
                null);

            #endregion
        }

        #endregion


        [TestMethod]
        public void TemplateService_Create_CallInsertsTemplateAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var template = new Template
            {
                Id = 4,
                Filename = "TemplateFour.xls",
                TemplateData = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 },
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            var response = _templateService.Create(template);

            #endregion

            #region Assert

            _mockTemplateRepository.Verify(x => x.Insert(It.IsAny<Template>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(4, response);

            #endregion
        }

        [TestMethod]
        public void TemplateService_Update_CallUpdateTemplateAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var template = new Template
            {
                Id = 3,
                TemplateData = new byte[] { 0x20, 0x20, 0x20, 0x20 },
                InsertedBy = UserNameOne,
                InsertedDate = dateTimeNow,
                UpdatedBy = UserNameOne,
                UpdatedDate = dateTimeNow,
            };

            #endregion

            #region Act

            _templateService.Update(template);

            #endregion

            #region Assert

            _mockTemplateRepository.Verify(x => x.Update(It.IsAny<Template>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void TemplateService_Delete_CallDeleteTemplateAndCallsSaveChanges()
        {
            #region Arrange

            var dateTimeNow = DateTime.Now;

            var template = new Template
            {
                Id = 3,
            };

            #endregion

            #region Act

            _templateService.Delete(template);

            #endregion

            #region Assert

            _mockTemplateRepository.Verify(x => x.Delete(It.IsAny<Template>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void TemplateService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _templateService.All();

            #endregion

            #region Assert

            _mockTemplateRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TemplateService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _templateService.GetById(1);

            #endregion

            #region Assert

            _mockTemplateRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        public void TemplateService_AllTemplates_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _templateService.AllTemplates();

            #endregion

            #region Assert

            _mockTemplateRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TemplateService_AllTemplateDomains_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _templateService.AllTemplateDomains("SORT");

            #endregion

            #region Assert

            _mockTemplateDomainRepository.Verify(x => x.Query(It.IsAny<Expression<Func<TemplateDomain, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void TemplateService_GetTemplateRows_CallsRepositoryQuery()
        {
            #region Arrange

            #endregion

            #region Act

            _templateService.GetTemplateRows(1);

            #endregion

            #region Assert

            _mockTemplateRowRepository.Verify(x => x.Query(It.IsAny<Expression<Func<TemplateRow, bool>>>()), Times.Once);

            #endregion
        }



    }
}
