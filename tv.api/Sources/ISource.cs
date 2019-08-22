using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tv.api.Common.Models;

namespace tv.api.Sources
{
    public interface ISource
    {
        IEnumerable<TvData> GetChannels();
        IEnumerable<TvData> GetShows(string targetUrl = "");
        IEnumerable<TvData> GetEpisodes(string targetUrl = "");
        TvPlayData GetPlayData(string targetUrl = "");
    }
}
