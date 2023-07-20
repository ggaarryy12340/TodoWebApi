using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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

        // GET: api/<TodoController>
        [HttpGet]
        public IActionResult Get([FromQuery] TodoGetParameters todoGetParameters)
        {
            var todos = _todoContext.TodoLists.AsQueryable();

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

            if (todos is null || todos.Count() <= 0)
            { 
                return NotFound("找不到資源");
            }

            return Ok(todos.ToList());
        }

        // GET: api/<TodoController>
        // 用 AutoMapper 轉成 TodoListSelectDtos 後傳出
        [HttpGet("GetByAutoMapper")]
        public IEnumerable<TodoListSelectDto> GetByAutoMapper([FromQuery] TodoGetParameters todoGetParameters)
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

        /// <summary>
        /// 透過 SQL 查詢 TodoList 後回傳
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        [HttpGet("GetTodoByRawSQL")]
        public IEnumerable<TodoList> GetTodoByRawSQL([FromQuery] string orders)
        {
            var sql = $"select * from todolist where 1 = 1";

            if (!string.IsNullOrEmpty(orders))
            { 
                sql += " and orders = {0}";
            }

            // 只能查 context 內定義的 dbset
            var todos = _todoContext.TodoLists.FromSqlRaw(sql, orders).ToList();

            // 可以查泛型的結果
            // var orderList = _todoContext.Database.SqlQueryRaw<string>("select name from todolist where orders = {0}", orders).ToList();

            return todos;
        }

        // GET api/<TodoController>/id
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var todo = _todoContext.TodoLists.Find(id);

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
            var insertData = new TodoList()
            {
                Name = model.Name,
                Enable = model.Enable,
                Orders = model.Orders,
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UploadFiles = model.UploadFiles
            };

            _todoContext.TodoLists.Add(insertData);
            _todoContext.SaveChanges();

            return NoContent();
        }

        [HttpPost("PostBySQL")]
        public IActionResult PostBySQL([FromBody] TodoList model)
        {
            var sql = @"INSERT INTO [dbo].[TodoList]([Name], [InsertTime], [UpdateTime], [Enable], [Orders], [InsertEmployeeId], [UpdateEmployeeId])
                    VALUES(@Name, @InsertTime, @UpdateTime, @Enable, @Orders, @InsertEmployeeId, @UpdateEmployeeId)";

            var pamameters = new List<SqlParameter>();
            pamameters.Add(new SqlParameter("name", model.Name));
            pamameters.Add(new SqlParameter("InsertTime", DateTime.Now));
            pamameters.Add(new SqlParameter("UpdateTime", DateTime.Now));
            pamameters.Add(new SqlParameter("Enable", model.Enable));
            pamameters.Add(new SqlParameter("Orders", model.Orders));
            pamameters.Add(new SqlParameter("InsertEmployeeId", Guid.Parse("00000000-0000-0000-0000-000000000001")));
            pamameters.Add(new SqlParameter("UpdateEmployeeId", Guid.Parse("00000000-0000-0000-0000-000000000001")));

            _todoContext.Database.ExecuteSqlRaw(sql, pamameters.ToArray());

            return NoContent();
        }

        // PUT api/<TodoController>/5
        [HttpPut("{todoId}")]
        public IActionResult Put(Guid todoId, [FromBody] TodoList model)
        {
            // 更新方法 1:
            //_todoContext.Entry(model).State = EntityState.Modified;
            //_todoContext.SaveChanges();

            // 更新方法 2: 
            //_todoContext.Update(model);
            //_todoContext.SaveChanges();

            // 更新方法 3: 
            var todoList = _todoContext.TodoLists.Find(todoId);

            if (todoList == null)
            {
                return NotFound("找不到資源");
            }

            todoList.Name = model.Name;
            todoList.Enable = model.Enable;
            todoList.Orders = model.Orders;

            todoList.UpdateTime = DateTime.Now;
            todoList.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            _todoContext.SaveChanges();

            return NoContent();
        }

        // PATCH api/<TodoController>/5
        [HttpPatch("{todoId}")]
        public IActionResult Patch(Guid todoId, [FromBody] JsonPatchDocument jsonPatchDocument)
        {
            // 更新完整資料用 Put，更新局部資料用 Patch
            var todoList = _todoContext.TodoLists.Find(todoId);

            if (todoList == null)
            {
                return NotFound("找不到資源");
            }

            todoList.UpdateTime = DateTime.Now;
            todoList.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            jsonPatchDocument.ApplyTo(todoList);
            _todoContext.SaveChanges();

            return NoContent();
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
