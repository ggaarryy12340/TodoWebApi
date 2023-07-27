using Todo.Models;
using System.Linq;
using System.Collections.Generic;
using Todo.Parameters;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Todo.Dtos;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.JsonPatch;

namespace Todo.Services
{
    public class TodoListService
    {
        private readonly TodoContext _todoContext;
        private readonly IMapper _mapper;
        public TodoListService(TodoContext todoContext, IMapper mapper)
        { 
            _todoContext = todoContext;
            _mapper = mapper;
        }

        public List<TodoList> GetAll(TodoGetParameters todoGetParameters)
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

            return todos.ToList();
        }

        public List<TodoListSelectDto> GetByAutoMapper(TodoGetParameters todoGetParameters)
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

            return _mapper.Map<List<TodoListSelectDto>>(todos);
        }

        public List<TodoList> GetByRawSQL(string orders)
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

        public TodoList GetSingle(Guid id)
        {
            return _todoContext.TodoLists.Find(id);
        }

        public TodoList Insert(TodoList model)
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

            return insertData;
        }

        public void InsertBySQL(TodoListPostDto model)
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
        }

        public int PutUpdate(Guid todoId, TodoList model)
        {
            // 更新方法 1:
            //_todoContext.Entry(model).State = EntityState.Modified;
            //_todoContext.SaveChanges();

            // 更新方法 2: 
            //_todoContext.Update(model);
            //_todoContext.SaveChanges();

            // 更新方法 3: 
            var rs = 0;
            var todoList = _todoContext.TodoLists.Find(todoId);

            if (todoList != null)
            {
                todoList.Name = model.Name;
                todoList.Enable = model.Enable;
                todoList.Orders = model.Orders;

                todoList.UpdateTime = DateTime.Now;
                todoList.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                rs = _todoContext.SaveChanges();
            }

            return rs;
        }

        public int PatchUpdate(Guid todoId, JsonPatchDocument jsonPatchDocument)
        {
            var rs = 0;
            var todoList = _todoContext.TodoLists.Find(todoId);

            if (todoList != null)
            {
                todoList.UpdateTime = DateTime.Now;
                todoList.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                jsonPatchDocument.ApplyTo(todoList);
                rs = _todoContext.SaveChanges();
            }

            return rs;
        }

        public int Delete(Guid todoId)
        {
            var rs = 0;
            var deleteTodo = _todoContext.TodoLists.Find(todoId);

            if (deleteTodo != null)
            {
                var deleteUploadFiles = _todoContext.UploadFiles.Where(x => x.TodoId == todoId);

                _todoContext.UploadFiles.RemoveRange(deleteUploadFiles);
                _todoContext.TodoLists.Remove(deleteTodo);

                rs = _todoContext.SaveChanges();
            }

            return rs;
        }
    }
}
