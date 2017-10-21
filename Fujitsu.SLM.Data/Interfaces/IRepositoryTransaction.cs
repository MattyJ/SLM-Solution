using System;

namespace Fujitsu.SLM.Data.Interfaces
{
    public interface IRepositoryTransaction : IDisposable
    {
        void Rollback();
        void Save();
    }
}
