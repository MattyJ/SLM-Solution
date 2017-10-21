using Fujitsu.SLM.Data.Interfaces;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Fujitsu.SLM.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SLMDataContext _dbContext;

        public UnitOfWork(SLMDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepositoryTransaction BeginTransaction()
        {
            return new RepositoryTransaction(_dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted));
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Refresh(RefreshMode refreshMode)
        {
            var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            var refreshableObjects = _dbContext.ChangeTracker
                .Entries()
                .Where(w => w.State != EntityState.Added)
                .Select(c => c.Entity)
                .ToList();
            objectContext.Refresh(refreshMode, refreshableObjects);
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_dbContext.Database.Connection.ConnectionString);
        }

        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection)
        {
            ExecuteNonQueryStoredProcedure(storedProcedureName, connection, null, null);
        }

        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection, SqlTransaction transaction)
        {
            ExecuteNonQueryStoredProcedure(storedProcedureName, connection, transaction, null);
        }

        public void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection, SqlTransaction transaction, params SqlParameter[] parameters)
        {
            // create the command
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedureName;
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            command.ExecuteNonQuery();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
