using firstmile.domain;
using firstmile.domain.Services;
using firstmile.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.data
{
    public sealed class RepositoryQuery<TEntity> : IRepositoryQuery<TEntity> where TEntity : BaseEntity
    {
        private readonly List<Expression<Func<TEntity, object>>> _includeProperties;
        private readonly GenericRepository<TEntity> _repository;
        private readonly List<Expression<Func<TEntity, bool>>> _filters;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderByQuerable;
        private int? _page;
        private int? _pageSize;
        private bool _asNoTracking;

        public RepositoryQuery(GenericRepository<TEntity> repository)
        {
            _repository = repository;
            _includeProperties = new List<Expression<Func<TEntity, object>>>();
            _filters = new List<Expression<Func<TEntity, bool>>>();
        }

        public IRepositoryQuery<TEntity> Filter(Expression<Func<TEntity, bool>> filter)
        {
            //_filter = filter;
            _filters.Add(filter);
            return this;
        }

        public IRepositoryQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderByQuerable = orderBy;
            return this;
        }

        public IRepositoryQuery<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            _includeProperties.Add(expression);
            return this;
        }

        public IRepositoryQuery<TEntity> AsNoTracking()
        {
            _asNoTracking = true;
            return this;
        }

        public IQueryable<TEntity> GetPage(int page, int pageSize, out int totalCount)
        {
            _page = page;
            _pageSize = pageSize;
            totalCount = _repository.Get(_filters).Count();

            return _repository.Get(_filters, _orderByQuerable, _includeProperties, _page, _pageSize, _asNoTracking);
        }

        public IQueryable<TEntity> Get()
        {
            return _repository.Get(_filters, _orderByQuerable, _includeProperties, _page, _pageSize, _asNoTracking);
        }

        public IQueryable<TEntity> SqlQuery(string query, params object[] parameters)
        {
            return _repository.SqlQuery(query, parameters).AsQueryable();
        }

        public IQueryable<T> SqlQuery<T>(string query, params object[] parameters) where T : class
        {
            return _repository.SqlQuery<T>(query, parameters).AsQueryable();
        }

        public IQueryable<TEntity> FilterEntity(IQueryable<TEntity> entity,
            string orderByDefault,
            string orderby,
            string sortAscDesc,
            int? limit,
            int? offset,
            dynamic filter,
            string search,
            out int totalCount,
            params string[] searchFields)
        {
            GridFilters gridFilters = GetGridFilters(filter);
            gridFilters.Logic = "and";
            AddSearchFilters(gridFilters, search, searchFields);

            List<GridSort> gridSort = GetSortBy(sortAscDesc, orderby);

            object[] parameters;
            string whereClause = GetWhereClause(gridFilters, out parameters);
            if (!String.IsNullOrEmpty(whereClause))
                entity = entity.Where(whereClause, parameters);

            if (gridSort != null && gridSort.Count > 0)
            {
                foreach (var s in gridSort)
                {
                    entity = entity.OrderBy(orderby + " " + sortAscDesc);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(orderByDefault))
                    entity = entity.OrderBy(orderByDefault);
            }

            var totalEntity = entity;
            totalCount = totalEntity.Select(s => 1).Count();
            entity = entity.Skip(Convert.ToInt32(offset)).Take(Convert.ToInt32(limit));

            return entity;
        }

        private GridFilters GetGridFilters(dynamic filter)
        {
            GridFilters gridFilters = new GridFilters();
            gridFilters.Filters = new List<GridFilter>();

            if (filter != null)
            {
                var filtersParam = ((IDictionary<String, object>)filter);
                if (filtersParam != null)
                {
                    foreach (var item in filtersParam)
                    {
                        var values = ((IDictionary<String, object>)item.Value);
                        foreach (var itemVal in values)
                        {
                            var data = (IList<object>)itemVal.Value;
                            List<string> paramList = new List<string>();
                            foreach (var dataItem in data)
                            {

                                if (dataItem != null)
                                {
                                    paramList.Add(dataItem.ToString());
                                }
                            }
                            if (paramList.Count > 0)
                                gridFilters.Filters.Add(new GridFilter() { _value = String.Join(",", paramList.ToArray()), Field = item.Key.Replace("_", ".").ToString(), Operator = "IN" });
                        }
                    }
                }
            }

            return gridFilters;
        }

        private void AddSearchFilters(GridFilters gridFilters, string search, params string[] searchFields)
        {
            string[] fieldsToSearch = searchFields;
            if (!String.IsNullOrEmpty(search))
            {
                foreach (var item in fieldsToSearch)
                {
                    gridFilters.Filters.Add(new GridFilter() { _value = search, Field = item.Replace("_", ".").ToString(), Operator = "LIKE" });
                }
            }
        }

        private List<GridSort> GetSortBy(string sortAscDesc, string orderBy)
        {
            GridSort gs = new GridSort();
            gs.field = orderBy;
            gs.dir = sortAscDesc;

            List<GridSort> sort = new List<GridSort>();
            if (!String.IsNullOrEmpty(gs.field))
                sort.Add(gs);

            return sort;
        }

        private string GetWhereClause(GridFilters filter, out object[] parameters)
        {
            int paramCount = 0;
            string whereClause = null;

            if (filter != null && (filter.Filters != null && filter.Filters.Count > 0))
            {
                parameters = new object[filter.Filters.Count];
                var valueLists = new List<string>();
                var filters = filter.Filters;

                bool firstLikeClause = true, firstEqualClause = true, hasInClause = false;

                for (var i = 0; i < filters.Count; i++)
                {
                    valueLists = new List<string>();
                    if (filters[i].Operator.ToUpper() == "IN")
                    {
                        hasInClause = true;

                        //values in FIlter needs to split by comma and added to List to work in IN clause
                        var values = filters[i]._value.Split(',');
                        valueLists.AddRange(values);
                        parameters[i] = valueLists;

                        if (i == 0)
                            whereClause += string.Format("@{0}.Contains(outerIt.{1}.ToString())", paramCount, filters[i].Field);
                        else
                            whereClause += string.Format(" && @{0}.Contains(outerIt.{1}.ToString())", paramCount, filters[i].Field);
                    }
                    else if (filters[i].Operator.ToUpper() == "LIKE")
                    {

                        //values need to use directly when using LIKE clause
                        parameters[i] = filters[i]._value;

                        if (i == 0)
                            whereClause += string.Format("{1}.ToString().Contains(@{0})", paramCount, filters[i].Field);
                        else if (firstLikeClause && hasInClause)
                            whereClause += string.Format(" && ({1}.ToString().Contains(@{0})", paramCount, filters[i].Field);
                        else
                            whereClause += string.Format(" || {1}.ToString().Contains(@{0})", paramCount, filters[i].Field);

                        firstLikeClause = false;
                    }
                    else//Use generic EQUAL clause
                    {
                        //values need to use directly when using LIKE clause
                        parameters[i] = filters[i]._value;

                        if (i == 0)
                            whereClause += string.Format("{1}=@{0}", paramCount, filters[i].Field);
                        else if (firstEqualClause && hasInClause)
                            whereClause += string.Format(" && ({1}=@{0}", paramCount, filters[i].Field);
                        else
                            whereClause += string.Format(" || {1}=@{0}", paramCount, filters[i].Field);

                        firstEqualClause = false;
                    }

                    paramCount++;
                }

                if (hasInClause && (!firstEqualClause || !firstLikeClause))
                {
                    //add extra closing ) to wrap OR clauses
                    whereClause += ")";
                }
            }
            else
            {
                parameters = null;
            }

            return whereClause;
        }
    }
}
