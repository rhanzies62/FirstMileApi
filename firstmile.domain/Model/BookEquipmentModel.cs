using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class BookEquipmentModel
    {
        public int SaleEquipmentId { get; set; }
        public int SaleId { get; set; }
        public int EquipmentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime BorrowedDateFrom { get; set; }
        public DateTime BorrowedDateTo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int ServiceTypeId { get; set; }
        public virtual EquipmentModel Equipment { get; set; }
        public virtual BookingModel Booking { get; set; }

        public string ServiceType { get; set; }
    }

    public class BookingHistory
    {
        public int SaleId { get; set; }
        public string ProjectName { get; set; }
        public List<BookEquipmentGridModel> Equipments { get; set; }
    }

    public class BookEquipmentGridModel
    {
        public int SaleEquipmentId { get; set; }
        public int ServiceTypeId { get; set; }
        public int? EquipmentId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string BorrowedDateFromST { get; set; }
        public string BorrowedDateToST { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Serial { get; set; }
        public int? GatewayId { get; set; }
        public int TotalUsage { get; set; }
        public int CellUsage { get; set; }
        public int OtherUsage { get; set; }
        public int CurrentUsage { get; set; }
        public int TotalCellUsage { get; set; }
        public DateTime BorrowedDateFrom { get; set; }
        public DateTime BorrowedDateTo { get; set; }
        public string ServiceType { get; set; }
        public int SalesId { get; set; }
        public int? TypeId { get; set; }
        public string ProjectName { get; set; }
        public string Comment { get; set; }
        public string ShippingInfo { get; set; }
        public DateTime? DropOffDate { get; set; }
        public string Discount { get; set; }

    }
}
