using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;

namespace tv.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            using (WebClient client = new WebClient())
            {
                //Use the default configuration for AngleSharp
                var config = Configuration.Default;

                //Create a new context for evaluating webpages with the given config
                var context = BrowsingContext.New(config);

                var parser = context.GetService<IHtmlParser>();
                
                if (id == 1)
                {

                    var htmlCode = client.DownloadString("http://desitvflix.site/");

                    var document = parser.ParseDocument(htmlCode);

                    var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                    var test = from node in list
                               select new
                               {
                                   name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   href = (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", ""),
                                   img = (node.FirstElementChild.FirstElementChild as IHtmlImageElement).Source.Replace("about:///", "")
                               };

                    return new JsonResult(test);
                }
                else if(id == 2)
                {
                    var htmlCode = client.DownloadString("http://desitvflix.site/shows.php?ch=ZeeMarathi&link=L3plZS1tYXJhdGhpLw==");

                    var document = parser.ParseDocument(htmlCode);

                    var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                    var test = from node in list
                               select new
                               {
                                   name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   href = (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", "")
                               };

                    return new JsonResult(test);
                }
                else if (id == 3)
                {
                    var htmlCode = client.DownloadString("http://desitvflix.site/dShows.php?ch=ZeeMarathi&link=L3plZS1tYXJhdGhpLw==");

                    var document = parser.ParseDocument(htmlCode);

                    var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                    var test = from node in list
                               select new
                               {
                                   name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   href = (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", "")
                               };

                    return new JsonResult(test);
                }
                else if (id == 4)
                {
                    var htmlCode = client.DownloadString("http://desitvflix.site/sdDates.php?ch=Zee-Marathi&link=L3plZS1tYXJhdGhpLw==");

                    var document = parser.ParseDocument(htmlCode);

                    var list = document.QuerySelectorAll("#blogtile-left #blogs .tile");

                    var test = from node in list
                               select new
                               {
                                   name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   href = (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", "")
                               };

                    return new JsonResult(test);
                }
                else if (id == 5)
                {
                    var htmlCode = client.DownloadString("http://desitvflix.site/sdTimeSlots.php?ch=zee-marathi&lang=marathi&sn=20-August-2019");

                    var document = parser.ParseDocument(htmlCode);

                    var list = document.QuerySelectorAll("#blogtile-left #portfolio .tile");

                    var test = from node in list
                               select new
                               {
                                   name = (node.FirstElementChild as IHtmlAnchorElement).Title,
                                   href = (node.FirstElementChild as IHtmlAnchorElement).Href.Replace("about:///", ""),
                                   img = (node.FirstElementChild.FirstElementChild as IHtmlImageElement).Source.Replace("about:///", "")
                               };

                    return new JsonResult(test);
                }
                else if (id == 6)
                {
                    var htmlCode = client.DownloadString("http://desitvflix.site/sEmbed.php?ch=zee-marathi&link=aHR0cDovLzE0NC4yMDguODguMTk3OjgwMDAvaW5kL3ZpZGVvcy9NYXJhdGhpL1plZV9NYXJhdGhpLzIwLUF1Z3VzdC0yMDE5LygwNS0wMCUyMElTVCklMjBTd2FyYWp5YSUyMFJha3NoYWslMjBTYW1iaGFqaS5tcDQvcGxheWxpc3QubTN1OA==&title=(05-00%20IST)%20Swarajya%20Rakshak%20Sambhaji");

                    var document = parser.ParseDocument(htmlCode);

                    var node = document.QuerySelector("video");

                    var test = new
                               {
                                   src = (node as IHtmlVideoElement).Source,
                                   playlist = (node.FirstElementChild as IHtmlSourceElement).Source
                               };

                    return new JsonResult(test);
                }
                else
                {
                    return new JsonResult("None");
                }
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
