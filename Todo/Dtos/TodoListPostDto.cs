using System;
using Todo.ValidationAttributes;

namespace Todo.Dtos
{
    [StartEndTimeCheck]
    public class TodoListPostDto
    {
        [TodoNameCheck]
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
