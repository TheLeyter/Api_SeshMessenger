using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AuthApiSesh.Database;
using AuthApiSesh.Model;
using AuthApiSesh.ClientModel;
using AuthApiSesh.Enums;
using AuthApiSesh.Constants;
using AuthApiSesh.Service;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using UAParser;
using Microsoft.Net.Http.Headers;
using AuthApiSesh.Model.ServerModel;
using Microsoft.Extensions.Configuration;

namespace AuthApiSesh.Controllers{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountController : ControllerBase{
        private readonly AppDbContext _db;
        private readonly IJwtService _jwt;
        private readonly IEmailService _email;
        private readonly IMemoryCache _cache;

        public AccountController(AppDbContext db, IJwtService jwt, IEmailService email, IMemoryCache cache)
        {
            this._db = db;
            this._jwt = jwt;
            this._email = email;
            this._cache = cache;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> SignUp(ClientUser clientUser){
            if(clientUser == null){
                return BadRequest();
            }
            if(string.IsNullOrWhiteSpace(clientUser.username) || _db.Users.FirstOrDefault(x => x.username == clientUser.username)!= null){
                return StatusCode((int)StatusСode.ErrorUsername,StatusCodTitle.ErrorUsername);
            }
            else if(string.IsNullOrWhiteSpace(clientUser.email) || _db.Users.FirstOrDefault(x => x.email == clientUser.email)!= null){
                return StatusCode((int)StatusСode.ErrorEmail,StatusCodTitle.ErrorEmail);
            }
            else if(string.IsNullOrWhiteSpace(clientUser.firstName)){
                return StatusCode((int)StatusСode.ErrorFirstName, StatusCodTitle.ErrorFirstName);
            }
            else if (string.IsNullOrWhiteSpace(clientUser.lastName))
            {
                return StatusCode((int)StatusСode.ErrorLastNsme, StatusCodTitle.ErrorLastName);
            }
            else if(string.IsNullOrWhiteSpace(clientUser.password)){
                return StatusCode((int)StatusСode.ErrorPassword,StatusCodTitle.ErrorPassword);
            }

            User user = new User(clientUser.username,clientUser.email,clientUser.firstName,clientUser.lastName,clientUser.password);

            await _db.Users.AddAsync(user);

            if(await _db.SaveChangesAsync()==0){
                return BadRequest();
            }

            var code = new Random().Next(1000,9999);

            _cache.Set(user.id, code, new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            await _email.SendVerificationAsync(code,user);

            var confirmToken = _jwt.GetConfirmToken(user);

            return StatusCode((int)StatusСode.NoVerification, confirmToken);
        }

        [Authorize(Policy = "Confirm")]
        //[HttpGet("verification/{code}")]
        [HttpGet("verification")]//get code from params
        public async Task<ActionResult> Verification(int code){

            var id = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            int value ;

            if(!_cache.TryGetValue(id,out value)){
                return StatusCode((int)StatusСode.ErrorVerifLifetime,StatusCodTitle.ErrorVerifLifetime);
            }
            else if(value != code){
                return StatusCode((int)StatusСode.ErrorVerifCode,StatusCodTitle.ErrorVerifCode);
            }

            var user = await _db.Users.FindAsync(id);
            user.confirm = true;

            string userAgent = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "User-Agent").ToString();

            var uaParser = Parser.GetDefault();

            ClientInfo info = uaParser.Parse(userAgent);

            string RefreshToken = await _jwt.GetRefreshTokenAsync(user);

            string AccessToken = await _jwt.GetAccessTokenAsync(user);

            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            string os = info.OS.Family + " " + info.OS.Major;

            _db.RefreshTokens.Add(new RefreshToken(user, RefreshToken, ip, os));

            await _db.SaveChangesAsync();

            return Ok(new TokenPair(AccessToken, RefreshToken));
        }

        [HttpPost("signin")]
        public async Task<ActionResult> SignIn(ClientUser clientUser){
            if (clientUser == null)
            {
                return BadRequest();
            }

            var user = _db.Users.FirstOrDefault(x => x.username == clientUser.username);

            if(user==null){
                return StatusCode((int)StatusСode.ErrorUsername,StatusCodTitle.ErrorUsername);
            }
            else if(user.password != clientUser.password){
                return StatusCode((int)StatusСode.ErrorPassword,StatusCodTitle.ErrorPassword);
            }
            else if(!user.confirm){
                var code = new Random().Next(1000,9999);

                _cache.Set(user.id, code, new MemoryCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                await _email.SendVerificationAsync(code,user);

                string confirmToken = _jwt.GetConfirmToken(user);

                return StatusCode((int)StatusСode.NoVerification, confirmToken);
            }

            string userAgent = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "User-Agent").ToString();

            var uaParser = Parser.GetDefault();

            ClientInfo info = uaParser.Parse(userAgent);

            string RefreshToken = await _jwt.GetRefreshTokenAsync(user);

            string AccessToken = await _jwt.GetAccessTokenAsync(user);

            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            string os = info.OS.Family + " " + info.OS.Major;

            _db.RefreshTokens.Add(new RefreshToken(user, RefreshToken, ip, os));

            await _db.SaveChangesAsync();

            return Ok(new TokenPair(AccessToken, RefreshToken));
        }

        [Authorize(Policy = "Access")]
        [HttpGet("getaccount")]
        public async Task<ActionResult> GetMyInfo()
        {
            var id = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            if (id == 0)
            {
                return Unauthorized();
            }

            User user = await _db.Users.FindAsync(id);

            if(user == null)
            {
                return Unauthorized();
            }

            UserInfo userinfo = new UserInfo(user);

            return Ok(userinfo);
        }

        [Authorize(Policy = "Refresh")]
        [HttpGet("validtoken")]
        public async Task<ActionResult> ChackRefToken()
        {
            string requestToken = base.HttpContext.Request.Headers[HeaderNames.Authorization].First().Split(' ').Last();
            var Token = _db.RefreshTokens.FirstOrDefault(x => x.token == requestToken);
            if (Token == null)
            {
                return BadRequest();
            }
            return Ok(true);
        }

        [Authorize(Policy = "Refresh")]
        [HttpGet("signout")]
        public async Task<ActionResult> signOut()
        {
            string requestToken = base.HttpContext.Request.Headers[HeaderNames.Authorization].First().Split(' ').Last();
            _db.RefreshTokens.Remove(_db.RefreshTokens.Where(x => x.token == requestToken).FirstOrDefault());
            await _db.SaveChangesAsync();
            return Ok(true);
        }

        [Authorize(Policy = "Refresh")]
        [HttpGet("accesstoken")]
        public async Task<ActionResult> GetAccesToken(){
            var id = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            if(id==0){
                return Unauthorized();
            }

            User user = await _db.Users.FindAsync(id);

            if(user == null)
            {
                return Unauthorized();
            }

            var requestToken = HttpContext.Request.Headers[HeaderNames.Authorization].First().Split(' ').Last();

            var Token = _db.RefreshTokens.FirstOrDefault(x => x.user == user && x.token == requestToken);
            
            if(Token == null){
                return Unauthorized();
            }

            string accessToken = await _jwt.GetAccessTokenAsync(user);
            string refreshToken = await _jwt.GetRefreshTokenAsync(user);

            var result = new TokenPair(accessToken,refreshToken);

            Token.lastActivity = DateTime.UtcNow;
            Token.token = refreshToken;

            await _db.SaveChangesAsync();

            return Ok(result);
        }

    }
}