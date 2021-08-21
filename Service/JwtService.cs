using System;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AuthApiSesh.Model;
using AuthApiSesh.Enums;
using AuthApiSesh.Constants;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using AuthApiSesh.Settings;
using System.Linq;
using System.Threading.Tasks;
using AuthApiSesh.Database;

namespace AuthApiSesh.Service
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings settings;
        private const string _algorithm = SecurityAlgorithms.HmacSha256;
        public readonly SymmetricSecurityKey _symmetricSecurityKey;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {

            settings = jwtSettings.Value;

            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SecretKey));
        }

        private string accesTokenHandler(IEnumerable<Claim> Claims)
        {
            var TimeNow = DateTime.Now;

            var token = new JwtSecurityToken(
               issuer: settings.Issuer,
               audience: settings.Audience,
               notBefore: TimeNow,
               expires: TimeNow.AddMinutes(settings.AccessTokenLifeTime),
               claims: Claims,
               signingCredentials: new SigningCredentials(_symmetricSecurityKey, _algorithm)
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string confirmTokenHandler(IEnumerable<Claim> Claims)
        {
            var TimeNow = DateTime.Now;

            var token = new JwtSecurityToken(
               issuer: settings.Issuer,
               audience: settings.Audience,
               notBefore: TimeNow,
               expires: TimeNow.AddMinutes(settings.ConfirmTokenLifeTime),
               claims: Claims,
               signingCredentials: new SigningCredentials(_symmetricSecurityKey, _algorithm)
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string refreshTokenHandler(IEnumerable<Claim> Claims)
        {
            var TimeNow = DateTime.Now;

            var token = new JwtSecurityToken(
               issuer: settings.Issuer,
               audience: settings.Audience,
               notBefore: TimeNow,
               expires: TimeNow.AddMonths(settings.RefreshTokenLifeTime),
               claims: Claims,
               signingCredentials: new SigningCredentials(_symmetricSecurityKey, _algorithm)
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetAccessToken(User user)//
        {
            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, user.id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, user.username), new Claim(TokenClaims.Type, TokenTypes.Access) };

            return accesTokenHandler(Claims);
        }


        public string GetAccessToken(long id, string username)//
        {
            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, username), new Claim(TokenClaims.Type, TokenTypes.Access) };

            return accesTokenHandler(Claims);
        }

        public string GetConfirmToken(long id, string email)
        {

            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.Email,email), new Claim(TokenClaims.Type, TokenTypes.Confirm) };

            return confirmTokenHandler(Claims);

        }


        public string GetConfirmToken(User User)
        {

            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, User.id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.Email, User.email), new Claim(TokenClaims.Type, TokenTypes.Confirm) };

            return confirmTokenHandler(Claims);

        }


        public async Task<string> GetAccessTokenAsync(User User)//
        {
            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, User.id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, User.username), new Claim(TokenClaims.Type, TokenTypes.Access) };

            return accesTokenHandler(Claims);
        }

        public async Task<string> GetAccessTokenAsync(long id, string username)//
        {
            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, username), new Claim(TokenClaims.Type, TokenTypes.Access) };

            return accesTokenHandler(Claims);
        }

        public async Task<string> GetConfirmTokenAsync(long id, string email)//
        {

            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.Email, email), new Claim(TokenClaims.Type, TokenTypes.Confirm) };

            return confirmTokenHandler(Claims);
        }

        public string GetRefreshToken(User user)
        {
            var TimeNow = DateTime.Now;

            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, user.id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, user.username), new Claim(TokenClaims.Type, TokenTypes.Refresh), new Claim("iat", new DateTimeOffset(TimeNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) };

            return refreshTokenHandler(Claims);
        }

        public string GetRefreshToken(long userId, string username)
        {
            var TimeNow = DateTime.Now;

            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, userId.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, username), new Claim(TokenClaims.Type, TokenTypes.Refresh), new Claim("iat", new DateTimeOffset(TimeNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) };

            return refreshTokenHandler(Claims);
        }

        public async Task<string> GetRefreshTokenAsync(User user)
        {
            var TimeNow = DateTime.Now;

            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, user.id.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, user.username), new Claim(TokenClaims.Type, TokenTypes.Refresh), new Claim("iat", new DateTimeOffset(TimeNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) };

            return refreshTokenHandler(Claims);
        }

        public async Task<string> GetRefreshTokenAsync(long userId, string username)
        {
            var TimeNow = DateTime.Now;

            var Claims = new List<Claim> { new Claim(TokenClaims.UserId, userId.ToString(), ClaimValueTypes.Integer64), new Claim(TokenClaims.UserName, username), new Claim(TokenClaims.Type, TokenTypes.Refresh), new Claim("iat", new DateTimeOffset(TimeNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) };

            return refreshTokenHandler(Claims);
        }

        public bool validateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    ValidateIssuer = true,

                    ValidateAudience = true,

                    ValidateLifetime = true,

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

        public string GetConfirmToken(long id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetConfirmTokenAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}

        