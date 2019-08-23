using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tv.api.Common.Models;

namespace tv.api.Sources
{
    public interface ISource
    {
        TvData GetChannels(string pData = "");
        TvData GetShows(string pData = "");
        TvData GetEpisodes(string pData = "");
        TvPlayData GetPlayData(string pData = "");
    }
}
