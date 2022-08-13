using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.Domain.Utilities
{
    public class GridFilters
    {
        public List<GridFilter> Filters { get; set; }
        public string Logic { get; set; }
    }

    public class GridFilter
    {
        public string Operator { get; set; }
        public string Field { get; set; }
        public string Direction { get; set; }
        public string _value { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public List<GridSearchFilter> Searchs { get; set; }

    }

    public class GridSearchFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }

        public string Value { get; set; }
    }
}
