using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
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

        public DF(IHtmlParser parser)
        {
            this._parser = parser;
        }

        public TvData GetChannels(string query)
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
                                   Link = string.Concat(URL.DF, (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", "")),
                                   ImgSrc = string.Concat(URL.DF, (node.FirstElementChild.FirstElementChild as IHtmlImageElement).Source.Replace("about:///", ""))
                               };

                var tvData = new TvData
                {
                    Page = 1,
                    ItemsPerPage = 9,
                    TotalItems = 9,
                    Items = channels
                };

                return tvData;
            }
        }


        public TvData GetSource(string query = "")
        {
            using (WebClient client = new WebClient())
            {
                var raw = client.DownloadString(query);

                var document = _parser.ParseDocument(raw);

                var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                var shows = from node in list
                            select new TvDataItem
                            {
                                Name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                Link = string.Concat(URL.DF, (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", ""))
                            };

                var count = shows.Count();

                var tvData = new TvData
                {
                    Page = 1,
                    ItemsPerPage = count,
                    TotalItems = count,
                    Items = shows
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
                //var source2Url = GetSource(query);

                var raw = client.DownloadString(query);

                var document = _parser.ParseDocument(raw);

                var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                var shows = from node in list
                            select new TvDataItem
                            {
                                Name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                Link = string.Concat(URL.DF, (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", ""))
                            };

                var tvData = new TvData
                {
                    Page = 1,
                    ItemsPerPage = 9,
                    TotalItems = 9,
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
                var raw = client.DownloadString(query);

                var document = _parser.ParseDocument(raw);

                var list = document.QuerySelectorAll("#blogtile-left #portfolio .tile");

                var episodes = from node in list
                               select new TvDataItem
                               {
                                   Name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   Link = string.Concat(URL.DF, (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", "")),
                                   ImgSrc = string.Concat(URL.DF, (node.FirstElementChild.FirstElementChild as IHtmlImageElement).Source.Replace("about:///", ""))
                               };

                var tvData = new TvData
                {
                    Page = 1,
                    ItemsPerPage = 9,
                    TotalItems = 9,
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
                var raw = client.DownloadString(query);

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
