using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tv.api.Common.Models;

namespace tv.api.Sources
{
    public class vk : ISource
    {
        public IEnumerable<TvData> GetChannels()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TvData> GetEpisodes(string targetUrl = "")
        {
            throw new NotImplementedException();
        }

        public TvPlayData GetPlayData(string targetUrl = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TvData> GetShows(string targetUrl = "")
        {
            throw new NotImplementedException();
        }
    }
}
