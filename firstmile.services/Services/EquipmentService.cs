using firstmile.domain;
using firstmile.domain.Model;
using firstmile.domain.Services;
using firstmile.Domain.Utilities;
using firstmile.services.DejeroApi;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using firstmile.data.Repository;
namespace firstmile.services.Services
{
    public class EquipmentService : BaseService, IEquipmentService
    {
        public EquipmentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public Response AddOrEditEquipment(EquipmentModel model, int userId)
        {
            Response response;
            try
            {
                var repo = _unitOfWork.Repository<FmEquipment>();
                if (model.TypeId == 3 || model.TypeId == 1)
                {
                    var equipment = repo.Query().Filter(i => i.Serial == model.Serial && i.EquipmentId != model.EquipmentId).Get();
                    if (equipment.Any()) return new Response(ResponseType.Error, FMServiceResource.CreateEquipment_EquipmentAlreadyExisting);
                }

                if (model.TypeId == 3)
                {
                    var checkGateway = new GatewayAPI().GetGatewayBySerial(model.Serial).GetAwaiter().GetResult();
                    if (checkGateway.Id == 0) return new Response(ResponseType.Error, FMServiceResource.CreateEquipment_SerialNumberIsNotExisting);
                }

                if (model.TypeId == 1)
                {
                    var checkGateway = new GatewayAPI().GetSourceBySerial(model.Serial, "ENGO").GetAwaiter().GetResult();
                    if (checkGateway.Id == 0) return new Response(ResponseType.Error, FMServiceResource.CreateEquipment_SerialNumberIsNotExisting);
                }

                if (model.TypeId == 0) return new Response(ResponseType.Error, FMServiceResource.CreateEquipment_EquipmentTypeNotValid);

                var equipmentEntity = repo.Query().Filter(i => i.EquipmentId == model.EquipmentId).Get().FirstOrDefault();
                if (equipmentEntity == null)
                {
                    equipmentEntity = new FmEquipment
                    {
                        Company = model.Company,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    repo.Insert(equipmentEntity);
                }

                equipmentEntity.Description = model.Description;
                equipmentEntity.Name = model.Name;
                equipmentEntity.Company = model.Company;
                equipmentEntity.Type = model.TypeId;
                equipmentEntity.GatewayId = model.GatewayId;
                equipmentEntity.Serial = model.Serial;
                equipmentEntity.UpdatedBy = userId;
                equipmentEntity.UpdatedDate = DateTime.UtcNow;
                equipmentEntity.IsActive = model.IsActive;

                _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }

            return response;
        }

        public GridResultGeneric<EquipmentModel> ListEquipments(GridFilter filter)
        {
            var result = new GridResultGeneric<EquipmentModel>();
            var db = _unitOfWork.Repository<FmEquipment>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListEquipments.ParseQuery(filter, "EquipmentId");
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                result.TotalCount = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();
                reader.NextResult();
                result.Data = ((IObjectContextAdapter)db).ObjectContext.Translate<EquipmentModel>(reader).ToList();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.TotalCount = 0;
                result.Data = new List<EquipmentModel>();
                result.IsSuccess = false;
                result.Message = FMServiceResource.CriticalErrorMessage;
            }

            return result;
        }

        public async Task<LocationData> ListEquipmentLocations()
        {
            var locationsData = new LocationData();
            var locations = new List<Location>();
            var gatewayAPI = new GatewayAPI();
            var db = _unitOfWork.Repository<FmEquipment>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListActiveEquipment;
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                var equipments = ((IObjectContextAdapter)db).ObjectContext.Translate<EquipmentModel>(reader).ToList();
                equipments.ForEach(async eq =>
                {
                    if (eq.GatewayId.HasValue)
                    {
                        Location location;
                        if (eq.TypeId == 1)
                            location = await gatewayAPI.GetSourceLocation(eq.GatewayId.Value);
                        else
                            location = await gatewayAPI.GetGatewayLocation(eq.GatewayId.Value);

                        location.Equipment = eq;
                        if (location != null) locations.Add(location);
                    }
                });

                var minLat = locations.OrderBy(i => i.Latitude).Select(i => i.Latitude).FirstOrDefault();
                var minLng = locations.OrderBy(i => i.Longitude).Select(i => i.Longitude).FirstOrDefault();
                var maxLat = locations.OrderByDescending(i => i.Latitude).Select(i => i.Latitude).FirstOrDefault();
                var maxLng = locations.OrderByDescending(i => i.Longitude).Select(i => i.Longitude).FirstOrDefault();

                locationsData.Bounds = new Bounds()
                {
                    NW = new BoundData
                    {
                        Latitude = minLat,
                        Longitude = minLng,
                    },
                    SE = new BoundData
                    {
                        Latitude = maxLat,
                        Longitude = maxLng,
                    }
                };
                locationsData.Locations = locations;

            }
            catch (Exception e)
            {

            }
            return locationsData;
        }

