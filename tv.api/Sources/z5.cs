using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using tv.api.Common.Constants;
using tv.api.Common.Models;

namespace tv.api.Sources
{
    public class z5 : ISource
    {
        private readonly string collectionId;

        public z5()
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.REGIONAL_DETAILS);

                dynamic jsonData = JArray.Parse(rawJson);

                collectionId = jsonData[0].collections.web_app.tvshows;
            }
        }

        public TvData GetChannels(string pData = "")
        {
            var shows = new TvDataItem[] { new TvDataItem
                {
                    Name ="Marathi",
                    Link = "mr"
                }
            };

            return new TvData
            {
                Page = 1,
                ItemsPerPage = 1,
                TotalItems = 1,
                Items = shows
            };
        }

        public TvData GetShows(string pData = "asset_subtype=tvshow&languages=mr&page=1&page_size=9")
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.ApiListShows(pData));

                JObject jsonData = JObject.Parse(rawJson);

                var shows = from item in jsonData["items"]
                            select new TvDataItem
                            {
                                Name = item["title"].ToObject<string>(),
                                Link = item["id"].ToObject<string>()
                            };

                return new TvData
                {
                    Page = jsonData["page"].ToObject<int>(),
                    ItemsPerPage = jsonData["page_size"].ToObject<int>(),
                    TotalItems = jsonData["total"].ToObject<int>(),
                    Items = shows
                };
            }
        }

        public TvData GetEpisodes(string pData = "page=1&page_size=9&asset_subtype=episode")
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.ApiShowDetails(pData));

                JObject showDetails = JObject.Parse(rawJson);

                var title = showDetails["title"].ToObject<string>();
                var seasons = showDetails["seasons"].ToArray();

                var latestSeasonId = seasons[seasons.Length - 1]["id"].ToObject<string>();

                rawJson = client.DownloadString(Z5api.ApiEpisodesForSeason(latestSeasonId, pData));

                JObject jsonData = JObject.Parse(rawJson);

                var shows = from item in jsonData["episodes"]
                            select new TvDataItem
                            {
                                Name = title,
                                Link = item["id"].ToObject<string>()
                            };

                return new TvData
                {
                    Page = jsonData["page"].ToObject<int>(),
                    ItemsPerPage = jsonData["page_size"].ToObject<int>(),
                    TotalItems = jsonData["total"].ToObject<int>(),
                    Items = shows
                };
            }
        }

        public TvPlayData GetPlayData(string pData = "")
        {
            using (WebClient client = new WebClient())
            {   
                var rawJson = client.DownloadString(Z5api.ApiEpisodeById(pData));

                JObject jsonData = JObject.Parse(rawJson);

                var title = jsonData["title"].ToObject<string>();
                var image = jsonData["list_image"].ToObject<string>();

                var dashUrl = jsonData["video"]["url"].ToObject<string>();
                var hlsUrl = jsonData["video"]["url"].ToObject<string>();
                var isDrm = jsonData["video"]["is_drm"].ToObject<string>();
                var drmKey = jsonData["video"]["drm_key_id"].ToObject<string>();

                var playData = new TvPlayData
                {
                    Name = title,
                    ImgSrc = drmKey, //passing drm key here
                    Links = new List<StreamLink>
                    {
                        new StreamLink
                        {
                            Type = "application/dash+xml",
                            Link = dashUrl                            
                        },
                        new StreamLink
                        {
                            Type = "application/x-mpegURL",
                            Link = hlsUrl
                        }
                    },
                };

                return playData;
            }
        }


    }
}
