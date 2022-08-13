using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Interface
{
    public interface IUsageService
    {
        Response SaveGatewayUsage(UsageModel model);
        Task<Response> SaveGatewayUsages(IEnumerable<UsageModel> model);
        Task<UsageModel> GetLastUsageDataOfGateway(string gatewayId);
    }
}
