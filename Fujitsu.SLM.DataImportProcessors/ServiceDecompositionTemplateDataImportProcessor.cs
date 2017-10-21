using Aspose.Cells;
using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.DataImporters.Extensions;
using Fujitsu.SLM.DataImporters.Interfaces;
using Fujitsu.SLM.DataImportProcessors.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Transformers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fujitsu.SLM.DataImportProcessors
{
    public class ServiceDecompositionTemplateDataImportProcessor : DataImportProcessorBase, IServiceDecompositionTemplateDataImportProcessor
    {
        private readonly IServiceDecompositionImporter _serviceDecompositionTemplateImporter;
        private readonly ITransformSpreadsheetToTemplate _transformSpreadsheetToTemplate;
        private readonly ITemplateService _templateService;
        private readonly IUserIdentity _userIdentity;

        public ServiceDecompositionTemplateDataImportProcessor(IServiceDecompositionImporter serviceDecompositionTemplateImporter,
            ITransformSpreadsheetToTemplate transformSpreadsheetToTemplate,
            ITemplateService templateService,
            IUserIdentity userIdentity)
        {
            if (serviceDecompositionTemplateImporter == null)
            {
                throw new ArgumentNullException(nameof(serviceDecompositionTemplateImporter));
            }
            if (transformSpreadsheetToTemplate == null)
            {
                throw new ArgumentNullException(nameof(transformSpreadsheetToTemplate));
            }
            if (templateService == null)
            {
                throw new ArgumentNullException(nameof(templateService));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            _serviceDecompositionTemplateImporter = serviceDecompositionTemplateImporter;
            _transformSpreadsheetToTemplate = transformSpreadsheetToTemplate;
            _templateService = templateService;
            _userIdentity = userIdentity;
        }

        public void Execute(string serviceDecompositionFilePath, string filename, int templateType)
        {
            // Set the license for Aspose
            var license = new License();
            license.SetLicense("Aspose.Total.lic");

            if (serviceDecompositionFilePath == null)
            {
                throw new ArgumentNullException(nameof(serviceDecompositionFilePath));
            }

            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (!Enum.IsDefined(typeof(TemplateType), templateType))
            {
                throw new ArgumentException(nameof(templateType));
            }

            if (!FileExists(serviceDecompositionFilePath))
            {
                throw new FileNotFoundException(serviceDecompositionFilePath);
            }

            if (templateType == TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>() && _templateService.All().Any(x => x.TemplateType == TemplateTypeNames.SORT.GetEnumIntFromText<TemplateType>()))
            {
                throw new DataImportException("A SORT Service Decomposition Template already exists, please delete before importing a new SORT spreadsheet.");
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

            var template = CreateTemplate(serviceDecompositionFilePath, filename, templateType);

            _transformSpreadsheetToTemplate.Transform(template, importResults.AsTemplateRows());

            _templateService.Create(template);
        }

        private Template CreateTemplate(string serviceDecompositionFilePath, string filename, int templateType)
        {
            var dateTimeNow = DateTime.Now;
            var template = new Template
            {
                Filename = filename,
                TemplateDomains = new List<TemplateDomain>(),
                TemplateType = templateType,
                TemplateRows = new List<TemplateRow>(),
                InsertedDate = dateTimeNow,
                InsertedBy = _userIdentity.Name,
                UpdatedDate = dateTimeNow,
                UpdatedBy = _userIdentity.Name
            };

            var workbook = new Workbook(serviceDecompositionFilePath);
            using (var responseStream = new MemoryStream())
            {
                workbook.Save(responseStream, new XlsSaveOptions(SaveFormat.Xlsx));
                template.TemplateData = responseStream.ToArray();
            }

            return template;
        }
    }
}
