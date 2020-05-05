using System;
using System.Collections.Generic;

namespace UserAPIServices.EFModels
{
    public partial class Organization
    {
        public Organization()
        {
            Access = new HashSet<Access>();
            Website = new HashSet<Website>();
        }

        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public bool Removed { get; set; }
        public long CreatedAt { get; set; }

        public virtual ICollection<Access> Access { get; set; }
        public virtual ICollection<Website> Website { get; set; }
    }
}
