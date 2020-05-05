using System;
using System.Collections.Generic;

namespace UserAPIServices.EFModels
{
    public partial class User
    {
        public User()
        {
            Access = new HashSet<Access>();
            Website = new HashSet<Website>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool Actived { get; set; }

        public virtual ICollection<Access> Access { get; set; }
        public virtual ICollection<Website> Website { get; set; }
    }
}
