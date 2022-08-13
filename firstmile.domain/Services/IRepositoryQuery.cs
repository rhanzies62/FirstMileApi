using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Services
{
    public interface IRepositoryQuery<TEntity> where TEntity : BaseEntity
    {
        IRepositoryQuery<TEntity> Filter(Expression<Func<TEntity, bool>> filter);
        IQueryable<TEntity> FilterEntity(IQueryable<TEntity> entity, string orderByDefault, string orderby, string sortAscDesc, int? limit, int? offset, dynamic filter, string search, out int totalCount, params string[] searchFields);
        IQueryable<TEntity> Get();
        IQueryable<TEntity> GetPage(int page, int pageSize, out int totalCount);
        IRepositoryQuery<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IRepositoryQuery<TEntity> AsNoTracking();
        IRepositoryQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
    }
}
