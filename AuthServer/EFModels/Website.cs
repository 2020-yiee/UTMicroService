using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class Website
    {
        public int WebId { get; set; }
        public int CustomerId { get; set; }
        public string WebUrl { get; set; }
    }
}
