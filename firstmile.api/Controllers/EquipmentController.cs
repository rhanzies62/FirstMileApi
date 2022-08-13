using firstmile.api.Authentication;
using firstmile.domain.Model;
using firstmile.Domain.Utilities;
using firstmile.services.DejeroApi;
using firstmile.services.Interface;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace firstmile.api.Controllers
{
    [FMAuthorizationRequired(userType: 1)]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EquipmentController : ApiController
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IUsageService _usageService;
        public EquipmentController(IEquipmentService equipmentService, IUsageService usageService)
        {
            _equipmentService = equipmentService;
            _usageService = usageService;
        }

        public HttpResponseMessage Post([FromBody] EquipmentModel model)
        {
            if (ModelState.IsValid)
            {
                var u = (FMIdentity)User.Identity;
                var response = _equipmentService.AddOrEditEquipment(model, u.GetUserId());
                if (response.IsSuccess)
                {
                    return Request.CreateResponse<Response>(HttpStatusCode.OK, new Response(ResponseType.Success, string.Empty));
                }
                return Request.CreateResponse<Response>(HttpStatusCode.BadRequest, response);
            }
            return Request.CreateResponse<Response>(HttpStatusCode.BadRequest, new Response(ResponseType.Error, "Incomplete Information", Utility.RetrieveErrorField(ModelState)));
        }

        [HttpPost, Route("Api/ListEquipment")]
        public HttpResponseMessage ListEquipment([FromBody] GridFilter filter)
        {
            return Request.CreateResponse<GridResultGeneric<EquipmentModel>>(HttpStatusCode.OK, _equipmentService.ListEquipments(filter));
        }

        [HttpGet, Route("Api/ListGateways")]
        public async Task<HttpResponseMessage> ListGateways(DateTime from, DateTime to)
        {
            var result = await _equipmentService.ListGateways(from, to);
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/SaveGatewayUsage")]
        public async Task<HttpResponseMessage> RetrieveAndSaveUsage(int gatewayId, DateTime from, DateTime to)
        {
            var gatewayAPI = new GatewayAPI();
            var usageResult = await gatewayAPI.GetGatewayUsage(gatewayId, from, to);
            if (!usageResult.HasError)
            {
                var result = _usageService.SaveGatewayUsage(new UsageModel
                {
                    CellUsage = usageResult.CellUsage,
                    DateFrom = from,
                    DateTo = to,
                    GatewayId = gatewayId.ToString(),
                    TotalUsage = usageResult.TotalUsage,
                    OtherUsage = usageResult.TotalUsage - usageResult.CellUsage
                });
                return Request.CreateResponse(result);
            }
            return Request.CreateResponse(new Response(ResponseType.Error, "Usage Download Failed"));
        }

        [HttpGet, Route("Api/GetLastUsageDataOfGateway")]
        public async Task<HttpResponseMessage> GetLastUsageDataOfGateway(int gatewayId)
        {
            var result = await _usageService.GetLastUsageDataOfGateway(gatewayId.ToString());
            return Request.CreateResponse(result);
        }

        [HttpGet, Route("Api/ListAvailableEngo")]
        public HttpResponseMessage ListAvailableEngo()
        {
            var result = _equipmentService.ListAvailableEngo();
            return Request.CreateResponse(result);
        }
    }
}
