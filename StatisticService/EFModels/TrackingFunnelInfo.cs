﻿using System;
using System.Collections.Generic;

namespace StatisticService.EFModels
{
    public partial class TrackingFunnelInfo
    {
        public int TrackingFunnelInfoId { get; set; }
        public int WebId { get; set; }
        public string Name { get; set; }
        public string Steps { get; set; }
        public bool Removed { get; set; }
        public long CreatedAt { get; set; }
        public int AuthorId { get; set; }

        public virtual Website Web { get; set; }
    }
}
