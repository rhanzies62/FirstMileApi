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
    public class UserController : ApiController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost, Route("Api/Users")]
        public HttpResponseMessage ListUsers([FromBody] GridFilter filter)
        {
            return Request.CreateResponse<GridResultGeneric<UserModel>>(HttpStatusCode.OK, _userService.ListUsers(filter));
        }

        [HttpGet, Route("Api/UpdateToken")]
        public HttpResponseMessage UpdateToken(string token)
        {
            var u = (FMIdentity)User.Identity;
            return Request.CreateResponse(HttpStatusCode.OK, _userService.UpdateFrameIOToken(u.GetUserId(), token));
        }

        [HttpGet, Route("Api/GetUserFrameIOToken")]
        public HttpResponseMessage GetUserFrameioToken()
        {
            var u = (FMIdentity)User.Identity;
            return Request.CreateResponse(HttpStatusCode.OK, _userService.GetUserFrameioToken(u.GetUserId()));
        }

        [HttpGet,Route("Api/ListUsers")]
        public HttpResponseMessage ListUsersWithFrameioToken()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _userService.ListUserWithFrameIOToken());
        }

        [HttpPost, Route("Api/AddEditMeili")]
        public HttpResponseMessage AddEditMeili([FromBody] MeiliModel model)
        {
            var u = (FMIdentity)User.Identity;
            model.UserId = u.GetUserId();
            return Request.CreateResponse<Response>(HttpStatusCode.OK, _userService.AddEditMeili(model));
        }

        [HttpPost, Route("Api/ListUserMeilie")]
        public HttpResponseMessage ListUserMeilie([FromBody] GridFilter filter)
        {
            var u = (FMIdentity)User.Identity;
            return Request.CreateResponse<GridResultGeneric<MeiliModel>>(HttpStatusCode.OK, _userService.ListUserMeilie(filter, u.GetUserId()));
        }

        [HttpPost, Route("Api/ListAllMeilie")]
        public HttpResponseMessage ListAllMeilie([FromBody] GridFilter filter)
        {
            return Request.CreateResponse<GridResultGeneric<MeiliModel>>(HttpStatusCode.OK, _userService.ListAllMeilie(filter));
        }

        [HttpGet, Route("Api/GetMeili")]
        public HttpResponseMessage GetMeili(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _userService.GetMeili(id));
        }
    }
}
