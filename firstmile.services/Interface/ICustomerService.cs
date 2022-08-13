using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Interface
{
    public interface ICustomerService
    {
        Response AddOrEditCustomer(CustomerModel model, int userId);
        GridResultGeneric<CustomerModel> ListCustomers(GridFilter filter);
        Response AddCustomerUser(CustomerModel model, int userId);
        IEnumerable<UserModel> ListAllAvailableUser(int? customerId = null);
        IEnumerable<UserModel> ListAssignedUserByCustomerId(int customerId);
    }
}
