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
                    Link = "kr"
                },
                new TvDataItem
                {
                    Name ="Japanese",
                    Link = "jp"
                },
                new TvDataItem
                {
                    Name ="Chinese",
                    Link = "cn"
                },
                new TvDataItem
                {
                    Name ="Taiwan",
                    Link = "tw"
                }
                ,
                new TvDataItem
                {
                    Name ="India",
                    Link = "in"
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

        public TvData GetShows(string query = "origin_country=kr&page=1")
        {
            using (WebClient client = new WebClient())
            {
                var getQuery = $"{VKapi.ApiListShows(query)}&sort=views_recent&per_page={Misc.PAGESIZE}&with_paging=true";

                var rawJson = client.DownloadString(getQuery);

                JObject jsonData = JObject.Parse(rawJson);

                var currentPage = int.Parse(QueryHelpers.ParseQuery(query)["page"]);
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
                    Page = currentPage,
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
                var currentPage = int.Parse(QueryHelpers.ParseQuery(query)["page"]);

                var getQuery = VKapi.ApiEpisodesForSeason(seriesId, $"sort=number&direction=asc&page={currentPage}&per_page={Misc.PAGESIZE}&with_paging=true");

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
                    Page = currentPage,
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
    }
}
