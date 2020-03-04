using System;
using System.Collections.Generic;

namespace HeatMapAPIServices.EFModels
{
    public partial class Website
    {
        public Website()
        {
            DataStore = new HashSet<DataStore>();
        }

        public int WebId { get; set; }
        public int CustomerId { get; set; }
        public string WebUrl { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<DataStore> DataStore { get; set; }
    }
}
