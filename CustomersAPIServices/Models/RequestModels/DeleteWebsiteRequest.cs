﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.RequestModels
{
    public class DeleteWebsiteRequest
    {
        public int customerId { get; set; }
        public int webId { get; set; }
    }
}