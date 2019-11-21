using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        private NameValueCollection GetRequestHeaders()
        {
            using (WebClient client = new WebClient())
            {
                var rawJson = client.DownloadString(Z5api.PLATFORM_TOKEN);
                JObject jsonData = JObject.Parse(rawJson);
                var xAccessToken = jsonData["token"].ToObject<string>();

                return new NameValueCollection
                {
                    //{ "Origin", "https://www.zee5.com" },
                    { "User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.80 Safari/537.36" },
                    //{ "Accept", "*/*" },
                    { "Referer", "https://www.zee5.com" },
                    //{ "Accept-Encoding", "gzip, deflate, br" },
                    //{ "Accept-Language", "en-US,en;q=0.9" },
                    { "Accept", "application/json, text/plain, */*" },
                    //{ "Accept-Language", "en-US,en;q=0.9" },
                    { "X-ACCESS-TOKEN", xAccessToken }
                };
            }
        }

        public TvData GetChannels(string query = "")
        {
            var shows = new TvDataItem[] {
                new TvDataItem
                {
                    Name ="Marathi",
                    ImgSrc="",
                    Link = "z5/s/l=mr&p=1"
                },
                new TvDataItem
                {
                    Name ="Hindi",
                    ImgSrc="",
                    Link = "z5/s/l=hi&p=1"
                },
                new TvDataItem
                {
                    Name ="English",
                    ImgSrc="",
                    Link = "z5/s/l=en&p=1"
                },
                new TvDataItem
                {
                    Name ="Marathi Movies",
                    ImgSrc="",
                    Link = "z5/s/l=mr&p=1&t=m"
                },
                new TvDataItem
                {
                    Name ="Hindi Movies",
                    ImgSrc="",
                    Link = "z5/s/l=hi&p=1&t=m"
                },
                new TvDataItem
                {
                    Name ="English Movies",
                    ImgSrc="",
                    Link = "z5/s/l=en&p=1&t=m"
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

        public TvData GetShows(string query = "l=mr&p=1")
        {
            using (WebClient client = new WebClient())
            {
                //client.Headers.Add(GetRequestHeaders());
                //var test = client.DownloadString("https://gwapi.zee5.com/content/collection/0-8-786?page=1&limit=5&item_limit=20&translation=en&languages=mr");


                var lang = QueryHelpers.ParseQuery(query)["l"].ToString();
                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);
                var ifMovie = query.IndexOf("&t=m") > -1;
                var type = ifMovie ? "movie" : "tvshow";
                var param = $"languages={lang}&page={page}&page_size={Misc.PAGESIZE}";
                var rawJson = client.DownloadString(Z5api.ApiList(param, type));

                string imgUrl(string id, string listImage) => string.IsNullOrWhiteSpace(listImage) ? "" : $"https://akamaividz1.zee5.com/resources/{id}/list/1170x658/{listImage}";

                JObject jsonData = JObject.Parse(rawJson);

                var shows = from item in jsonData["items"]
                            select new TvDataItem
                            {
                                Name = item["title"].ToObject<string>(),
                                Link = $"z5/e/{item["id"].ToObject<string>()}&p=1{(ifMovie ? "&t=m" : "")}",
                                ImgSrc = imgUrl(item["id"].ToObject<string>(), item["list_image"]?.ToObject<string>())
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

        public TvData GetEpisodes(string query = "p=1")
        {
            using (WebClient client = new WebClient())
            {
                //var lang = QueryHelpers.ParseQuery(query)["l"].ToString();
                var showId = QueryHelpers.ParseQuery(query).Keys.First();
                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);
                //var param = $"asset_subtype=episode&page={page}&page_size={Misc.PAGESIZE}";

                var ifMovie = query.IndexOf("&t=m") > -1;

                var type = ifMovie ? "movie" : "tvshow";

                var rawJson = client.DownloadString(Z5api.ApiShowDetails(showId, type));

                JObject showDetails = JObject.Parse(rawJson);

                var title = showDetails["title"].ToObject<string>();

                if (ifMovie)
                {
                    // gwapi requires platform token
                    client.Headers.Add(GetRequestHeaders());

                    string imgUrl(string id, string listImage) => string.IsNullOrWhiteSpace(listImage) ? "" : $"https://akamaividz1.zee5.com/resources/{id}/list/1170x658/{listImage}";

                    return new TvData
                    {
                        Page = 1,
                        ItemsPerPage = Misc.PAGESIZE, // jsonData["limit"].ToObject<int>(),
                        TotalItems = 1,
                        Items = new[]{ new TvDataItem
                            {
                                Name = title,
                                Link = $"z5/p/m|{showId}|1",
                                ImgSrc = imgUrl(showId, showDetails["list_image"]?.ToObject<string>())
                            }
                        }
                    };
                }
                else
                {
                    var seasons = showDetails["seasons"].ToArray();

                    var latestSeasonId = seasons[seasons.Length - 1]["id"].ToObject<string>();

                    // gwapi requires platform token
                    client.Headers.Add(GetRequestHeaders());

                    rawJson = client.DownloadString(Z5api.ApiEpisodesForSeason(latestSeasonId, page));
                    //rawJson = client.DownloadString(Z5api.ApiEpisodesForShow(showId, page));

                    JObject jsonData = JObject.Parse(rawJson);

                    //var title = jsonData["title"].ToObject<string>();
                    //var showId = jsonData["id"].ToObject<string>();

                    //var shows = from item in jsonData["seasons"][0]["episodes"]
                    var shows = from item in jsonData["episode"]
                                select new TvDataItem
                                {
                                    //Name = $"{title} - {item["release_date"].ToObject<DateTime>().ToString("yyyy MMM dd")} - Ep {item["episode_number"].ToObject<string>()}",
                                    Name = $"Ep {item["episode_number"]?.ToObject<string>()} - {item["release_date"]?.ToObject<DateTime>().ToString("yyyy MMM dd")}",
                                    Link = string.Concat("z5/p/", latestSeasonId, "|", item["id"].ToObject<string>(), "|", page),
                                    ImgSrc = item["image_url"]?.ToObject<string>()
                                };

                    return new TvData
                    {
                        Page = page,
                        ItemsPerPage = Misc.PAGESIZE, // jsonData["limit"].ToObject<int>(),
                        TotalItems = jsonData["total_episodes"].ToObject<int>(),
                        Items = shows.Take(9)
                    };
                }
            }
        }


        /// <summary>
        /// https://zee5vodnd.akamaized.net/hls1/elemental/hls/ON_AIR/DOMESTIC/ZEE_MARATHI/Aug2019/27082019/Seamless/Agga_Bai_Sasubai_Ep32_Seamless_27082019_mr_87a3b34dcd93e4ff8372c6ba5b934e00/index.m3u8?VIDEO_TOKEN
        /// 
        /// https://zee5vodnd.akamaized.net/hls1/elemental/hls/ON_AIR/DOMESTIC/ZEE_MARATHI/Aug2019/27082019/Seamless/Agga_Bai_Sasubai_Ep32_Seamless_27082019_mr_87a3b34dcd93e4ff8372c6ba5b934e00/index.m3u8?hdnea=st=1566958397~exp=1566961397~acl=/*~hmac=16ed5c992dab0d439fbc1d0968f7c007c4c1657a94910a7f2562eabb30814439
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public TvPlayData GetPlayData(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var videoToken = JObject.Parse(client.DownloadString(Z5api.VIDEO_TOKEN))["video_token"].ToObject<string>();

                var arrQuery = query.Split('|');
                var seasonId = arrQuery[0];
                var ifMovie = seasonId == "m";
                var episodeId = arrQuery[1];
                var page = int.Parse(arrQuery[2]);

                var playData = new TvPlayData();

                if (ifMovie)
                {
                    var rawJson = client.DownloadString(Z5api.ApiShowDetails(episodeId, "movie"));

                    JObject showDetails = JObject.Parse(rawJson);
                                       
                    var videoUrl = showDetails["video"]["hls_url"].ToObject<string>().Replace("drm", "hls");

                    playData.Name = showDetails["title"].ToObject<string>();
                    playData.ImgSrc = showDetails["list_image"]?.ToObject<string>();
                    playData.Links = new List<StreamLink>
                    {
                        new StreamLink
                        {
                            Type = "application/x-mpegURL",
                            Link = string.Concat(Z5api.AKAMAI_URL, videoUrl, videoToken)
                        },
                        //new StreamLink
                        //{
                        //    Type = "preview",
                        //    Link = previewUrl == "NYA" ? previewUrl : string.Concat(Z5api.AKAMAI_URL, previewUrl, videoToken)
                        //},
                    };
                }
                else
                {

                    //var rawJson = client.DownloadString(Z5api.ApiEpisodesForShow(showId,page));
                    var rawJson = client.DownloadString(Z5api.ApiEpisodesForSeason(seasonId, page));

                    JObject jsonData = JObject.Parse(rawJson);

                    var episode = jsonData["episode"].FirstOrDefault(q => q["id"].ToObject<string>() == episodeId);
                    //var episode = jsonData["seasons"][0]["episodes"].FirstOrDefault(q => q["id"].ToObject<string>() == episodeId);

                    //var preview_orderid = episode["orderid"].ToObject<int>() + 1;
                    //var preview = jsonData["seasons"][0]["previews"].FirstOrDefault(q => q["orderid"].ToObject<int>() == preview_orderid);

                    var title = episode["title"].ToObject<string>();
                    var image = episode["image_url"]?.ToObject<string>();


                    var episodeUrl = episode["video_details"]["hls_url"].ToObject<string>().Replace("drm", "hls");

                    //var previewUrl = preview == null ? "NYA" : preview["video_details"]["hls_url"].ToObject<string>().Replace("drm", "hls");

                    playData.Name = title;
                    playData.ImgSrc = "";
                    playData.Links = new List<StreamLink>
                    {
                        new StreamLink
                        {
                            Type = "application/x-mpegURL",
                            Link = string.Concat(Z5api.AKAMAI_URL, episodeUrl, videoToken)
                        },
                        //new StreamLink
                        //{
                        //    Type = "preview",
                        //    Link = previewUrl == "NYA" ? previewUrl : string.Concat(Z5api.AKAMAI_URL, previewUrl, videoToken)
                        //},
                    };
                }

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
