using firstmile.domain;
using firstmile.domain.Model;
using firstmile.domain.Services;
using firstmile.Domain.Common;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Services
{
    public class LookUpService : BaseService, ILookUpService
    {
        public LookUpService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IEnumerable<LookUpModel> ListLookUpByCode(string lookUpCode)
        {
            return _unitOfWork.Repository<FmLookUpType>().Query().Filter(i => i.EntityCode == lookUpCode).Get().Select(i => new LookUpModel
            {
                Description = i.Description,
                Id = i.LookUpValue,
                Ordinal = i.Ordinal
            });
        }

        public IEnumerable<LookUpModel> ListProvinceByCountryId(int countryId)
        {
            return _unitOfWork.Repository<FmProvince>().Query().Filter(i => i.CountryId == countryId).Get().Select(i => new LookUpModel
            {
                Description = i.Description,
                Id = i.ProvinceId,
            });
        }

        public IEnumerable<LookUpModel> ListCustomers()
        {
            return _unitOfWork.Repository<FmCustomer>().Query().Get().Select(i => new LookUpModel
            {
                Description = i.Name,
                Id = i.CustomerId
            }).OrderBy(i => i.Description);
        }

        public IEnumerable<LookUpModel> ListAvailableEquipment()
        {
            return _unitOfWork.Repository<FmEquipment>().Query().Filter(eq => eq.IsActive).Get().Select(i => new LookUpModel
            {
                Description = i.Name,
                Id = i.EquipmentId,
                Type = i.Type,
                GatewayId = i.GatewayId
            });
        }

        public IEnumerable<LookUpModel> ListEquipmentAvailability(int equipmentId)
        {
            return _unitOfWork.Repository<FmBookEquipment>().Query().Filter(i => i.BorrowedDateFrom >= DateTime.UtcNow && i.EquipmentId == equipmentId).Get().Select(i => new LookUpModel
            {
                DateFrom = i.BorrowedDateFrom,
                DateTo = i.BorrowedDateTo
            });
        }

        public IEnumerable<LookUpModel> ListProjectsByCustomerId(int customerId)
        {
            return _unitOfWork.Repository<FmBooking>().Query().Filter(i => i.CustomerId == customerId).Get().Select(i => new LookUpModel
            {
                Id = i.SalesId,
                Description = i.ProjectName
            });
        }
    }
}
