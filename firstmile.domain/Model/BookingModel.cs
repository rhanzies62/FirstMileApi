using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class BookingModel
    {
        public BookingModel()
        {
        }

        public int SalesId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        /*for grid result only*/
        public string CustomerName { get; set; }
        public int EquipmentCount { get; set; }
        public string CreatedByUserName { get; set; }
        public IEnumerable<BookEquipmentModel> BookEquipments { get; set; }
        public CustomerModel Customer { get; set; }
        public string ProjectName { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string CreatedDateString { get; set; }
        public string Color { get; set; }
        public string Comment { get; set; }
        public string ShippingInfo { get; set; }
        public DateTime? DropOffDate { get; set; }
        public string Discount { get; set; }
    }

    public class BookingScheduleModel
    {
        
        public int SalesId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
    }
}
