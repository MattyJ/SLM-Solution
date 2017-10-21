using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IRepository<Parameter> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentity _userIdentity;
        private readonly ICacheManager _cacheManager;

        public ParameterService(IRepository<Parameter> repository,
            IUnitOfWork unitOfWork,
            IUserIdentity userIdentity,
            ICacheManager cacheManager)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }
            if (cacheManager == null)
            {
                throw new ArgumentNullException(nameof(cacheManager));
            }

            _repository = repository;
            _unitOfWork = unitOfWork;
            _userIdentity = userIdentity;
            _cacheManager = cacheManager;
        }

        public IEnumerable<Parameter> All()
        {
            IEnumerable<Parameter> result = null;
            RetryableOperation.Invoke(ExceptionPolicies.General, () => { result = _repository.All().ToList(); });
            return result;
        }

        public int Create(Parameter entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _repository.Insert(entity);
                _unitOfWork.Save();
            });

            return entity.Id;
        }

        public void Update(Parameter entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                // Invalidate the cache.
                _cacheManager.Remove(entity.ParameterName);

                _repository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(Parameter entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _repository.Delete(entity);
                _unitOfWork.Save();
            });
        }

        public Parameter Find(string parameterName)
        {
            Parameter result = null;
            RetryableOperation.Invoke(ExceptionPolicies.General, () => { result = _repository.SingleOrDefault(x => x.ParameterName == parameterName); });
            return result;
        }

        public T GetParameterByName<T>(string parameterName)
        {
            var result = default(T);
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                var paramValue = _repository
                    .Query(x => x.ParameterName == parameterName)
                    .Select(s => s.ParameterValue)
                    .SingleOrDefault();
                if (!string.IsNullOrEmpty(paramValue))
                {
                    result = paramValue.ConvertStringToGenericValue<T>();
                }
            });
            return result;
        }

        public T GetParameterByNameAndCache<T>(string parameterName)
        {
            return _cacheManager.ExecuteAndCache(parameterName,
                () => this.GetParameterByName<T>(parameterName));
        }

        public T GetParameterByNameOrCreate<T>(string parameterName, T defaultValue, ParameterType parameterType)
        {
            var result = default(T);

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
                {
                    var paramValue = _repository
                        .Query(x => x.ParameterName == parameterName)
                        .Select(s => s.ParameterValue)
                        .SingleOrDefault();

                    if (string.IsNullOrEmpty(paramValue))
                    {
                        var entity = new Parameter
                        {
                            ParameterName = parameterName,
                            ParameterValue = defaultValue.ConvertGenericValueToString(),
                            Type = parameterType,
                            InsertedBy = _userIdentity.Name,
                            InsertedDate = DateTime.Now,
                            UpdatedBy = _userIdentity.Name,
                            UpdatedDate = DateTime.Now
                        };
                        _repository.Insert(entity);
                        result = defaultValue;
                    }
                    else
                    {
                        result = paramValue.ConvertStringToGenericValue<T>();
                    }
                });

            return result;
        }

        public void SaveParameter<T>(string parameterName, T value, ParameterType parameterType)
        {
            var dateTime = DateTime.Now;
            var valueString = value.ConvertGenericValueToString();

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                // Invalidate the cache.
                _cacheManager.Remove(parameterName);

                var entity = _repository.SingleOrDefault(d => d.ParameterName == parameterName);

                if (entity != null)
                {
                    entity.ParameterValue = valueString;
                    entity.UpdatedDate = dateTime;
                    entity.UpdatedBy = _userIdentity.Name;
                    _repository.Update(entity);
                }
                else
                {
                    entity = new Parameter
                    {
                        Type = parameterType,
                        ParameterName = parameterName,
                        ParameterValue = valueString,
                        InsertedBy = _userIdentity.Name,
                        InsertedDate = dateTime,
                        UpdatedBy = _userIdentity.Name,
                        UpdatedDate = dateTime
                    };

                    _repository.Insert(entity);
                }

                _unitOfWork.Save();
            });
        }
    }
}