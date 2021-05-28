using AuthApiSesh.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthApiSesh.Attributes
{
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private SymmetricSecurityKey _symmetricSecurityKey;
        private JwtSettings settings;


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault().Split(' ').Last();

            Console.WriteLine(token);

            settings = context.HttpContext.RequestServices.GetService<IOptions<JwtSettings>>().Value;

            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SecretKey));

            if (!validateToken(token))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private bool validateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    ValidateIssuer = true,

                    ValidateAudience = true,

                    ValidIssuer = settings.Issuer,

                    ValidAudience = settings.Audience,

                    IssuerSigningKey = _symmetricSecurityKey
                },
                out SecurityToken validatetToken);

            }
            catch
            {

                return false;
            }
            return true;

        }
    }
}
