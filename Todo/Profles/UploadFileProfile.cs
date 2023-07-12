using AutoMapper;
using Todo.Dtos;
using Todo.Models;

namespace Todo.Profles
{
    public class UploadFileProfile: Profile
    {
        public UploadFileProfile()
        {
            CreateMap<UploadFile, UploadFileDto>();
        }
    }
}
