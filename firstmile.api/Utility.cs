using firstmile.domain.Utilities;
using Microsoft.Ajax.Utilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http.ModelBinding;

namespace firstmile.api
{
    public static class Utility
    {
        public static List<EntityErrorField> RetrieveErrorField(ModelStateDictionary ModelState)
        {
            var errFields = new List<EntityErrorField>();
            int index = 0;
            var keys = ModelState.Keys.ToArray();
            ModelState.Values.ForEach(m =>
            {
                m.Errors.ForEach(e =>
                {
                    string field = keys[index].Replace("model.", "");
                    string message = string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception.GetBaseException().Message : e.ErrorMessage;
                    errFields.Add(new EntityErrorField
                    {
                        Field = field,
                        Message = message
                    });
                });
                index++;
            });

            return errFields;
        }

        public static string CreateJWTToken(int userId, int userType)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(FMApiResource.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: new List<Claim>() { new Claim("usrid", userId.ToString()), new Claim("ust", userType.ToString()) },
                issuer: "http://topan-dev-web.azurewebsites.net/",
                audience: "http://topan-dev-web.azurewebsites.net/",
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}