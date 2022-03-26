using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Models
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private AppDbContext _db;
        public ProfileController(AppDbContext context)
        {
            this._db = context;
        }
        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(IFormFile files)
        {
            Console.WriteLine(files);
            if(files == null)
            {
                return BadRequest();
            }
            var dir = Path.Combine("./MyStaticFiles/images/");
            var filePath = Path.Combine(dir, files.FileName);
            Console.WriteLine(filePath);
                
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await files.CopyToAsync(fileStream);
            }
                
            

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok();
        }
        
    }
}