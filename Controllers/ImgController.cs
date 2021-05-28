using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthApiSesh.Constants;
using AuthApiSesh.Database;
using AuthApiSesh.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace AuthApiSesh.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImgController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ImgController(AppDbContext db, IHostingEnvironment hostingEnvironment)
        {
            this._db = db;
            this._hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Policy = "Access")]
        [HttpPost("setuserphoto")]
        public async Task<ActionResult> setUserPhoto([FromForm(Name = "img")] IFormFile photo)
        {
            var id = Int64.Parse(User.Claims.Where(x => x.Type == TokenClaims.UserId).First().Value);
            string fileName = "sesh" + "_" + DateTime.UtcNow.ToShortDateString() + "_" + id.ToString() + "." + photo.FileName.Split('.').Last();
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot\\UserPhoto\\" + fileName);
            try
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            User user = await _db.Users.FindAsync(id);
            user.avatar = path;
            await _db.SaveChangesAsync();
            return Ok(fileName);
        }

        [HttpGet("getuserphoto/{id}")]
        public async Task<ActionResult> getUserPhoto(long id)
        {
            string avatar = _db.Users.FindAsync(id).Result.avatar;
            if (avatar == null)
            {
                return NotFound();
            }
            Bitmap bmp = new Bitmap(avatar);
            return PhysicalFile(avatar, "image/png");
        }
    }
}
