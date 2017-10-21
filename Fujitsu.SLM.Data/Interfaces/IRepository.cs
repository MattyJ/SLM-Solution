using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fujitsu.SLM.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity GetById(object id);
        void AddRange(IEnumerable<TEntity> entities);
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Delete(IEnumerable<TEntity> entities);
        void Delete(Expression<Func<TEntity, bool>> filterExpression);
        TEntity Update(TEntity entityToUpdate);
        TEntity Update(TEntity entityToUpdate, string primaryKeyName);

        int Update(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression);
        int Update(Expression<Func<TEntity, TEntity>> updateExpression);
        void Detach(TEntity entity);

        IEnumerable<TEntity> All();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);
        bool Any(Expression<Func<TEntity, bool>> predicate);
        TEntity First(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        int ExecuteSqlCommand(string command, object[] parameters);
    }
}
