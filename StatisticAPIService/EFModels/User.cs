﻿using System;
using System.Collections.Generic;

namespace StatisticAPIService.EFModels
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool Actived { get; set; }
    }
}