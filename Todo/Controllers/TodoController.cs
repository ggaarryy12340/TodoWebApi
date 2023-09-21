using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Dtos;
using Todo.Models;
using Todo.Parameters;
using Todo.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Todo")]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _iMapper;
        private readonly TodoListService _todoListService;

        public TodoController(TodoContext todoContext, IMapper iMapper, TodoListService todoListService)
        { 
            _todoContext = todoContext;
            _iMapper = iMapper;
            _todoListService = todoListService;
        }

        // GET: api/<TodoController>
        [HttpGet]
        public IActionResult Get([FromQuery] TodoGetParameters todoGetParameters)
        {
            var todos = _todoListService.GetAll(todoGetParameters);

            if (todos.Count() <= 0)
            { 
                return NotFound("找不到資源");
            }

            return Ok(todos);
        }

        // GET: api/<TodoController>
        // 用 AutoMapper 轉成 TodoListSelectDtos 後傳出
        [HttpGet("GetByAutoMapper")]
        [AllowAnonymous]
        public IEnumerable<TodoListSelectDto> GetByAutoMapper([FromQuery] TodoGetParameters todoGetParameters)
        {
            return _todoListService.GetByAutoMapper(todoGetParameters);
        }

        /// <summary>
        /// 透過 SQL 查詢 TodoList 後回傳
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpGet("GetTodoByRawSQL")]
        public IEnumerable<TodoList> GetTodoByRawSQL([FromQuery] string orders)
        {
            return _todoListService.GetByRawSQL(orders);
        }

        // GET api/<TodoController>/id
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var todo = _todoListService.GetSingle(id);

            if (todo is null)
            { 
                return NotFound("找不到 id: " + id + " 的資料");
            }

            return Ok(todo);
        }

        // POST api/<TodoController>
        [HttpPost]
        public IActionResult Post([FromBody] TodoList model)
        {
            var insertData = _todoListService.Insert(model);

            return Ok(insertData.TodoId);
        }

        [HttpPost("PostBySQL")]
        public IActionResult PostBySQL([FromBody] TodoListPostDto model)
        {
            _todoListService.InsertBySQL(model);

            return NoContent();
        }

        // PUT api/<TodoController>/5
        [HttpPut("{todoId}")]
        public IActionResult Put(Guid todoId, [FromBody] TodoList model)
        {
            if (model.TodoId != todoId)
            {
                return BadRequest();
            }

            if (_todoListService.PutUpdate(todoId, model) == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // PATCH api/<TodoController>/5
        [HttpPatch("{todoId}")]
        public IActionResult Patch(Guid todoId, [FromBody] JsonPatchDocument jsonPatchDocument)
        {
            // 更新完整資料用 Put，更新局部資料用 Patch
            if (_todoListService.PatchUpdate(todoId, jsonPatchDocument) == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{todoId}")]
        public IActionResult Delete(Guid todoId)
        {
            if (_todoListService.Delete(todoId) == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
