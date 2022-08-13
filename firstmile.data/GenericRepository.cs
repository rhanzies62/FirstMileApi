using firstmile.domain;
using firstmile.domain.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.data
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        internal FirstMileEntities _context;
        internal DbSet<TEntity> _dbSet;

        public GenericRepository(FirstMileEntities context)
        {

            this._context = context;
            this._dbSet = context.Set<TEntity>();
        }

        public FirstMileEntities GetDbContext()
        {
            return _context;
        }

        public virtual IRepositoryQuery<TEntity> Query()
        {
            return new RepositoryQuery<TEntity>(this);
        }

        internal IQueryable<TEntity> Get(
            List<Expression<Func<TEntity, bool>>> filters = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includeProperties = null,
            int? page = null,
            int? pageSize = null,
            bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filters != null && filters.Count > 0)
                filters.ForEach(f => query = query.Where(f));

            if (includeProperties != null)
                includeProperties.ForEach(i => query = query.Include(i));

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            if (asNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void BulkInsert(List<TEntity> entities)
        {
            _dbSet.AddRange(entities);
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

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void Detach(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public virtual IQueryable<TEntity> SqlQuery(string query, params object[] parameters)
        {
            return _context.Database.SqlQuery<TEntity>(query, parameters).AsQueryable();
        }

        public virtual IQueryable<T> SqlQuery<T>(string query, params object[] parameters) where T : class
        {
            return _context.Database.SqlQuery<T>(query, parameters).AsQueryable();
        }

        public virtual int ExecuteSqlCommand(string query, params object[] parameters)
        {
            return _context.Database.ExecuteSqlCommand(query, parameters);
        }

        private void FixEfProviderServicesProblem()
        {
            // The Entity Framework provider type 'System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer'
            // for the 'System.Data.SqlClient' ADO.NET provider could not be loaded. 
            // Make sure the provider assembly is available to the running application. 
            // See http://go.microsoft.com/fwlink/?LinkId=260882 for more information.
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
