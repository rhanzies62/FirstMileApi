using firstmile.domain;
using firstmile.domain.Model;
using firstmile.domain.Services;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Services
{
    public class LoggerService : BaseService, ILoggerService
    {
        public LoggerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void CreateAPILog(ApiLogModel log)
        {
            try
            {
                var apiLogRepo = _unitOfWork.Repository<ApiLog>();
                apiLogRepo.Insert(new ApiLog
                {
                    IPAddress = log.IPAddress,
                    RequestContentType = log.RequestContentType,
                    RequestHeader = log.RequestHeader,
                    RequestMethod = log.RequestMethod,
                    RequestTimestamp = log.RequestTimestamp,
                    RequestUri = log.RequestUri,
                    ResponseContentType = log.ResponseContentType,
                    ResponseStatusCode = (int)log.ResponseStatusCode,
                    ResponseTimestamp = log.ResponseTimestamp,
                    RequestBody = log.RequestBody
                });

                if(log.IPAddress != "::1") _unitOfWork.Save();
            }
            catch (Exception e)
            {

            }
        }
    }
}
