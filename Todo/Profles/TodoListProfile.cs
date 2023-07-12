using AutoMapper;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Profles
{
    public class TodoListProfile: Profile
    {
        public TodoListProfile()
        {
            CreateMap<TodoList, TodoListSelectDto>()
                .ForMember(a => a.InsertEmployeeName, b => b.MapFrom(c => c.InsertEmployee.Name))
                .ForMember(a => a.UpdateEmployeeName, b => b.MapFrom(c => c.UpdateEmployee.Name));
        }
    }
}
