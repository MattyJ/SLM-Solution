using Fujitsu.SLM.Data.Interfaces;
using System.Data.Entity;

namespace Fujitsu.SLM.Data.Repository
{
    public class RepositoryTransaction : IRepositoryTransaction
    {
        private readonly DbContextTransaction _transaction;

        public RepositoryTransaction(DbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Save()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
