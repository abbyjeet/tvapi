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
    public class Z5 : ISource
    {
        private readonly string collectionId;

        public Z5()
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.REGIONAL_DETAILS);

                dynamic jsonData = JArray.Parse(rawJson);

                collectionId = jsonData[0].collections.web_app.tvshows;
            }
        }

        public TvData GetChannels(string query = "")
        {
            var shows = new TvDataItem[] {
                new TvDataItem
                {
                    Name ="Marathi",
                    Link = "mr"
                },
                new TvDataItem
                {
                    Name ="Hindi",
                    Link = "hi"
                },
                new TvDataItem
                {
                    Name ="English",
                    Link = "en"
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

        public TvData GetShows(string query = "asset_subtype=tvshow&languages=mr&page=1&page_size=9")
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.ApiListShows(query));

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

        public TvData GetEpisodes(string query = "page=1&page_size=9&asset_subtype=episode")
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.ApiShowDetails(query));

                JObject showDetails = JObject.Parse(rawJson);

                var title = showDetails["title"].ToObject<string>();
                var seasons = showDetails["seasons"].ToArray();

                var latestSeasonId = seasons[seasons.Length - 1]["id"].ToObject<string>();

                rawJson = client.DownloadString(Z5api.ApiEpisodesForSeason(latestSeasonId, query));

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

        public TvPlayData GetPlayData(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.ApiEpisodeById(query));

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

        public TvData GetSource(string query = "")
        {
            return new TvData
            {
                Page = 0,
                ItemsPerPage = 0,
                TotalItems = 0,
                Items = new[]
                {
                    new TvDataItem
                    {
                        Name = "Not Applicable"
                    }
                }
            };
        }
    }
}
