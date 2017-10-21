using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IService<T> where T : class
    {
        T GetById(int id);

        IEnumerable<T> All();

        int Create(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}