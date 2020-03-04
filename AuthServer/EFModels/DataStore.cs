using System;
using System.Collections.Generic;

namespace AuthServer.EFModels
{
    public partial class DataStore
    {
        public int Id { get; set; }
        public int? WebId { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
    }
}
