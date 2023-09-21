using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Todo.Filters;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public FileUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // POST api/<FileUpload>
        [HttpPost]
        [FileLimit(ExtentionLimit = ".txt", SizeLimit = 1)]
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
