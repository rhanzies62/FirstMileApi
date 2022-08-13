using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.domain.Model
{
    public class CustomerUserModel
    {
        public int CustomerUserId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public virtual CustomerModel FmCustomer { get; set; }
        public virtual UserModel FmUser { get; set; }
    }
}
