using System;

namespace Todo.Dtos
{
    public class UploadFileDto
    {
        public Guid UploadFileId { get; set; }
        public string Name { get; set; }
        public string Src { get; set; }
        public Guid TodoId { get; set; }
    }
}
