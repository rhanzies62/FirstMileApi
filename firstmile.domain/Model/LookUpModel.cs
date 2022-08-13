using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class LookUpModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Ordinal { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public int Type { get; set; }
        public int? GatewayId { get; set; }
    }
}
