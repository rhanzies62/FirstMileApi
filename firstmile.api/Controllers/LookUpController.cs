using firstmile.domain.Model;
using firstmile.Domain.Common;
using firstmile.services.DejeroApi;
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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LookUpController : ApiController
    {
        private readonly ILookUpService _lookUpService;
        private readonly IEquipmentService _equipmentService;
        public LookUpController(ILookUpService lookUpService, IEquipmentService equipmentService)
        {
            _lookUpService = lookUpService;
            _equipmentService = equipmentService;
        }

        [HttpGet, Route("Api/LookUp/ListEquipmentTypes")]
        public HttpResponseMessage ListEquipmentTypes() => Request.CreateResponse(_lookUpService.ListLookUpByCode(LookUpTypes.EquipmentType));

        [HttpGet, Route("Api/LookUp/ListCountry")]
        public HttpResponseMessage ListCountry() => Request.CreateResponse(_lookUpService.ListLookUpByCode(LookUpTypes.Country));

        [HttpGet, Route("Api/LookUp/ListProvince")]
        public HttpResponseMessage ListProvince(int countryId) => Request.CreateResponse(_lookUpService.ListProvinceByCountryId(countryId));

        [HttpGet, Route("Api/LookUp/ListCustomers")]
        public HttpResponseMessage ListCustomers() => Request.CreateResponse(_lookUpService.ListCustomers());

        [HttpGet, Route("Api/LookUp/ListAvailableEquipment")]
        public HttpResponseMessage ListAvailableEquipment() => Request.CreateResponse(_lookUpService.ListAvailableEquipment());

        [HttpGet, Route("Api/LookUp/ListEquipmentAvailability")]
        public HttpResponseMessage ListEquipmentAvailability(int equipmentId) => Request.CreateResponse(_lookUpService.ListEquipmentAvailability(equipmentId));

        [HttpGet, Route("Api/LookUp/ListGateway")]
        public HttpResponseMessage ListGateway()
        {
            var result = new GatewayAPI().ListGateways().GetAwaiter().GetResult();
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/LookUp/GetGatewayBySerial")]
        public HttpResponseMessage GetGatewayBySerial(string serial)
        {
            var result = new GatewayAPI().GetGatewayBySerial(serial).GetAwaiter().GetResult();
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/LookUp/GetGatewayUsage")]
        public async Task<HttpResponseMessage> GetGatewayUsage(int gatewayId, DateTime from, DateTime to)
        {
            var result = await _equipmentService.GetGatewayUsage(gatewayId, from, to);
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/LookUp/GetSourceUsage")]
        public async Task<HttpResponseMessage> GetSourceUsage(int sourceId, DateTime from, DateTime to)
        {
            var result = await _equipmentService.GetSourceUsage(sourceId, from, to);
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/LookUp/ListProjectsByCustomerId")]
        public HttpResponseMessage ListProjectsByCustomerId(int customerId)
        {
            var result = _lookUpService.ListProjectsByCustomerId(customerId);
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/LookUp/ListStatusTypes")]
        public HttpResponseMessage ListStatusTypes() => Request.CreateResponse(_lookUpService.ListLookUpByCode(LookUpTypes.Status));

        [HttpGet, Route("Api/LookUp/GetSourceBySerial")]
        public HttpResponseMessage GetSourceBySerial(string serial, string type)
        {
            var result = new GatewayAPI().GetSourceBySerial(serial, type).GetAwaiter().GetResult();
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/LookUp/ListActivityType")]
        public HttpResponseMessage ListActivityType() => Request.CreateResponse(_lookUpService.ListLookUpByCode(LookUpTypes.ActivityType));

        [HttpGet, Route("Api/LookUp/ListEquipmentLocations")]
        public async Task<HttpResponseMessage> ListEquipmentLocations()
        {
            var locations = await _equipmentService.ListEquipmentLocations();
            return Request.CreateResponse(locations);
        }

        [HttpGet, Route("Api/LookUp/GetGatewayLocation")]
        public async Task<HttpResponseMessage> GetGatewayLocation(int dejeroId)
        {
            var location = await new GatewayAPI().GetGatewayLocation(dejeroId);
            return Request.CreateResponse(location);
        }

        [HttpGet, Route("Api/LookUp/GetSourceLocation")]
        public async Task<HttpResponseMessage> GetSourceLocation(int dejeroId)
        {
            var location = await new GatewayAPI().GetSourceLocation(dejeroId);
            return Request.CreateResponse(location);
        }
    }

}
