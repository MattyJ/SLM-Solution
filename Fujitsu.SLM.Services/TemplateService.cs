using AutoMapper;
using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<TemplateDomain> _templateDomainRepository;
        private readonly IRepository<TemplateRow> _templateRowRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TemplateService(IRepository<Template> templateRepository,
            IRepository<TemplateDomain> templateDomainRepository,
            IRepository<TemplateRow> templateRowRepository,
            IUnitOfWork unitOfWork)
        {
            if (templateRepository == null)
            {
                throw new ArgumentNullException(nameof(templateRepository));
            }
            if (templateDomainRepository == null)
            {
                throw new ArgumentNullException(nameof(templateDomainRepository));
            }
            if (templateRowRepository == null)
            {
                throw new ArgumentNullException(nameof(templateRowRepository));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _templateRepository = templateRepository;
            _templateDomainRepository = templateDomainRepository;
            _templateRowRepository = templateRowRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Template> All()
        {
            IEnumerable<Template> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _templateRepository.All().ToList();
                });

            return result;
        }

        public IEnumerable<TemplateListItem> AllTemplates()
        {
            IEnumerable<TemplateListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _templateRepository.All().Select(Mapper.Map<TemplateListItem>).ToList();
                });

            return result;
        }

        public IEnumerable<TemplateDomainListItem> AllTemplateDomains(string templateType)
        {
            IEnumerable<TemplateDomainListItem> result = null;
            var type = templateType.GetEnumIntFromText<TemplateType>();

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _templateDomainRepository
                        .Query(x => x.Template.TemplateType == type)
                        .AsNoTracking().Select(Mapper.Map<TemplateDomainListItem>).AsEnumerable();
                });

            return result;
        }

        public Template GetById(int id)
        {
            Template result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _templateRepository.GetById(id);
                });
            return result;
        }

        public int Create(Template entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _templateRepository.Insert(entity);
                    _unitOfWork.Save();
                });
            return entity.Id;
        }

        public void Update(Template entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _templateRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(Template entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _templateRepository.Delete(entity);
                    _unitOfWork.Save();
                });
        }

        public IEnumerable<TemplateRowListItem> GetTemplateRows(int id)
        {
            IEnumerable<TemplateRowListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _templateRowRepository
                        .Query(x => x.Template.Id == id)
                        .AsNoTracking().Select(Mapper.Map<TemplateRowListItem>).AsEnumerable();
                });

            return result;
        }
    }
}