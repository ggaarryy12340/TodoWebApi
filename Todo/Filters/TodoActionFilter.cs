using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;

namespace Todo.Filters
{
    public class TodoActionFilter : IActionFilter
    {
        private readonly IWebHostEnvironment _env;

        public TodoActionFilter(IWebHostEnvironment env)
        { 
            _env = env;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var rootPath = _env.ContentRootPath + "\\Log\\";

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var employeeId = context.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;
            string text = $"結束: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}, path: {path}, method:{method}, employeeId:{employeeId}\n";
            File.AppendAllText(rootPath + DateTime.Now.ToString("yyyyMMdd") + ".txt", text);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var rootPath = _env.ContentRootPath + "\\Log\\";

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            var employeeId = context.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;
            string text = $"開始: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}, path: {path}, method:{method}, employeeId:{employeeId}\n";
            File.AppendAllText(rootPath + DateTime.Now.ToString("yyyyMMdd") + ".txt", text);
        }
    }
}
