using firstmile.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services
{
    public static class Extension
    {
        public static string ParseQuery(this string query, GridFilter filter,string defaultOrderBy)
        {
            if (filter.Searchs != null && filter.Searchs.Any())
            {
                string where = "Where ";
                filter.Searchs.ForEach((search) =>
                {
                    if (search.Operator == "like")
                    {
                        where += $" {search.Field} {search.Operator} '%{search.Value}%' or";
                    }
                    else
                    {
                        where += $" {search.Field} {search.Operator} '{search.Value}' or";
                    }
                });
                where = where.Substring(0, where.Length - 3);
                query = query.Replace(FMServiceResource.QueryToken_Where, where);
            }
            else { query = query.Replace(FMServiceResource.QueryToken_Where, string.Empty); }

            if (!string.IsNullOrEmpty(filter.Field) && !string.IsNullOrEmpty(filter.Direction))
            {
                query = query.Replace(FMServiceResource.QueryToken_OrderBy, $"{filter.Field} {filter.Direction}");
            }
            else
            {
                query = query.Replace(FMServiceResource.QueryToken_OrderBy, defaultOrderBy);
            }

            query = query.Replace(FMServiceResource.QueryToken_Take, filter.Take.ToString());
            query = query.Replace(FMServiceResource.QueryToken_Skip, filter.Skip.ToString());

            return query;
        }
    }
}
