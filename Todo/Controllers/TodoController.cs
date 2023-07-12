using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Dtos;
using Todo.Models;
using Todo.Parameters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _iMapper;

        public TodoController(TodoContext todoContext, IMapper iMapper)
        { 
            _todoContext = todoContext;
            _iMapper = iMapper;
        }

        //// GET: api/<TodoController>
        //[HttpGet]
        //[HttpGet]
        //public IActionResult Get([FromQuery] TodoGetParameters todoGetParameters)
        //{
        //    var todos = _todoContext.TodoLists.AsQueryable();

        //    if (!string.IsNullOrEmpty(todoGetParameters.name))
        //    {
        //        todos = todos.Where(x => x.Name.Contains(todoGetParameters.name));
        //    }

        //    if (todoGetParameters.enable != null)
        //    {
        //        todos = todos.Where(x => x.Enable == todoGetParameters.enable);
        //    }

        //    if (todoGetParameters.minOrder != null && todoGetParameters.maxOrder != null)
        //    {
        //        todos = todos.Where(x => x.Orders >= todoGetParameters.minOrder && x.Orders <= todoGetParameters.maxOrder);
        //    }

        //    return Ok(todos.ToList());
        //}

        // GET: api/<TodoController>
        // 用 AutoMapper 轉成 TodoListSelectDtos 後傳出
        [HttpGet]
        public IEnumerable<TodoListSelectDto> Get([FromQuery] TodoGetParameters todoGetParameters)
        {
            var todos = _todoContext.TodoLists
                .Include(x => x.InsertEmployee)
                .Include(x => x.UpdateEmployee)
                .Include(x => x.UploadFiles).AsQueryable();

            if (!string.IsNullOrEmpty(todoGetParameters.name))
            {
                todos = todos.Where(x => x.Name.Contains(todoGetParameters.name));
            }

            if (todoGetParameters.enable != null)
            {
                todos = todos.Where(x => x.Enable == todoGetParameters.enable);
            }

            if (todoGetParameters.minOrder != null && todoGetParameters.maxOrder != null)
            {
                todos = todos.Where(x => x.Orders >= todoGetParameters.minOrder && x.Orders <= todoGetParameters.maxOrder);
            }

            return _iMapper.Map<IEnumerable<TodoListSelectDto>>(todos);
        }

        // GET api/<TodoController>/id
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(_todoContext.TodoLists.Find(id));
        }

        // POST api/<TodoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
