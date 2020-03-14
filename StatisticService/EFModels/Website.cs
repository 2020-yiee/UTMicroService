using System;
using System.Collections.Generic;

namespace StatisticService.EFModels
{
    public partial class Website
    {
        public int WebId { get; set; }
        public string DomainUrl { get; set; }
        public bool Removed { get; set; }
        public int OrganizationId { get; set; }
        public bool Verified { get; set; }
    }
}
