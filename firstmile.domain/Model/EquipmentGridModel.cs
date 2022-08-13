using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class EquipmentGridModel
    {
        public int TotalCount { get; set; }
        public IEnumerable<EquipmentModel> Data { get; set; }
    }
}
