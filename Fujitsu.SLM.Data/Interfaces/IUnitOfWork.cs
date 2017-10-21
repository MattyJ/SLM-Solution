using System;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;

namespace Fujitsu.SLM.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryTransaction BeginTransaction();

        void Save();

        void Refresh(RefreshMode refreshMode);

        SqlConnection CreateConnection();

        void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection);

        void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection,
            SqlTransaction transaction);

        void ExecuteNonQueryStoredProcedure(string storedProcedureName, SqlConnection connection,
            SqlTransaction transaction, params SqlParameter[] parameters);
    }
}
