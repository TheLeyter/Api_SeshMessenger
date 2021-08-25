using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthApiSesh.Constants;
using AuthApiSesh.Database;
using AuthApiSesh.Model;
using AuthApiSesh.Model.ServerModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApiSesh.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ChatController(AppDbContext db)
        {
            _db = db;
        }


        [HttpPost("create")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> createChat([FromBody]long id)
        {
            long targetUserId = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            User user = await _db.Users.FindAsync(id);
            User targetUser = await _db.Users.FindAsync(targetUserId);

            if (targetUserId == id) return BadRequest();

            if(user is null)
            {
                return NotFound(); //TODO: create custom status code "UserNotFound"
            }

            if(_db.Chats.Where(x=> (x.userCreator.id == targetUserId && x.user.id == id) || (x.userCreator.id == id && x.user.id == targetUserId)).Count() > 0)
            {
                return BadRequest(); //TODO: create custom status code "ChatAlreadyCreated"
            }

            var chat = await _db.Chats.AddAsync(new Chat(targetUser,user));

            await _db.SaveChangesAsync();

            return new JsonResult(chat.Entity);

        }


        [HttpGet("userchats")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> getAllUserChats()
        {
            long targetUserId = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            var chats = _db.Chats.Where(x => x.user.id == targetUserId || x.userCreator.id == targetUserId);

            if(chats.Count() == 0)
            {
                return NotFound();
            }

            return new JsonResult(chats.ToList());
        }

        [HttpGet("getchat")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> getChatById(long id)
        {
            long targetUserId = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            var chat = _db.Chats.FirstOrDefault(x => x.id == id && (x.user.id == targetUserId || x.userCreator.id == targetUserId));

            if (chat is null) return NotFound();

            return new JsonResult(chat);
        }
    }
}
