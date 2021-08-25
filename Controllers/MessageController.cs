using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthApiSesh.Constants;
using AuthApiSesh.Database;
using AuthApiSesh.Model.ServerModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApiSesh.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MessageController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("lastten")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> getLast10FromChat(long chatid)
        {
            long targetUserId = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            var chat = await _db.Chats.FindAsync(chatid);

            if(chat is null)
            {
                return BadRequest(); //TODO: create custom status code "ChatNotFound"
            }

            List<Message> messages = (from m in chat.messages
                            orderby m.timeSend
                            select m).TakeLast(10).ToList();

            return new JsonResult(messages);
        }

        [HttpGet("last")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> getLastFromChat(long chatid)
        {
            long targetUserId = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            var chat = await _db.Chats.FindAsync(chatid);

            if (chat is null)
            {
                return BadRequest(); //TODO: create custom status code "ChatNotFound"
            }

            var messages = chat.messages.Last();

            return new JsonResult(messages);
        }

        [HttpGet("lastfrom")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> get10FromChat(long chatid, long messageid)
        {
            long targetUserId = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            var chat = await _db.Chats.FindAsync(chatid);

            if (chat is null)
            {
                return BadRequest(); //TODO: create custom status code "ChatNotFound"
            }

            var messages = (from m in chat.messages
                            orderby m.timeSend
                            select m)
                           .SkipWhile(x => x.id != messageid)
                           .Skip(1)
                           .TakeLast(10)
                           .ToList();


            return new JsonResult(messages);

        }

        [HttpGet("messagebyid")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> getMessageById(long messageid)
        {
            long targetUserId = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);

            var message = await _db.Messages.FindAsync(messageid);

            if (message is null)
            {
                return NotFound(); //TODO: create custom status code "ChatNotFound"
            }


            if(message.chat.userId != targetUserId || message.chat.userCreatorId != targetUserId)
            {
                return new ForbidResult();
            }


            return new JsonResult(message);

        }
    }
}
