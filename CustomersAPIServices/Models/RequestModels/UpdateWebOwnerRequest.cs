﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class UpdateUserRequest
    {
        public int userID { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
    }
}