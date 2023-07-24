using Todo.ValidationAttributes;

namespace Todo.Dtos
{
    public class TodoListPostDto
    {
        [TodoNameCheck]
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
    }
}
