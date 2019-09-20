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
                    ImgSrc = "kr",
                    Link="s/l=kr&p=1"
                },
                new TvDataItem
                {
                    Name ="Japanese",
                    ImgSrc = "jp",
                    Link="s/l=jp&p=1"
                },
                new TvDataItem
                {
                    Name ="Chinese",
                    ImgSrc = "cn",
                    Link="s/l=cn&p=1"
                },
                new TvDataItem
                {
                    Name ="Taiwan",
                    ImgSrc = "tw",
                    Link="s/l=tw&p=1"
                }
                ,
                new TvDataItem
                {
                    Name ="India",
                    ImgSrc = "in",
                    Link="s/l=in&p=1"
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
                var lang = int.Parse(QueryHelpers.ParseQuery(query)["l"]);
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
                                Link = item["id"].ToObject<string>()
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
                                Name = "Episode " + item["number"].ToObject<string>(),
                                Link = item["id"].ToObject<string>()
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
                            Type = "application/dash+xml",
                            Link = jsonData["streams"]["mpd"]["http"]["url"].ToObject<string>()
                        },
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
