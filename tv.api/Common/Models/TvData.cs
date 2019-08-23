using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tv.api.Common.Models
{
    public class TvData
    {
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<TvDataItem> Items { get; set; }
    }
}
