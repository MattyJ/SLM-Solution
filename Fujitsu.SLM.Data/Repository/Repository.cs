using EntityFramework.Extensions;
using Fujitsu.SLM.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Fujitsu.SLM.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly SLMDataContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ObjectStateManager _objectStateManager;

        public Repository(SLMDataContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _objectStateManager = ((IObjectContextAdapter)_context).ObjectContext.ObjectStateManager;
        }

        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> filterExpression)
        {
            _dbSet.Where(filterExpression).Delete();
        }

        public virtual int Update(Expression<Func<TEntity, TEntity>> updateExpression)
        {
            return _dbSet.Update(updateExpression);
        }

        public virtual int Update(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            return _dbSet.Where(filterExpression).Update(updateExpression);
        }

        public virtual TEntity Update(TEntity entityToUpdate)
        {
            return Update(entityToUpdate, "Id");
        }

        public virtual TEntity Update(TEntity entityToUpdate, string primaryKeyName)
        {
            var entry = _context.Entry(entityToUpdate);

            if (entry.State == EntityState.Detached)
            {
                // Retreive the Id through reflection
                var pkey = _dbSet.Create().GetType().GetProperty(primaryKeyName).GetValue(entityToUpdate);

                TEntity attachedEntity = _dbSet.Find(pkey);  // access the key
                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entityToUpdate);
                    return attachedEntity;
                }
            }

            entry.State = EntityState.Modified; // attach the entity and return
            return entityToUpdate;

        }

        public virtual void Detach(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Detached;

        }

        public IEnumerable<TEntity> All()
        {
            return _dbSet;
        }

        public IEnumerable<TEntity> Find(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IQueryable<TEntity> Query(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.First(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }

        public int ExecuteSqlCommand(string command, object[] parameters)
        {
            return _context.Database.ExecuteSqlCommand(command, parameters);
        }

    }

}
