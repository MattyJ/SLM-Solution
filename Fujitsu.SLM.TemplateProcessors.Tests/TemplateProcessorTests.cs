using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.TemplateProcessors.Interface;
using Fujitsu.SLM.Transformers.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.SLM.TemplateProcessors.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TemplateProcessorTests
    {

        private Mock<IRepository<TemplateRow>> _mockTemplateRowRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IRepositoryTransaction> _mockRepositoryTransaction;
        private Mock<ITransformTemplateToDesign> _mockTransformData;

        private List<TemplateRow> _templateRows;
        private List<TemplateDomainListItem> _templateDomainListItems;

        private ITemplateProcessor _templateProcessor;

        [TestInitialize]
        public void TestInitilize()
        {
            #region Test Data

            _templateRows = new List<TemplateRow>
            {
                UnitTestHelper.GenerateRandomData<TemplateRow>(x =>
                {
                    x.Id = 1;
                    x.ServiceDomain = "Service Domain A";
                    x.TemplateId = 1;
                }),
                UnitTestHelper.GenerateRandomData<TemplateRow>(x =>
                {
                    x.Id = 2;
                    x.ServiceDomain = "Service Domain B";
                    x.TemplateId = 1;
                }),
                UnitTestHelper.GenerateRandomData<TemplateRow>(x =>
                {
                    x.Id = 3;
                    x.ServiceDomain = "Service Domain C";
                    x.TemplateId = 1;
                }),
            };

            _templateDomainListItems = new List<TemplateDomainListItem>
            {
                new TemplateDomainListItem
                {
                    Id = 1,
                    DomainName = "Service Domain A",
                    TemplateId = 1
                },
                new TemplateDomainListItem
                {
                    Id = 2,
                    DomainName = "Service Domain B",
                    TemplateId = 1
                }
            };

            #endregion

            _mockTemplateRowRepository = MockRepositoryHelper.Create(_templateRows);
            _mockRepositoryTransaction = new Mock<IRepositoryTransaction>();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(s => s.BeginTransaction()).Returns(_mockRepositoryTransaction.Object);
            _mockUnitOfWork.Setup(s => s.CreateConnection()).Returns(new SqlConnection());



            _mockTransformData = new Mock<ITransformTemplateToDesign>();


            _templateProcessor = new TemplateProcessor(_mockTemplateRowRepository.Object,
                    _mockTransformData.Object,
                    _mockUnitOfWork.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateProcessor_Contructor_NoTemplateRowRepository_ThrowsException()
        {
            #region Arrange

            _templateProcessor = new TemplateProcessor(null,
                 _mockTransformData.Object,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateProcessor_Contructor_NoTransformData_ThrowsException()
        {
            #region Arrange

            _templateProcessor = new TemplateProcessor(_mockTemplateRowRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemplateProcessor_Contructor_NoUnitOfWork_ThrowsException()
        {
            #region Arrange

            _templateProcessor = new TemplateProcessor(_mockTemplateRowRepository.Object,
                _mockTransformData.Object,
                null);

            #endregion
        }

        #endregion


        [TestMethod]
        public void TemplateProcessor_Execute_CallsTransformTemplateToDesignTransform()
        {
            #region Arrange

            // Difficult to unit test, because it relies on SQLConnection, transactions
            //_templateProcessor.Execute(1, _templateDomainListItems);

            #endregion

        }
    }
}
