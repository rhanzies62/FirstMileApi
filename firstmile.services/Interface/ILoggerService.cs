using firstmile.domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Interface
{
    public interface ILoggerService
    {
        void CreateAPILog(ApiLogModel log);
    }
}
