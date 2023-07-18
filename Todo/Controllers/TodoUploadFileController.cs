using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Todo.Dtos;
using Todo.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Route("api/todo/{todoId}/uploadFile")]
    [ApiController]
    public class TodoUploadFileController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _iMapper;

        public TodoUploadFileController(TodoContext todoContext, IMapper iMapper)
        {
            _todoContext = todoContext;
            _iMapper = iMapper;
        }

        // GET: api/<TodoUploadFileController>
        [HttpGet]
        public IActionResult Get(Guid todoId)
        {
            if (!_todoContext.TodoLists.Any(x => x.TodoId == todoId))
            {
                return NotFound("找不到 todoId: " + todoId + "。");
            }

            var uploadFiles = from a in _todoContext.UploadFiles
                              where a.TodoId == todoId
                              select new UploadFileDto 
                              { 
                                Name = a.Name,
                                Src = a.Src,
                                TodoId = a.TodoId,
                                UploadFileId = a.UploadFileId,
                              };

            if (uploadFiles == null || uploadFiles.Count() <= 0)
            {
                return NotFound("todoId: " + todoId + " 查無 Uploadfiles");
            }

            return Ok(uploadFiles);
        }


        [HttpGet("{uploadFileId}")]
        public IActionResult GetSingleUploadFile(Guid todoId, Guid UploadFileId)
        {
            if (!_todoContext.TodoLists.Any(x => x.TodoId == todoId))
            {
                return NotFound("找不到 todoId: " + todoId + "。");
            }

            var uploadFile = (from a in _todoContext.UploadFiles
                              where a.TodoId == todoId
                              && a.UploadFileId == UploadFileId
                              select new UploadFileDto
                              {
                                  Name = a.Name,
                                  Src = a.Src,
                                  TodoId = a.TodoId,
                                  UploadFileId = a.UploadFileId,
                              })
                              .SingleOrDefault();

            if (uploadFile == null)
            {
                return NotFound("todoId: " + todoId + " 查無此 UploadFileId(" + UploadFileId + ")");
            }

            return Ok(uploadFile);
        }

        // Post: api/<TodoUploadFileController>
        [HttpPost]
        public IActionResult Post(Guid todoId, [FromBody]UploadFile model)
        {
            if (!_todoContext.TodoLists.Any(x => x.TodoId == todoId))
            {
                return NotFound("找不到 todoId: " + todoId + "。");
            }

            var insertData = new UploadFile()
            {
                TodoId = model.TodoId,
                Name = model.Name,
                Src = model.Src
            };

            _todoContext.UploadFiles.Add(insertData);
            _todoContext.SaveChanges();

            return NoContent();
        }
    }
}
