using Microsoft.AspNetCore.WebUtilities;
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
    public class VK : ISource
    {
        public TvData GetChannels(string query = "")
        {
            // Link = origin_country
            var shows = new TvDataItem[] {
                new TvDataItem
                {
                    Name ="Korean",
                    ImgSrc = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/Flag_of_South_Korea.svg/320px-Flag_of_South_Korea.svg.png",
                    Link="vk/s/l=kr&p=1"
                },
                new TvDataItem
                {
                    Name ="Japanese",
                    ImgSrc = "https://upload.wikimedia.org/wikipedia/en/thumb/9/9e/Flag_of_Japan.svg/320px-Flag_of_Japan.svg.png",
                    Link="vk/s/l=jp&p=1"
                },
                new TvDataItem
                {
                    Name ="Chinese",
                    ImgSrc = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fa/Flag_of_the_People%27s_Republic_of_China.svg/320px-Flag_of_the_People%27s_Republic_of_China.svg.png",
                    Link="vk/s/l=cn&p=1"
                },
                new TvDataItem
                {
                    Name ="Taiwan",
                    ImgSrc = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/72/Flag_of_the_Republic_of_China.svg/320px-Flag_of_the_Republic_of_China.svg.png",
                    Link="vk/s/l=tw&p=1"
                }
                ,
                new TvDataItem
                {
                    Name ="India",
                    ImgSrc = "https://upload.wikimedia.org/wikipedia/en/thumb/4/41/Flag_of_India.svg/320px-Flag_of_India.svg.png",
                    Link="vk/s/l=in&p=1"
                }
            };

            return new TvData
            {
                Page = 1,
                ItemsPerPage = 9,
                TotalItems = 5,
                Items = shows
            };
        }

        public TvData GetShows(string query = "l=kr&p=1")
        {
            using (WebClient client = new WebClient())
            {
                var lang = QueryHelpers.ParseQuery(query)["l"].ToString();
                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);
                var param = $"origin_country={lang}&page={page}";

                var getQuery = $"{VKapi.ApiListShows(param)}&sort=views_recent&per_page={Misc.PAGESIZE}&with_paging=true";

                var rawJson = client.DownloadString(getQuery);

                JObject jsonData = JObject.Parse(rawJson);

                
                var totalItems = jsonData["count"].ToObject<int>();
                //var totalPages = totalItems / 9;

                var shows = from item in jsonData["response"]
                            select new TvDataItem
                            {
                                Name = item["titles"]["en"].ToObject<string>(),
                                ImgSrc = item["images"]["poster"]["url"]?.ToObject<string>(),
                                Link = $"vk/e/{item["id"].ToObject<string>()}&p=1"
                            };

                return new TvData
                {
                    Page = page,
                    ItemsPerPage = Misc.PAGESIZE,
                    TotalItems = totalItems,
                    Items = shows
                };
            }
        }

        public TvData GetEpisodes(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var seriesId = QueryHelpers.ParseQuery(query).Keys.First();
                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);

                var getQuery = VKapi.ApiEpisodesForSeason(seriesId, $"sort=number&direction=asc&page={page}&per_page={Misc.PAGESIZE}&with_paging=true");

                var rawJson = client.DownloadString(getQuery);

                JObject jsonData = JObject.Parse(rawJson);
                var totalItems = jsonData["count"].ToObject<int>();

                var shows = from item in jsonData["response"]
                            select new TvDataItem
                            {
                                Name = "Episode " + item["number"]?.ToObject<string>(),
                                ImgSrc = item["images"]["poster"]["url"]?.ToObject<string>(),
                                Link = "vk/p/" + item["id"].ToObject<string>()
                            };

                return new TvData
                {
                    Page = page,
                    ItemsPerPage = Misc.PAGESIZE,
                    TotalItems = totalItems,
                    Items = shows
                };
            }
        }

        public TvPlayData GetPlayData(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var getQuery = VKapi.ApiEpisodeById(query);

                var rawJson = client.DownloadString(getQuery);

                JObject jsonData = JObject.Parse(rawJson);

                var playData = new TvPlayData
                {
                    Name = "",
                    ImgSrc = VKapi.ApiEpisodeSubtitleById(query), //passing subtitle url here
                    Links = new List<StreamLink>
                    {
                        new StreamLink
                        {
                            Type = "application/x-mpegURL",
                            Link = jsonData["streams"]["480p"]["https"]["url"].ToObject<string>()
                        },                        
                        new StreamLink
                        {
                            Type = "application/x-mpegURL",
                            Link = jsonData["streams"]["360p"]["https"]["url"].ToObject<string>()
                        },
                        new StreamLink
                        {
                            Type = "application/x-mpegURL",
                            Link = jsonData["streams"]["240p"]["https"]["url"].ToObject<string>()
                        },
                        new StreamLink
                        {
                            Type = "application/dash+xml",
                            Link = jsonData["streams"]["mpd"]["http"]["url"].ToObject<string>()
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
