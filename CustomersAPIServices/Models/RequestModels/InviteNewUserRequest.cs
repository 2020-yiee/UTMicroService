﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class InviteNewUserRequest
    {
        public string fullName { get; set; }
        public string password { get; set; }
    }
}
