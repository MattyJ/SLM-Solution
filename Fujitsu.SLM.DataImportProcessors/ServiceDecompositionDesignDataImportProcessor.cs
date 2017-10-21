using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.DataImporters.Extensions;
using Fujitsu.SLM.DataImporters.Interfaces;
using Fujitsu.SLM.DataImportProcessors.Interfaces;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Transformers.Interfaces;
using System;
using System.IO;

namespace Fujitsu.SLM.DataImportProcessors
{
    public class ServiceDecompositionDesignDataImportProcessor : IServiceDecompositionDesignDataImportProcessor
    {
        private readonly IServiceDecompositionImporter _serviceDecompositionTemplateImporter;
        private readonly ITransformTemplateToDesign _transformTemplateToDesign;
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IUnitOfWork _unitOfWork;
        private IRepositoryTransaction _repositoryTransaction;

        public ServiceDecompositionDesignDataImportProcessor(IServiceDecompositionImporter serviceDecompositionTemplateImporter,
            ITransformTemplateToDesign transformTemplateToDesign,
            IServiceDeskService serviceDeskService,
            IUnitOfWork unitOfWork)
        {
            if (serviceDecompositionTemplateImporter == null)
            {
                throw new ArgumentNullException(nameof(serviceDecompositionTemplateImporter));
            }
            if (transformTemplateToDesign == null)
            {
                throw new ArgumentNullException(nameof(transformTemplateToDesign));
            }
            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            _serviceDecompositionTemplateImporter = serviceDecompositionTemplateImporter;
            _transformTemplateToDesign = transformTemplateToDesign;
            _serviceDeskService = serviceDeskService;
            _unitOfWork = unitOfWork;
        }

        public void Execute(string serviceDecompositionFilePath, string filename, int serviceDeskId)
        {
            if (serviceDecompositionFilePath == null)
            {
                throw new ArgumentNullException(nameof(serviceDecompositionFilePath));
            }

            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (!File.Exists(serviceDecompositionFilePath))
            {
                throw new FileNotFoundException(serviceDecompositionFilePath);
            }

            // Read the contents of Service Decomposition spreadsheet (This will have been uploaded via the Web UI)
            var importResults = _serviceDecompositionTemplateImporter.ImportSpreadsheet(serviceDecompositionFilePath);


            if (importResults.ValidationResults.Count > 0)
            {
                throw new DataImportException(
                    $"Error reading Service Decomposition Template spreadsheet ({filename}) - ", importResults.ValidationResults);
            }

            if (importResults.Results.Count == 0)
            {
                throw new DataImportException(
                    $"Error reading Service Decomposition Template spreadsheet ({filename}) - Spreadsheet does not contain any valid data.");
            }

            using (var dbConnection = _unitOfWork.CreateConnection())
            {
                try
                {
                    // Open the connection and begin a transaction
                    dbConnection.Open();
                    _repositoryTransaction = _unitOfWork.BeginTransaction();

                    _serviceDeskService.Clear(serviceDeskId);

                    _transformTemplateToDesign.Transform(serviceDeskId, importResults.AsTemplateRows());

                    Save();
                }
                catch (Exception)
                {
                    // If we have a transaction then roll it back
                    Rollback();

                    // Throw the exception
                    throw;
                }

            }
        }

        public void Save()
        {
            _unitOfWork.Save();
            _repositoryTransaction?.Save();
        }

        public void Rollback()
        {
            _repositoryTransaction?.Rollback();
        }
        public void Dispose()
        {
            _repositoryTransaction?.Dispose();
            _unitOfWork.Dispose();
        }
    }
}
