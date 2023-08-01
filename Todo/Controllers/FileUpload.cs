using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUpload : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public FileUpload(IWebHostEnvironment env)
        {
            _env = env;
        }

        // POST api/<FileUpload>
        [HttpPost]
        public void Post(IFormFile file)
        {
            var rootPath = _env.ContentRootPath + "\\Upload\\";
            var filePath = file.FileName;

            using (var stream = System.IO.File.Create(rootPath + filePath))
            {
                file.CopyTo(stream);
            }
        }

        // POST api/MultipleUpload
        [HttpPost("MultipleUpload")]
        public void MultipleUpload(List<IFormFile> files)
        {
            var rootPath = _env.ContentRootPath + "\\Upload\\";

            foreach (var file in files)
            {
                var filePath = file.FileName;

                using (var stream = System.IO.File.Create(rootPath + filePath))
                {
                    file.CopyTo(stream);
                }
            }
        }
    }
}
