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
    public class CustomerController : ApiController
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public HttpResponseMessage CreateCustomer([FromBody] CustomerModel model)
        {
            if (!ModelState.IsValid && model.CustomerId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new Response(ResponseType.Error, "Incomplete Information", Utility.RetrieveErrorField(ModelState)));
            }
            var u = (FMIdentity)User.Identity;
            var response = _customerService.AddOrEditCustomer(model, u.GetUserId());
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new Response(ResponseType.Success, string.Empty));
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

        [HttpPost, Route("Api/Customer/ListCustomers")]
        public HttpResponseMessage ListCustomers([FromBody] GridFilter filter)
        {
            var result = _customerService.ListCustomers(filter);
            return Request.CreateResponse(result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest, result);
        }

        [HttpPost, Route("Api/Customer/AddCustomerUser")]
        public HttpResponseMessage AddCustomerUser([FromBody] CustomerModel model)
        {
            var u = (FMIdentity)User.Identity;
            var result = _customerService.AddCustomerUser(model, u.GetUserId());
            return Request.CreateResponse(result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.BadRequest, result);
        }

        [HttpGet, Route("Api/Customer/ListAllAvailableUser")]
        public HttpResponseMessage ListAllAvailableUser(int? customerId = null) => Request.CreateResponse(_customerService.ListAllAvailableUser(customerId));

        [HttpGet, Route("Api/Customer/ListAssignedUserByCustomerId")]
        public HttpResponseMessage ListAssignedUserByCustomerId(int customerId)
        {
            return Request.CreateResponse(_customerService.ListAssignedUserByCustomerId(customerId));
        }
    }
}
