﻿using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class Access
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int Role { get; set; }
    }
}