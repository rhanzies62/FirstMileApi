using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.Domain.Utilities
{
    public class GridResult
    {
        public dynamic Data { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
    }

    public class GridResultGeneric<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
