using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using Todo.Dtos;

namespace Todo.Filters
{
    public class FileLimitAttribute : Attribute, IResourceFilter
    {
        public string ExtentionLimit = ".txt";
        public int SizeLimit = 1;
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var files = context.HttpContext.Request.Form.Files;

            foreach (var file in files)
            {
                // 檔案大小大於 {SizeLimit} MB
                if (file.Length > (1024 * 1024 * SizeLimit))
                {
                    context.Result = new JsonResult(new ReturnJson()
                    {
                        Data = "Test1",
                        HttpCode = 500,
                        ErrorMessage = $"檔案大小大於 {SizeLimit}MB!"
                    });
                }

                if (Path.GetExtension(file.FileName) != ExtentionLimit)
                {
                    context.Result = new JsonResult(new ReturnJson()
                    {
                        Data = "Test1",
                        HttpCode = 500,
                        ErrorMessage = $"附檔名必須是 {ExtentionLimit} !"
                    });
                }
            }
        }
    }
}
