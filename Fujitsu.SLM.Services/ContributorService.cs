using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class ContributorService : IContributorService
    {
        private readonly IRepository<Contributor> _contributorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContributorService(IRepository<Contributor> contributorRepository, IUnitOfWork unitOfWork)
        {
            if (contributorRepository == null)
            {
                throw new ArgumentNullException(nameof(contributorRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _contributorRepository = contributorRepository;
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Contributor> GetCustomersContributors(int customerId)
        {
            IQueryable<Contributor> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _contributorRepository.Query(c => c.CustomerId == customerId).Include("Customer");
                });

            return result;
        }

        public bool DeleteUserContributors(string userId)
        {
            var result = false;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    var contributors = GetUserContributors(userId).ToList();
                    foreach (var contributor in contributors)
                    {
                        Delete(contributor, false);
                    }
                    _unitOfWork.Save();
                    result = true;
                });

            return result;

        }

        public IEnumerable<Contributor> All()
        {
            IEnumerable<Contributor> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _contributorRepository.All().ToList();
                });

            return result;
        }

        public Contributor GetById(int id)
        {
            Contributor result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _contributorRepository.GetById(id);
                                      });
            return result;
        }

        public int Create(Contributor entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _contributorRepository.Insert(entity);
                    _unitOfWork.Save();
                });

            return entity.Id;
        }

        public void Update(Contributor entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _contributorRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(Contributor entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _contributorRepository.Delete(entity);
                                          _unitOfWork.Save();
                                      });
        }


        public bool IsContributor(int customerId, string emailAddress)
        {
            var result = false;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                          () =>
                          {
                              result = _contributorRepository.Any(c => c.CustomerId == customerId && c.EmailAddress == emailAddress);
                          });

            return result;
        }

        private IEnumerable<Contributor> GetUserContributors(string userId)
        {
            IEnumerable<Contributor> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _contributorRepository.Query(c => c.UserId == userId).AsNoTracking();
                });

            return result;
        }
        private void Delete(Contributor entity, bool save)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _contributorRepository.Delete(entity);
                                          if (save)
                                              _unitOfWork.Save();
                                      });
        }

    }
}
