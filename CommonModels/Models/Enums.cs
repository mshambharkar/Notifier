﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public enum RoleEnum
    {
        A,//Admin
        U//User
    }
    public enum NotificationRegion
    {
        None,
        TaskAssigned,
        TaskCompleted,
        TaskDue
    }
}
