﻿using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class Customer
    {
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}