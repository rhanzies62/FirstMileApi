using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class ApiLogModel
    {
        public int LogID { get; set; }
        public string IPAddress { get; set; }
        public string RequestHeader { get; set; }
        public string RequestContentType { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public System.DateTime? RequestTimestamp { get; set; }
        public string ResponseContentType { get; set; }
        public int ResponseStatusCode { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
        public string RequestBody { get; set; }
    }
}
