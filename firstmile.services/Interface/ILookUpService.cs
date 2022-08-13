using firstmile.domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Interface
{
    public interface ILookUpService
    {
        IEnumerable<LookUpModel> ListLookUpByCode(string lookUpCode);
        IEnumerable<LookUpModel> ListProvinceByCountryId(int countryId);
        IEnumerable<LookUpModel> ListCustomers();
        IEnumerable<LookUpModel> ListAvailableEquipment();
        IEnumerable<LookUpModel> ListEquipmentAvailability(int equipmentId);
        IEnumerable<LookUpModel> ListProjectsByCustomerId(int customerId);
    }
}
