﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class User
    {
        public string UserId
        {
            get;


            set;

        }

        public string Password
        {
            get;

            set;

        }

        public RoleEnum Role
        {
            get;


            set;

        }
    }
}
