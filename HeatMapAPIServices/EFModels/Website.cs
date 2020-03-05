using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class Website
    {
        public Website()
        {
            TrackingInfor = new HashSet<TrackingInfor>();
        }

        public int WebId { get; set; }
        public int WebOwnerId { get; set; }
        public string WebUrl { get; set; }
        public bool IsRemoved { get; set; }

        public virtual WebOwner WebOwner { get; set; }
        public virtual ICollection<TrackingInfor> TrackingInfor { get; set; }
    }
}
