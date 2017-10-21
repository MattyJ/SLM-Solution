using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Diagram = Fujitsu.SLM.Model.Diagram;

namespace Fujitsu.SLM.Services
{
    public class DiagramService : IDiagramService
    {
        private readonly IRepository<Diagram> _diagramRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DiagramService(IRepository<Diagram> diagramRepository, IUnitOfWork unitOfWork)
        {
            if (diagramRepository == null)
            {
                throw new ArgumentNullException(nameof(diagramRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _diagramRepository = diagramRepository;
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Diagram> LevelDiagrams(int level, int id)
        {
            IQueryable<Diagram> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _diagramRepository.Query(c => c.CustomerId == id && c.Level == level);
                });

            return result;
        }

        public IQueryable<Diagram> Diagrams(int id)
        {
            IQueryable<Diagram> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _diagramRepository.Query(c => c.CustomerId == id);
                });

            return result;
        }

        public IEnumerable<Diagram> All()
        {
            IEnumerable<Diagram> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _diagramRepository.All().ToList();
                });

            return result;
        }

        public Diagram GetById(int id)
        {
            Diagram result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _diagramRepository.GetById(id);
                                      });
            return result;
        }

        public IQueryable<Diagram> GetByCustomerId(int customerId)
        {
            IQueryable<Diagram> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _diagramRepository
                    .Query(q => q.CustomerId == customerId)
                    .AsNoTracking();
            });

            return result;
        }

        public Diagram GetByCustomerAndId(int customerId, int id)
        {
            Diagram result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _diagramRepository
                    .Query(q => q.CustomerId == customerId && q.Id == id)
                    .SingleOrDefault();
            });

            return result;
        }

        public int Create(Diagram entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _diagramRepository.Insert(entity);
                    _unitOfWork.Save();
                });

            return entity.Id;
        }

        public void Update(Diagram entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _diagramRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(Diagram entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _diagramRepository.Delete(entity);
                                          _unitOfWork.Save();
                                      });
        }
    }
}
