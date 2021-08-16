using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthApiSesh.Database;
using AuthApiSesh.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApiSesh.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db)
        {
            this._db = db;
        }


        [HttpGet("getuser/{id}")]
        public async Task<ActionResult> getUserById(long id)
        {
            User user = await _db.Users.FindAsync(id);

            if(user is null)
            {
                return NotFound();
            }

            return new JsonResult(user);
        }

        [HttpGet("startnameusers/{str}")]
        public async Task<ActionResult> usersNameStartWith(string str)
        {
            var users = _db.Users.Where(x => x.username.Contains(str));
            if (users.Count() == 0)
            {
                return NotFound();
            }

            return new JsonResult(users.ToList());
        }



    }
}
