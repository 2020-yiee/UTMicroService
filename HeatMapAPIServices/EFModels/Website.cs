using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class Website
    {
        public int WebId { get; set; }
        public int UserId { get; set; }
        public string DomainUrl { get; set; }
        public bool Removed { get; set; }
    }
}
