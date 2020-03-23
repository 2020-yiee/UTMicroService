﻿using System;
using System.Collections.Generic;

namespace TrackingAPIServices.EFModels
{
    public partial class Website
    {
        public int WebId { get; set; }
        public string DomainUrl { get; set; }
        public bool Removed { get; set; }
        public int OrganizationId { get; set; }
        public bool Verified { get; set; }
        public long CreatedAt { get; set; }
        public int AuthorId { get; set; }
    }
}