using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public partial class CustomerModel
    {

        public int CustomerId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual List<BookingModel> Bookings { get; set; }
        public virtual List<CustomerUserModel> CustomerUsers { get; set; }

        public int? CountryId { get; set; }

        public int? ProvinceId { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Address { get; set; }

        public string CreatedDateString { get; set; }

        public string Country { get; set; }
        public string Province { get; set; }
    }
}
