using firstmile.domain;
using firstmile.domain.Model;
using firstmile.domain.Services;
using firstmile.Domain.Utilities;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Services
{
    public class UsageService : BaseService, IUsageService
    {
        public UsageService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public Response SaveGatewayUsage(UsageModel model)
        {
            Response response;
            try
            {
                var usageEntity = _unitOfWork.Repository<FmUsage>().GetByID(model.Id);
                if (usageEntity == null)
                {
                    usageEntity = new FmUsage()
                    {
                        GatewayId = model.GatewayId
                    };
                    _unitOfWork.Repository<FmUsage>().Insert(usageEntity);
                }
                usageEntity.CellUsage = model.CellUsage;
                usageEntity.OtherUsage = model.OtherUsage;
                usageEntity.TotalUsage = model.TotalUsage;
                usageEntity.DateFrom = model.DateFrom;
                usageEntity.DateTo = model.DateTo;

                _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public async Task<UsageModel> GetLastUsageDataOfGateway(string gatewayId)
        {
            return _unitOfWork.Repository<FmUsage>().Query().Filter(i => i.GatewayId == gatewayId).Get().Select(entity => new UsageModel
            {
                Id = entity.Id,
                GatewayId = entity.GatewayId,
                DateFrom = entity.DateFrom,
                DateTo = entity.DateTo
            }).OrderByDescending(i => i.Id).FirstOrDefault();
        }

        public async Task<Response> SaveGatewayUsages(IEnumerable<UsageModel> model)
        {
            Response response;
            try
            {
                _unitOfWork.Repository<FmUsage>().BulkInsert(model.Select(m => new FmUsage
                {
                    GatewayId = m.GatewayId,
                    CellUsage = m.CellUsage,
                    OtherUsage = m.OtherUsage,
                    TotalUsage = m.TotalUsage,
                    DateFrom = m.DateFrom,
                    DateTo = m.DateTo,
                }).ToList());
                _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }
    }
}
