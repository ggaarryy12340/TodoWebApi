using System;
using Todo.ValidationAttributes;

namespace Todo.Dtos
{
    [StartEndTimeCheck]
    public class TodoListPostDto
    {
        [TodoNameCheck]
        [CustomErrMsg(true, "客製自訂模型驗證的錯誤訊息!")]
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
