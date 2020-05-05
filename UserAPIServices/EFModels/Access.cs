using System;
using System.Collections.Generic;

namespace UserAPIServices.EFModels
{
    public partial class Access
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int Role { get; set; }
        public long DayJoin { get; set; }

        public virtual Organization Organization { get; set; }
        public virtual User User { get; set; }
    }
}
