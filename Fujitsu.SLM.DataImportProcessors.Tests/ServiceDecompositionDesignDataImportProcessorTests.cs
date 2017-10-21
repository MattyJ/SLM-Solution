using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.DataImporters;
using Fujitsu.SLM.DataImporters.Interfaces;
using Fujitsu.SLM.DataImportProcessors.Interfaces;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Transformers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fujitsu.SLM.DataImportProcessors.Tests
{
    [TestClass]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Robeco Service Decomposition.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Robeco Service Decomposition v05_230415.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Empty Service Decomposition.xlsx")]
    [DeploymentItem("Fujitsu.SLM.DataImportProcessors.Tests\\Aspose.Total.lic")]
    [ExcludeFromCodeCoverage]
    public class ServiceDecompositionDesignDataImportProcessorTests
    {

        private Mock<IServiceDecompositionImporter> _mockServiceDecompositionImporter;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<ITransformTemplateToDesign> _mockTransformData;
        private Mock<IServiceDeskService> _mockServiceDeskService;

        private IServiceDecompositionDesignDataImportProcessor _serviceDecompositionDesignDataImportProcessor;
        private ServiceDecompositionImporter _serviceDecompositionImporter;

        private const string ServiceDecompositionFilename = "Robeco Service Decomposition.xlsx";
        private const string OldServiceDecompositionTemplateFilename = "Robeco Service Decomposition v05_230415.xlsx";
        private const string EmptyServiceDecompositionFilename = "Empty Service Decomposition.xlsx";

        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTransformData = new Mock<ITransformTemplateToDesign>();
            _mockServiceDecompositionImporter = new Mock<IServiceDecompositionImporter>();
            _mockServiceDeskService = new Mock<IServiceDeskService>();

            _serviceDecompositionImporter = new ServiceDecompositionImporter();


            _serviceDecompositionDesignDataImportProcessor = new ServiceDecompositionDesignDataImportProcessor(_serviceDecompositionImporter,
                    _mockTransformData.Object,
                    _mockServiceDeskService.Object,
                    _mockUnitOfWork.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionDesignDataImportProcessor_Contructor_NoServiceDecompositionDesignDataImportProcessor_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionDesignDataImportProcessor = new ServiceDecompositionDesignDataImportProcessor(null,
                 _mockTransformData.Object,
                 _mockServiceDeskService.Object,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionDesignDataImportProcessor_Contructor_NoTransformData_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionDesignDataImportProcessor = new ServiceDecompositionDesignDataImportProcessor(_mockServiceDecompositionImporter.Object,
                null,
                _mockServiceDeskService.Object,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionDesignDataImportProcessor_Contructor_NoServiceDeskService_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionDesignDataImportProcessor = new ServiceDecompositionDesignDataImportProcessor(_mockServiceDecompositionImporter.Object,
                _mockTransformData.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionDesignDataImportProcessor_Contructor_NoUnitOfWork_ThrowsException()
        {
            #region Arrange

            _serviceDecompositionDesignDataImportProcessor = new ServiceDecompositionDesignDataImportProcessor(_mockServiceDecompositionImporter.Object,
                _mockTransformData.Object,
                _mockServiceDeskService.Object,
                null);

            #endregion
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionDesignDataImportProcessor_NullServiceDecompositionTemplateFileName_ThrowsException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionDesignDataImportProcessor.Execute(null, null, 1);

            #region Act

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceDecompositionDesignDataImportProcessor_NullFileName_ThrowsException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionDesignDataImportProcessor.Execute($@"C:\{ServiceDecompositionFilename}", null, 1);

            #region Act

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ServiceDecompositionDesignDataImportProcessor_FileNameDoesNotExist_ThrowsException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionDesignDataImportProcessor.Execute($@"..//Fujitsu.SLM.DataImportProcessors.Tests//non_existent_file.xlsx",
                ServiceDecompositionFilename,
                1);

            #region Act

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(DataImportException))]
        public void ServiceDecompositionDesignDataImportProcessor_OldTemplateFile_ThrowsDataImportException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionDesignDataImportProcessor.Execute($@"{OldServiceDecompositionTemplateFilename}",
                OldServiceDecompositionTemplateFilename,
                1);

            #region Act

            #endregion

        }

        [TestMethod]
        [ExpectedException(typeof(DataImportException))]
        public void ServiceDecompositionDesignDataImportProcessor_EmptyFile_ThrowsDataImportException()
        {
            #region Arrange

            #endregion

            _serviceDecompositionDesignDataImportProcessor.Execute($@"{EmptyServiceDecompositionFilename}",
                EmptyServiceDecompositionFilename,
                1);

            #region Act

            #endregion

        }

        #region Happy Path

        // Difficult to unit test, because it relies on a SQLConnection and a transaction.

        #endregion

    }
}