        public async Task<IEnumerable<EquipmentModel>> ListGateways(DateTime from, DateTime to)
        {
            var repo = _unitOfWork.Repository<FmEquipment>();
            var gatewayAPI = new GatewayAPI();
            var equipmentModels = repo.Query().Filter(i => i.GatewayId != null && (i.Type == 3 || i.Type == 1)).Get().Select(e => new EquipmentModel
            {
                Name = e.Name,
                EquipmentId = e.EquipmentId,
                Serial = e.Serial,
                GatewayId = e.GatewayId,
                TypeId = e.Type
            }).ToList();
            equipmentModels.ForEach(async e =>
            {
                if (e.TypeId == 3)
                {
                    var usage = await this.GetGatewayUsage(e.GatewayId.Value, from, to);
                    e.TotalUsage = usage.TotalUsage;
                    e.TotalCellUsage = usage.CellUsage;
                    e.OtherUsage = usage.OtherUsage;
                }
                if (e.TypeId == 1)
                {
                    //var usage = await this.GetSourceUsage(e.GatewayId.Value, from, to);
                    //e.TotalUsage = usage.TotalUsage;
                    //e.TotalCellUsage = usage.TotalCellUsage;
                    //e.OtherUsage = usage.TotalCellUsage - usage.TotalCellUsage;
                }


                var location = await gatewayAPI.GetGatewayLocation(e.GatewayId.Value);
                e.Latitude = location.Latitude;
                e.Longitude = location.Longitude;
            });

            return equipmentModels;
        }

        public async Task<IEnumerable<EquipmentModel>> ListGateways()
        {
            var repo = _unitOfWork.Repository<FmEquipment>();
            return repo.Query().Filter(i => i.Type == 3).Get().Select(e => new EquipmentModel
            {
                Name = e.Name,
                EquipmentId = e.EquipmentId,
                Serial = e.Serial,
                GatewayId = e.GatewayId
            }).ToList();
        }

        public async Task<GatewayUsage> GetGatewayUsage(int gatewayId, DateTime from, DateTime to)
        {
            var gatewayAPI = new GatewayAPI();
            var totalUsage = new GatewayUsage();
            var usageEntity = _unitOfWork.Repository<FmUsage>().GetGatewayUsage(gatewayId, from, to);
            if (usageEntity != null)
            {
                totalUsage = usageEntity;
            }
            else
            {
                totalUsage = new GatewayUsage();
                totalUsage.StartTime = from;
                totalUsage.FinishTime = to;
                //var range = 44;
                //var diff = to - from;
                //var loops = diff.Days / range;
                //if (diff.Days > range)
                //{
                //    for (int i = 0; i <= loops - 1; i++)
                //    {
                //        var usage = await gatewayAPI.GetGatewayUsage(gatewayId, from.AddDays(range * i), from.AddDays(range * (i + 1)));
                //        totalUsage.TotalUsage += usage.TotalUsage;
                //        totalUsage.CellUsage += usage.CellUsage;
                //    }
                //    var _usage = await gatewayAPI.GetGatewayUsage(gatewayId, from.AddDays(range * loops), to);
                //    totalUsage.TotalUsage += _usage.TotalUsage;
                //    totalUsage.CellUsage += _usage.CellUsage;
                //}
                //else
                //{
                //    var usage = await gatewayAPI.GetGatewayUsage(gatewayId, from, to);
                //    totalUsage.TotalUsage += usage.TotalUsage;
                //    totalUsage.CellUsage += usage.CellUsage;
                //}
            }
            return totalUsage;
        }

        public async Task<SourceUsage> GetSourceUsage(int sourceId, DateTime from, DateTime to)
        {
            var gatewayAPI = new GatewayAPI();
            var totalUsage = new SourceUsage();
            //var usageEntity = _unitOfWork.Repository<FmUsage>().GetGatewayUsage(sourceId, from, to);
            //if (usageEntity != null)
            //{
            //    totalUsage = usageEntity;
            //}
            //else
            //{
            var range = 44;
            var diff = to - from;
            var loops = diff.Days / range;
            if (diff.Days > range)
            {
                for (int i = 0; i <= loops - 1; i++)
                {
                    var usage = await gatewayAPI.GetSourceUsage(sourceId, from.AddDays(range * i), from.AddDays(range * (i + 1)));
                    //totalUsage = usage;
                    //totalUsage.TotalUsage += usage.TotalUsage;
                    //totalUsage.CellUsage += usage.CellUsage;
                }
                var _usage = await gatewayAPI.GetSourceUsage(sourceId, from.AddDays(range * loops), to);
                totalUsage = _usage;
                //totalUsage.TotalUsage += _usage.TotalUsage;
                //totalUsage.CellUsage += _usage.CellUsage;
            }
            else
            {
                var usage = await gatewayAPI.GetSourceUsage(sourceId, from, to);
                totalUsage = usage;
                //totalUsage.TotalUsage += usage.TotalUsage;
                //totalUsage.CellUsage += usage.CellUsage;
            }
            //}
            return totalUsage;
        }

        public List<EquipmentModel> ListAvailableEngo()
        {
            var db = _unitOfWork.Repository<FmUsage>().GetDbContext();
            return db.FmEquipments.Where(i => i.Type == 1).Select(i => new EquipmentModel
            {
                EquipmentId = i.EquipmentId,
                Name = i.Name
            }).ToList();
        }
    }
}
