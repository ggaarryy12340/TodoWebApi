﻿namespace Todo.Parameters
{
    public class TodoGetParameters
    {
        public string name { get; set; }
        public bool? enable { get; set; }
        public int? minOrder { get; set; }
        public int? maxOrder { get; set; }
    }
}
