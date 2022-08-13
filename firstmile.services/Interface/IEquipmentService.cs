using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Interface
{
    public interface IEquipmentService
    {
        Response AddOrEditEquipment(EquipmentModel model, int userId);
        GridResultGeneric<EquipmentModel> ListEquipments(GridFilter filter);
        Task<LocationData> ListEquipmentLocations();
        Task<IEnumerable<EquipmentModel>> ListGateways(DateTime from, DateTime to);
        Task<IEnumerable<EquipmentModel>> ListGateways();
        Task<GatewayUsage> GetGatewayUsage(int gatewayId, DateTime from, DateTime to);
        Task<SourceUsage> GetSourceUsage(int sourceId, DateTime from, DateTime to);
        List<EquipmentModel> ListAvailableEngo();
    }
}
