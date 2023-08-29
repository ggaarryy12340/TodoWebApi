using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Todo.Models;

namespace Todo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsyncController : ControllerBase
    {
        // async 方法內一定要有 await，並回傳 Task<T>
        // 進入 async 方法後不會馬上變成兩條 thread，是碰到 await 才會
        // 主 thread 遇到 await 會立即回到呼叫程式，支線 thread 背景執行 await 的內容
        // 如果 await 是有回傳值的 async function，就會等到有結果才往下
        // 能用 await 就以 await 為主，避免使用 Task.Result 有機率 DeadLock
        // 若 async 方法沒有返回值，建議回傳 Task 而不是 void
        // 只要 function 內部有用到一個以上 async 方法，這個 function 就必須是 async 方法
        // 如果要在同步方法中呼叫非同步方法，可使用 Result() 或在沒有回傳時呼叫 Wait()，但有 deadlock 的風險，所以盡量不要
        // await 調用的等待期間，會把當下的 thread 還到 threadpool。待非同步方法執行完畢後，會再從 threadpool 中取出一個 thread 執行後續程式
        // 若非同步方法是回傳 Task<T>，且沒有使用 await。可以考慮不要加 async 關鍵字，避免"拆了再裝"。
        // 非同步程式以 await Task.Delay() 取代 Thread.Sleep()。因為 Thread.Sleep 會阻塞主線程。
        private readonly TodoContext _todoContext;

        public AsyncController(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Log($"Get Start: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");
            var rs = Execute();
            Log($"Get End: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");
            return Ok(rs);
        }

        public int Execute()
        {
            Log($"Execute Start: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");
            var rs3 = Job3Async();
            var rs2 = Job2Async();
            var rs1 = Job1Async();

            var result = rs1.Result + rs2.Result + rs3.Result;
            Log($"Execute End: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");
            return result;
        }

        [HttpGet("Async")]
        public async Task<IActionResult> GetAsync()
        {
            Log($"GetAsync Start: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");
            var rs = await ExecuteAsync();
            Log($"GetAsync End: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");
            return Ok(rs);
        }

        public async Task<int> ExecuteAsync()
        {
            Log($"ExecuteAsync Start: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");
            var rs3 = Job3Async();
            var rs2 = Job2Async();
            var rs1 = Job1Async();
            Log($"ExecuteAsync End: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");

            return await rs1 + await rs2 + await rs3;
        }

        [HttpGet("SQLTest")]
        public async Task<IActionResult> SQLTest()
        {
            Log($"SQLTest Start: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");

            var Task3 = GetByRawSQL3Second("6");
            var Task5 = GetByRawSQL5Second("6");

            Log($"SQLTest End: TreadId: {System.Environment.CurrentManagedThreadId}\r\n");

            var rs3 = await Task3;
            var rs5 = await Task5;

            return Ok(rs3);
        }

        public async Task<int> Job1Async()
        {
            Log($"Job1 Start: TreadId: {System.Environment.CurrentManagedThreadId} \r\n");
            await Task.Delay(1000);
            Log($"Job1 End: TreadId: {System.Environment.CurrentManagedThreadId} \r\n");
            return 1;
        }

        public async Task<int> Job2Async()
        {
            Log($"Job2 Start: TreadId: {System.Environment.CurrentManagedThreadId} \r\n");
            await Task.Delay(2000);
            Log($"Job2 End: TreadId: {System.Environment.CurrentManagedThreadId} \r\n");
            return 2;
        }

        public async Task<int> Job3Async()
        {
            Log($"Job3 Start: TreadId: {System.Environment.CurrentManagedThreadId} \r\n");
            await Task.Delay(3000);
            Log($"Job3 End: TreadId: {System.Environment.CurrentManagedThreadId} \r\n");
            return 3;
        }

        public async Task<List<TodoList>> GetByRawSQL3Second(string orders)
        {
            var sql = $"select * from todolist where 1 = 1";

            if (!string.IsNullOrEmpty(orders))
            {
                sql += " and orders = {0}";
            }

            sql += " WAITFOR DELAY '00:00:03'";

            // 只能查 context 內定義的 dbset
            var todos = _todoContext.TodoLists.FromSqlRaw(sql, orders).ToListAsync();

            // 可以查泛型的結果
            // var orderList = _todoContext.Database.SqlQueryRaw<string>("select name from todolist where orders = {0}", orders).ToList();

            return await todos;
        }

        public async Task<List<TodoList>> GetByRawSQL5Second(string orders)
        {
            var sql = $"select * from todolist where 1 = 1";

            if (!string.IsNullOrEmpty(orders))
            {
                sql += " and orders = {0}";
            }

            sql += " WAITFOR DELAY '00:00:05'";

            // 只能查 context 內定義的 dbset
            var todos = _todoContext.TodoLists.FromSqlRaw(sql, orders).ToListAsync();

            // 可以查泛型的結果
            // var orderList = _todoContext.Database.SqlQueryRaw<string>("select name from todolist where orders = {0}", orders).ToList();

            return  await todos;
        }

        public void Log(string msg)
        {
            using (FileStream fileStream = new FileStream(@"C:\\Test\\ntfs8.txt", FileMode.Append, FileAccess.Write))
            {
                // 创建 BinaryWriter 以便将数据写入 FileStream
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    // 将日志信息写入二进制文件
                    writer.Write(msg);
                }
            }
        }
    }
}
