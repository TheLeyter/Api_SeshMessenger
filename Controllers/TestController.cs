using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthApiSesh.Database;
using AuthApiSesh.ClientModel;
using AuthApiSesh.Model;
using AuthApiSesh.Service;
using AuthApiSesh.Enums;
using AuthApiSesh.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using UAParser;
using AuthApiSesh.Attributes;

namespace AuthApiSesh.Controllers{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController : ControllerBase{
        private readonly AppDbContext _db;
        private readonly IJwtService _jwt;
        public TestController(AppDbContext db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ClientUser>> CreateUser(ClientUser clientUser){
            if(clientUser == null){
                return BadRequest();
            }
            User user = new User(clientUser.username,clientUser.email,clientUser.firstName,clientUser.lastName,clientUser.password);
            await _db.Users.AddAsync(user);
            int count = await _db.SaveChangesAsync();
            if(count == 0){
                return BadRequest();
            }
            

            return Ok(user.id); 
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<User>> GetUserById(long id){
            // var user =  _db.Users.FirstOrDefault(x => x.id == id);
            var user = await _db.Users.FindAsync(id);
            if(user == null){
                return NotFound();
            }

            return user;
        }

        [HttpGet("usertoken/{id}")]
        public async Task<ActionResult<User>> GetUserTokenById(long id){
            // var user =  _db.Users.FirstOrDefault(x => x.id == id);
            var user = await _db.Users.FindAsync(id);
            if(user == null){
                return NotFound();
            }

            return Ok(_jwt.GetAccessToken(user));
        }

        [HttpPost("access")]
        [CustomAuthorize]
        public async Task<ActionResult> AccesTets(string str){
            return Ok("Hello World!!!");
        }

        [Authorize(Policy = "Confirm")]
        [HttpPost("confirm")]
        public async Task<ActionResult> ConfirmTest(string str){
            return Ok("Hello World!!!");
        }
        
        //[HttpPost("createfriends")]
        //public async Task<ActionResult> CreateFriends(ClientFriends friends){
        //    if(friends == null){
        //        return BadRequest();
        //    }
        //     var users = _db.Users.Where(x => new long[]{friends.User1,friends.User2}.Contains(x.id)).ToList();
        //     if(users.Count <= 0){
        //        return BadRequest();
        //     }
             
        //    var res = await _db.Friends.AddAsync(new Friend(users[0],users[1]));
        //    await _db.SaveChangesAsync();

        //    return Ok(res.Entity);
        //}

        //[HttpGet("getfriend/{id}")]
        //public async Task<ActionResult> GetFriendsById(long id){
        //    if(id == null){
        //        return BadRequest();
        //    }

        //    var res = (_db.Friends.Where(x=>x.id==id).Include(x=>x.user1).Include(x=>x.user2)).FirstOrDefault();

        //    if(res == null){
        //        return BadRequest();
        //    }

        //    return Ok(res);
        //}

        //[HttpGet("geterror/{id}")]
        //public ActionResult GetError(int id){
        //    if(id == null){
        //        return BadRequest();
        //    }
        //    switch (id)
        //    {
        //        case 461:
        //            return StatusCode((int)StatusСode.ErrorEmail,StatusCodTitle.ErrorEmail);
        //        case 462:
        //            return StatusCode((int)StatusСode.ErrorUsername,StatusCodTitle.ErrorUsername);
        //        case 463:
        //            return StatusCode((int)StatusСode.ErrorPassword,StatusCodTitle.ErrorPassword);
        //        case 465:
        //            return StatusCode((int)StatusСode.ErrorSignIn,StatusCodTitle.ErrorSignIn);
        //    }
        //    return BadRequest();
        //}

        //[HttpGet("getallfriend/{id}")]
        //public async Task<ActionResult> GetAllFriendsById(long id){
        //    if(id == null){
        //        return BadRequest();
        //    }

        //    var res = _db.Friends.Where(x => x.user1.id == id).Include(x=>x.user2).Select(x=>x.user2);

        //    if(res.Count() == 0){
        //        return BadRequest();
        //    }

        //    return Ok(res);
        //}

        [HttpGet("getreftoken/{id}")]
        public async Task<ActionResult> GetRefreshToken(long id){
            var user = _db.Users.Find(id);
            return Ok(_jwt.GetRefreshToken(user));
        }

        [HttpGet("http")]
        public async Task<ActionResult> TestHttp(){
            string userAgent = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "User-Agent").ToString();

            var uaParser = Parser.GetDefault();

            ClientInfo info = uaParser.Parse(userAgent);

            return Ok(info);
        }
    }
}