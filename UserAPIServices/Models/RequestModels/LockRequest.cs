﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models
{
    public class LockRequest
    {
        public int ID { get; set; }
        public bool locked { get; set; }
    }
}
