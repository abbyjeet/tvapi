using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Js;
using AngleSharp.Scripting;
using Jint.Native.Json;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tv.api.Common.Constants;
using tv.api.Common.Models;

namespace tv.api.Sources
{
    public class MX : ISource
    {

        private readonly IHtmlParser _parser;

        public MX(IHtmlParser parser)
        {
            _parser = parser;
        }

        private NameValueCollection GetRequestHeaders()
        {
            //using (WebClient client = new WebClient())
            //{
                //var rawJson = client.DownloadString(Z5api.PLATFORM_TOKEN);
                //JObject jsonData = JObject.Parse(rawJson);
                //var xAccessToken = jsonData["token"].ToObject<string>();

                return new NameValueCollection
                {
                    //{ "Origin", "https://www.zee5.com" },
                    { "User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.80 Safari/537.36" },
                    //{ "Accept", "*/*" },
                    { "Referer", "https://www.mxplayer.in/" },
                    //{ "Accept-Encoding", "gzip, deflate, br" },
                    //{ "Accept-Language", "en-US,en;q=0.9" },
                    { "Accept", "application/json, text/plain, */*" },
                    //{ "Accept-Language", "en-US,en;q=0.9" },
                    //{ "X-ACCESS-TOKEN", xAccessToken }
                    { "ETag", "e6c486f793039c8299886cfcc943872c" },
                    { "Server", "AmazonS3" },
                    { "X-Amz-Cf-Id", "IDsM0D5mTKMqip290NYYsFjkUXDHvg7o93lucYuBhPW2R6yUUJ38xw==" },
                    { "x-amz-version-id", "QAh42F5_lCNbCTHV3fVwqn.Afjm1yWdd" }
                };
            //}
        }


        //using (WebClient client = new WebClient())
        //{
        //    var raw = client.DownloadString(URL.MX);
        //    var jsValue = new JArray();
        //    var jsonRaw = "";

        //    var pattern = "\"tabIdsArr\":\\[(\\{\"id\":\".+\",\"name\":\".+\"\\})+\\],\"isBot";

        //    Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

        //    Match m = r.Match(raw);

        //    if (m.Success && m.Groups.Count == 2)
        //    {
        //        jsonRaw = $"[{m.Groups[1].Value.Replace("\"", "'")}]";
        //        jsValue = JArray.Parse(jsonRaw);
        //    }

        //    var channels = from item in jsValue
        //                   select new TvDataItem
        //                   {
        //                       Name = item["name"].ToObject<string>(),
        //                       Link = item["id"].ToObject<string>(),
        //                   };

        //    return new TvData
        //    {
        //        Page = 1,
        //        ItemsPerPage = 1,
        //        TotalItems = 1,
        //        Items = channels
        //    };

        //}


        //Get all videos types
        //1 = movie
        //2 = tvshow
        //3 = music
        //4 = shorts

        //https://api.mxplay.com/v1/web/detail/browseItem?&pageSize=9&isCustomized=true&pageNum=0&type=1&userid=aef3d23f-6684-484f-8428-365b4959b728&platform=com.mxplay.desktop&content-languages=mr

        //shorten
        //https://api.mxplay.com/v1/web/detail/browseItem/?&pageSize=9&isCustomized=true&pageNum=1&type=1&platform=com.mxplay.desktop

        public TvData GetChannels(string query = "")
        {
            var channels = new TvDataItem[] {
                new TvDataItem
                {
                    Name ="Movies",
                    ImgSrc="",
                    Link = "mx/s/t=1&p=1"
                },
                new TvDataItem
                {
                    Name ="Tv Shows",
                    ImgSrc="",
                    Link = "mx/s/t=2&p=1"
                },
                new TvDataItem
                {
                    Name ="Music",
                    ImgSrc="",
                    Link = "mx/s/t=3&p=1"
                },
                new TvDataItem
                {
                    Name ="Shorts",
                    ImgSrc="",
                    Link = "mx/s/t=4&p=1"
                }
            };

            return new TvData
            {
                Page = 1,
                ItemsPerPage = 9,
                TotalItems = 4,
                Items = channels
            };
        }

        public TvData GetShows(string query = "t=1&p=1")
        {
            using (WebClient client = new WebClient())
            {
                var type = int.Parse(QueryHelpers.ParseQuery(query)["t"]);
                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);
                var rawJson = client.DownloadString(MXapi.ApiList(type,page));

                JObject jsonData = JObject.Parse(rawJson);

                //https://is-1.mxplay.com/media/images/a6d403f3dadad011494931095c818466/16x9/9x/test_pic1573716634548.jpg

                ///media/images/b0122f955d7c92b44903a17a575c18e7/test_pic1574426294884.jpg

                string imgUrl(string id, string listImage) => string.IsNullOrWhiteSpace(listImage) ? "" : $"https://is-1.mxplay.com{(listImage.Replace(id,$"{id}/16x9/9x"))}";

                //where !(item["genres"] as JArray).Any(c => c.ToString() == "Erotic")

                var shows = from item in jsonData["items"]
                            select new TvDataItem
                            {
                                Name = item["title"].ToObject<string>(),
                                Link = $"mx/e/{item["id"].ToObject<string>()}&p=1&t=1",
                                ImgSrc = imgUrl(item["id"].ToObject<string>(), item["image"]["16x9"]?.ToObject<string>())
                            };

                return new TvData
                {
                    Page = page,
                    ItemsPerPage = 9,
                    TotalItems = jsonData["totalCount"].ToObject<int>(),
                    Items = shows
                };                
            }
        }


        //https://api.mxplay.com/v1/web/detail/collection?type=tvshow&id=e376b18ab176ce47ee576c904882ecb3&platform=com.mxplay.desktop

        //https://api.mxplay.com/v1/web/detail/tab/tvshowepisodes?type=season&id=086984a961f38bfee323cc83ac11252a&userid=e0c96965-b729-4d73-a1f4-2278a918852b&platform=com.mxplay.desktop&content-languages=mr

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
