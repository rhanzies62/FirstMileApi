using firstmile.api.Authentication;
using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace firstmile.api.Controllers
{
    [FMAuthorizationRequired(userType: 1)]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BookingController : ApiController
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] BookingModel model)
        {
            if (ModelState.IsValid)
            {
                var u = (FMIdentity)User.Identity;
                var response = _bookingService.CreateBooking(model, u.GetUserId());
                if (response.IsSuccess)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new Response(ResponseType.Success, string.Empty));
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new Response(ResponseType.Error, "Incomplete Information", Utility.RetrieveErrorField(ModelState)));
        }

        [HttpPost, Route("Api/Booking/UpdateBookingEquipment")]
        public HttpResponseMessage UpdateBookingEquipment([FromBody] List<BookEquipmentModel> models)
        {
            var u = (FMIdentity)User.Identity;
            var result = _bookingService.UpdateBookingEquipment(models, u.GetUserId());
            return Request.CreateResponse(result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest, result);
        }

        [HttpPost, Route("Api/Booking/ListBookings")]
        public HttpResponseMessage ListBookings([FromBody] GridFilter filter)
        {
            var result = _bookingService.ListBookings(filter);
            return Request.CreateResponse(result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest, result);
        }

        [HttpPost, Route("Api/Booking/CheckIfEquipmentAvailable")]
        public HttpResponseMessage CheckIfEquipmentAvailable([FromBody] BookEquipmentModel model)
            => Request.CreateResponse(HttpStatusCode.OK, _bookingService.CheckIfEquipmentAvailable(model));

        [HttpPost, Route("Api/Booking/ListBookingsEquipment")]
        public HttpResponseMessage ListBookings([FromBody] GridFilter filter, int salesId)
        {
            var result = _bookingService.ListBookingEquipment(filter, salesId);
            return Request.CreateResponse(result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest, result);
        }

        [HttpGet, Route("Api/Booking/ListBookingsSchedules")]
        public HttpResponseMessage ListBookingsSchedules(int? customerId = null)
        {
            var result = _bookingService.ListBookingSchedules(customerId);
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/Booking/ListBookingSchedulesByProjectId")]
        public HttpResponseMessage ListBookingSchedulesByProjectId(int customerId, int? saleId = null)
        {
            var result = _bookingService.ListBookingSchedulesByProjectId(customerId, saleId);
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/Booking/ListEquipmentBooking")]
        public HttpResponseMessage ListEquipmentBooking(int equipmentId)
            => Request.CreateResponse(_bookingService.ListEquipmentBooking(equipmentId));

        [HttpGet, Route("Api/Booking/GetBooking")]
        public HttpResponseMessage GetBooking(int equipmentId, int salesId)
            => Request.CreateResponse(_bookingService.GetBooking(equipmentId, salesId));

        [HttpGet, Route("Api/Booking/GetBookingBySalesId")]
        public HttpResponseMessage GetBookingBySalesId(int salesId) => Request.CreateResponse(_bookingService.GetBookingBySalesId(salesId));

        [HttpGet, Route("Api/Booking/ListBookingByCustomerId")]
        public HttpResponseMessage ListBookingByCustomerId(int customerId) => Request.CreateResponse(_bookingService.ListBookingByCustomerId(customerId));

        [HttpDelete]
        public HttpResponseMessage DeleteBooking(int bookingId) => Request.CreateResponse(_bookingService.DeleteBooking(bookingId));

        [HttpDelete, Route("Api/Booking/DeleteBookingEquipment")]
        public HttpResponseMessage DeleteBookingEquipment(int bookingEquipmentId) => Request.CreateResponse(_bookingService.DeleteBookingEquipment(bookingEquipmentId));
    }
}
