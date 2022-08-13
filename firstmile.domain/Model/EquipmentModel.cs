using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class EquipmentModel
    {

        public EquipmentModel()
        {

        }

        public EquipmentModel(FmEquipment fmBookEquipment)
        {
            this.EquipmentId = fmBookEquipment.EquipmentId;
            this.Name = fmBookEquipment.Name;
            this.TypeId = fmBookEquipment.Type;
            this.Company = fmBookEquipment.Company;
            this.Description = fmBookEquipment.Description;
            this.CreatedDate = fmBookEquipment.CreatedDate;
        }
        public int EquipmentId { get; set; }

        [Required]
        public string Name { get; set; }

        public int TypeId { get; set; }

        public string Type { get; set; }

        [Required]
        public string Company { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public string Username { get; set; }

        public int? GatewayId { get; set; }

        public string Serial { get; set; }

        public bool IsActive { get; set; }

        public string CustomerName { get; set; }

        public string ProjectName { get; set; }

        public string BorrowedDateFrom { get; set; }

        public string BorrowedDateTo { get; set; }

        public string Color { get; set; }

        public string UpdatedByUsername { get; set; }

        public int SalesId { get; set; }

        public float TotalUsage { get; set; }

        public float TotalCellUsage { get; set; }

        public float OtherUsage { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }
    }
}
