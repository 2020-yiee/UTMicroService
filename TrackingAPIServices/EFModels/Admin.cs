﻿using System;
using System.Collections.Generic;

namespace TrackingAPIServices.EFModels
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Actived { get; set; }
    }
}