using firstmile.domain;
using firstmile.domain.Model;
using firstmile.domain.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.data.Repository
{
    public static class UsageRepository
    {
        public static GatewayUsage GetGatewayUsage(this IGenericRepository<FmUsage> repo, int gatewayId, DateTime from, DateTime to)
        {
            var db = repo.GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FirstMileDataResource.GetUsage.Replace(FirstMileDataResource.GatewayId, gatewayId.ToString())
                                                            .Replace(FirstMileDataResource.DateFrom, from.ToString("yyyy-MM-dd HH:mm:ss"))
                                                            .Replace(FirstMileDataResource.DateTo, to.ToString("yyyy-MM-dd HH:mm:ss"));
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                var result = ((IObjectContextAdapter)db).ObjectContext.Translate<GatewayUsage>(reader).FirstOrDefault();
                db.Database.Connection.Close();
                return result;
            }
            catch (Exception e)
            {
                db.Database.Connection.Close();
                return null;
            }
        }
    }
}
