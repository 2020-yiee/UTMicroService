using System;
using System.Collections.Generic;

namespace CustomersAPIServices.EFModels
{
    public partial class WebOwner
    {
        public WebOwner()
        {
            Website = new HashSet<Website>();
        }

        public int WebOwnerId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsRemoved { get; set; }

        public virtual ICollection<Website> Website { get; set; }
    }
}
