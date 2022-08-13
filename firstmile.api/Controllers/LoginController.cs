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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : ApiController
    {
        private readonly IUserService _userService;
        public LoginController(IUserService userService)
        {
            _userService = userService;
        }
        public HttpResponseMessage Post([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.AuthenticateUser(model);
                if (response.IsSuccess)
                {
                    var authenticatedUser = new AuthenticatedUserModel(response.Data);
                    authenticatedUser.Token = Utility.CreateJWTToken(response.Data.UserId, response.Data.UserTypeId);
                    return Request.CreateResponse<Response>(HttpStatusCode.OK, new Response(ResponseType.Success, string.Empty, authenticatedUser));
                }
                return Request.CreateResponse<Response>(HttpStatusCode.BadRequest, response);
            }
            return Request.CreateResponse<Response>(HttpStatusCode.BadRequest, new Response(ResponseType.Error, "Incomplete Information", Utility.RetrieveErrorField(ModelState)));
        }
    }
}
