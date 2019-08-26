using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tv.api.Common.Models;

namespace tv.api.Sources
{
    public interface ISource
    {
        TvData GetChannels(string query = "");
        TvData GetSource(string query = "");
        TvData GetShows(string query = "");
        TvData GetEpisodes(string query = "");
        TvPlayData GetPlayData(string query = "");
    }
}
