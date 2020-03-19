using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomersAPIServices.Models.ResponseModels
{
    public class MemberResponse
    {
        public int userID { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public int role { get; set; }
        public long? dayJoin { get; set; }

        public MemberResponse(int userID, string email, string fullName, int role, long? dayJoin)
        {
            this.userID = userID;
            this.email = email;
            this.fullName = fullName;
            this.role = role;
            this.dayJoin = dayJoin;
        }
    }
}
