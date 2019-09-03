using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tv.api.Common.Models;

namespace tv.api.Common.Constants
{
    public static class Misc
    {
        public static List<TvDataItem> Sources = new List<TvDataItem>
        {
            new TvDataItem { Name = "DesiTvFlix",Link="df", ImgSrc = "" },
            new TvDataItem { Name = "Zee5",Link="z5", ImgSrc = "" },
            new TvDataItem { Name = "Viki",Link="vk", ImgSrc = "" },
        };

        public const int PAGESIZE = 9;

        public static int CurrentTimeStamp
        {
            get
            {
                return (int)DateTime.UtcNow.Subtract(DateTimeOffset.UnixEpoch.UtcDateTime).TotalSeconds;
            }
        }
    }
}
