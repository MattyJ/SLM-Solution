using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class AssetService : IAssetService
    {
        private readonly IRepository<Asset> _assetRepository;
        private readonly IRepository<ContextHelpRefData> _contextHelpRefData;
        private readonly IUnitOfWork _unitOfWork;

        public AssetService(IRepository<Asset> assetRepository, IRepository<ContextHelpRefData> contextHelpRefData, IUnitOfWork unitOfWork)
        {
            if (assetRepository == null)
            {
                throw new ArgumentNullException(nameof(assetRepository));
            }
            if (contextHelpRefData == null)
            {
                throw new ArgumentNullException(nameof(contextHelpRefData));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _assetRepository = assetRepository;
            _contextHelpRefData = contextHelpRefData;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Asset> All()
        {
            IEnumerable<Asset> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _assetRepository.All().ToList();
                });

            return result;
        }

        public Asset GetById(int id)
        {
            Asset result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _assetRepository.GetById(id);
                                      });
            return result;
        }

        public int Create(Asset entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _assetRepository.Insert(entity);
                    _unitOfWork.Save();
                });

            return entity.Id;
        }

        public void Update(Asset entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _assetRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(Asset entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                _assetRepository.Delete(entity);
                _unitOfWork.Save();

                // Tidy up the file if it has not been deleted
                if (System.IO.File.Exists(entity.FullPath))
                {
                    System.IO.File.Delete(entity.FullPath);
                }
            });
        }

        public int GetNumberOfAssetReferences(int id)
        {
            return _contextHelpRefData.Find(x => x.Asset != null && x.Asset.Id == id).Count();
        }
    }
}
