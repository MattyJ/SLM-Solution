using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.DataImporters;
using Fujitsu.SLM.DataImportProcessors.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Transformers;
using Fujitsu.SLM.Transformers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fujitsu.SLM.DataImportProcessors.Tests
{
    [TestClass]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Robeco Service Decomposition.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Robeco Service Decomposition v05_230415.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Empty Service Decomposition.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Invalid SDO Robeco Service Decomposition.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Robeco Service Decomposition Multiple Resolvers per Component.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Aspose.Total.lic")]
    [ExcludeFromCodeCoverage]
    public class ServiceDecompositionTemplateDataImportProcessorTests
    {
        private IServiceDecompositionTemplateDataImportProcessor _serviceDecompositionTemplateDataImportProcessor;
        private ITransformSpreadsheetToTemplate _transformSpreadsheetToTemplate;
        private Mock<ITransformSpreadsheetToTemplate> _mockTransformSpreadsheetToTemplate;
        private Mock<ITemplateService> _mockTemplateService;
        private Mock<IUserIdentity> _mockUserIdentity;
        private ServiceDecompositionImporter _serviceDecompositionImporter;
        private List<Template> _templates;
        private const string ServiceDecompositionFilename = "Robeco Service Decomposition.xlsx";
        private const string OldServiceDecompositionTemplateFilename = "Robeco Service Decomposition v05_230415.xlsx";
        private const string EmptyServiceDecompositionFilename = "Empty Service Decomposition.xlsx";
        private const string InvalidServiceDeliveryOrganisationServiceDecompositionFilename = "Invalid SDO Robeco Service Decomposition.xlsx";
        private const string MultipleResolversPerComponentFilename = "Robeco Service Decomposition Multiple Resolvers per Component.xlsx";

        [TestInitialize]
        public void TestInitialize()
        {
            _templates = new List<Template>
            {
                new Template {
                Id = 1,
                TemplateType = TemplateTypeNames.SLM.GetEnumIntFromText<TemplateType>(),
                Filename = ServiceDecompositionFilename
            }};

            _mockTemplateService = new Mock<ITemplateService>();
            _mockUserIdentity = new Mock<IUserIdentity>();
            _serviceDecompositionImporter = new ServiceDecompositionImporter();
            _mockTransformSpreadsheetToTemplate = new Mock<ITransformSpreadsheetToTemplate>();

            _transformSpreadsheetToTemplate = new TransformSpreadsheetToTemplate(_mockUserIdentity.Object);

            _serviceDecompositionTemplateDataImportProcessor = new ServiceDecompositionTemplateDataImportProcessor(_serviceDecompositionImporter,
                _transformSpreadsheetToTemplate,
                _mockTemplateService.Object,
                _mockUserIdentity.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionTemplateDataImportProcessor_Contructor_NoServiceDecompositionTemplateImporter_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionTemplateDataImportProcessor = new ServiceDecompositionTemplateDataImportProcessor(null,
                                                                    _mockTransformSpreadsheetToTemplate.Object,
                                                                    _mockTemplateService.Object,
                                                                    _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionTemplateDataImportProcessor_Contructor_NoTransformSpreadsheetToTemplat_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionTemplateDataImportProcessor = new ServiceDecompositionTemplateDataImportProcessor(_serviceDecompositionImporter,
                                                                    null,
                                                                    _mockTemplateService.Object,
                                                                    _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionTemplateDataImportProcessor_Contructor_NoTemplateService_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionTemplateDataImportProcessor = new ServiceDecompositionTemplateDataImportProcessor(_serviceDecompositionImporter,
                                                                    _mockTransformSpreadsheetToTemplate.Object,
                                                                    null,
                                                                    _mockUserIdentity.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionTemplateDataImportProcessor_Contructor_NoUserIdentity_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionTemplateDataImportProcessor = new ServiceDecompositionTemplateDataImportProcessor(_serviceDecompositionImporter,
                                                                    _mockTransformSpreadsheetToTemplate.Object,
                                                                    _mockTemplateService.Object,
                                                                    null);

            #endregion
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionTemplateDataImportProcessor_NullServiceDecompositionTemplateFileName_ThrowsException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute(null, null, TemplateTypeNames.SLM.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionTemplateDataImportProcessor_NullFileName_ThrowsException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"C:\{ServiceDecompositionFilename}", null, TemplateTypeNames.SLM.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ServiceDecompositionTemplateDataImportProcessor_InvalidTemplateType_ThrowsException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"C:\{ServiceDecompositionFilename}", ServiceDecompositionFilename, 99);

            #region Act

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ServiceDecompositionTemplateDataImportProcessor_FileNameDoesNotExist_ThrowsException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"..//Fujitsu.SLM.DataImportProcessors.Tests//non_existent_file.xlsx",
                ServiceDecompositionFilename,
                TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(DataImportException))]
        public void ServiceDecompositionTemplateDataImportProcessor_OldTemplateFile_ThrowsDataImportException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"{OldServiceDecompositionTemplateFilename}",
                OldServiceDecompositionTemplateFilename,
                TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

        }

        [TestMethod]
        [ExpectedException(typeof(DataImportException))]
        public void ServiceDecompositionTemplateDataImportProcessor_EmptyFile_ThrowsDataImportException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"{EmptyServiceDecompositionFilename}",
                EmptyServiceDecompositionFilename,
                TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

        }

        [TestMethod]
        [ExpectedException(typeof(DataImportException))]
        public void ServiceDecompositionTemplateDataImportProcessor_SORTTemplateAlreadyExists_ThrowsDataImportException()
        {
            #region Arrange

            _templates.Add(new Template
            {
                Id = 2,
                TemplateType = TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>(),
                Filename = ServiceDecompositionFilename
            });

            _mockTemplateService.Setup(s => s.All()).Returns(_templates);

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"{ServiceDecompositionFilename}",
                ServiceDecompositionFilename,
                TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

            #region Assert

            _mockTemplateService.Verify(v => v.Create(It.IsAny<Template>()), Times.Once);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(DataImportException))]
        public void ServiceDecompositionTemplateDataImportProcessor_InvalidServiceDeliveryOrganisation_ThrowsDataImportException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"{InvalidServiceDeliveryOrganisationServiceDecompositionFilename}",
                InvalidServiceDeliveryOrganisationServiceDecompositionFilename,
                TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

        }

        [TestMethod]
        [ExpectedException(typeof(DataImportException))]
        public void ServiceDecompositionTemplateDataImportProcessor_MultipleResolversPerComponent_ThrowsDataImportException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"{MultipleResolversPerComponentFilename}",
                MultipleResolversPerComponentFilename,
                TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

        }

        [TestMethod]
        public void ServiceDecompositionTemplateDataImportProcessor_ValidFile_CreateIsCalled()
        {
            #region Arrange

            #endregion

            _serviceDecompositionTemplateDataImportProcessor.Execute($@"{ServiceDecompositionFilename}",
                ServiceDecompositionFilename,
                TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>());

            #region Act

            #endregion

            #region Assert

            _mockTemplateService.Verify(v => v.Create(It.IsAny<Template>()), Times.Once);

            #endregion
        }
    }
}
