using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class Customer
    {
        public Customer()
        {
            Website = new HashSet<Website>();
        }

        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Website> Website { get; set; }
    }
}
