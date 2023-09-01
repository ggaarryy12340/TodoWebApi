using System;
using System.Collections.Generic;

namespace Todo.Models;

public partial class Role
{
    public Guid RoldId { get; set; }

    public string Name { get; set; }

    public Guid? EmployeeId { get; set; }

    public virtual Employee Employee { get; set; }
}
