using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using tv.api.Common.Constants;
using tv.api.Common.Models;

namespace tv.api.Sources
{
    public class DF : ISource
    {
        private readonly IHtmlParser _parser;

        private static string LinkToQueryString(IHtmlAnchorElement linkEl)
        {
            //return string.Join("|", QueryHelpers.ParseQuery(queryString).Select(x => x.Key + "=" + x.Value).ToArray());
            var querystring = linkEl.Search.Replace("?", "").Replace("&", "|");

            return $"{linkEl.PathName.Substring(1)};{querystring}";
        }

        private static string QueryStringToFullPath(string listString)
        {
            var arr = listString.Split(';');

            var path = arr[0];
            var query = arr[1];

            return $"{URL.DF}{path}?{query.Replace("|","&")}";
        }


        public DF(IHtmlParser parser)
        {
            _parser = parser;
        }

        public TvData GetChannels(string query = "p=1")
        {
            using (WebClient client = new WebClient())
            {
                var raw = client.DownloadString(URL.DF);

                var document = _parser.ParseDocument(raw);

                var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                var channels = from node in list
                               select new TvDataItem
                               {
                                   Name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   Link = "df/s/" + LinkToQueryString((node.FirstElementChild as IHtmlAnchorElement)),
                                   ImgSrc = string.Concat(URL.DF, (node.FirstElementChild.FirstElementChild as IHtmlImageElement).Source.Replace("about:///", ""))
                               };

                var page = int.Parse(QueryHelpers.ParseQuery(query)["p"]);

                var takeChannels = channels.Reverse().Skip(Misc.PAGESIZE * (page - 1)).Take(Misc.PAGESIZE);

                var count = channels.Count();                

                var tvData = new TvData
                {
                    Page = page,
                    ItemsPerPage = Misc.PAGESIZE,
                    TotalItems = count,
                    Items = takeChannels
                };

                return tvData;
            }
        }


        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public TvData GetSource(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var raw = client.DownloadString(QueryStringToFullPath(query));

                var document = _parser.ParseDocument(raw);

                var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                //QueryHelpers.ParseQuery(query)
                //string s = string.Join(";", myDict.Select(x => x.Key + "=" + x.Value).ToArray());

                var sources = from node in list
                            select new TvDataItem
                            {
                                Name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                Link = LinkToQueryString((node.FirstElementChild as IHtmlAnchorElement)),
                            };

                var count = sources.Count();

                var tvData = new TvData
                {
                    Page = 1,
                    ItemsPerPage = count,
                    TotalItems = count,
                    Items = sources
                };

                return tvData; //.FirstOrDefault(s=>s.Name == "Source 2" || s.Name.Contains("2"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetUrl">LIKE sdDates.php?ch=Zee-Marathi&link=L3plZS1tYXJhdGhpLw==</param>
        /// <returns></returns>
        public TvData GetShows(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var sources = GetSource(query).Items;

                var source = sources.FirstOrDefault(s => s.Link.Contains("Dates.php"));
                if (source == null)
                {
                    source = sources.FirstOrDefault(s => s.Name == "Source 2" || s.Name.Contains("2"));

                    if (source == null)
                    {
                        source = sources.FirstOrDefault(s => s.Name == "Source 1" || s.Name.Contains("1"));

                        if (source == null)
                        {
                            source = sources.FirstOrDefault(s => s.Name.Contains("Source ("));

                            if (source == null)
                            {
                                source = sources.FirstOrDefault(s => s.Link.Contains("tShows.php"));

                                if (source == null)
                                {
                                    source = sources.FirstOrDefault();
                                }
                            }
                        }
                    }
                }

                var sourceUrl = source.Link;

                var raw = client.DownloadString(QueryStringToFullPath(sourceUrl));

                var document = _parser.ParseDocument(raw);

                var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                var shows = from node in list
                            select new TvDataItem
                            {
                                Name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                Link = "df/e/" + LinkToQueryString((node.FirstElementChild as IHtmlAnchorElement)),
                            };

                var count = shows.Count();

                var tvData = new TvData
                {
                    Page = 1,
                    ItemsPerPage = count,
                    TotalItems = count,
                    Items = shows
                };

                return tvData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetUrl">LIKE sdTimeSlots.php?ch=zee-marathi&lang=marathi&sn=20-August-2019</param>
        /// <returns></returns>
        public TvData GetEpisodes(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var raw = client.DownloadString(QueryStringToFullPath(query));

                var document = _parser.ParseDocument(raw);

                var list = document.QuerySelectorAll("#blogtile-left #portfolio .tile");

                var episodes = from node in list
                               select new TvDataItem
                               {
                                   Name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   Link = "df/p/" + LinkToQueryString((node.FirstElementChild as IHtmlAnchorElement)),
                                   ImgSrc = (node.FirstElementChild.FirstElementChild as IHtmlImageElement)?.Source?.Replace("about:///", "")
                               };

                var count = episodes.Count();

                var tvData = new TvData
                {
                    Page = 1,
                    ItemsPerPage = count,
                    TotalItems = count,
                    Items = episodes
                };

                return tvData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetUrl">LIKE sEmbed.php?ch=zee-marathi&link=aHR0cDovLzE0NC4yMDguODguMTk3OjgwMDAvaW5kL3ZpZGVvcy9NYXJhdGhpL1plZV9NYXJhdGhpLzIwLUF1Z3VzdC0yMDE5LygwNS0wMCUyMElTVCklMjBTd2FyYWp5YSUyMFJha3NoYWslMjBTYW1iaGFqaS5tcDQvcGxheWxpc3QubTN1OA==&title=(05-00%20IST)%20Swarajya%20Rakshak%20Sambhaji</param>
        /// <returns></returns>
        public TvPlayData GetPlayData(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var raw = client.DownloadString(QueryStringToFullPath(query));

                var document = _parser.ParseDocument(raw);

                var title = document.QuerySelector("#content-title");
                var node = document.QuerySelector("video");

                var playData = new TvPlayData
                {
                    Name = title.TextContent.Trim(),
                    ImgSrc = "",
                    Links = new List<StreamLink>
                    {
                        new StreamLink
                        {
                            Type = (node.FirstElementChild as IHtmlSourceElement).Type,
                            Link = (node.FirstElementChild as IHtmlSourceElement).Source
                        }
                    },
                };

                return playData;
            }
        }
    }
}
