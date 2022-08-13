using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class UsageModel
    {
        public int Id { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public double CellUsage { get; set; }
        public double TotalUsage { get; set; }
        public double OtherUsage { get; set; }
        public string GatewayId { get; set; }
    }
}
