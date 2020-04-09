using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPIServices.Models.RequestModels
{
    public class OrganizationRequest
    {
        public string organirzationName { get; set; }

        public OrganizationRequest(string organirzationName)
        {
            this.organirzationName = organirzationName;
        }
    }
}
