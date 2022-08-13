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
    public class RegisterController : ApiController
    {
        private readonly IUserService _userService;
        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        public HttpResponseMessage Post([FromBody] UserModel model)
        {
            if (ModelState.IsValid)
            {
                var response = _userService.CreateUser(model);
                return Request.CreateResponse<Response>(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse<Response>(HttpStatusCode.BadRequest, new Response(ResponseType.Error, "Incomplete Information", Utility.RetrieveErrorField(ModelState)));
        }
    }
}
