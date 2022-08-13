using firstmile.api.Authentication;
using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using firstmile.services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace firstmile.api.Controllers
{
    [FMAuthorizationRequired(userType: 2)]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserBookingController : ApiController
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public UserBookingController(IBookingService bookingService, IUserService userService)
        {
            _bookingService = bookingService;
            _userService = userService;
        }

        [HttpGet, Route("Api/UserBooking/ListActiveBooking")]
        public HttpResponseMessage ListActiveBooking()
        {
            var u = (FMIdentity)User.Identity;
            return Request.CreateResponse(_bookingService.ListActiveBooking(u.GetUserId()));
        }

        [HttpPost, Route("Api/UpdatePassword")]
        public HttpResponseMessage UpdatePassword([FromBody] ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var u = (FMIdentity)User.Identity;
                var response = _userService.UpdateUserPassword(model, u.GetUserId());
                if (response.IsSuccess)
                {
                    return Request.CreateResponse<Response>(HttpStatusCode.OK, new Response(ResponseType.Success, string.Empty));
                }
                return Request.CreateResponse<Response>(HttpStatusCode.BadRequest, response);
            }
            return Request.CreateResponse<Response>(HttpStatusCode.BadRequest, new Response(ResponseType.Error, "Incomplete Information", Utility.RetrieveErrorField(ModelState)));
        }

        [HttpGet, Route("Api/UserBooking/GetBooking")]
        public HttpResponseMessage GetBooking(int equipmentId, int salesId) => Request.CreateResponse(_bookingService.GetBooking(equipmentId, salesId));

        [HttpGet, Route("Api/UserBooking/ListBookingHistory")]
        public async Task<HttpResponseMessage> ListBookingHistory()
        {
            var u = (FMIdentity)User.Identity;
            var result = await _bookingService.ListBookingHistoryByUserId(u.GetUserId());
            return Request.CreateResponse(result);
        }

    }
}
