using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Services
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        FirstMileEntities GetDbContext();
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Detach(TEntity entity);
        TEntity GetByID(object id);
        void Insert(TEntity entity);
        void BulkInsert(List<TEntity> entities);
        IRepositoryQuery<TEntity> Query();
        void Update(TEntity entityToUpdate);
        IQueryable<TEntity> SqlQuery(string query, params object[] parameters);
        IQueryable<T> SqlQuery<T>(string query, params object[] parameters) where T : class;
        int ExecuteSqlCommand(string query, params object[] parameters);
    }
}
