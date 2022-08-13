using firstmile.domain;
using firstmile.domain.Model;
using firstmile.domain.Services;
using firstmile.domain.Utilities;
using firstmile.Domain.Common;
using firstmile.Domain.Utilities;
using firstmile.services.DejeroApi;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Services
{
    public class BookingService : BaseService, IBookingService
    {
        public BookingService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public Response CreateBooking(BookingModel model, int userId)
        {
            Response response;
            try
            {
                var repo = _unitOfWork.Repository<FmBooking>();
                var eqBookRepo = _unitOfWork.Repository<FmBookEquipment>();
                var booking = repo.Query().Filter(i => i.SalesId == model.SalesId).Get().FirstOrDefault();
                if (booking == null)
                {
                    booking = new FmBooking()
                    {
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        Color = Utility.GenerateRandomColor(),
                    };
                    repo.Insert(booking);
                }

                booking.CustomerId = model.CustomerId;
                booking.ProjectName = model.ProjectName;
                booking.StatusId = model.StatusId;
                booking.Comment = model.Comment;
                booking.ShippingInfo = model.ShippingInfo;
                booking.DropOffDate = model.DropOffDate;
                booking.Discount = model.Discount;

                var hasEqInRent = false;
                List<BookEquipmentModel> BookedEquipments = new List<BookEquipmentModel>();
                model.BookEquipments.ToList().ForEach(bookEq =>
                {
                    using (var context = new FirstMileEntities())
                    {
                        DateTime fromDate = DateTime.Parse($"{DateTime.Parse(bookEq.StartDate).ToShortDateString()} 00:00:00");
                        DateTime toDate = DateTime.Parse($"{DateTime.Parse(bookEq.EndDate).ToShortDateString()} 23:59:59");
                        var eq = (from be in context.FmBookEquipments
                                  join e in context.FmEquipments on be.EquipmentId equals e.EquipmentId
                                  where e.Type != 5 && bookEq.SaleEquipmentId != be.SaleEquipmentId && bookEq.EquipmentId == e.EquipmentId && ((fromDate >= be.BorrowedDateFrom && fromDate <= be.BorrowedDateTo) ||
                                        (toDate >= be.BorrowedDateFrom && toDate <= be.BorrowedDateTo))
                                  select e);
                        if (eq.Any())
                        {
                            var bookedEquipment = eq.FirstOrDefault();
                            hasEqInRent = true;
                            bookEq.Equipment = new EquipmentModel(bookedEquipment);
                            BookedEquipments.Add(bookEq);
                        }
                        else
                        {
                            var bookedEqEntity = booking.FmBookEquipments.Where(i => bookEq.SaleEquipmentId == i.SaleEquipmentId && i.SaleEquipmentId != 0).FirstOrDefault();
                            if (bookedEqEntity == null)
                            {
                                bookedEqEntity = new FmBookEquipment
                                {
                                    CreatedBy = userId,
                                    CreatedDate = DateTime.UtcNow,
                                };
                                booking.FmBookEquipments.Add(bookedEqEntity);
                            }
                            bookedEqEntity.BorrowedDateFrom = fromDate;
                            bookedEqEntity.BorrowedDateTo = toDate;
                            bookedEqEntity.EquipmentId = bookEq.EquipmentId;
                            bookedEqEntity.UpdatedBy = userId;
                            bookedEqEntity.UpdatedDate = DateTime.UtcNow;
                            bookedEqEntity.ServiceTypeId = bookEq.ServiceTypeId;
                        }
                    }
                });
                if (hasEqInRent)
                {
                    response = new Response(ResponseType.Success, FMServiceResource.CreateBooking_EquipmentAlreadyBooked, BookedEquipments);
                }
                else
                {
                    _unitOfWork.Save();
                }
                response = new Response(ResponseType.Success, string.Empty);

            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public Response UpdateBookingEquipment(List<BookEquipmentModel> models, int userid)
        {
            Response response;
            try
            {
                var hasEqInRent = true;
                var eqBookRepo = _unitOfWork.Repository<FmBookEquipment>();
                List<BookEquipmentModel> BookedEquipments = new List<BookEquipmentModel>();
                models.ForEach(bookEq =>
                {
                    var eq = eqBookRepo.Query().Filter(i => (bookEq.BorrowedDateFrom >= i.BorrowedDateFrom && bookEq.BorrowedDateFrom <= i.BorrowedDateTo) ||
                                                            (bookEq.BorrowedDateTo >= i.BorrowedDateFrom && bookEq.BorrowedDateTo <= i.BorrowedDateTo) && i.SaleEquipmentId != bookEq.SaleEquipmentId).Get();
                    if (eq.Any())
                    {
                        var bookedEquipment = eq.FirstOrDefault();
                        hasEqInRent = true;
                        //bookEq.Equipment = new EquipmentModel(bookedEquipment.FmEquipment);
                        BookedEquipments.Add(bookEq);
                    }
                    else
                    {
                        var eqEntity = eqBookRepo.Query().Filter(i => i.SaleEquipmentId == bookEq.SaleEquipmentId).Get().FirstOrDefault();
                        eqEntity.BorrowedDateFrom = bookEq.BorrowedDateFrom;
                        eqEntity.BorrowedDateTo = bookEq.BorrowedDateTo;
                        eqEntity.UpdatedBy = userid;
                        eqEntity.UpdatedDate = DateTime.UtcNow;
                    }
                });
                if (hasEqInRent)
                    response = new Response(ResponseType.Success, FMServiceResource.CreateBooking_EquipmentAlreadyBooked, BookedEquipments);
                else
                    _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public GridResultGeneric<BookingModel> ListBookings(GridFilter filter)
        {
            var result = new GridResultGeneric<BookingModel>();
            var db = _unitOfWork.Repository<FmBooking>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListBooking.ParseQuery(filter, "SalesId");
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                result.TotalCount = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();
                reader.NextResult();
                result.Data = ((IObjectContextAdapter)db).ObjectContext.Translate<BookingModel>(reader).ToList();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.TotalCount = 0;
                result.Data = new List<BookingModel>();
                result.IsSuccess = false;
                result.Message = FMServiceResource.CriticalErrorMessage;
            }

            return result;
        }

        public Response CheckIfEquipmentAvailable(BookEquipmentModel model)
        {
            var eqBookRepo = _unitOfWork.Repository<FmBookEquipment>();
            var eq = eqBookRepo.Query().Filter(i => model.SaleEquipmentId != i.SaleEquipmentId && model.EquipmentId == i.EquipmentId && ((model.BorrowedDateFrom >= i.BorrowedDateFrom && model.BorrowedDateFrom <= i.BorrowedDateTo) ||
                                                            (model.BorrowedDateTo >= i.BorrowedDateFrom && model.BorrowedDateTo <= i.BorrowedDateTo))).Get();
            var hasRecord = eq.Any();
            var q = eq.FirstOrDefault();
            return new Response(ResponseType.Success, hasRecord ? FMServiceResource.CheckIfEquipmentAvailable_EquipmentNotAvailable : string.Empty, hasRecord);
        }

        public GridResultGeneric<BookEquipmentGridModel> ListBookingEquipment(GridFilter filter, int salesId)
        {
            var result = new GridResultGeneric<BookEquipmentGridModel>();
            var db = _unitOfWork.Repository<FmBooking>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListBookEquipment.ParseQuery(filter, "SaleEquipmentId").Replace("##SalesID##", salesId.ToString());
            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                result.TotalCount = ((IObjectContextAdapter)db).ObjectContext.Translate<int>(reader).FirstOrDefault();
                reader.NextResult();
                result.Data = ((IObjectContextAdapter)db).ObjectContext.Translate<BookEquipmentGridModel>(reader).ToList();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.TotalCount = 0;
                result.Data = new List<BookEquipmentGridModel>();
                result.IsSuccess = false;
                result.Message = FMServiceResource.CriticalErrorMessage;
            }

            return result;
        }

        public IEnumerable<BookEquipmentGridModel> ListActiveBooking(int userId)
        {
            var db = _unitOfWork.Repository<FmBooking>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListActiveBookEquipment.Replace("##UserId##", userId.ToString());
            db.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            return ((IObjectContextAdapter)db).ObjectContext.Translate<BookEquipmentGridModel>(reader).ToList().OrderByDescending(i => i.TypeId);
        }

        public IEnumerable<BookingScheduleModel> ListBookingSchedules(int? customerId = null)
        {
            var db = _unitOfWork.Repository<FmBooking>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.BookingSchedules.Replace("##CustomerId##", customerId.HasValue ? $"where b.CustomerId = {customerId.Value}" : string.Empty);
            db.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            return ((IObjectContextAdapter)db).ObjectContext.Translate<BookingScheduleModel>(reader).ToList();
        }

        public IEnumerable<BookingScheduleModel> ListBookingSchedulesByProjectId(int customerId, int? saleId = null)
        {
            var db = _unitOfWork.Repository<FmBooking>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.BookingSchedulesByProjectId.Replace("##customerId##", customerId.ToString()).Replace("##salesId##", saleId.HasValue ? saleId.Value.ToString() : string.Empty);
            db.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            return ((IObjectContextAdapter)db).ObjectContext.Translate<BookingScheduleModel>(reader).ToList();
        }

        public IEnumerable<BookEquipmentModel> ListEquipmentBooking(int equipmentId)
        {
            using (var context = new FirstMileEntities())
            {
                var equipmentBookingHistory = (from be in context.FmBookEquipments
                                               join b in context.FmBookings on be.SaleId equals b.SalesId
                                               join e in context.FmEquipments on be.EquipmentId equals e.EquipmentId
                                               join sts in context.FmLookUpTypes on new { EntityCode = LookUpTypes.Status, LookUpValue = b.StatusId } equals new { sts.EntityCode, sts.LookUpValue }
                                               where be.EquipmentId == equipmentId
                                               select new BookEquipmentModel
                                               {
                                                   EquipmentId = e.EquipmentId,
                                                   SaleId = be.SaleId,
                                                   SaleEquipmentId = be.SaleEquipmentId,
                                                   BorrowedDateFrom = be.BorrowedDateFrom,
                                                   BorrowedDateTo = be.BorrowedDateTo,
                                                   Booking = new BookingModel
                                                   {
                                                       ProjectName = be.FmBooking.ProjectName,
                                                       Customer = new CustomerModel
                                                       {
                                                           Name = be.FmBooking.FmCustomer.Name
                                                       },
                                                       Status = sts.Description
                                                   },
                                                   Equipment = new EquipmentModel
                                                   {
                                                       GatewayId = e.GatewayId
                                                   }
                                               }).OrderByDescending(i => i.SaleId);
                return equipmentBookingHistory.ToList();
            }
        }

        public BookEquipmentModel GetBooking(int equipmentId, int salesId)
        {
            using (var context = new FirstMileEntities())
            {
                var equipmentBookingHistory = (from be in context.FmBookEquipments
                                               join b in context.FmBookings on be.SaleId equals b.SalesId
                                               join e in context.FmEquipments on be.EquipmentId equals e.EquipmentId
                                               join sts in context.FmLookUpTypes on new { EntityCode = LookUpTypes.Status, LookUpValue = b.StatusId } equals new { sts.EntityCode, sts.LookUpValue }
                                               where be.EquipmentId == equipmentId && b.SalesId == salesId
                                               select new BookEquipmentModel
                                               {
                                                   EquipmentId = e.EquipmentId,
                                                   SaleId = be.SaleId,
                                                   SaleEquipmentId = be.SaleEquipmentId,
                                                   BorrowedDateFrom = be.BorrowedDateFrom,
                                                   BorrowedDateTo = be.BorrowedDateTo,
                                                   Booking = new BookingModel
                                                   {
                                                       ProjectName = be.FmBooking.ProjectName,
                                                       Customer = new CustomerModel
                                                       {
                                                           Name = be.FmBooking.FmCustomer.Name
                                                       },
                                                       Status = sts.Description
                                                   },
                                                   Equipment = new EquipmentModel
                                                   {
                                                       Name = e.Name,
                                                       Serial = e.Serial,
                                                       GatewayId = e.GatewayId,
                                                       TypeId = e.Type
                                                   }
                                               });
                return equipmentBookingHistory.FirstOrDefault();
            }

        }

        public BookingModel GetBookingBySalesId(int salesId)
        {
            using (var context = new FirstMileEntities())
            {
                var model = (from b in context.FmBookings
                             join cb in context.FmUsers on b.CreatedBy equals cb.UserId
                             join sts in context.FmLookUpTypes on new { ID = b.StatusId, Code = LookUpTypes.Status } equals new { ID = sts.LookUpValue, Code = sts.EntityCode }
                             join c in context.FmCustomers on b.CustomerId equals c.CustomerId
                             where b.SalesId == salesId
                             select new BookingModel
                             {
                                 SalesId = b.SalesId,
                                 CreatedByUserName = cb.Username,
                                 CreatedDate = b.CreatedDate,
                                 Status = sts.Description,
                                 ProjectName = b.ProjectName,
                                 CustomerName = c.Name,
                                 Comment = b.Comment,
                                 Discount = b.Discount,
                                 ShippingInfo = b.ShippingInfo,
                                 DropOffDate = b.DropOffDate,
                                 BookEquipments = (from be in b.FmBookEquipments
                                                   join act in context.FmLookUpTypes on new { ID = be.ServiceTypeId, Code = LookUpTypes.ActivityType } equals new { ID = act.LookUpValue, Code = act.EntityCode }

                                                   join el in context.FmEquipments on be.EquipmentId equals el.EquipmentId into eLeft
                                                   from e in eLeft.DefaultIfEmpty()

                                                   join eqtL in context.FmLookUpTypes on new { ID = e.Type, Code = LookUpTypes.EquipmentType } equals new { ID = eqtL.LookUpValue, Code = eqtL.EntityCode } into eqtLeft
                                                   from eqt in eqtLeft.DefaultIfEmpty()

                                                   orderby be.ServiceTypeId
                                                   select new BookEquipmentModel
                                                   {
                                                       SaleEquipmentId = be.SaleEquipmentId,
                                                       ServiceTypeId = be.ServiceTypeId,
                                                       ServiceType = act.Description,
                                                       Equipment = new EquipmentModel
                                                       {
                                                           EquipmentId = e == null ? 0 : e.EquipmentId,
                                                           Name = e == null ? string.Empty : e.Name,
                                                           Serial = e == null ? string.Empty : e.Serial,
                                                           GatewayId = e == null ? 0 : e.GatewayId,
                                                           TypeId = e == null ? 0 : e.Type,
                                                           Type = e == null ? string.Empty : eqt.Description
                                                       },
                                                       BorrowedDateFrom = be.BorrowedDateFrom,
                                                       BorrowedDateTo = be.BorrowedDateTo
                                                   })
                             }).FirstOrDefault();
                return model;
            }
        }

        public IEnumerable<BookingModel> ListBookingByCustomerId(int customerId)
        {
            using (var context = new FirstMileEntities())
            {
                var model = (from b in context.FmBookings
                             join cb in context.FmUsers on b.CreatedBy equals cb.UserId
                             join sts in context.FmLookUpTypes on new { ID = b.StatusId, Code = LookUpTypes.Status } equals new { ID = sts.LookUpValue, Code = sts.EntityCode }
                             join c in context.FmCustomers on b.CustomerId equals c.CustomerId
                             where b.CustomerId == customerId
                             select new BookingModel
                             {
                                 SalesId = b.SalesId,
                                 CreatedByUserName = cb.Username,
                                 CreatedDate = b.CreatedDate,
                                 Status = sts.Description,
                                 ProjectName = b.ProjectName,
                                 CustomerName = c.Name,
                                 Color = b.Color
                             }).ToList();
                return model;
            }
        }

        public async Task<IEnumerable<BookingHistory>> ListBookingHistoryByUserId(int userId)
        {
            var gatewayApi = new GatewayAPI();
            var bookingHistory = new List<BookingHistory>();
            var db = _unitOfWork.Repository<FmBooking>().GetDbContext();
            var cmd = db.Database.Connection.CreateCommand();
            cmd.CommandText = FMServiceResource.ListBookingHistoryByUserId.Replace("##UserID##", userId.ToString());
            db.Database.Connection.Open();
            var reader = cmd.ExecuteReader();
            var result = ((IObjectContextAdapter)db).ObjectContext.Translate<BookEquipmentGridModel>(reader).ToList().OrderByDescending(i => i.TypeId);
            foreach (var r in result)
            {
                if (r.TypeId == 3)
                {
                    var to = DateTime.Parse($"{r.BorrowedDateToST} 23:59:59") > DateTime.UtcNow ? DateTime.UtcNow : DateTime.Parse($"{r.BorrowedDateToST} 23:59:59");
                    var usage = await gatewayApi.GetGatewayUsage(r.GatewayId.Value, DateTime.Parse($"{r.BorrowedDateFromST} 00:00:00"), to);
                    r.CellUsage = usage.CellUsage;
                    r.TotalUsage = usage.TotalUsage;
                    r.OtherUsage = usage.TotalUsage - usage.CellUsage;
                }
                if(r.TypeId == 1 && r.GatewayId.HasValue)
                {
                    var to = DateTime.Parse($"{r.BorrowedDateToST} 23:59:59") > DateTime.UtcNow ? DateTime.UtcNow : DateTime.Parse($"{r.BorrowedDateToST} 23:59:59");
                    var usage = await gatewayApi.GetSourceUsage(r.GatewayId.Value, DateTime.Parse($"{r.BorrowedDateFromST} 00:00:00"), to);
                    r.CellUsage = usage.TotalCellUsage;
                    r.TotalUsage = usage.TotalUsage;
                    r.OtherUsage = usage.TotalUsage - usage.TotalCellUsage;
                }
                var history = bookingHistory.FirstOrDefault(i => i.SaleId == r.SalesId);
                if (history == null)
                {
                    bookingHistory.Add(new BookingHistory
                    {
                        SaleId = r.SalesId,
                        Equipments = new List<BookEquipmentGridModel>()
                         {
                             r
                         },
                        ProjectName = r.ProjectName
                    });
                }
                else
                {
                    history.Equipments.Add(r);
                }
            }
            return bookingHistory;
        }

        public Response DeleteBooking(int bookingId)
        {
            Response response;
            try
            {
                var bookingRepo = _unitOfWork.Repository<FmBooking>();
                var bookEquipmentRepo = _unitOfWork.Repository<FmBookEquipment>();
                var bookEquipments = bookEquipmentRepo.Query().Filter(be => be.SaleId == bookingId).Get().ToList();
                bookEquipments.ForEach(be => bookEquipmentRepo.Delete(be));
                bookingRepo.Delete(bookingId);
                _unitOfWork.Save();
                response = new Response(ResponseType.Success, string.Empty);
            }
            catch (Exception e)
            {
                response = new Response(ResponseType.Critical, FMServiceResource.CriticalErrorMessage);
            }
            return response;
        }

        public Response DeleteBookingEquipment(int bookingEquipmentId)
        {
            Response response;
            try
            {
                var bookEquipmentRepo = _unitOfWork.Repository<FmBookEquipment>();
                bookEquipmentRepo.Delete(bookingEquipmentId);
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
