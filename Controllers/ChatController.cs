using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthApiSesh.Database;
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


        [HttpGet("create/{id}")]
        [Authorize(Policy = "Access")]
        public async Task<ActionResult> createChat(long id)
        {

        }

    }
}
