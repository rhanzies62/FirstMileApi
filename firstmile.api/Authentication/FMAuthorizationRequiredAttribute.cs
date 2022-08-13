using firstmile.Domain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace firstmile.api.Authentication
{
    public class FMAuthorizationRequiredAttribute : Attribute, IAuthenticationFilter
    {
        private readonly int _userType;
        public FMAuthorizationRequiredAttribute(int userType)
        {
            _userType = userType;
        }
        private HttpRequestMessage request;
        public bool AllowMultiple { get; set; }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            request = context.Request;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                if (request.Headers.Authorization != null)
                {
                    IEnumerable<string> listValues = new List<string>();


                    var tokenValue = request.Headers.Authorization.Parameter;
                    var _token = tokenHandler.ReadJwtToken(tokenValue);
                    
                        //check if token is already expired
                    var dateDiff = _token.ValidTo - DateTime.UtcNow;
                    if (dateDiff.TotalSeconds < 0)
                    {
                        context.ErrorResult = GenerateUnauthorizedResponse(Unauthorization.Expired);
                    }
                    else
                    {
                        //TODO: 
                        //var userType = _token.Claims.Where(i => i.Type == "ust").FirstOrDefault();
                        //if (userType.Value == _userType.ToString())
                        //{
                            var identity = new FMIdentity("FirstMile", "WebAPI", true)
                            {
                                Claims = _token.Claims
                            };

                            var genericPrincipal = new FMPrincipal(identity, null);
                            context.Principal = genericPrincipal;
                        //}
                        //else
                        //{
                        //    context.ErrorResult = GenerateUnauthorizedResponse(Unauthorization.InvalidAccess);
                        //}
                    }
                }
                else
                {
                    context.ErrorResult = GenerateUnauthorizedResponse(Unauthorization.Missing);
                }
            }
            catch (Exception e)
            {
                context.ErrorResult = GenerateUnauthorizedResponse(Unauthorization.InvalidToken);
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        private AuthenticationFailureResult GenerateUnauthorizedResponse(Unauthorization type)
        {
            string reasonPhrase = "";
            switch (type)
            {
                case Unauthorization.Expired:
                    {
                        reasonPhrase = "Token Expired";
                    }
                    break;
                case Unauthorization.InvalidToken:
                    {
                        reasonPhrase = "Invalid Token";
                    }
                    break;
                case Unauthorization.Missing:
                    {
                        reasonPhrase = "Missing Token";
                    }
                    break;
                case Unauthorization.InvalidAccess:
                    {
                        reasonPhrase = "Your account is not allowed to call this API";
                    }
                    break;
            }

            return new AuthenticationFailureResult(new { Error = true, Message = reasonPhrase }, request);
        }
    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult(object jsonContent, HttpRequestMessage request)
        {
            JsonContent = jsonContent;
            Request = request;
        }

        public HttpRequestMessage Request { get; private set; }

        public Object JsonContent { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            response.Content = new ObjectContent(JsonContent.GetType(), JsonContent, new JsonMediaTypeFormatter());
            return response;
        }
    }

    public class FMIdentity : IIdentity
    {
        public FMIdentity(string name, string authenticationType, bool isAuthenticated)
        {
            this.Name = name;
            this.AuthenticationType = authenticationType;
            this.IsAuthenticated = isAuthenticated;
        }

        public string Name { get; set; }

        public string AuthenticationType { get; set; }

        public IEnumerable<Claim> Claims { get; set; }

        public bool IsAuthenticated { get; set; }

        public int GetUserId()
        {
            var claim = this.Claims.FirstOrDefault(t => t.Type == "usrid");
            if (claim == null) return 0;
            else return int.Parse(claim.Value);
        }

        public List<int> GetRecyclerAssigned()
        {
            var claim = this.Claims.FirstOrDefault(t => t.Type == "aur");
            if (claim == null) return new List<int>();
            else return JsonConvert.DeserializeObject<List<int>>(claim.Value);
        }
    }

    public class FMPrincipal : IPrincipal
    {
        public FMPrincipal(IIdentity identity, string[] roles = null)
        {
            Identity = identity;
        }
        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }

}