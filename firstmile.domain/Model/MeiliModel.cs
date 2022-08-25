using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class MeiliModel
    {
        public int MeiliId { get; set; }
        public int EquipmentId { get; set; }
        public int SubscriptionId { get; set; }
        public int CameraId { get; set; }
        public int FileDestination { get; set; }
        public string ProjectName { get; set; }
        public int EncoderId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateString {  get { return CreatedDate.ToShortDateString(); } }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedDateString { get { return UpdatedDate.ToShortDateString(); } }
        public int StatusId { get; set; }
    }
}
