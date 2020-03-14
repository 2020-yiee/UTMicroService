using System;
using System.Collections.Generic;

namespace StatisticService.EFModels
{
    public partial class Organization
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public bool Removed { get; set; }
    }
}
