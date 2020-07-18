using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tv.api.Common.Models;

namespace tv.api.Common.Constants
{
    public static class Misc
    {
        public static TvData Sources = new TvData
        {
            ItemsPerPage = 9,
            Page = 1,
            TotalItems = 3,
            Items = new List<TvDataItem>
            {
                new TvDataItem { Name = "DesiTvFlix",Link="df/&p=1", ImgSrc = "http://desitvflix.com/images/namelogo.png" },
                new TvDataItem { Name = "Zee5",Link="z5", ImgSrc = "https://upload.wikimedia.org/wikipedia/en/thumb/5/5a/Zee5-official-logo.jpeg/600px-Zee5-official-logo.jpeg" },
                new TvDataItem { Name = "Viki",Link="vk", ImgSrc = "https://images-eu.ssl-images-amazon.com/images/I/41fLQDXrS3L.png" },
                //new TvDataItem { Name = "MXPlayer",Link="mx", ImgSrc = "https://j2apps.s.llnwi.net/assets-origin/static/images/logo_main_v1.png" },
            }
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
