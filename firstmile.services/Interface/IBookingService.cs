using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.services.Interface
{
    public interface IBookingService
    {
        Response CreateBooking(BookingModel model, int userId);
        Response UpdateBookingEquipment(List<BookEquipmentModel> models, int userid);
        GridResultGeneric<BookingModel> ListBookings(GridFilter filter);
        Response CheckIfEquipmentAvailable(BookEquipmentModel model);
        GridResultGeneric<BookEquipmentGridModel> ListBookingEquipment(GridFilter filter, int salesId);
        IEnumerable<BookEquipmentGridModel> ListActiveBooking(int userId);
        IEnumerable<BookingScheduleModel> ListBookingSchedules(int? customerId = null);
        IEnumerable<BookingScheduleModel> ListBookingSchedulesByProjectId(int customerId, int? saleId = null);
        IEnumerable<BookEquipmentModel> ListEquipmentBooking(int equipmentId);
        BookEquipmentModel GetBooking(int equipmentId, int salesId);
        BookingModel GetBookingBySalesId(int salesId);
        IEnumerable<BookingModel> ListBookingByCustomerId(int customerId);
        Task<IEnumerable<BookingHistory>> ListBookingHistoryByUserId(int userId);
        Response DeleteBooking(int bookingId);
        Response DeleteBookingEquipment(int bookingEquipmentId);
    }
}
