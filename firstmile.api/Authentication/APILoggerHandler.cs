using firstmile.domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using firstmile.services.Services;
using firstmile.services.Interface;

namespace firstmile.api.Authentication
{
    public class APILoggerHandler : DelegatingHandler
    {
        string requestBody = "";
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = request.Content;

            //excluding attachment since attachments are too large to save in the log file
            requestBody = request.RequestUri.ToString().ToLower().Contains("attachment") ? string.Empty : await content.ReadAsStringAsync();

            var logMetadata = BuildRequestMetadata(request);
            var response = await base.SendAsync(request, cancellationToken);
            logMetadata = BuildResponseMetadata(logMetadata, response);
            await SendToLog(logMetadata);
            return response;
        }
        private ApiLogModel BuildRequestMetadata(HttpRequestMessage request)
        {
            ApiLogModel log = new ApiLogModel
            {
                RequestBody = requestBody,
                RequestMethod = request.Method.Method,
                RequestTimestamp = DateTime.Now,
                RequestUri = request.RequestUri.ToString(),
                RequestHeader = JsonConvert.SerializeObject(request.Headers),
                IPAddress = GetClientIp(request),
                RequestContentType = request.Content.Headers.ContentType == null ? string.Empty : request.Content.Headers.ContentType.MediaType
            };
            return log;
        }
        private ApiLogModel BuildResponseMetadata(ApiLogModel logMetadata, HttpResponseMessage response)
        {
            logMetadata.ResponseStatusCode = (int)response.StatusCode;
            logMetadata.ResponseTimestamp = DateTime.Now;
            logMetadata.ResponseContentType = response.Content == null ? string.Empty : response.Content.Headers.ContentType.MediaType;
            return logMetadata;
        }
        private async Task<bool> SendToLog(ApiLogModel logMetadata)
        {
            if (!string.IsNullOrEmpty(logMetadata.ResponseContentType))
            {
                var service = DependencyResolver.Current.GetService<ILoggerService>();
                service.CreateAPILog(logMetadata);
            }
            return true;
        }
        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            return null;
        }
    }
}