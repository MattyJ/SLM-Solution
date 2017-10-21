using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.TemplateProcessors.Interface;
using Fujitsu.SLM.Transformers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.TemplateProcessors
{
    public class TemplateProcessor : ITemplateProcessor
    {
        private readonly IRepository<TemplateRow> _templateRowRepository;
        private readonly ITransformTemplateToDesign _transformTemplateToDesign;
        private readonly IUnitOfWork _unitOfWork;
        private IRepositoryTransaction _repositoryTransaction;

        public TemplateProcessor(IRepository<TemplateRow> templateRowRepository,
            ITransformTemplateToDesign transformTemplateToDesign,
            IUnitOfWork unitOfWork)
        {
            if (templateRowRepository == null)
            {
                throw new ArgumentNullException(nameof(templateRowRepository));
            }
            if (transformTemplateToDesign == null)
            {
                throw new ArgumentNullException(nameof(transformTemplateToDesign));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            _templateRowRepository = templateRowRepository;
            _transformTemplateToDesign = transformTemplateToDesign;
            _unitOfWork = unitOfWork;
        }

        public void Execute(int serviceDeskId, IReadOnlyCollection<TemplateDomainListItem> selectedDomains)
        {
            using (var dbConnection = _unitOfWork.CreateConnection())
            {
                try
                {
                    // Open the connection and begin a transaction
                    dbConnection.Open();
                    _repositoryTransaction = _unitOfWork.BeginTransaction();

                    foreach (var selectedDomain in selectedDomains)
                    {
                        var templateRows = _templateRowRepository.All().Where(x => x.TemplateId == selectedDomain.TemplateId && x.ServiceDomain == selectedDomain.DomainName).ToList();
                        _transformTemplateToDesign.Transform(serviceDeskId, templateRows);
                    }

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
