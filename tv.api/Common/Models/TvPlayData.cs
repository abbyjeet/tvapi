using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tv.api.Common.Models
{
    public class TvPlayData
    {
        public string Name { get; set; }
        public IList<StreamLink> Links { get; set; }
        public string ImgSrc { get; set; }

    }
}
