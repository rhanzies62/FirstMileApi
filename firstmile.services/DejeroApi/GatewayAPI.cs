using firstmile.domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.DejeroApi
{
    public class GatewayAPI
    {
        public async Task<IEnumerable<Gateway>> ListGateways()
        {
            var result = await new ApiCaller<GatewayResponse<IEnumerable<Gateway>>>().InvokeAPI("/api/v2/gateways", "GET");
            return result.Data;
        }

        public async Task<Gateway> GetGatewayBySerial(string serial)
        {
            var result = await new ApiCaller<GatewayResponse<IEnumerable<Gateway>>>().InvokeAPI("/api/v2/gateways", "GET");
            var gateway = result.Data.Where(i => i.Serial == serial).FirstOrDefault();
            return gateway == null ? new Gateway() : gateway;
        }

        public async Task<GatewayUsage> GetGatewayUsage(int gatewayId, DateTime from, DateTime to)
        {
            string uri = $"/api/v2/gateways/{gatewayId}/usage?start_time={from.AddHours(5):yyyy-MM-ddTHH:mm:ss}Z&finish_time={to.AddHours(5):yyyy-MM-ddTHH:mm:ss}Z";
            var result = await new ApiCaller<GatewayResponse<GatewayUsage>>().InvokeAPI(uri, "GET");
            if(result != null)
            {
                return result.Data == null ? new GatewayUsage(true) : result.Data;
            }
            return new GatewayUsage(true);
        }

        public async Task<SourceUsage> GetSourceUsage(int sourceId, DateTime from, DateTime to)
        {
            string uri = $"/api/v2/sources/{sourceId}/usage?start_time={from.AddHours(5):yyyy-MM-ddTHH:mm:ss}Z&finish_time={to.AddHours(5):yyyy-MM-ddTHH:mm:ss}Z";
            var result = await new ApiCaller<GatewayResponse<SourceUsage>>().InvokeAPI(uri, "GET");
            if (result != null)
            {
                return result.Data == null ? new SourceUsage(true) : result.Data;
            }
            return new SourceUsage(true);
        }

        public async Task<Gateway> GetSourceBySerial(string serial, string sourceType)
        {
            var result = await new ApiCaller<GatewayResponse<IEnumerable<Gateway>>>().InvokeAPI($"/api/v2/sources?type[]={sourceType}", "GET");
            var gateway = result.Data.Where(i => i.Serial == serial).FirstOrDefault();
            return gateway == null ? new Gateway() : gateway;
        }

        public async Task<Location> GetGatewayLocation(int gatewayId)
        {
            var result = await new ApiCaller<GatewayResponse<Location>>().InvokeAPI($"/api/v2/gateways/{gatewayId}/location", "GET");
            if (result != null)
            {
                if (result.Data != null)
                {
                    return result.Data;
                }
            }
            return new Location();
        }

        public async Task<Location> GetSourceLocation(int sourceId)
        {
            var result = await new ApiCaller<GatewayResponse<Location>>().InvokeAPI($"/api/v2/sources/{sourceId}/location", "GET");
            if (result != null)
            {
                if (result.Data != null)
                {
                    return result.Data;
                }
            }
            return new Location();
        }
    }
}
