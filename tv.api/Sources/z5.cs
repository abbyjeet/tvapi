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
                    ImgSrc="mr",
                    Link = "s/l=mr&p=1"
                },
                new TvDataItem
                {
                    Name ="Hindi",
                    ImgSrc="hi",
                    Link = "s/l=hi&p=1"
                },
                new TvDataItem
                {
                    Name ="English",
                    ImgSrc="en",
                    Link = "s/l=en&p=1"
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

                var lang = QueryHelpers.ParseQuery(query)["l"].ToString();
                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);
                var param = $"asset_subtype=tvshow&languages={lang}&page={page}&page_size={Misc.PAGESIZE}";

                var rawJson = client.DownloadString(Z5api.ApiListShows(param));
                
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

        public TvData GetEpisodes(string query = "p=1")
        {
            using (WebClient client = new WebClient())
            {
                //var rawJson = client.DownloadString(Z5api.ApiShowDetails(query));

                //JObject showDetails = JObject.Parse(rawJson);

                //var title = showDetails["title"].ToObject<string>();
                //var seasons = showDetails["seasons"].ToArray();

                //var latestSeasonId = seasons[seasons.Length - 1]["id"].ToObject<string>();


                // gwapi requires platform token
                client.Headers.Add(GetRequestHeaders());

                //var lang = QueryHelpers.ParseQuery(query)["l"].ToString();
                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);
                var param = $"asset_subtype=episode&page={page}&page_size={Misc.PAGESIZE}";

                var rawJson = client.DownloadString(Z5api.ApiEpisodesForShow(param));

                JObject jsonData = JObject.Parse(rawJson);

                var title = jsonData["title"].ToObject<string>();
                var showId = jsonData["id"].ToObject<string>();

                var shows = from item in jsonData["seasons"][0]["episodes"]
                            select new TvDataItem
                            {
                                Name = $"{title} - {item["release_date"].ToObject<DateTime>().ToString("yyyy MMM dd")} - Ep {item["episode_number"].ToObject<string>()}",
                                Link = string.Concat(showId, "|", item["id"].ToObject<string>()),
                                ImgSrc = item["image_url"].ToObject<string>()
                            };

                return new TvData
                {
                    Page = 1, //jsonData["page"].ToObject<int>(),
                    ItemsPerPage = Misc.PAGESIZE, // jsonData["limit"].ToObject<int>(),
                    TotalItems = jsonData["seasons"][0]["total_episodes"].ToObject<int>(),
                    Items = shows.Take(9)
                };
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
                var showId = arrQuery[0];
                var episodeId = arrQuery[1];

                var rawJson = client.DownloadString(Z5api.ApiEpisodesForShow(showId));

                JObject jsonData = JObject.Parse(rawJson);

                var episode = jsonData["seasons"][0]["episodes"].FirstOrDefault(q => q["id"].ToObject<string>() == episodeId);

                var preview_orderid = episode["orderid"].ToObject<int>() + 1;
                var preview = jsonData["seasons"][0]["previews"].FirstOrDefault(q => q["orderid"].ToObject<int>() == preview_orderid);

                var title = episode["title"].ToObject<string>();
                var image = episode["image_url"].ToObject<string>();

                //var dashUrl = episode["video_details"]["url"].ToObject<string>().Replace("drm","dash");
                var episodeUrl = episode["video_details"]["hls_url"].ToObject<string>().Replace("drm", "hls");

                var previewUrl = preview == null ? "NYA" : preview["video_details"]["hls_url"].ToObject<string>().Replace("drm", "hls");

                //var isDrm = jsonData["is_drm"].ToObject<string>();
                //var drmKey = jsonData["drm_key_id"].ToObject<string>();

                var playData = new TvPlayData
                {
                    Name = title,
                    ImgSrc = image,
                    Links = new List<StreamLink>
                    {
                        new StreamLink
                        {
                            Type = "episode",
                            Link = string.Concat(Z5api.AKAMAI_URL, episodeUrl, videoToken)
                        },
                        new StreamLink
                        {
                            Type = "preview",
                            Link = previewUrl == "NYA" ? previewUrl : string.Concat(Z5api.AKAMAI_URL, previewUrl, videoToken)
                        },
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
